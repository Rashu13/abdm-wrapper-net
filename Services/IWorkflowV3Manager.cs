using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public interface IWorkflowV3Manager
{
    Task<GenericV3Response?> DiscoverAsync(DiscoverRequest request, IHeaderDictionary headers);
    Task InitiateOnInitAsync(InitV3Response response, IHeaderDictionary headers);
    Task InitiateOnConfirmCallAsync(ConfirmResponse response, IHeaderDictionary headers);
    Task<FacadeV3Response> InitiateCareContextLinkingAsync(LinkRecordsV3Request linkRecordsV3Request);
    Task<RequestStatusV3Response> GetCareContextRequestStatusAsync(string requestId);
    Task<FacadeV3Response> AddPatientsAsync(List<Patient> patients);
    Task HipNotifyAsync(HIPNotifyRequest request, IHeaderDictionary headers);
    Task HealthInformationAsync(HIPHealthInformationRequest request, IHeaderDictionary headers);
    Task ProfileShareAsync(ProfileShareV3Request request, IHeaderDictionary headers);
    Task<FacadeV3Response> SendDeepLinkingSmsAsync(DeepLinkingRequest deepLinkingRequest);
    Task HandleAddCareContextsAsync(OnGenerateTokenResponse response, IHeaderDictionary headers);
}
