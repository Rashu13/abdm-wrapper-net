// ─── App-wide Constants ──────────────────────────────────────────────────────

export const APP_NAME = 'ABHA Portal';
export const APP_SUBTITLE = 'Ayushman Bharat Health Account';
export const NHA_FULL_NAME = 'National Health Authority';

export const ROUTES = {
  HOME: '/',
  ABHA_CREATE: '/abha/create',
  ABHA_VERIFY: '/abha/verify',
  ABHA_OTP: '/abha/otp',
} as const;

export const AADHAAR_LENGTH = 12;
export const OTP_LENGTH = 6;

export const API_ENDPOINTS = {
  // Auth / Cert
  GET_CERT: '/v1/auth/cert',

  // ABHA Registration via Aadhaar
  GENERATE_OTP: '/v1/registration/aadhaar/generateOtp',
  VERIFY_OTP: '/v1/registration/aadhaar/verifyOtp',
  VERIFY_MOBILE_OTP: '/v1/registration/aadhaar/verifyMobileOtp',
  CREATE_ABHA: '/v1/registration/aadhaar/createHealthIdByAdhaar',

  // ABHA Verification
  SEARCH_ABHA: '/v1/search/searchByHealthId',

  // reCAPTCHA server-side verify (your backend endpoint)
  VERIFY_CAPTCHA: '/v1/captcha/verify',
} as const;

export const ERROR_MESSAGES = {
  INVALID_AADHAAR: 'Invalid Aadhaar number. Please enter a valid Aadhaar linked with your mobile.',
  INVALID_OTP: 'Please enter a valid OTP. Entered OTP is either expired or incorrect.',
  CONSENT_REQUIRED: 'Please accept all required consents to proceed.',
  CAPTCHA_FAILED: 'CAPTCHA verification failed. Please try again.',
  NETWORK_ERROR: 'Network error. Please check your connection and try again.',
  SERVER_ERROR: 'Server error. Please try again after some time.',
  SESSION_EXPIRED: 'Your session has expired. Please start again.',
} as const;

export const RECAPTCHA_ACTION = {
  SEND_OTP: 'abha_send_otp',
  VERIFY_OTP: 'abha_verify_otp',
} as const;
