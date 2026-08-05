using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class DiscoveryV3ServiceStub : IDiscoveryV3Service
{
    public Task<GenericV3Response?> DiscoverAsync(DiscoverRequest discoverRequest, IHeaderDictionary headers)
    {
        return Task.FromResult<GenericV3Response?>(new GenericV3Response { Status = "SUCCESS", Message = "Stub Discover success" });
    }
}

public class LinkV3ServiceStub : ILinkV3Service
{
    public Task OnInitAsync(InitV3Response response, IHeaderDictionary headers)
    {
        return Task.CompletedTask;
    }

    public Task OnConfirmAsync(ConfirmResponse response, IHeaderDictionary headers)
    {
        return Task.CompletedTask;
    }
}

public class HIPLinkV3ServiceStub : IHIPLinkV3Service
{
    public Task<FacadeV3Response> AddCareContextsAsync(LinkRecordsV3Request request)
    {
        return Task.FromResult(new FacadeV3Response { Status = "Accepted", Message = "Stub AddCareContexts success" });
    }

    public Task HandleAddCareContextsAsync(OnGenerateTokenResponse response, IHeaderDictionary headers)
    {
        return Task.CompletedTask;
    }
}

public class ConsentV3ServiceStub : IConsentV3Service
{
    public Task HipNotifyAsync(HIPNotifyRequest request, IHeaderDictionary headers)
    {
        return Task.CompletedTask;
    }
}

public class HIPHealthInformationV3ServiceStub : IHIPHealthInformationV3Service
{
    public Task HealthInformationAsync(HIPHealthInformationRequest request, IHeaderDictionary headers)
    {
        return Task.CompletedTask;
    }
}

public class ProfileShareV3ServiceStub : IProfileShareV3Service
{
    public Task ShareProfileAsync(ProfileShareV3Request request, IHeaderDictionary headers)
    {
        return Task.CompletedTask;
    }
}

public class DeepLinkingV3ServiceStub : IDeepLinkingV3Service
{
    public Task<FacadeV3Response> SendDeepLinkingSmsAsync(DeepLinkingRequest request)
    {
        return Task.FromResult(new FacadeV3Response { Status = "Accepted", Message = "Stub SendDeepLinkingSms success" });
    }
}
