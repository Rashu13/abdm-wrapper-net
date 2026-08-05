import React, { useState } from 'react';
import { useAbhaCreation } from '@/hooks/useAbhaCreation';
import CaptchaWidget from './CaptchaWidget';

interface AbhaCreationFormProps {
  creationState: ReturnType<typeof useAbhaCreation>;
  onToggleVerify?: () => void;
}

const AbhaCreationForm: React.FC<AbhaCreationFormProps> = ({ creationState }) => {
  const {
    aadhaar,
    consent,
    isLoading,
    error,
    captchaImage,
    userCaptchaInput,
    setUserCaptchaInput,
    refreshCaptcha,
    isFormValid,
    setAadhaarPart,
    setConsentState,
    handleSendOtp,
    clearError,
  } = creationState;

  const [showDigits, setShowDigits] = useState(false);
  const [commMobile, setCommMobile] = useState('');
  const [isAadhaarLinkedSame, setIsAadhaarLinkedSame] = useState<'yes' | 'no'>('yes');

  const fullAadhaar = `${aadhaar.part1}${aadhaar.part2}${aadhaar.part3}`;

  const handleAadhaarChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value.replace(/\D/g, '').slice(0, 12);
    setAadhaarPart('part1', val.slice(0, 4));
    setAadhaarPart('part2', val.slice(4, 8));
    setAadhaarPart('part3', val.slice(8, 12));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (isFormValid && !isLoading) {
      handleSendOtp();
    }
  };

  const toggleConsentKey = (key: keyof typeof consent) => {
    if (typeof consent[key] === 'boolean') {
      setConsentState({
        ...consent,
        [key]: !consent[key],
      });
    }
  };

  const allConsents = consent.aadhaarSharing && consent.nonAadhaarCreation && consent.legacyRecordsLink && consent.facilityConfirmation;

  const toggleSelectAllConsents = () => {
    const nextVal = !allConsents;
    setConsentState({
      aadhaarSharing: nextVal,
      nonAadhaarCreation: nextVal,
      legacyRecordsLink: nextVal,
      facilityConfirmation: nextVal,
      healthRecordsSharing: false,
      anonymization: false,
      beneficiaryConsent: false,
      beneficiaryName: ''
    });
  };

  return (
    <div className="w-full max-w-xl mx-auto my-4">
      <form
        onSubmit={handleSubmit}
        className="bg-white border border-slate-200 rounded-2xl shadow-sm overflow-hidden"
        autoComplete="off"
      >
        {/* Premium Gradient Header */}
        <div
          className="px-7 py-5 flex items-center justify-between border-b border-slate-100"
          style={{ background: 'linear-gradient(135deg, #f0f7ff 0%, #e8f4fd 100%)' }}
        >
          <div className="flex items-center gap-3">
            <div
              className="w-9 h-9 rounded-xl flex items-center justify-center shadow-sm"
              style={{ background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' }}
            >
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2.2">
                <rect x="3" y="4" width="18" height="16" rx="3" />
                <circle cx="9" cy="10" r="2.5" />
                <path d="M15 8h2" /><path d="M15 12h2" />
                <path d="M7 16h10" />
              </svg>
            </div>
            <div>
              <h2 className="text-base font-extrabold text-slate-800 tracking-tight m-0 leading-tight">
                Create New ABHA Card
              </h2>
              <p className="text-xs text-slate-500 m-0 font-medium mt-0.5">
                Aadhaar Authentication & Enrollment
              </p>
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
              <button type="button" className="text-red-500 hover:text-red-700 font-bold" onClick={clearError}>✕</button>
            </div>
          )}

          {/* Aadhaar Input Box */}
          <div className="relative">
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wide">
              Aadhaar Number <span className="text-red-500">*</span>
            </label>
            <div className="flex items-center border border-slate-200 rounded-xl bg-slate-50/50 px-4 py-3 gap-3 focus-within:border-brand focus-within:bg-white focus-within:ring-2 focus-within:ring-brand/10 transition-all duration-200 shadow-sm">
              <div className="text-slate-400 shrink-0">
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                  <rect x="3" y="5" width="18" height="14" rx="2" /><path d="M3 10h18" />
                </svg>
              </div>
              <input
                type={showDigits ? 'text' : 'password'}
                className="flex-1 bg-transparent border-none outline-none text-slate-800 placeholder-slate-400 font-semibold text-sm tracking-wider"
                placeholder="Enter 12-digit Aadhaar Number"
                value={fullAadhaar}
                onChange={handleAadhaarChange}
                maxLength={12}
                disabled={isLoading}
              />
              <button
                type="button"
                onClick={() => setShowDigits(!showDigits)}
                className="text-slate-400 hover:text-brand transition-colors focus:outline-none cursor-pointer p-0.5"
                title={showDigits ? 'Hide Digits' : 'Show Digits'}
              >
                {showDigits ? (
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
            </div>
          </div>

          {/* Communication Mobile Input Box */}
          <div className="relative">
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wide">
              Communication Mobile Number <span className="text-slate-400 font-normal">(Optional)</span>
            </label>
            <div className="flex items-center border border-slate-200 rounded-xl bg-slate-50/50 px-4 py-3 gap-3 focus-within:border-brand focus-within:bg-white focus-within:ring-2 focus-within:ring-brand/10 transition-all duration-200 shadow-sm">
              <div className="text-slate-400 shrink-0">
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                  <rect x="5" y="2" width="14" height="20" rx="2" /><line x1="12" y1="18" x2="12.01" y2="18" />
                </svg>
              </div>
              <input
                type="text"
                className="flex-1 bg-transparent border-none outline-none text-slate-800 placeholder-slate-400 font-semibold text-sm"
                placeholder="Enter 10-digit Mobile Number"
                value={commMobile}
                onChange={(e) => setCommMobile(e.target.value.replace(/\D/g, '').slice(0, 10))}
                maxLength={10}
                disabled={isLoading}
              />
            </div>
          </div>

          {/* Question & Radio Options Box */}
          <div className="bg-slate-50 border border-slate-200 rounded-xl p-4 flex flex-col gap-3">
            <p className="text-xs font-bold text-slate-600 leading-snug m-0">
              Is communication mobile number the same as your Aadhaar-linked mobile number?
            </p>
            <div className="flex items-center gap-6">
              <label className="flex items-center gap-2 text-sm font-semibold text-slate-700 cursor-pointer">
                <input
                  type="radio"
                  name="isAadhaarLinkedSame"
                  checked={isAadhaarLinkedSame === 'yes'}
                  onChange={() => setIsAadhaarLinkedSame('yes')}
                  className="accent-brand w-4 h-4"
                />
                <span>Yes</span>
              </label>

              <label className="flex items-center gap-2 text-sm font-semibold text-slate-700 cursor-pointer">
                <input
                  type="radio"
                  name="isAadhaarLinkedSame"
                  checked={isAadhaarLinkedSame === 'no'}
                  onChange={() => setIsAadhaarLinkedSame('no')}
                  className="accent-brand w-4 h-4"
                />
                <span>No</span>
              </label>
            </div>
          </div>

          {/* Consent Checkboxes Section */}
          <div className="flex flex-col gap-2">
            <div className="flex items-center justify-between px-1">
              <label className="text-xs font-bold text-slate-500 uppercase tracking-wide">
                Patient Consent Declarations
              </label>
              <button
                type="button"
                className="text-xs font-bold text-brand hover:text-brand-dark cursor-pointer transition-colors"
                onClick={toggleSelectAllConsents}
              >
                {allConsents ? '✓ Deselect All' : '+ Select All Consents'}
              </button>
            </div>

            <div className="max-h-52 overflow-y-auto border border-slate-200 rounded-xl p-4 flex flex-col gap-3 bg-slate-50/50 scrollbar-thin">
              <label className="flex items-start gap-3 text-xs text-slate-600 font-medium cursor-pointer leading-relaxed hover:text-slate-900 transition-colors">
                <input
                  type="checkbox"
                  checked={consent.aadhaarSharing}
                  onChange={() => toggleConsentKey('aadhaarSharing')}
                  className="mt-0.5 accent-brand w-4 h-4 rounded shrink-0 cursor-pointer"
                />
                <span>
                  I am voluntarily sharing my Aadhaar Number / Virtual ID issued to UIDAI for creating an Ayushman Bharat Health Account number ("ABHA number") and ABHA Address. I authorize NHA to perform Aadhaar-based authentication.
                </span>
              </label>

              <label className="flex items-start gap-3 text-xs text-slate-600 font-medium cursor-pointer leading-relaxed hover:text-slate-900 transition-colors">
                <input
                  type="checkbox"
                  checked={consent.nonAadhaarCreation}
                  onChange={() => toggleConsentKey('nonAadhaarCreation')}
                  className="mt-0.5 accent-brand w-4 h-4 rounded shrink-0 cursor-pointer"
                />
                <span>
                  I intend to create Ayushman Bharat Health Account Number ("ABHA number") and ABHA Address for healthcare registration.
                </span>
              </label>

              <label className="flex items-start gap-3 text-xs text-slate-600 font-medium cursor-pointer leading-relaxed hover:text-slate-900 transition-colors">
                <input
                  type="checkbox"
                  checked={consent.legacyRecordsLink}
                  onChange={() => toggleConsentKey('legacyRecordsLink')}
                  className="mt-0.5 accent-brand w-4 h-4 rounded shrink-0 cursor-pointer"
                />
                <span>
                  I consent to usage of my ABHA address and ABHA number for linking of my legacy government health records and encounter details.
                </span>
              </label>

              <label className="flex items-start gap-3 text-xs text-slate-600 font-medium cursor-pointer leading-relaxed hover:text-slate-900 transition-colors">
                <input
                  type="checkbox"
                  checked={consent.facilityConfirmation}
                  onChange={() => toggleConsentKey('facilityConfirmation')}
                  className="mt-0.5 accent-brand w-4 h-4 rounded shrink-0 cursor-pointer"
                />
                <span>
                  I confirm that I am visiting this healthcare facility and authorize creation of my ABHA profile.
                </span>
              </label>
            </div>
          </div>

          {/* Security CAPTCHA Widget */}
          <div className="mt-1">
            <CaptchaWidget
              captchaImage={captchaImage}
              userCaptchaInput={userCaptchaInput}
              onCaptchaInputChange={setUserCaptchaInput}
              onRefreshCaptcha={refreshCaptcha}
              isLoading={isLoading}
              error={error?.includes('CAPTCHA') || error?.includes('captcha') ? error : null}
            />
          </div>

          {/* Generate OTP Button */}
          <button
            type="submit"
            id="send-otp-btn"
            className={`w-full font-bold py-3.5 px-6 rounded-xl text-sm transition-all duration-200 flex items-center justify-center gap-2 ${!isFormValid || isLoading
                ? 'bg-slate-200 text-slate-400 border border-slate-300/80 cursor-not-allowed shadow-none'
                : 'text-white shadow-md cursor-pointer hover:opacity-95'
              }`}
            style={!isFormValid || isLoading ? {} : { background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' }}
            disabled={!isFormValid || isLoading}
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
        </div>
      </form>
    </div>
  );
};

export default AbhaCreationForm;
