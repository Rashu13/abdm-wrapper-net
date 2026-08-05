import React from 'react';

interface CaptchaWidgetProps {
  captchaImage: string;
  userCaptchaInput: string;
  onCaptchaInputChange: (value: string) => void;
  onRefreshCaptcha: () => void;
  isLoading: boolean;
  error?: string | null;
}

const CaptchaWidget: React.FC<CaptchaWidgetProps> = ({
  captchaImage,
  userCaptchaInput,
  onCaptchaInputChange,
  onRefreshCaptcha,
  isLoading,
  error,
}) => {
  return (
    <div className="backend-captcha-wrapper">
      <div className="captcha-label-row">
        <label htmlFor="captcha-input" className="captcha-input-label">
          Security CAPTCHA <span className="required-star">*</span>
        </label>
        {/* <span className="captcha-hint">Backend SHA256 Verified</span> */}
      </div>

      <div className="captcha-controls-row">
        {/* Captcha Image Display from Backend */}
        <div className="captcha-image-box" title="Security Captcha">
          {isLoading ? (
            <div className="captcha-loading">Loading...</div>
          ) : (
            <img src={captchaImage} alt="Security Captcha" className="captcha-img" />
          )}
        </div>

        {/* Refresh Button */}
        <button
          type="button"
          className="captcha-refresh-btn"
          onClick={onRefreshCaptcha}
          disabled={isLoading}
          title="Refresh Captcha"
        >
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2">
            <path d="M23 4v6h-6" />
            <path d="M1 20v-6h6" />
            <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15" />
          </svg>
        </button>

        {/* User Code Input */}
        <input
          id="captcha-input"
          type="text"
          maxLength={6}
          className={`captcha-code-input ${error ? 'error' : ''}`}
          placeholder="Enter Code"
          value={userCaptchaInput}
          onChange={(e) => onCaptchaInputChange(e.target.value.toUpperCase())}
          autoComplete="off"
        />
      </div>

      {error && <div className="captcha-error-text">{error}</div>}
    </div>
  );
};

export default CaptchaWidget;
