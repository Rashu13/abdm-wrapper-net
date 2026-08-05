class ApiEndpoints {
  static String baseUrl = "https://sbx.wati.digital";

  // ─── M1: ABHA Creation & Auth (AbdmM1Controller) ─────────────────────────
  static String generateAadhaarOtp = "$baseUrl/api/v3/m1/generate-otp";
  static String verifyAadhaarOtp = "$baseUrl/api/v3/m1/verify-otp";
  static String loginOtp = "$baseUrl/api/v3/m1/login-otp";
  static String loginVerify = "$baseUrl/api/v3/m1/login-verify";
  static String getAbhaSuggestions(String txnId) => "$baseUrl/api/v3/m1/suggestions/$txnId";
  static String createAbhaAddress = "$baseUrl/api/v3/m1/create-abha";
  static String getAbhaProfile = "$baseUrl/api/v3/m1/profile";
  static String downloadAbhaCard = "$baseUrl/api/v3/m1/card";
  static String getScanShareRequests = "$baseUrl/api/v3/m1/scan-share-requests";
  static String mobileVerifyOtp = "$baseUrl/api/v3/m1/mobile-verify-otp";
  static String mobileVerifyConfirm = "$baseUrl/api/v3/m1/mobile-verify-confirm";
  static String emailVerifyLink = "$baseUrl/api/v3/m1/email-verify-link";
  static String reKycOtp = "$baseUrl/api/v3/m1/re-kyc-otp";
  static String reKycVerify = "$baseUrl/api/v3/m1/re-kyc-verify";

  // ─── M2: HIP Care Context Linking (HIPFacadeLinkV3Controller) ────────────
  static String addPatients = "$baseUrl/v3/add-patients";
  static String linkCareContext = "$baseUrl/v3/link-carecontexts";
  static String getLinkStatus(String requestId) => "$baseUrl/v3/link-status/$requestId";
  static String smsDeepLinkingNotify = "$baseUrl/v3/sms/notify";

  // ─── M3: HIU Consents & Health Records (HIUFacadeControllers) ───────────
  static String createConsentRequest = "$baseUrl/v3/consent-init";
  static String getConsentStatus(String requestId) => "$baseUrl/v3/consent-status/$requestId";
  static String fetchEncryptedHealthRecord = "$baseUrl/v3/health-information/fetch-records";
  static String getHealthInformationStatus(String requestId) => "$baseUrl/v3/health-information/status/$requestId";

  // ─── M3: Subscriptions (HIUFacadeSubscriptionV3Controller) ─────────────
  static String subscriptionInit = "$baseUrl/v3/subscription-init";
  static String getSubscriptionStatus(String requestId) => "$baseUrl/v3/subscription-status/$requestId";

  // ─── Patient Profile & Health Data (PatientV3Controller) ─────────────────
  static String getPatientDetails(String patientId, String hipId) => "$baseUrl/v3/patient/$patientId?hipId=$hipId";
  static String saveHealthDataRecord = "$baseUrl/v3/patient/health-data";

  // ─── System Config & Bridge Status (ConfigController) ───────────────────
  static String registerBridgeUrl = "$baseUrl/v3/config/register-bridge";
  static String checkBridgeServices = "$baseUrl/v3/config/bridge-services";
  static String getRequestLogs = "$baseUrl/v3/config/request-logs";
}
