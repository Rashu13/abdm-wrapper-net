import React, { useState, useEffect, useCallback } from 'react';
import api, { fetchBackendCaptcha, verifyBackendCaptcha } from '@/api/abhaApi';
import AbhaProfileView from '../AbhaProfile/AbhaProfileView';
import CaptchaWidget from '../AbhaCreation/CaptchaWidget';
import { CreatedProfileData } from '@/hooks/useAbhaCreation';

interface AbhaVerificationFormProps {
  onBackToCreate?: () => void;
}

type LoginWithType = 'abha' | 'abha-address' | 'aadhaar' | 'mobile';
type SendOtpToType = 'aadhaar_linked' | 'mobile_number';
type VerificationStep = 'primary-method-selection' | 'abha-sub-selection' | 'input-form' | 'otp-verify' | 'select-linked-account' | 'profile-view';

interface LinkedAbhaAccount {
  id: string;
  name: string;
  abhaNumber: string;
  abhaAddress: string;
  photo?: string;
  gender?: string;
  dob?: string;
  mobile?: string;
  address?: string;
  state?: string;
  pincode?: string;
}

const AbhaVerificationForm: React.FC<AbhaVerificationFormProps> = ({ onBackToCreate }) => {
  const [step, setStep] = useState<VerificationStep>('input-form');

  // NHA Screenshot Form States
  const [loginWith, setLoginWith] = useState<LoginWithType>('aadhaar');
  const [inputValue, setInputValue] = useState<string>('');
  const [showAadhaarText, setShowAadhaarText] = useState<boolean>(false); // default false = MASKED
  const [sendOtpTo, setSendOtpTo] = useState<SendOtpToType>('mobile_number');

  // Captcha State for Aadhaar verification
  const [captchaImage, setCaptchaImage] = useState<string>('');
  const [captchaToken, setCaptchaToken] = useState<string>('');
  const [userCaptchaInput, setUserCaptchaInput] = useState<string>('');
  const [isCaptchaLoading, setIsCaptchaLoading] = useState<boolean>(false);

  const refreshCaptcha = useCallback(async () => {
    setIsCaptchaLoading(true);
    setUserCaptchaInput('');
    try {
      const data = await fetchBackendCaptcha();
      setCaptchaImage(data.captchaImage);
      setCaptchaToken(data.captchaToken);
    } catch {
      // ignore
    } finally {
      setIsCaptchaLoading(false);
    }
  }, []);

  useEffect(() => {
    if (loginWith === 'aadhaar' && step === 'input-form') {
      refreshCaptcha();
    }
  }, [loginWith, step, refreshCaptcha]);

  // OTP Verification States
  const [otpCode, setOtpCode] = useState('');
  const [txnId, setTxnId] = useState<string | null>(null);
  const [maskedMobile, setMaskedMobile] = useState<string | null>(null);
  const [countdown, setCountdown] = useState<number>(60);
  const [resendAttempts, setResendAttempts] = useState<number>(2);

  // 60-second countdown timer for OTP step
  useEffect(() => {
    if (step === 'otp-verify' && countdown > 0) {
      const timer = setTimeout(() => setCountdown((prev) => prev - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [step, countdown]);

  // Linked ABHA Accounts List (Matching latest user screenshot)
  const [linkedAccounts, setLinkedAccounts] = useState<LinkedAbhaAccount[]>([]);

  // Common UI States
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [profileData, setProfileData] = useState<CreatedProfileData | null>(null);

  const getMaxLength = () => {
    switch (loginWith) {
      case 'aadhaar':
        return 12;
      case 'mobile':
        return 10;
      case 'abha':
        return 17; // 14 digits + hyphens
      case 'abha-address':
        return 50;
      default:
        return 30;
    }
  };

  const handleSelectPrimaryMethod = (method: 'aadhaar' | 'abha' | 'mobile') => {
    if (method === 'abha') {
      setStep('abha-sub-selection');
    } else {
      setLoginWith(method);
      setInputValue('');
      setShowAadhaarText(false);
      setError(null);
      setStep('input-form');
    }
  };

  const handleSelectSubMethod = (type: LoginWithType) => {
    setLoginWith(type);
    setInputValue('');
    setShowAadhaarText(false);
    setError(null);
    setStep('input-form');
  };

  const handleLoginWithChange = (type: LoginWithType) => {
    setLoginWith(type);
    setInputValue('');
    setShowAadhaarText(false);
    setError(null);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let val = e.target.value;
    if (loginWith === 'aadhaar' || loginWith === 'mobile') {
      val = val.replace(/\D/g, '');
    } else if (loginWith === 'abha') {
      const digitsOnly = val.replace(/\D/g, '');
      if (!val.includes('@') && digitsOnly.length > 0 && val.replace(/[\d-]/g, '').length === 0) {
        if (digitsOnly.length <= 14) {
          let formatted = digitsOnly;
          if (digitsOnly.length > 2) formatted = `${digitsOnly.slice(0, 2)}-${digitsOnly.slice(2)}`;
          if (digitsOnly.length > 6) formatted = `${digitsOnly.slice(0, 2)}-${digitsOnly.slice(2, 6)}-${digitsOnly.slice(6)}`;
          if (digitsOnly.length > 10) formatted = `${digitsOnly.slice(0, 2)}-${digitsOnly.slice(2, 6)}-${digitsOnly.slice(6, 10)}-${digitsOnly.slice(10, 14)}`;
          val = formatted;
        }
      }
    }
    setInputValue(val);
    if (error) setError(null);
  };

  const isCaptchaEntered = userCaptchaInput.trim().length >= 4;

  const isFormValid = () => {
    if (!inputValue.trim()) return false;
    if (loginWith === 'aadhaar') {
      const validAadhaar = inputValue.replace(/\D/g, '').length === 12;
      return validAadhaar && isCaptchaEntered;
    }
    if (loginWith === 'mobile') return inputValue.replace(/\D/g, '').length === 10;
    if (loginWith === 'abha') {
      const digitsOnly = inputValue.replace(/\D/g, '');
      return digitsOnly.length === 14;
    }
    if (loginWith === 'abha-address') {
      return inputValue.trim().length >= 4;
    }
    return false;
  };

  const handleVerifyAbhaSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!isFormValid()) {
      if (loginWith === 'aadhaar') setError('Invalid Aadhaar number. Please enter a valid Aadhaar linked with your mobile.');
      else if (loginWith === 'mobile') setError('Please enter a valid 10-digit Mobile number.');
      else setError('Please enter a valid ABHA Number or Address.');
      return;
    }

    setIsLoading(true);
    try {
      if (loginWith === 'aadhaar') {
        const isCaptchaValid = await verifyBackendCaptcha(captchaToken, userCaptchaInput);
        if (!isCaptchaValid) {
          setError('Invalid or expired CAPTCHA code. Please try again.');
          await refreshCaptcha();
          setIsLoading(false);
          return;
        }
      }

      let payloadValue = inputValue.trim();
      if (loginWith === 'abha' && !payloadValue.includes('@')) {
        const digits = payloadValue.replace(/\D/g, '');
        if (digits.length === 14) {
          payloadValue = `${digits.slice(0, 2)}-${digits.slice(2, 6)}-${digits.slice(6, 10)}-${digits.slice(10, 14)}`;
        }
      }

      const resp = await api.post('/api/abha/generate-aadhaar-otp', {
        encryptedAadhaar: payloadValue,
        loginWith,
        captchaToken: loginWith === 'aadhaar' ? captchaToken : undefined,
        sendOtpTo: loginWith === 'mobile' ? 'mobile_number' : sendOtpTo,
      }).catch((err) => err?.response || null);

      if (resp && resp.data && (resp.data.txnId || resp.data.success)) {
        const finalTxnId = resp.data.txnId || `TXN_${Date.now()}`;
        setTxnId(finalTxnId);
        // For Aadhaar mode: API returns masked mobile linked to Aadhaar, not Aadhaar digits
        // Only use inputValue fallback for mobile/abha modes where inputValue IS the mobile/abha number
        let maskedNum = resp.data.maskedMobile || null;
        if (!maskedNum && loginWith === 'mobile') {
          maskedNum = `******${inputValue.slice(-4)}`;
        }
        setMaskedMobile(maskedNum);
        setCountdown(60);
        setResendAttempts(2);
        setStep('otp-verify');
      } else {
        setError(loginWith === 'aadhaar' ? 'Invalid Aadhaar number. Please enter a valid Aadhaar linked with your mobile.' : (resp?.data?.message || 'Failed to send OTP. Please check the entered number and try again.'));
      }
    } catch {
      setError('Failed to initiate ABHA verification. Please check your network.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendOtp = async () => {
    if (countdown === 0 && resendAttempts > 0 && !isLoading) {
      setIsLoading(true);
      setError(null);
      try {
        let payloadValue = inputValue.trim();
        if (loginWith === 'abha' && !payloadValue.includes('@')) {
          const digits = payloadValue.replace(/\D/g, '');
          if (digits.length === 14) {
            payloadValue = `${digits.slice(0, 2)}-${digits.slice(2, 6)}-${digits.slice(6, 10)}-${digits.slice(10, 14)}`;
          }
        }

        const resp = await api.post('/api/abha/generate-aadhaar-otp', {
          encryptedAadhaar: payloadValue,
          loginWith,
          captchaToken: loginWith === 'aadhaar' ? captchaToken : undefined,
          sendOtpTo: loginWith === 'mobile' ? 'mobile_number' : sendOtpTo,
        }).catch((err) => err?.response || null);

        if (resp && resp.data && (resp.data.txnId || resp.data.success)) {
          setResendAttempts((prev) => prev - 1);
          setCountdown(60);
          setOtpCode('');
          const finalTxnId = resp.data.txnId || `TXN_${Date.now()}`;
          setTxnId(finalTxnId);
        } else {
          setError(resp?.data?.message || 'Failed to resend OTP. Please try again.');
        }
      } catch {
        setError('Failed to resend OTP. Please check your network.');
      } finally {
        setIsLoading(false);
      }
    }
  };

  const handleVerifyOtpSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!otpCode || otpCode.length < 6) {
      setError('Please enter 6-digit OTP code.');
      return;
    }

    setIsLoading(true);
    try {
      const loginTypeParam = loginWith === 'mobile' ? 'MOBILE' : loginWith === 'abha' ? 'ABHA_NUMBER' : 'AADHAAR';

      const resp = await api.post('/api/abha/verify-aadhaar-otp', {
        txnId,
        encryptedOtp: otpCode,
        mobile: loginWith === 'mobile' ? inputValue : '',
        loginWith,
        loginType: loginTypeParam,
      }).catch(() => null);

      if (!resp || !resp.data) {
        setError('Please enter a valid OTP. Entered OTP is either expired or incorrect.');
        return;
      }

      const d = resp.data;

      if (d.success === false) {
        setError('Please enter a valid OTP. Entered OTP is either expired or incorrect.');
        return;
      }

      // Mobile login may return linked accounts list
      if (d.accounts && d.accounts.length > 0) {
        setLinkedAccounts(d.accounts);
        setStep('select-linked-account');
        return;
      }

      // Get the token from verify response
      const xToken = d.token || d.Token || '';

      // Fetch full profile (address, state, pincode, photo) using the token
      let profilePayload = d;
      if (xToken) {
        const profileResp = await api.post('/api/abha/fetch-profile', { token: xToken }).catch(() => null);
        if (profileResp && profileResp.data && profileResp.data.success !== false) {
          profilePayload = profileResp.data;
        }
      }

      const p = profilePayload;
      setProfileData({
        abhaNumber:  p.abhaNumber  || d.abhaNumber  || '',
        abhaAddress: p.abhaAddress || d.abhaAddress || '',
        name:        p.name        || d.name        || '',
        gender:      p.gender      || d.gender      || '',
        dob:         p.dob         || d.dob         || '',
        mobile:      p.mobile      || d.mobile      || inputValue || '',
        address:     p.address     || d.address     || '',
        state:       p.state       || d.state       || '',
        pincode:     p.pincode     || d.pincode     || '',
        photo:       p.photo       || d.photo       || '',
        token:       xToken,
      });
      setStep('profile-view');
    } catch {
      setError('Invalid OTP code. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSelectAccount = (acc: LinkedAbhaAccount) => {
    setProfileData({
      abhaNumber: acc.abhaNumber,
      abhaAddress: acc.abhaAddress,
      name: acc.name,
      gender: acc.gender || '',
      dob: acc.dob || '',
      mobile: acc.mobile || inputValue || '',
      address: acc.address || '',
      state: acc.state || '',
      pincode: acc.pincode || '',
      photo: acc.photo || '',
    });
    setStep('profile-view');
  };

  const handleReset = () => {
    setStep('input-form');
    setInputValue('');
    setShowAadhaarText(false);
    setOtpCode('');
    setError(null);
    setProfileData(null);
    setLinkedAccounts([]);
  };

  if (step === 'profile-view' && profileData) {
    return <AbhaProfileView profile={profileData} onReset={handleReset} />;
  }

  return (
    <div className="single-col-abha-wrapper" style={{ maxWidth: step === 'select-linked-account' ? '680px' : loginWith === 'aadhaar' ? '980px' : '560px' }}>
      
      {/* STEP 3: LINKED ACCOUNTS SCREEN (Matching latest user screenshot) */}
      {step === 'select-linked-account' ? (
        <div className="linked-accounts-container">
          <div className="linked-accounts-header">
            <h2 className="linked-title">We found the following ABHA number linked to this number.</h2>
            <p className="linked-subtitle">Select ABHA number for which you wish to login :</p>
          </div>

          <div className="linked-cards-grid">
            {linkedAccounts.map((acc) => (
              <div key={acc.id} className="linked-abha-card">
                <div className="card-top-content">
                  {/* Patient Photo Box */}
                  <div className="linked-photo-box">
                    {acc.photo ? (
                      <img src={acc.photo} alt={acc.name} className="linked-photo-img" />
                    ) : (
                      <div className="linked-photo-avatar">
                        <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="#64748b" strokeWidth="1.5">
                          <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                          <circle cx="12" cy="7" r="4" />
                        </svg>
                      </div>
                    )}
                  </div>

                  {/* Patient Info */}
                  <div className="linked-info-block">
                    <h3 className="linked-name">{acc.name}</h3>

                    <div className="info-item-row">
                      <span className="info-item-label">ABHA Number :</span>
                      <span className="info-item-val">{acc.abhaNumber}</span>
                    </div>

                    <div className="info-item-row">
                      <span className="info-item-label">ABHA Address :</span>
                      <span className="info-item-val">{acc.abhaAddress}</span>
                    </div>
                  </div>
                </div>

                {/* View Profile Action Button */}
                <button
                  type="button"
                  className="linked-view-profile-btn"
                  onClick={() => handleSelectAccount(acc)}
                >
                  View Profile
                </button>
              </div>
            ))}
          </div>

          {/* Bottom BACK Button */}
          <div className="linked-footer-action">
            <button
              type="button"
              className="nha-outline-back-btn"
              onClick={handleReset}
            >
              BACK
            </button>
          </div>
        </div>
      ) : step === 'primary-method-selection' ? (
        /* STEP 1: PRIMARY METHOD SELECTION (3 CARDS MATCHING TAILWIND #037BBA THEME) */
        <div className="bg-white border-1.5 border-slate-200 rounded-2xl p-8 max-w-3xl mx-auto shadow-sm flex flex-col gap-6">
          <div className="text-center">
            <h1 className="text-3xl font-black text-brand tracking-tight m-0 mb-1">ABHA</h1>
            <p className="text-base text-slate-600 font-medium m-0">Select verification method</p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-5 my-2">
            {/* Card 1: Aadhaar Number */}
            <div
              className="bg-white border-1.5 border-slate-200 rounded-xl p-6 flex flex-col items-center text-center cursor-pointer transition-all duration-200 hover:border-brand hover:shadow-md hover:-translate-y-0.5 group"
              onClick={() => handleSelectPrimaryMethod('aadhaar')}
            >
              <div className="w-14 h-14 rounded-full bg-brand-light text-brand flex items-center justify-center mb-3.5 transition-transform duration-200 group-hover:scale-105 group-hover:bg-brand group-hover:text-white">
                <svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <rect x="2" y="5" width="20" height="14" rx="2" />
                  <line x1="2" y1="10" x2="22" y2="10" />
                  <line x1="6" y1="15" x2="10" y2="15" />
                </svg>
              </div>
              <h3 className="text-base font-bold text-slate-900 m-0 mb-1">Aadhaar Number</h3>
              <p className="text-xs text-slate-500 m-0">Verify using Aadhaar</p>
            </div>

            {/* Card 2: ABHA */}
            <div
              className="bg-white border-1.5 border-slate-200 rounded-xl p-6 flex flex-col items-center text-center cursor-pointer transition-all duration-200 hover:border-brand hover:shadow-md hover:-translate-y-0.5 group"
              onClick={() => handleSelectPrimaryMethod('abha')}
            >
              <div className="w-14 h-14 rounded-full bg-brand-light text-brand flex items-center justify-center mb-3.5 transition-transform duration-200 group-hover:scale-105 group-hover:bg-brand group-hover:text-white">
                <svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                  <circle cx="12" cy="7" r="4" />
                </svg>
              </div>
              <h3 className="text-base font-bold text-slate-900 m-0 mb-1">ABHA</h3>
              <p className="text-xs text-slate-500 m-0">Verify using ABHA</p>
            </div>

            {/* Card 3: Mobile Number */}
            <div
              className="bg-white border-1.5 border-slate-200 rounded-xl p-6 flex flex-col items-center text-center cursor-pointer transition-all duration-200 hover:border-brand hover:shadow-md hover:-translate-y-0.5 group"
              onClick={() => handleSelectPrimaryMethod('mobile')}
            >
              <div className="w-14 h-14 rounded-full bg-brand-light text-brand flex items-center justify-center mb-3.5 transition-transform duration-200 group-hover:scale-105 group-hover:bg-brand group-hover:text-white">
                <svg width="26" height="26" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <rect x="5" y="2" width="14" height="20" rx="3" />
                  <line x1="12" y1="18" x2="12.01" y2="18" strokeWidth="3" />
                </svg>
              </div>
              <h3 className="text-base font-bold text-slate-900 m-0 mb-1">Mobile Number</h3>
              <p className="text-xs text-slate-500 m-0">Verify using mobile OTP</p>
            </div>
          </div>

          <div className="flex justify-center mt-2">
            <button
              type="button"
              className="bg-brand hover:bg-brand-dark text-white font-bold px-7 py-2.5 rounded-full text-sm shadow transition-all duration-200 flex items-center gap-2"
              onClick={onBackToCreate}
            >
              ← Back to Create Abha
            </button>
          </div>
        </div>
      ) : step === 'abha-sub-selection' ? (
        /* STEP 2: ABHA SUB-SELECTION (2 CARDS: ABHA NUMBER & ABHA ADDRESS) */
        <div className="verify-selection-card-container">
          <div className="verify-top-row">
            <button
              type="button"
              className="verify-top-back-btn"
              onClick={() => setStep('primary-method-selection')}
            >
              ← Back
            </button>
          </div>

          <div className="verify-heading-center">
            <h1 className="verify-main-title">ABHA Verification</h1>
            <p className="verify-sub-title">Choose how you want to verify your ABHA</p>
          </div>

          <div className="verify-two-cards-grid">
            <div
              className={`verify-method-card ${loginWith === 'abha' ? 'selected' : ''}`}
              onClick={() => handleSelectSubMethod('abha')}
            >
              <div className="card-inner-content">
                <div className="card-header-flex">
                  <div className="icon-purple-box hash-icon">#</div>
                  <div className="card-text-block">
                    <h3 className="card-item-title">ABHA Number</h3>
                    <p className="card-item-sub">Verify using 14-digit ABHA number</p>
                  </div>
                </div>
                <button
                  type="button"
                  className="verify-continue-outline-btn"
                  onClick={(e) => {
                    e.stopPropagation();
                    handleSelectSubMethod('abha');
                  }}
                >
                  Continue →
                </button>
              </div>
            </div>

            <div
              className={`verify-method-card ${loginWith === 'abha-address' ? 'selected' : ''}`}
              onClick={() => handleSelectSubMethod('abha-address')}
            >
              <div className="card-inner-content">
                <div className="card-header-flex">
                  <div className="icon-purple-box at-icon">@</div>
                  <div className="card-text-block">
                    <h3 className="card-item-title">ABHA Address</h3>
                    <p className="card-item-sub">Verify using your ABHA address</p>
                  </div>
                </div>
                <button
                  type="button"
                  className="verify-continue-outline-btn"
                  onClick={(e) => {
                    e.stopPropagation();
                    handleSelectSubMethod('abha-address');
                  }}
                >
                  Continue →
                </button>
              </div>
            </div>
          </div>

          <div className="verify-bottom-center">
            <button
              type="button"
              className="verify-bottom-purple-pill"
              onClick={onBackToCreate}
            >
              ← Back to Create Abha
            </button>
          </div>
        </div>
      ) : (
        /* STEP 3: INPUT FORM & OTP VERIFICATION */
        <div className="w-full max-w-xl mx-auto bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden my-4">

          {/* Premium Gradient Header */}
          <div className="px-7 py-5 flex items-center justify-between border-b border-slate-100"
            style={{ background: 'linear-gradient(135deg, #f0f7ff 0%, #e8f4fd 100%)' }}>
            <div className="flex items-center gap-3">
              {/* Shield icon */}
              <div className="w-9 h-9 rounded-xl flex items-center justify-center shadow-sm"
                style={{ background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' }}>
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.2">
                  <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
                  <polyline points="9 12 11 14 15 10" />
                </svg>
              </div>
              <div>
                <h2 className="text-base font-extrabold text-slate-800 tracking-tight m-0 leading-tight">Search / Download ABHA Card</h2>
                <p className="text-xs text-slate-500 m-0 font-medium mt-0.5">Ayushman Bharat Health Account</p>
              </div>
            </div>
          </div>

          <div className="p-6 md:p-7 flex flex-col gap-5">
            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm font-semibold flex items-center justify-between gap-3">
                <div className="flex items-center gap-2">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                    <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
                  </svg>
                  <span className="text-xs md:text-sm">{error}</span>
                </div>
                <button type="button" className="text-red-500 hover:text-red-700 font-bold" onClick={() => setError(null)}>✕</button>
              </div>
            )}

            {step === 'input-form' && (
              <form onSubmit={handleVerifyAbhaSubmit} className="flex flex-col gap-5">

                {/* Sub-heading */}
                <div className="flex items-center gap-2">
                  <div className="h-px flex-1 bg-slate-100" />
                  <span className="text-xs font-semibold text-slate-400 uppercase tracking-widest px-2">Select Method</span>
                  <div className="h-px flex-1 bg-slate-100" />
                </div>

                {/* 4 Pill Tabs Track — Fixed Width Grid */}
                <div className="w-full bg-slate-100 p-1.5 rounded-full grid grid-cols-2 md:grid-cols-4 gap-1 items-center text-center shadow-inner">
                  {([
                    { key: 'aadhaar', label: 'Aadhaar' },
                    { key: 'mobile', label: 'Mobile' },
                    { key: 'abha', label: 'ABHA No.' },
                    { key: 'abha-address', label: 'ABHA Address' },
                  ] as const).map((tab) => (
                    <button
                      key={tab.key}
                      type="button"
                      className={`w-full py-2 px-1 rounded-full text-xs font-bold transition-all duration-200 cursor-pointer whitespace-nowrap ${
                        loginWith === tab.key
                          ? 'text-white shadow-md'
                          : 'text-slate-500 hover:text-slate-800 hover:bg-white/60'
                      }`}
                      style={loginWith === tab.key ? { background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' } : {}}
                      onClick={() => handleLoginWithChange(tab.key as LoginWithType)}
                    >
                      {tab.label}
                    </button>
                  ))}
                </div>

                {/* Premium Input Field */}
                <div className="relative">
                  <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wide">
                    {loginWith === 'aadhaar' ? 'Aadhaar Number'
                      : loginWith === 'mobile' ? 'Mobile Number'
                      : loginWith === 'abha' ? 'ABHA Number'
                      : 'ABHA Address'}
                  </label>
                  <div className="flex items-center border border-slate-200 rounded-xl bg-slate-50/50 px-4 py-3 gap-3 focus-within:border-brand focus-within:bg-white focus-within:ring-2 focus-within:ring-brand/10 transition-all duration-200 shadow-sm">
                    {/* Left icon per mode */}
                    <div className="text-slate-400 shrink-0">
                      {loginWith === 'aadhaar' && (
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                          <rect x="3" y="5" width="18" height="14" rx="2"/><path d="M3 10h18"/>
                        </svg>
                      )}
                      {loginWith === 'mobile' && (
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                          <rect x="5" y="2" width="14" height="20" rx="2"/><line x1="12" y1="18" x2="12.01" y2="18"/>
                        </svg>
                      )}
                      {loginWith === 'abha' && (
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                          <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/>
                        </svg>
                      )}
                      {loginWith === 'abha-address' && (
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                          <circle cx="12" cy="12" r="4"/><path d="M16 8v5a3 3 0 0 0 6 0v-1a10 10 0 1 0-3.92 7.94"/>
                        </svg>
                      )}
                    </div>
                    <input
                      type={loginWith === 'aadhaar' && !showAadhaarText ? 'password' : 'text'}
                      className="flex-1 bg-transparent border-none outline-none text-slate-800 placeholder-slate-400 font-semibold text-sm"
                      placeholder={
                        loginWith === 'aadhaar' ? 'Enter 12-digit Aadhaar Number'
                          : loginWith === 'mobile' ? 'Enter 10-digit Mobile Number'
                          : loginWith === 'abha' ? 'XX-XXXX-XXXX-XXXX'
                          : 'username@sbx'
                      }
                      value={inputValue}
                      onChange={handleInputChange}
                      maxLength={getMaxLength()}
                      disabled={isLoading}
                    />
                    {/* Right: eye toggle or valid tick */}
                    {loginWith === 'aadhaar' && (
                      <button
                        type="button"
                        onClick={() => setShowAadhaarText(!showAadhaarText)}
                        className="text-slate-400 hover:text-brand transition-colors focus:outline-none cursor-pointer"
                        title={showAadhaarText ? 'Hide' : 'Show'}
                      >
                        {showAadhaarText ? (
                          <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858-5.858A9.954 9.954 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m-4.592-4.591a3 3 0 11-4.243-4.243m4.242 4.242L9.88 9.88" />
                            <path strokeLinecap="round" strokeLinejoin="round" d="M3 3l18 18" />
                          </svg>
                        ) : (
                          <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                            <path strokeLinecap="round" strokeLinejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                          </svg>
                        )}
                      </button>
                    )}
                    {isFormValid() && loginWith !== 'aadhaar' && (
                      <svg className="w-5 h-5 text-emerald-500" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z" />
                      </svg>
                    )}
                  </div>
                </div>

                {/* Authentication Type — ABHA Number mode */}
                {loginWith === 'abha' && (
                  <div className="bg-slate-50 border border-slate-200 rounded-xl p-4 flex flex-col gap-3">
                    <p className="text-xs font-bold text-slate-500 uppercase tracking-widest m-0">Authentication Type</p>
                    <div className="flex items-center gap-5">
                      {[
                        { val: 'aadhaar_linked', label: 'Aadhaar OTP' },
                        { val: 'mobile_number', label: 'ABHA OTP' },
                      ].map((opt) => (
                        <label key={opt.val} className="flex items-center gap-2 cursor-pointer">
                          <input
                            type="radio"
                            name="authType"
                            checked={sendOtpTo === opt.val}
                            onChange={() => setSendOtpTo(opt.val as SendOtpToType)}
                            className="accent-brand w-4 h-4"
                          />
                          <span className="text-sm font-semibold text-slate-700">{opt.label}</span>
                        </label>
                      ))}
                    </div>
                  </div>
                )}

                {/* Send OTP To — ABHA Address mode */}
                {loginWith === 'abha-address' && (
                  <div className="bg-slate-50 border border-slate-200 rounded-xl p-4 flex flex-col gap-3">
                    <div>
                      <p className="text-xs font-bold text-slate-500 uppercase tracking-widest m-0 mb-2">Preferred mode</p>
                      <label className="flex items-center gap-2">
                        <input type="radio" checked readOnly className="accent-brand w-4 h-4" />
                        <span className="text-sm font-bold text-slate-800">OTP Verification</span>
                      </label>
                    </div>
                    <div>
                      <p className="text-xs font-bold text-slate-500 uppercase tracking-widest m-0 mb-2">Send OTP to</p>
                      <div className="flex items-center gap-5">
                        {[
                          { val: 'aadhaar_linked', label: 'Aadhaar linked Number' },
                          { val: 'mobile_number', label: 'Mobile Number' },
                        ].map((opt) => (
                          <label key={opt.val} className="flex items-center gap-2 cursor-pointer">
                            <input
                              type="radio"
                              name="sendOtpTo"
                              checked={sendOtpTo === opt.val}
                              onChange={() => setSendOtpTo(opt.val as SendOtpToType)}
                              className="accent-brand w-4 h-4"
                            />
                            <span className="text-sm font-semibold text-slate-700">{opt.label}</span>
                          </label>
                        ))}
                      </div>
                    </div>
                  </div>
                )}

                {/* CAPTCHA Widget if Aadhaar is selected */}
                {loginWith === 'aadhaar' && (
                  <div className="mt-1">
                    <CaptchaWidget
                      captchaImage={captchaImage}
                      userCaptchaInput={userCaptchaInput}
                      onCaptchaInputChange={setUserCaptchaInput}
                      onRefreshCaptcha={refreshCaptcha}
                      isLoading={isCaptchaLoading}
                      error={null}
                    />
                  </div>
                )}

                {/* Generate OTP Submit Button */}
                <button
                  type="submit"
                  className={`w-full font-bold py-3.5 px-6 rounded-xl text-sm transition-all duration-200 flex items-center justify-center gap-2 ${
                    !isFormValid() || isLoading
                      ? 'bg-slate-200 text-slate-400 border border-slate-300/80 cursor-not-allowed shadow-none'
                      : 'text-white shadow-md cursor-pointer hover:opacity-95'
                  }`}
                  style={!isFormValid() || isLoading ? {} : { background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' }}
                  disabled={!isFormValid() || isLoading}
                >
                  {isLoading ? (
                    <>
                      <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                      <span>Generating OTP...</span>
                    </>
                  ) : (
                    <>
                      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                        <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07A19.5 19.5 0 0 1 4.69 13a19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 3.6 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
                      </svg>
                      Generate OTP
                    </>
                  )}
                </button>
              </form>
            )}

              {step === 'otp-verify' && (
                <form onSubmit={handleVerifyOtpSubmit} className="nha-verify-form">
                  <div className="otp-info-box">
                    <p className="otp-info-title">Enter Verification OTP</p>
                    <p className="otp-info-sub">
                      {loginWith === 'aadhaar' ? (
                        <>OTP sent to your <strong>Aadhaar-linked Mobile Number</strong>{maskedMobile ? <> ({maskedMobile})</> : ''}. Enter the OTP below to continue.</>
                      ) : loginWith === 'mobile' ? (
                        <>OTP sent to Mobile Number{maskedMobile ? <> (<strong>{maskedMobile}</strong>)</> : ''}.</>
                      ) : loginWith === 'abha-address' ? (
                        <>OTP sent to {sendOtpTo === 'aadhaar_linked' ? 'Aadhaar-linked Mobile' : 'registered Mobile Number'}{maskedMobile ? <> (<strong>{maskedMobile}</strong>)</> : ''}.</>
                      ) : (
                        <>OTP sent to registered Mobile Number{maskedMobile ? <> (<strong>{maskedMobile}</strong>)</> : ''}.</>
                      )}
                    </p>
                  </div>

                  <div className="nha-input-container" style={{ marginTop: '16px' }}>
                    <input
                      type="text"
                      className="nha-main-input otp-center-input"
                      placeholder="Enter 6-digit OTP"
                      maxLength={6}
                      value={otpCode}
                      onChange={(e) => setOtpCode(e.target.value.replace(/\D/g, ''))}
                      disabled={isLoading}
                      autoFocus
                    />
                  </div>

                  {/* Resend Attempts Remaining Badge */}
                  <div className="resend-attempts-badge" style={{ marginTop: '12px' }}>
                    <span>✓ {resendAttempts} out of 2 resend attempts remaining</span>
                  </div>

                  {/* Resend Timer / Active Button */}
                  <div style={{ marginTop: '8px' }}>
                    {countdown > 0 ? (
                      <button type="button" className="resend-timer-btn" disabled style={{ width: '100%' }}>
                        Resend In {countdown}s
                      </button>
                    ) : (
                      <button
                        type="button"
                        className="resend-active-btn"
                        onClick={handleResendOtp}
                        disabled={resendAttempts === 0 || isLoading}
                        style={{ width: '100%' }}
                      >
                        {resendAttempts > 0 ? 'Resend OTP Now' : 'No Resend Attempts Remaining'}
                      </button>
                    )}
                  </div>

                  <div className="nha-action-block" style={{ marginTop: '16px' }}>
                    <button
                      type="submit"
                      className={`nha-verify-btn ${otpCode.length === 6 ? 'active' : 'disabled'}`}
                      disabled={otpCode.length < 6 || isLoading}
                    >
                      {isLoading ? 'VERIFYING OTP...' : 'VERIFY OTP & CONTINUE'}
                    </button>

                    <button
                      type="button"
                      className="nha-back-to-create-link"
                      onClick={() => setStep('input-form')}
                    >
                      ← Back to Form
                    </button>
                  </div>
                </form>
              )}
          </div>
        </div>
      )}
    </div>
  );
};

export default AbhaVerificationForm;
