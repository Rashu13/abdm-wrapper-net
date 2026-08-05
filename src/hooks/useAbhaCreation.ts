import { useState, useCallback, useEffect } from 'react';
import { fetchBackendCaptcha, verifyBackendCaptcha, generateAadhaarOtp, fetchAndInitCert } from '@/api/abhaApi';
import { validateAadhaar } from '@/utils/validators';
import { AadhaarInputState, AbhaFlowStep } from '@/types/abha.types';
import { PatientConsentState } from '@/components/abha/AbhaCreation/ConsentSection';
import { ERROR_MESSAGES } from '@/constants/app.constants';
import api from '@/api/abhaApi';

export interface CreatedProfileData {
  abhaNumber: string;
  abhaAddress: string;
  abhaAddresses?: string[];
  name: string;
  gender?: string;
  dob?: string;
  mobile?: string;
  address?: string;
  state?: string;
  pincode?: string;
  photo?: string;
  token?: string;
}

interface UseAbhaCreationReturn {
  aadhaar: AadhaarInputState;
  consent: PatientConsentState;
  step: AbhaFlowStep;
  txnId: string | null;
  otpMessage: string | null;
  maskedMobile: string | null;
  profileData: CreatedProfileData | null;
  isLoading: boolean;
  error: string | null;

  // Captcha State
  captchaImage: string;
  captchaToken: string;
  userCaptchaInput: string;
  setUserCaptchaInput: (val: string) => void;
  refreshCaptcha: () => Promise<void>;

  isFormValid: boolean;
  isAadhaarValid: boolean;
  allConsentsAccepted: boolean;

  setAadhaarPart: (part: 'part1' | 'part2' | 'part3', value: string) => void;
  setConsentState: (newConsent: PatientConsentState) => void;
  handleSendOtp: () => Promise<void>;
  handleVerifyOtp: (otpCode: string, mobileNumber?: string) => Promise<boolean>;
  resetToAadhaarStep: () => void;
  clearError: () => void;
}

export function useAbhaCreation(): UseAbhaCreationReturn {
  const [aadhaar, setAadhaar] = useState<AadhaarInputState>({
    part1: '',
    part2: '',
    part3: '',
  });

  const [consent, setConsentState] = useState<PatientConsentState>({
    aadhaarSharing: false,
    nonAadhaarCreation: false,
    legacyRecordsLink: false,
    healthRecordsSharing: false,
    anonymization: false,
    facilityConfirmation: false,
    beneficiaryConsent: false,
    beneficiaryName: '',
  });

  // Captcha State
  const [captchaImage, setCaptchaImage] = useState<string>('');
  const [captchaToken, setCaptchaToken] = useState<string>('');
  const [userCaptchaInput, setUserCaptchaInput] = useState<string>('');
  const [isCaptchaLoading, setIsCaptchaLoading] = useState<boolean>(false);

  const [step, setStep] = useState<AbhaFlowStep>('aadhaar-input');
  const [txnId, setTxnId] = useState<string | null>(null);
  const [otpMessage, setOtpMessage] = useState<string | null>(null);
  const [maskedMobile, setMaskedMobile] = useState<string | null>(null);
  const [profileData, setProfileData] = useState<CreatedProfileData | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Fetch captcha on initial mount
  const refreshCaptcha = useCallback(async () => {
    setIsCaptchaLoading(true);
    setUserCaptchaInput('');
    try {
      const data = await fetchBackendCaptcha();
      setCaptchaImage(data.captchaImage);
      setCaptchaToken(data.captchaToken);
    } catch {
      setError('Failed to load captcha. Please try again.');
    } finally {
      setIsCaptchaLoading(false);
    }
  }, []);

  useEffect(() => {
    refreshCaptcha();
  }, [refreshCaptcha]);

  const fullAadhaar = `${aadhaar.part1}${aadhaar.part2}${aadhaar.part3}`;
  const isAadhaarValid = validateAadhaar(fullAadhaar).valid;

  const allConsentsAccepted =
    consent.aadhaarSharing &&
    consent.legacyRecordsLink &&
    consent.healthRecordsSharing &&
    consent.anonymization &&
    consent.facilityConfirmation &&
    consent.beneficiaryConsent &&
    consent.beneficiaryName.trim().length > 0;

  const isCaptchaEntered = userCaptchaInput.trim().length >= 4;
  const isFormValid = isAadhaarValid && allConsentsAccepted && isCaptchaEntered;

  const setAadhaarPart = useCallback(
    (part: 'part1' | 'part2' | 'part3', value: string) => {
      setAadhaar((prev) => ({ ...prev, [part]: value }));
      if (error) setError(null);
    },
    [error],
  );

  const clearError = useCallback(() => setError(null), []);

  const resetToAadhaarStep = useCallback(() => {
    setStep('aadhaar-input');
    setError(null);
    refreshCaptcha();
  }, [refreshCaptcha]);

  const handleSendOtp = useCallback(async () => {
    try {
      setError(null);

      const aadhaarValidation = validateAadhaar(fullAadhaar);
      if (!aadhaarValidation.valid) {
        setError(aadhaarValidation.error || ERROR_MESSAGES.INVALID_AADHAAR);
        return;
      }

      if (!allConsentsAccepted) {
        setError('Please accept all required consents and enter beneficiary name.');
        return;
      }

      if (!isCaptchaEntered) {
        setError('Please enter the captcha code shown in the image.');
        return;
      }

      setIsLoading(true);

      const isCaptchaValid = await verifyBackendCaptcha(captchaToken, userCaptchaInput);
      if (!isCaptchaValid) {
        setError('Invalid or expired CAPTCHA code. Please try again.');
        await refreshCaptcha();
        setIsLoading(false);
        return;
      }

      await fetchAndInitCert();

      const otpResponse = await generateAadhaarOtp(fullAadhaar, captchaToken);

      if (!otpResponse.txnId) {
        throw new Error(otpResponse.message || 'Failed to send Aadhaar OTP');
      }

      setTxnId(otpResponse.txnId);
      setOtpMessage(otpResponse.message || 'OTP sent successfully to registered Aadhaar mobile number.');
      setMaskedMobile(otpResponse.maskedMobile || null);
      setStep('otp-verify');
    } catch (err: unknown) {
      const axiosError = err as { response?: { data?: { message?: string } } };
      const message =
        axiosError?.response?.data?.message || (err as Error)?.message || ERROR_MESSAGES.SERVER_ERROR;
      setError(message);
      await refreshCaptcha();
    } finally {
      setIsLoading(false);
    }
  }, [fullAadhaar, allConsentsAccepted, isCaptchaEntered, captchaToken, userCaptchaInput, refreshCaptcha]);

  const handleVerifyOtp = useCallback(
    async (otpCode: string, mobileNumber?: string): Promise<boolean> => {
      try {
        setError(null);
        setIsLoading(true);

        const response = await api.post<{
          success: boolean;
          message: string;
          abhaNumber?: string;
          abhaAddress?: string;
          name?: string;
          gender?: string;
          dob?: string;
          mobile?: string;
          address?: string;
          state?: string;
          pincode?: string;
          photo?: string;
        }>('/api/abha/verify-aadhaar-otp', {
          txnId,
          encryptedOtp: otpCode,
          mobile: mobileNumber || '',
        });

        if (response.data.success) {
          const profile: CreatedProfileData = {
            abhaNumber: response.data.abhaNumber || '',
            abhaAddress: response.data.abhaAddress || '',
            name: response.data.name || consent.beneficiaryName || '',
            gender: response.data.gender || '',
            dob: response.data.dob || '',
            mobile: response.data.mobile || mobileNumber || '',
            address: response.data.address || '',
            state: response.data.state || '',
            pincode: response.data.pincode || '',
            photo: response.data.photo || '',
            token: (response.data as { token?: string }).token || '',
          };

          setProfileData(profile);
          setStep('profile-view');
          return true;
        } else {
          setError('Please enter a valid OTP. Entered OTP is either expired or incorrect.');
          return false;
        }
      } catch {
        setError('Please enter a valid OTP. Entered OTP is either expired or incorrect.');
        return false;
      } finally {
        setIsLoading(false);
      }
    },
    [txnId, consent.beneficiaryName],
  );

  return {
    aadhaar,
    consent,
    step,
    txnId,
    otpMessage,
    maskedMobile,
    profileData,
    isLoading: isLoading || isCaptchaLoading,
    error,
    captchaImage,
    captchaToken,
    userCaptchaInput,
    setUserCaptchaInput,
    refreshCaptcha,
    isFormValid,
    isAadhaarValid,
    allConsentsAccepted,
    setAadhaarPart,
    setConsentState,
    handleSendOtp,
    handleVerifyOtp,
    resetToAadhaarStep,
    clearError,
  };
}
