using System.Collections.Generic;
using System.Threading.Tasks;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public interface IRequestLogV3Service
{
    Task UpdateConsentStatusAsync(string requestId, RequestStatus status);
    Task UpdateErrorAsync(string requestId, object error, RequestStatus requestStatus);
    Task UpdateErrorAsync(string requestId, object error, string status);
    Task UpdateErrorLinkErrorAsync(string linkTokenRequestId, object error, RequestStatus requestStatus);
    Task LinkTokenUpdateErrorAsync(string requestId, object error, RequestStatus requestStatus);
    Task LinkTokenUpdateErrorAsync(string requestId, object error, string status);
    Task UpdateStatusAsync(string requestId, RequestStatus requestStatus);
    Task UpdateConsentResponseAsync<T>(string requestId, string identifier, RequestStatus requestStatus, T consentDetails);
    Task<RequestLog?> FindRequestLogByTransactionIdAsync(string transactionId);
    Task PersistHipLinkRequestAsync(LinkRecordsV3Request linkRecordsV3Request, RequestStatus status, object? errors);
    Task<RequestStatusV3Response> GetStatusAsync(string requestId);
    Task SaveHealthInformationRequestAsync(HIPHealthInformationRequest hipHealthInformationRequest, RequestStatus requestStatus);
    Task<RequestLog?> FindByConsentIdAsync(string consentId, string entityType);
    Task<RequestLog?> FindByConsentIdAsync(string consentId, string entityType, string hipId);
    Task DataTransferNotifyAsync(HIPNotifyRequest hipNotifyRequest, RequestStatus requestStatus, HIPOnNotifyRequest hipOnNotifyRequest, string hipId);
    Task<string> GetPatientIdAsync(string linkRefNumber);
    Task<string> GetPatientReferenceAsync(string linkRefNumber);
    Task SetDiscoverResponseAsync(DiscoverRequest discoverRequest, object onDiscoverV3Request, string patientReference);
    Task SetLinkResponseAsync(InitV3Response initResponse, string requestId, string referenceNumber, string hipId, string clientRequestId);
    Task<List<CareContext>?> GetSelectedCareContextsAsync(string abhaAddress, string linkRefNumber);
    Task UpdateTransactionIdAsync(string requestId, string transactionId);
    Task SaveLinkTokenRequestAsync(LinkRecordsV3Request linkRecordsV3Request, string linkTokenRequestId, RequestStatus status, List<ErrorV3Response>? errors);
    Task SetHipOnAddCareContextResponseAsync(LinkOnAddCareContextsV3Response response);
    Task<RequestLog?> GetLogsByAbhaAddressAsync(string abhaAddress, string hipId);
    Task SaveScanAndShareDetailsAsync(ProfileShareV3Request profileShareV3Request, object onShareV3Request);
    Task SaveConsentRequestAsync(InitConsentRequest request, RequestStatus status);
    Task<RequestLog?> FindByClientRequestIdAsync(string clientRequestId);
}
