import React, { useState, useEffect } from 'react';

interface OtpVerifyFormProps {
  txnId: string | null;
  otpMessage: string | null;
  maskedMobile: string | null;
  isLoading: boolean;
  error: string | null;
  onVerifyOtp: (otpCode: string, mobileNumber?: string) => Promise<boolean>;
  onResendOtp: () => Promise<void>;
  onBack: () => void;
  clearError: () => void;
}

const OtpVerifyForm: React.FC<OtpVerifyFormProps> = ({
  txnId: _txnId,
  otpMessage: _otpMessage,
  maskedMobile,
  isLoading,
  error,
  onVerifyOtp,
  onResendOtp,
  onBack,
  clearError,
}) => {
  const [otpCode, setOtpCode] = useState<string>('');
  const [countdown, setCountdown] = useState<number>(60);
  const [resendAttempts, setResendAttempts] = useState<number>(2);

  // 60-second countdown timer
  useEffect(() => {
    if (countdown > 0) {
      const timer = setTimeout(() => setCountdown(countdown - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [countdown]);

  const handleOtpChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value.replace(/\D/g, '').slice(0, 6);
    setOtpCode(val);
    if (error) clearError();
  };

  const isOtpComplete = otpCode.length === 6;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (isOtpComplete && !isLoading) {
      await onVerifyOtp(otpCode);
    }
  };

  const handleResend = async () => {
    if (countdown === 0 && resendAttempts > 0 && !isLoading) {
      await onResendOtp();
      setResendAttempts((prev) => prev - 1);
      setCountdown(60);
      setOtpCode('');
    }
  };

  const formattedSeconds = countdown.toString().padStart(2, '0');

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
                <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07A19.5 19.5 0 0 1 4.69 13a19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 3.6 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
              </svg>
            </div>
            <div>
              <h2 className="text-base font-extrabold text-slate-800 tracking-tight m-0 leading-tight">
                Enter Verification OTP
              </h2>
              <p className="text-xs text-slate-500 m-0 font-medium mt-0.5">
                Step 2 of 2 • Aadhaar Verification
              </p>
            </div>
          </div>
          <button
            type="button"
            className="flex items-center gap-1.5 text-xs font-bold text-brand hover:text-brand-dark cursor-pointer transition-colors bg-white border border-brand/20 hover:border-brand/40 rounded-full px-3 py-1.5 shadow-sm"
            onClick={onBack}
          >
            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <polyline points="15 18 9 12 15 6" />
            </svg>
            Back
          </button>
        </div>

        <div className="p-6 md:p-7 flex flex-col gap-5">
          {/* Error Toast */}
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

          {/* Info Banner Box */}
          <div className="bg-sky-50/70 border border-sky-200/80 rounded-xl p-4 flex items-start gap-3">
            <div className="w-5 h-5 rounded-full bg-brand text-white flex items-center justify-center font-bold text-xs shrink-0 mt-0.5">
              ℹ
            </div>
            <p className="text-xs md:text-sm font-semibold text-slate-700 leading-snug m-0">
              OTP sent to registered Aadhaar-linked Mobile Number {maskedMobile ? <strong className="text-slate-900">({maskedMobile})</strong> : ''}. Enter the 6-digit OTP below.
            </p>
          </div>

          {/* Input Box: 6-Digit OTP */}
          <div className="relative">
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wide">
              Verification Code (OTP)
            </label>
            <div className="flex items-center border border-slate-200 rounded-xl bg-slate-50/50 px-4 py-3 gap-3 focus-within:border-brand focus-within:bg-white focus-within:ring-2 focus-within:ring-brand/10 transition-all duration-200 shadow-sm">
              <div className="text-slate-400 shrink-0">
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                  <rect x="3" y="11" width="18" height="11" rx="2" /><path d="M7 11V7a5 5 0 0 1 10 0v4" />
                </svg>
              </div>
              <input
                type="text"
                maxLength={6}
                className="flex-1 bg-transparent border-none outline-none text-slate-800 font-bold text-lg tracking-widest placeholder-slate-400"
                placeholder="0 0 0 0 0 0"
                value={otpCode}
                onChange={handleOtpChange}
                disabled={isLoading}
                autoFocus
              />
              {isOtpComplete && (
                <svg className="w-5 h-5 text-emerald-500 shrink-0" viewBox="0 0 24 24" fill="currentColor">
                  <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z" />
                </svg>
              )}
            </div>
          </div>

          {/* Resend Timer / Action Row */}
          <div className="flex items-center justify-between text-xs font-semibold text-slate-500 px-1">
            {countdown > 0 ? (
              <span className="text-slate-500 font-medium">Resend OTP in <strong className="text-slate-700">00:{formattedSeconds}</strong></span>
            ) : (
              <button
                type="button"
                className="text-brand hover:text-brand-dark font-bold cursor-pointer underline disabled:opacity-50 disabled:no-underline"
                onClick={handleResend}
                disabled={resendAttempts === 0 || isLoading}
              >
                {resendAttempts > 0 ? 'Resend OTP Now' : 'No Resend Attempts Remaining'}
              </button>
            )}

            <span className="text-slate-400">✓ {resendAttempts} out of 2 attempts left</span>
          </div>

          {/* Verify Action Button */}
          <button
            type="submit"
            className={`w-full font-bold py-3.5 px-6 rounded-xl text-sm transition-all duration-200 flex items-center justify-center gap-2 ${
              !isOtpComplete || isLoading
                ? 'bg-slate-200 text-slate-400 border border-slate-300/80 cursor-not-allowed shadow-none'
                : 'text-white shadow-md cursor-pointer hover:opacity-95'
            }`}
            style={!isOtpComplete || isLoading ? {} : { background: 'linear-gradient(135deg, #037BBA 0%, #026296 100%)' }}
            disabled={!isOtpComplete || isLoading}
          >
            {isLoading ? (
              <>
                <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                <span>Verifying OTP...</span>
              </>
            ) : (
              <>
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                  <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
                  <polyline points="22 4 12 14.01 9 11.01" />
                </svg>
                Verify OTP & Create ABHA
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default OtpVerifyForm;
