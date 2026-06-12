using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

/// <summary>
/// HIP Consent Service — migrated from Java ConsentV3Service.java
/// Handles consent/notify from gateway and sends acknowledgement back.
/// </summary>
public class ConsentV3Service : IConsentV3Service
{
    private readonly IGatewayClient _gateway;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly AbdmConfig _config;
    private readonly ILogger<ConsentV3Service> _logger;

    public ConsentV3Service(
        IGatewayClient gateway,
        IRequestLogV3Service requestLogService,
        IOptions<AbdmConfig> config,
        ILogger<ConsentV3Service> logger)
    {
        _gateway = gateway;
        _requestLogService = requestLogService;
        _config = config.Value;
        _logger = logger;
    }

    /// <summary>
    /// Gateway notifies HIP about a consent grant/revoke/expire.
    /// HIP must acknowledge with /consent/hip/on-notify.
    /// </summary>
    public async Task HipNotifyAsync(HIPNotifyRequest request, IHeaderDictionary headers)
    {
        headers.TryGetValue("REQUEST-ID", out var incomingRequestId);
        headers.TryGetValue("X-HIP-ID", out var hipId);

        try
        {
            _logger.LogInformation($"HIP consent notify: {request.Notification?.ConsentId}, status: {request.Notification?.Status}");

            await _requestLogService.DataTransferNotifyAsync(
                request,
                RequestStatus.HIP_ON_NOTIFY_SUCCESS,
                new HIPOnNotifyRequest
                {
                    RequestId = Guid.NewGuid().ToString(),
                    Timestamp = Utils.GetCurrentTimeStamp(),
                    Acknowledgement = new ConsentAcknowledgement
                    {
                        Status = "OK",
                        ConsentId = request.Notification?.ConsentId ?? string.Empty
                    }
                },
                hipId.ToString());

            // Send on-notify acknowledgement to gateway
            var onNotify = new
            {
                requestId = Guid.NewGuid().ToString(),
                timestamp = Utils.GetCurrentTimeStamp(),
                acknowledgement = new
                {
                    status = "OK",
                    consentId = request.Notification?.ConsentId
                },
                response = new { requestId = incomingRequestId.ToString() }
            };

            await _gateway.PostToGatewayAsync(
                _config.Gateway.ConsentOnNotifyPath ?? "api/v3/consent/hip/on-notify",
                onNotify,
                new Dictionary<string, string>
                {
                    ["X-HIP-ID"] = hipId.ToString(),
                    ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HipNotifyAsync");
        }
    }
}

/// <summary>
/// HIP Health Information Service — migrated from Java HIPHealthInformationV3Service.java
/// Gateway sends a request to HIP to push FHIR health data to HIU.
/// HIP reads its HIS, builds FHIR bundles, encrypts and pushes to dataPushUrl.
/// </summary>
public class HIPHealthInformationV3Service : IHIPHealthInformationV3Service
{
    private readonly IRequestLogV3Service _requestLogService;
    private readonly IPatientV3Service _patientService;
    private readonly ICryptographyService _cryptographyService;
    private readonly ILogger<HIPHealthInformationV3Service> _logger;

    public HIPHealthInformationV3Service(
        IRequestLogV3Service requestLogService,
        IPatientV3Service patientService,
        ICryptographyService cryptographyService,
        ILogger<HIPHealthInformationV3Service> logger)
    {
        _requestLogService = requestLogService;
        _patientService = patientService;
        _cryptographyService = cryptographyService;
        _logger = logger;
    }

    /// <summary>
    /// Gateway sends health-information request → HIP fetches FHIR data from its HIS and pushes to HIU.
    /// NOTE: This saves the request to DB. Actual FHIR bundle generation depends on HIS integration.
    /// </summary>
    public async Task HealthInformationAsync(HIPHealthInformationRequest request, IHeaderDictionary headers)
    {
        try
        {
            _logger.LogInformation($"Health information request received. TransactionId: {request.TransactionId}");

            // Persist the incoming health information request for tracking
            await _requestLogService.SaveHealthInformationRequestAsync(
                request, RequestStatus.HEALTH_INFORMATION_REQUEST_SUCCESS);

            // TODO: Hospital-specific implementation:
            // 1. Fetch FHIR bundles from HIS using consent details (consentId, date range)
            // 2. Encrypt bundles using keyMaterial from the request (ECDH)
            // 3. POST encrypted bundles to request.HiRequest.DataPushUrl
            // Example push structure documented in ABDM specification.
            _logger.LogInformation("Health info saved. FHIR bundle push requires HIS integration.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HealthInformationAsync");
        }
    }
}

/// <summary>
/// Profile Share Service — migrated from Java ProfileShareV3Service.java
/// Gateway sends patient profile on scan & share → HIP acknowledges.
/// </summary>
public class ProfileShareV3Service : IProfileShareV3Service
{
    private readonly IGatewayClient _gateway;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly IPatientV3Service _patientService;
    private readonly AbdmConfig _config;
    private readonly ILogger<ProfileShareV3Service> _logger;

    public ProfileShareV3Service(
        IGatewayClient gateway,
        IRequestLogV3Service requestLogService,
        IPatientV3Service patientService,
        IOptions<AbdmConfig> config,
        ILogger<ProfileShareV3Service> logger)
    {
        _gateway = gateway;
        _requestLogService = requestLogService;
        _patientService = patientService;
        _config = config.Value;
        _logger = logger;
    }

    public async Task ShareProfileAsync(ProfileShareV3Request request, IHeaderDictionary headers)
    {
        headers.TryGetValue("REQUEST-ID", out var incomingRequestId);
        headers.TryGetValue("X-HIP-ID", out var hipId);

        try
        {
            _logger.LogInformation($"Profile share received for: {request.Profile?.Patient?.AbhaAddress}");

            // Upsert patient from profile data
            if (request.Profile?.Patient != null)
            {
                var p = request.Profile.Patient;
                var patient = new Patient
                {
                    AbhaAddress = p.AbhaAddress,
                    Name = p.Name,
                    Gender = p.Gender,
                    PatientMobile = p.PhoneNumber,
                    HipId = hipId.ToString()
                };
                await _patientService.UpsertPatientsAsync(new List<Patient> { patient });
            }

            // Persist the scan & share request
            var acknowledgement = new
            {
                requestId = Guid.NewGuid().ToString(),
                timestamp = Utils.GetCurrentTimeStamp(),
                status = "SUCCESS",
                healthId = request.Profile?.Patient?.AbhaAddress
            };
            await _requestLogService.SaveScanAndShareDetailsAsync(request, acknowledgement);

            // Send on-share acknowledgement to gateway
            var onShare = new
            {
                requestId = Guid.NewGuid().ToString(),
                timestamp = Utils.GetCurrentTimeStamp(),
                acknowledgement = new { status = "SUCCESS", healthId = request.Profile?.Patient?.AbhaAddress },
                response = new { requestId = incomingRequestId.ToString() }
            };

            await _gateway.PostToGatewayAsync(
                _config.Gateway.ProfileOnSharePath ?? "api/v3/hip/patient/on-share",
                onShare,
                new Dictionary<string, string>
                {
                    ["X-HIP-ID"] = hipId.ToString(),
                    ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ShareProfileAsync");
        }
    }
}

/// <summary>
/// Deep Linking SMS Service — migrated from Java DeepLinkingV3Service.java
/// HIP-initiated: sends SMS deep link to patient mobile.
/// </summary>
public class DeepLinkingV3Service : IDeepLinkingV3Service
{
    private readonly IGatewayClient _gateway;
    private readonly AbdmConfig _config;
    private readonly ILogger<DeepLinkingV3Service> _logger;

    public DeepLinkingV3Service(
        IGatewayClient gateway,
        IOptions<AbdmConfig> config,
        ILogger<DeepLinkingV3Service> logger)
    {
        _gateway = gateway;
        _config = config.Value;
        _logger = logger;
    }

    public async Task<FacadeV3Response> SendDeepLinkingSmsAsync(DeepLinkingRequest request)
    {
        try
        {
            _logger.LogInformation($"Sending deep linking SMS to: {request.Notification?.PhoneNo}");

            var gatewayRequest = new
            {
                requestId = request.RequestId,
                timestamp = Utils.GetCurrentTimeStamp(),
                notification = new
                {
                    phoneNo = request.Notification?.PhoneNo,
                    hip = new { name = request.Notification?.Hip?.Name, id = request.Notification?.Hip?.Id }
                }
            };

            var response = await _gateway.PostToGatewayAsync(
                _config.Gateway.DeepLinkingSmsNotifyPath ?? "api/v3/patients/sms/notify",
                gatewayRequest,
                new Dictionary<string, string>
                {
                    ["REQUEST-ID"] = request.RequestId,
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });

            bool ok = response?.HttpStatus is "OK" or "Accepted" || response?.HttpStatus?.StartsWith("2") == true;
            return new FacadeV3Response
            {
                ClientRequestId = request.RequestId,
                Message = ok ? "SMS notification sent" : "Failed to send SMS notification",
                HttpStatusCode = ok ? 202 : 400
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendDeepLinkingSmsAsync");
            return new FacadeV3Response
            {
                ClientRequestId = request.RequestId,
                Message = ex.Message,
                HttpStatusCode = 500
            };
        }
    }
}
