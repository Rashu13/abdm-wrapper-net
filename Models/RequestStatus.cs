namespace AbdmWrapperNet.Models;

public enum RequestStatus
{
    PATIENT_NOT_FOUND,
    INITIATING,
    LINK_TOKEN_REQUEST_ACCEPTED,
    LINK_TOKEN_REQUEST_ERROR,
    AUTH_INIT_ACCEPTED,
    AUTH_INIT_ERROR,
    AUTH_ON_INIT_ERROR,
    AUTH_CONFIRM_ACCEPTED,
    AUTH_CONFIRM_ERROR,
    AUTH_ON_CONFIRM_ERROR,
    ADD_CARE_CONTEXT_ACCEPTED,
    ADD_CARE_CONTEXT_ERROR,
    AUTH_ON_ADD_CARE_CONTEXT_ERROR,
    CARE_CONTEXT_LINKED,
    DEEP_LINKING_SMS_INITIATED,
    DEEP_LINKING_SMS_ERROR,
    CARECONTEXT_NOTIFY_ERROR,
    CARECONTEXT_NOTIFY_ACCEPTED,
    USER_INIT_REQUEST_RECEIVED_BY_WRAPPER,
    CONSENT_INIT_ACCEPTED,
    CONSENT_INIT_ERROR,
    CONSENT_ON_INIT_ERROR,
    CONSENT_ON_INIT_RESPONSE_RECEIVED,
    CONSENT_STATUS_ACCEPTED,
    CONSENT_STATUS_ERROR,
    CONSENT_ON_STATUS_ERROR,
    CONSENT_ON_STATUS_RESPONSE_RECEIVED,
    CONSENT_HIU_NOTIFY_ERROR,
    CONSENT_ON_NOTIFY_RESPONSE_RECEIVED,
    CONSENT_FETCH_ACCEPTED,
    CONSENT_FETCH_ERROR,
    CONSENT_NOTIFY_ERROR,
    CONSENT_ON_FETCH_SUCCESS,
    CONSENT_ON_FETCH_ERROR,
    CONSENT_REVOKED,
    CONSENT_EXPIRED,
    CONSENT_DENIED,
    HIP_ON_NOTIFY_SUCCESS,
    HIP_ON_NOTIFY_ERROR,
    HEALTH_INFORMATION_REQUEST_SUCCESS,
    HEALTH_INFORMATION_REQUEST_ERROR,
    HEALTH_INFORMATION_ON_REQUEST_SUCCESS,
    HEALTH_INFORMATION_ON_REQUEST_ERROR,
    ENCRYPTED_HEALTH_INFORMATION_RECEIVED,
    ENCRYPTED_HEALTH_INFORMATION_ERROR,
    DECRYPTION_ERROR
}

public static class RequestStatusExtensions
{
    public static string GetValue(this RequestStatus status)
    {
        return status switch
        {
            RequestStatus.PATIENT_NOT_FOUND => "Patient not found or invalid abhaAddress",
            RequestStatus.INITIATING => "Request is being initiated",
            RequestStatus.LINK_TOKEN_REQUEST_ACCEPTED => "Request for link token generation ACCEPTED by gateway, records will be linked automatically soon.",
            RequestStatus.LINK_TOKEN_REQUEST_ERROR => "Request for link token generation FAILED by gateway",
            RequestStatus.AUTH_INIT_ACCEPTED => "HIP Initiated link auth init request accepted by gateway",
            RequestStatus.AUTH_INIT_ERROR => "Error thrown by Gateway for HIP Initiated link auth init",
            RequestStatus.AUTH_ON_INIT_ERROR => "Error thrown by Gateway for HIP Initiated link auth on init",
            RequestStatus.AUTH_CONFIRM_ACCEPTED => "HIP Initiated link aut confirm request accepted by gateway",
            RequestStatus.AUTH_CONFIRM_ERROR => "Error thrown by Gateway for HIP Initiated link auth confirm",
            RequestStatus.AUTH_ON_CONFIRM_ERROR => "Error thrown by Gateway for HIP Initiated link auth on confirm",
            RequestStatus.ADD_CARE_CONTEXT_ACCEPTED => "Add Care Context request accepted by gateway",
            RequestStatus.ADD_CARE_CONTEXT_ERROR => "Error thrown by Gateway for HIP Initiated link add care context",
            RequestStatus.AUTH_ON_ADD_CARE_CONTEXT_ERROR => "Error thrown by Gateway for HIP Initiated link auth on add care context",
            RequestStatus.CARE_CONTEXT_LINKED => "Care Context(s) were linked",
            RequestStatus.DEEP_LINKING_SMS_INITIATED => "DeepLinking request has been accepted by gateway",
            RequestStatus.DEEP_LINKING_SMS_ERROR => "Error thrown by Gateway for DeepLinking SMS request",
            RequestStatus.CARECONTEXT_NOTIFY_ERROR => "Error thrown by Gateway on context/notify for updation of careContext",
            RequestStatus.CARECONTEXT_NOTIFY_ACCEPTED => "updation of careContext context/notify, has been ACCEPTED by Gateway",
            RequestStatus.USER_INIT_REQUEST_RECEIVED_BY_WRAPPER => "User initiated link request received by wrapper from gateway",
            RequestStatus.CONSENT_INIT_ACCEPTED => "Consent init request accepted by gateway",
            RequestStatus.CONSENT_INIT_ERROR => "Error thrown by Gateway for consent init",
            RequestStatus.CONSENT_ON_INIT_ERROR => "Error thrown by Gateway for consent on init",
            RequestStatus.CONSENT_ON_INIT_RESPONSE_RECEIVED => "Response received from gateway for consent on init",
            RequestStatus.CONSENT_STATUS_ACCEPTED => "Consent status request accepted by gateway",
            RequestStatus.CONSENT_STATUS_ERROR => "Error thrown by Gateway for consent status",
            RequestStatus.CONSENT_ON_STATUS_ERROR => "Error thrown by Gateway for on consent status",
            RequestStatus.CONSENT_ON_STATUS_RESPONSE_RECEIVED => "Response received from gateway for consent status",
            RequestStatus.CONSENT_HIU_NOTIFY_ERROR => "Something went wrong while executing consent hiu notify",
            RequestStatus.CONSENT_ON_NOTIFY_RESPONSE_RECEIVED => "Response received from gateway for hiu notify",
            RequestStatus.CONSENT_FETCH_ACCEPTED => "Consent fetch request accepted by gateway",
            RequestStatus.CONSENT_FETCH_ERROR => "Error thrown by Gateway for consent fetch",
            RequestStatus.CONSENT_NOTIFY_ERROR => "None of the careContexts present for the requested HI-Type",
            RequestStatus.CONSENT_ON_FETCH_SUCCESS => "Response received from gateway for consent fetch",
            RequestStatus.CONSENT_ON_FETCH_ERROR => "Error thrown by Gateway for on consent fetch",
            RequestStatus.CONSENT_REVOKED => "Consent has been revoked by user",
            RequestStatus.CONSENT_EXPIRED => "Consent has expired",
            RequestStatus.CONSENT_DENIED => "Consent has been denied by user",
            RequestStatus.HIP_ON_NOTIFY_SUCCESS => "Data onNotify accepted by gateway",
            RequestStatus.HIP_ON_NOTIFY_ERROR => "Error thrown by Gateway for on Notify",
            RequestStatus.HEALTH_INFORMATION_REQUEST_SUCCESS => "Health Information request done by HIU accepted by gateway",
            RequestStatus.HEALTH_INFORMATION_REQUEST_ERROR => "Error thrown by Gateway for request done by HIU",
            RequestStatus.HEALTH_INFORMATION_ON_REQUEST_SUCCESS => "Health Information onRequest done by HIP accepted by gateway",
            RequestStatus.HEALTH_INFORMATION_ON_REQUEST_ERROR => "Error thrown by Gateway for onRequest done by HIP",
            RequestStatus.ENCRYPTED_HEALTH_INFORMATION_RECEIVED => "Encrypted Health Information received by HIU from HIP",
            RequestStatus.ENCRYPTED_HEALTH_INFORMATION_ERROR => "Error while receiving encrypted Health Information by HIU from HIP",
            RequestStatus.DECRYPTION_ERROR => "Unable to decrypt the data sent by HIP",
            _ => status.ToString()
        };
     }

     public static string GetDescription(string statusName)
     {
         if (System.Enum.TryParse<RequestStatus>(statusName, true, out var status))
         {
             return status.GetValue();
         }
         return statusName;
     }
}
