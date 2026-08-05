export type AbhaFlowStep = 'aadhaar-input' | 'otp-verify' | 'confirmation-required' | 'abha-id-setup' | 'profile-view';

export interface AadhaarInputState {
  part1: string;
  part2: string;
  part3: string;
}

export interface ConsentState {
  aadhaarConsent: boolean;
  healthRecordLinkConsent: boolean;
  healthRecordShareConsent: boolean;
  anonymizationConsent: boolean;
  beneficiaryInformedConsent: boolean;
  beneficiaryExplainedConsent: boolean;
}

export interface GenerateOtpResponse {
  txnId: string;
  message: string;
  maskedMobile?: string;
}

export interface VerifyOtpResponse {
  txnId: string;
  tokens: {
    token: string;
    expiresIn: number;
    refreshToken: string;
  };
  abhaProfile: AbhaProfile;
}

export interface AbhaProfile {
  healthIdNumber: string;
  preferredAbhaAddress: string;
  name: string;
  gender: 'M' | 'F' | 'O';
  dateOfBirth: string;
  mobile: string;
  address: string;
  districtName: string;
  stateName: string;
  pincode: string;
  photo: string;
}
