using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public interface IDiscoveryV3Service
{
    Task<GenericV3Response?> DiscoverAsync(DiscoverRequest discoverRequest, IHeaderDictionary headers);
}

public interface ILinkV3Service
{
    Task OnInitAsync(InitV3Response response, IHeaderDictionary headers);
    Task OnConfirmAsync(ConfirmResponse response, IHeaderDictionary headers);
}

public interface IHIPLinkV3Service
{
    Task<FacadeV3Response> AddCareContextsAsync(LinkRecordsV3Request request);
    Task HandleAddCareContextsAsync(OnGenerateTokenResponse response, IHeaderDictionary headers);
}

public interface IConsentV3Service
{
    Task HipNotifyAsync(HIPNotifyRequest request, IHeaderDictionary headers);
}

public interface IHIPHealthInformationV3Service
{
    Task HealthInformationAsync(HIPHealthInformationRequest request, IHeaderDictionary headers);
}

public interface IProfileShareV3Service
{
    Task ShareProfileAsync(ProfileShareV3Request request, IHeaderDictionary headers);
}

public interface IDeepLinkingV3Service
{
    Task<FacadeV3Response> SendDeepLinkingSmsAsync(DeepLinkingRequest request);
}
