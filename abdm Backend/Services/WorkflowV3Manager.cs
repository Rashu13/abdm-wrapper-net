using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class WorkflowV3Manager : IWorkflowV3Manager
{
    private readonly ILogger<WorkflowV3Manager> _logger;
    private readonly IDiscoveryV3Service _discoveryService;
    private readonly ILinkV3Service _linkService;
    private readonly IHIPLinkV3Service _hipLinkService;
    private readonly IConsentV3Service _consentService;
    private readonly IHIPHealthInformationV3Service _healthInformationService;
    private readonly IProfileShareV3Service _profileShareService;
    private readonly IDeepLinkingV3Service _deepLinkingService;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly IPatientV3Service _patientService;

    public WorkflowV3Manager(
        ILogger<WorkflowV3Manager> logger,
        IDiscoveryV3Service discoveryService,
        ILinkV3Service linkService,
        IHIPLinkV3Service hipLinkService,
        IConsentV3Service consentService,
        IHIPHealthInformationV3Service healthInformationService,
        IProfileShareV3Service profileShareService,
        IDeepLinkingV3Service deepLinkingService,
        IRequestLogV3Service requestLogService,
        IPatientV3Service patientService)
    {
        _logger = logger;
        _discoveryService = discoveryService;
        _linkService = linkService;
        _hipLinkService = hipLinkService;
        _consentService = consentService;
        _healthInformationService = healthInformationService;
        _profileShareService = profileShareService;
        _deepLinkingService = deepLinkingService;
        _requestLogService = requestLogService;
        _patientService = patientService;
    }

    public async Task<GenericV3Response?> DiscoverAsync(DiscoverRequest request, IHeaderDictionary headers)
    {
        return await _discoveryService.DiscoverAsync(request, headers);
    }

    public async Task InitiateOnInitAsync(InitV3Response response, IHeaderDictionary headers)
    {
        if (response != null)
        {
            await _linkService.OnInitAsync(response, headers);
        }
        else
        {
            _logger.LogError("Error in Init response from gateway");
        }
    }

    public async Task InitiateOnConfirmCallAsync(ConfirmResponse response, IHeaderDictionary headers)
    {
        if (response != null)
        {
            await _linkService.OnConfirmAsync(response, headers);
        }
        else
        {
            _logger.LogError("Error in Confirm response from gateway");
        }
    }

    public async Task<FacadeV3Response> InitiateCareContextLinkingAsync(LinkRecordsV3Request linkRecordsV3Request)
    {
        _logger.LogInformation("Initiating careContext Linking");
        return await _hipLinkService.AddCareContextsAsync(linkRecordsV3Request);
    }

    public async Task<RequestStatusV3Response> GetCareContextRequestStatusAsync(string requestId)
    {
        return await _requestLogService.GetStatusAsync(requestId);
    }

    public async Task<FacadeV3Response> AddPatientsAsync(List<Patient> patients)
    {
        return await _patientService.UpsertPatientsAsync(patients);
    }

    public async Task HipNotifyAsync(HIPNotifyRequest request, IHeaderDictionary headers)
    {
        _logger.LogDebug($"HIP Notify request: {request}");
        await _consentService.HipNotifyAsync(request, headers);
    }

    public async Task HealthInformationAsync(HIPHealthInformationRequest request, IHeaderDictionary headers)
    {
        _logger.LogDebug($"Health Information request: {request}");
        await _healthInformationService.HealthInformationAsync(request, headers);
    }

    public async Task ProfileShareAsync(ProfileShareV3Request request, IHeaderDictionary headers)
    {
        _logger.LogDebug($"Profile share request: {request}");
        await _profileShareService.ShareProfileAsync(request, headers);
    }

    public async Task<FacadeV3Response> SendDeepLinkingSmsAsync(DeepLinkingRequest deepLinkingRequest)
    {
        return await _deepLinkingService.SendDeepLinkingSmsAsync(deepLinkingRequest);
    }

    public async Task HandleAddCareContextsAsync(OnGenerateTokenResponse response, IHeaderDictionary headers)
    {
        await _hipLinkService.HandleAddCareContextsAsync(response, headers);
    }
}
