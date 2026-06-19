using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

/// <summary>
/// HIP-Initiated Linking — migrated from Java HIPLinkV3Service.java
/// Flow: Facade calls AddCareContexts → check linkToken → generate if missing → POST to gateway
/// </summary>
public class HIPLinkV3Service : IHIPLinkV3Service
{
    private readonly IGatewayClient _gateway;
    private readonly IPatientV3Service _patientService;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly ILinkTokenService _linkTokenService;
    private readonly AbdmConfig _config;
    private readonly ILogger<HIPLinkV3Service> _logger;

    public HIPLinkV3Service(
        IGatewayClient gateway,
        IPatientV3Service patientService,
        IRequestLogV3Service requestLogService,
        ILinkTokenService linkTokenService,
        IOptions<AbdmConfig> config,
        ILogger<HIPLinkV3Service> logger)
    {
        _gateway = gateway;
        _patientService = patientService;
        _requestLogService = requestLogService;
        _linkTokenService = linkTokenService;
        _config = config.Value;
        _logger = logger;
    }

    /// <summary>
    /// Entry point — either uses existing link token or generates a new one
    /// </summary>
    public async Task<FacadeV3Response> AddCareContextsAsync(LinkRecordsV3Request request)
    {
        var patient = await _patientService.GetPatientAsync(request.AbhaAddress, request.RequesterId);
        if (patient == null)
        {
            return Error(request.RequestId, "Patient not found to generate linkToken");
        }

        string? linkToken = await _linkTokenService.GetLinkTokenAsync(patient.AbhaAddress, request.RequesterId);

        if (string.IsNullOrEmpty(linkToken))
        {
            return await GenerateLinkTokenAsync(patient, request);
        }

        // Check for already-linked care contexts
        var alreadyLinked = await _patientService.GetSameCareContextsAsync(
            request.AbhaAddress, request.RequesterId, request.CareContexts);

        if (alreadyLinked != null && alreadyLinked.Any())
        {
            return await HipContextNotifyAsync(request, patient, alreadyLinked);
        }

        return await ProcessAddCareContextsAsync(request, patient, linkToken);
    }

    /// <summary>
    /// After on-generate-token callback: save link token → retry AddCareContexts
    /// </summary>
    public async Task HandleAddCareContextsAsync(OnGenerateTokenResponse response, IHeaderDictionary headers)
    {
        headers.TryGetValue("X-HIP-ID", out var hipId);

        try
        {
            if (response.Error != null)
            {
                await _requestLogService.UpdateErrorLinkErrorAsync(
                    response.Response?.RequestId ?? string.Empty,
                    response.Error, RequestStatus.LINK_TOKEN_REQUEST_ERROR);
                return;
            }

            await _linkTokenService.SaveLinkTokenAsync(
                response.AbhaAddress, response.LinkToken, hipId.ToString());

            var requestLog = await _requestLogService.GetLogsByAbhaAddressAsync(
                response.AbhaAddress, hipId.ToString());

            if (requestLog == null)
            {
                _logger.LogError("Request log not found for on-generate-token");
                return;
            }

            // Rebuild the original link request from DB and retry
            var originalRequest = new LinkRecordsV3Request
            {
                RequestId = requestLog.ClientRequestId ?? Guid.NewGuid().ToString(),
                RequesterId = hipId.ToString(),
                AbhaAddress = response.AbhaAddress,
                CareContexts = new List<CareContext>()  // re-fetched on retry via addCareContexts flow
            };

            await AddCareContextsAsync(originalRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleAddCareContextsAsync");
        }
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private async Task<FacadeV3Response> GenerateLinkTokenAsync(Patient patient, LinkRecordsV3Request request)
    {
        string generateTokenRequestId = Guid.NewGuid().ToString();
        try
        {
            await _linkTokenService.SaveLinkTokenRequestIdAsync(
                patient.AbhaAddress, patient.HipId ?? string.Empty, generateTokenRequestId);

            var tokenReq = new
            {
                requestId = generateTokenRequestId,
                timestamp = Utils.GetCurrentTimeStamp(),
                abhaAddress = patient.AbhaAddress,
                name = patient.Name,
                gender = patient.Gender,
                yearOfBirth = int.TryParse(patient.DateOfBirth?.Length >= 4 ? patient.DateOfBirth[..4] : "0", out var yr) ? yr : 0
            };

            var response = await _gateway.PostToGatewayAsync(
                _config.Gateway.GenerateLinkTokenPath ?? "api/v3/hip/token/generate-token",
                tokenReq,
                new Dictionary<string, string>
                {
                    ["X-HIP-ID"] = request.RequesterId,
                    ["REQUEST-ID"] = generateTokenRequestId,
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });

            bool ok = response?.HttpStatus is "OK" or "Accepted" || response?.HttpStatus?.StartsWith("2") == true;
            if (ok)
            {
                await _requestLogService.SaveLinkTokenRequestAsync(
                    request, generateTokenRequestId, RequestStatus.LINK_TOKEN_REQUEST_ACCEPTED, null);
                return new FacadeV3Response
                {
                    ClientRequestId = request.RequestId,
                    Message = RequestStatus.LINK_TOKEN_REQUEST_ACCEPTED.GetValue(),
                    HttpStatusCode = 202
                };
            }

            var gatewayErrorMsg = !string.IsNullOrEmpty(response?.Message) ? response.Message : "Unable to generate linkToken";
            await _requestLogService.SaveLinkTokenRequestAsync(
                request, generateTokenRequestId, RequestStatus.LINK_TOKEN_REQUEST_ERROR,
                new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = gatewayErrorMsg } } });

            return Error(request.RequestId, $"LinkToken generation failed. Gateway Response: {gatewayErrorMsg}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating link token");
            return Error(request.RequestId, ex.Message);
        }
    }

    private async Task<FacadeV3Response> ProcessAddCareContextsAsync(LinkRecordsV3Request request, Patient patient, string linkToken)
    {
        try
        {
            var grouped = request.CareContexts
                .GroupBy(c => string.IsNullOrEmpty(c.HiType) ? "HealthDocumentRecord" : c.HiType)
                .Select(g => new
                {
                    referenceNumber = patient.PatientReference,
                    display = patient.PatientDisplay,
                    hiType = g.Key,
                    count = g.Count(),
                    careContexts = g.Select(cc => new { display = cc.Display, referenceNumber = cc.ReferenceNumber }).ToList()
                }).ToList();

            var addCareContextsBody = new
            {
                requestId = request.RequestId,
                timestamp = Utils.GetCurrentTimeStamp(),
                abhaAddress = patient.AbhaAddress,
                patient = grouped
            };

            var headers = new Dictionary<string, string>
            {
                ["X-HIP-ID"] = request.RequesterId,
                ["X-LINK-TOKEN"] = $"Bearer {linkToken}",
                ["REQUEST-ID"] = request.RequestId,
                ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
            };

            var response = await _gateway.PostToGatewayAsync(
                _config.Gateway.AddCareContextsPath ?? "api/v3/link/carecontext",
                addCareContextsBody, headers);

            bool ok = response?.HttpStatus is "OK" or "Accepted" || response?.HttpStatus?.StartsWith("2") == true;
            if (ok)
            {
                await _requestLogService.PersistHipLinkRequestAsync(
                    request, RequestStatus.ADD_CARE_CONTEXT_ACCEPTED, null);
                return new FacadeV3Response
                {
                    ClientRequestId = request.RequestId,
                    Message = RequestStatus.ADD_CARE_CONTEXT_ACCEPTED.GetValue(),
                    HttpStatusCode = 202
                };
            }

            var errorMsg = !string.IsNullOrEmpty(response?.Message) ? response.Message : "Unable to link care contexts";
            await _requestLogService.PersistHipLinkRequestAsync(
                request, RequestStatus.ADD_CARE_CONTEXT_ERROR,
                new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = errorMsg } } });

            return Error(request.RequestId, $"Gateway Error: {errorMsg}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessAddCareContextsAsync");
            return Error(request.RequestId, ex.Message);
        }
    }

    private async Task<FacadeV3Response> HipContextNotifyAsync(
        LinkRecordsV3Request request, Patient patient, List<CareContext> careContexts)
    {
        try
        {
            foreach (var cc in careContexts)
            {
                var notifyBody = new
                {
                    requestId = Guid.NewGuid().ToString(),
                    timestamp = Utils.GetCurrentTimeStamp(),
                    notification = new
                    {
                        patient = new { id = request.AbhaAddress },
                        hip = new { id = request.RequesterId },
                        hiTypes = new[] { cc.HiType ?? "HealthDocumentRecord" },
                        date = Utils.GetCurrentTimeStamp(),
                        careContext = new
                        {
                            careContextReference = cc.ReferenceNumber,
                            patientReference = patient.PatientReference
                        }
                    }
                };

                await _gateway.PostToGatewayAsync(
                    _config.Gateway.LinkContextNotifyPath ?? "api/v3/links/context/notify",
                    notifyBody,
                    new Dictionary<string, string>
                    {
                        ["X-HIP-ID"] = patient.HipId ?? string.Empty,
                        ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                        ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                    });
            }

            await _requestLogService.PersistHipLinkRequestAsync(
                request, RequestStatus.CARECONTEXT_NOTIFY_ACCEPTED, null);

            return new FacadeV3Response
            {
                ClientRequestId = request.RequestId,
                Message = RequestStatus.CARECONTEXT_NOTIFY_ACCEPTED.GetValue(),
                HttpStatusCode = 202
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HipContextNotifyAsync");
            return Error(request.RequestId, ex.Message);
        }
    }

    private static FacadeV3Response Error(string requestId, string msg) => new()
    {
        ClientRequestId = requestId,
        Errors = new List<ErrorResponse> { new() { Code = "1000", Message = msg } },
        HttpStatusCode = 400
    };
}
