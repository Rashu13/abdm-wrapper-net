namespace AbdmWrapperNet.Constants;

public static class GatewayUrl
{
    // Profile Share
    public const string ProfileSharePath = "api/v3/hip/patient/share";

    // HIP Initiated Linking
    public const string OnGenerateLinkTokenPath = "api/v3/hip/token/on-generate-token";
    public const string OnAddCareContextPath = "api/v3/link/on_carecontext";

    // HIP Context notify
    public const string LinkContextOnNotifyPath = "api/v3/links/context/on-notify";

    // HIP Deep Linking
    public const string DeepLinkingOnNotifyPath = "api/v3/patients/sms/on-notify";

    // DISCOVER
    public const string DiscoverPath = "api/v3/hip/patient/care-context/discover";
    public const string InitLinkingPath = "api/v3/hip/link/care-context/init";
    public const string ConfirmLinkingPath = "api/v3/hip/link/care-context/confirm";

    // HIP Data Transfer
    public const string HipConsentNotifyPath = "api/v3/consent/request/hip/notify";
    public const string HipHealthInformationRequestPath = "api/v3/hip/health-information/request";

    // HIU Consent
    public const string ConsentOnInitPath = "api/v3/hiu/consent/request/on-init";
    public const string ConsentOnStatusPath = "api/v3/hiu/consent/request/on-status";
    public const string ConsentHiuNotifyPath = "api/v3/hiu/consent/request/notify";
    public const string ConsentOnFetchPath = "api/v3/hiu/consent/on-fetch";

    // HIU Data Transfer
    public const string HiuHealthInformationOnRequestPath = "api/v3/hiu/health-information/on-request";
}
