using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

/// <summary>
/// Handles HIU Consent operations: initiating consent, fetching status, and hiu/on-notify acknowledgement.
/// </summary>
public class HIUConsentV3Service
{
    private readonly IGatewayClient _gatewayClient;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly IPatientV3Service _patientService;
    private readonly IConsentPatientV3Service _consentPatientService;
    private readonly IConsentRequestMappingService _consentRequestMappingService;
    private readonly AbdmConfig _config;
    private readonly ILogger<HIUConsentV3Service> _logger;

    public HIUConsentV3Service(
        IGatewayClient gatewayClient,
        IRequestLogV3Service requestLogService,
        IPatientV3Service patientService,
        IConsentPatientV3Service consentPatientService,
        IConsentRequestMappingService consentRequestMappingService,
        IOptions<AbdmConfig> config,
        ILogger<HIUConsentV3Service> logger)
    {
        _gatewayClient = gatewayClient;
        _requestLogService = requestLogService;
        _patientService = patientService;
        _consentPatientService = consentPatientService;
        _consentRequestMappingService = consentRequestMappingService;
        _config = config.Value;
        _logger = logger;
    }

    /// <summary>
    /// Initiates a consent request to the ABDM Gateway.
    /// </summary>
    public async Task<FacadeV3Response> InitiateConsentRequestAsync(InitConsentRequest request)
    {
        try
        {
            _logger.LogInformation($"Initiating consent request: {request.RequestId}");

            await _requestLogService.SaveConsentRequestAsync(request, RequestStatus.INITIATING);

            var headers = new Dictionary<string, string>
            {
                ["X-HIU-ID"] = request.Consent?.Hiu?.Id ?? _config.HiuSetup?.HiuId ?? string.Empty,
                ["REQUEST-ID"] = request.RequestId,
                ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
            };

            var response = await _gatewayClient.PostToGatewayAsync(
                _config.Gateway?.ConsentInitPath ?? "api/v3/consent/request/init",
                request, headers);

            bool success = response?.HttpStatus?.StartsWith("2") == true
                           || response?.HttpStatus == "OK"
                           || response?.HttpStatus == "Accepted";

            if (success)
            {
                await _requestLogService.UpdateConsentStatusAsync(request.RequestId, RequestStatus.CONSENT_INIT_ACCEPTED);
                return new FacadeV3Response
                {
                    ClientRequestId = request.RequestId,
                    Message = RequestStatus.CONSENT_INIT_ACCEPTED.GetValue(),
                    HttpStatusCode = 202
                };
            }
            else
            {
                var errorMsg = response?.Message ?? "Error from gateway while initiating consent request";
                _logger.LogError(errorMsg);
                await _requestLogService.UpdateErrorAsync(request.RequestId,
                    new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = errorMsg } } },
                    RequestStatus.CONSENT_INIT_ERROR);
                return new FacadeV3Response
                {
                    ClientRequestId = request.RequestId,
                    Errors = new List<ErrorResponse> { new() { Code = "1000", Message = errorMsg } },
                    HttpStatusCode = 400
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while initiating consent request");
            await _requestLogService.UpdateErrorAsync(request.RequestId,
                new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = ex.Message } } },
                RequestStatus.CONSENT_INIT_ERROR);
            return new FacadeV3Response
            {
                ClientRequestId = request.RequestId,
                Message = ex.Message,
                HttpStatusCode = 500
            };
        }
    }

    /// <summary>
    /// Returns the consent status by request ID (polls DB for state).
    /// </summary>
    public async Task<ConsentStatusV3Response> GetConsentStatusAsync(string clientRequestId)
    {
        try
        {
            var statusResponse = await _requestLogService.GetStatusAsync(clientRequestId);
            var log = await _requestLogService.FindByClientRequestIdAsync(clientRequestId);

            FacadeConsentDetails? consentDetails = null;
            if (log != null)
            {
                var consentId = log.ConsentId;
                if (string.IsNullOrEmpty(consentId) || consentId == "CONSENT_ON_NOTIFY_RESPONSE" || consentId == "CONSENT_ON_STATUS_RESPONSE")
                {
                    if (log.RequestDetails != null && log.RequestDetails.Contains("consentDetails"))
                    {
                        var details = log.RequestDetails["consentDetails"];
                        if (details.IsBsonDocument)
                        {
                            var detailsDoc = details.AsBsonDocument;
                            var notifKey = detailsDoc.Contains("notification") ? "notification" : (detailsDoc.Contains("Notification") ? "Notification" : null);
                            if (notifKey != null && detailsDoc[notifKey].IsBsonDocument)
                            {
                                var notif = detailsDoc[notifKey].AsBsonDocument;
                                var artKey = notif.Contains("consentArtefacts") ? "consentArtefacts" : (notif.Contains("ConsentArtefacts") ? "ConsentArtefacts" : null);
                                if (artKey != null && notif[artKey].IsBsonArray)
                                {
                                    var arr = notif[artKey].AsBsonArray;
                                    if (arr.Count > 0 && arr[0].IsBsonDocument)
                                    {
                                        var art = arr[0].AsBsonDocument;
                                        var idKey = art.Contains("id") ? "id" : (art.Contains("Id") ? "Id" : null);
                                        if (idKey != null)
                                        {
                                            consentId = art[idKey].ToString();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var artKeyDirect = detailsDoc.Contains("consentArtefacts") ? "consentArtefacts" : (detailsDoc.Contains("ConsentArtefacts") ? "ConsentArtefacts" : null);
                                if (artKeyDirect != null && detailsDoc[artKeyDirect].IsBsonArray)
                                {
                                    var arr = detailsDoc[artKeyDirect].AsBsonArray;
                                    if (arr.Count > 0 && arr[0].IsBsonDocument)
                                    {
                                        var art = arr[0].AsBsonDocument;
                                        var idKey = art.Contains("id") ? "id" : (art.Contains("Id") ? "Id" : null);
                                        if (idKey != null)
                                        {
                                            consentId = art[idKey].ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(consentId) && consentId != "CONSENT_ON_NOTIFY_RESPONSE" && consentId != "CONSENT_ON_STATUS_RESPONSE")
                {
                    consentDetails = new FacadeConsentDetails
                    {
                        Consent = new List<ConsentStatus>
                        {
                            new ConsentStatus
                            {
                                Status = log.Status,
                                ConsentArtefacts = new List<ConsentArtefact>
                                {
                                    new ConsentArtefact { Id = consentId }
                                }
                            }
                        }
                    };
                }
            }

            return new ConsentStatusV3Response
            {
                Status = statusResponse.Status,
                ConsentDetails = consentDetails,
                Errors = statusResponse.Errors != null
                    ? statusResponse.Errors.ConvertAll(e => new ErrorV3Response { Error = e })
                    : null,
                HttpStatusCode = statusResponse.Errors != null ? 400 : 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception fetching consent status for {clientRequestId}");
            return new ConsentStatusV3Response
            {
                Errors = new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = ex.Message } } },
                HttpStatusCode = 500
            };
        }
    }

    // ─── Gateway Callback Handlers ────────────────────────────────────────────

    /// <summary>
    /// Handles Gateway callback: /v3/hiu/consent/request/on-init
    /// Saves the consentRequestId ↔ gatewayRequestId mapping for later lookups.
    /// </summary>
    public async Task<int> OnInitConsentAsync(ConsentOnInitV3Request request, string hiuId)
    {
        if (request?.ConsentRequest != null && request.Response != null)
        {
            // Map consentRequestId → original gatewayRequestId for future callbacks
            await _consentRequestMappingService.SaveAsync(
                request.ConsentRequest.Id,
                request.Response.RequestId);

            await _requestLogService.UpdateConsentResponseAsync(
                request.Response.RequestId,
                "CONSENT_ON_INIT_RESPONSE",
                RequestStatus.CONSENT_ON_INIT_RESPONSE_RECEIVED,
                request.ConsentRequest.Id);

            return 202;
        }
        else if (request?.Error != null && request.Response != null)
        {
            await _requestLogService.UpdateErrorAsync(
                request.Response.RequestId,
                new List<ErrorV3Response> { new() { Error = request.Error } },
                RequestStatus.CONSENT_ON_INIT_ERROR);
        }
        else if (request?.Response != null)
        {
            await _requestLogService.UpdateErrorAsync(
                request.Response.RequestId,
                new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = "Something went wrong in consent on-init" } } },
                RequestStatus.CONSENT_ON_INIT_ERROR);
            return 400;
        }

        return 202;
    }

    /// <summary>
    /// Handles Gateway callback: /v3/hiu/consent/request/on-status
    /// </summary>
    public async Task<int> ConsentOnStatusAsync(HIUConsentOnStatusV3Request request)
    {
        if (request?.ConsentRequest != null && request.Response != null)
        {
            var gatewayRequestId = await _consentRequestMappingService.GetGatewayRequestIdAsync(
                request.Response.RequestId);

            if (gatewayRequestId == null)
            {
                _logger.LogError("No gateway request ID found for on-status response");
                return 400;
            }

            var consentId = "CONSENT_ON_STATUS_RESPONSE";
            if (request.ConsentRequest.ConsentArtefacts != null && request.ConsentRequest.ConsentArtefacts.Count > 0)
            {
                consentId = request.ConsentRequest.ConsentArtefacts[0].Id;
            }

            await _requestLogService.UpdateConsentResponseAsync(
                gatewayRequestId,
                consentId,
                RequestStatus.CONSENT_ON_STATUS_RESPONSE_RECEIVED,
                request.ConsentRequest);

            return 202;
        }

        _logger.LogError("Something went wrong while executing consent on-status");
        return 400;
    }

    /// <summary>
    /// Handles Gateway callback: /v3/hiu/consent/request/notify
    /// </summary>
    public async Task<int> HiuNotifyAsync(NotifyHIURequest request, string hiuId, string incomingRequestId)
    {
        _logger.LogInformation($"HiuNotifyAsync raw request: {System.Text.Json.JsonSerializer.Serialize(request)}");

        if (request?.Notification == null || request.Error != null)
        {
            if (request?.Error != null && request.Notification != null)
            {
                var gwReqId = await _consentRequestMappingService.GetGatewayRequestIdAsync(
                    request.Notification.ConsentRequestId);
                if (gwReqId != null)
                {
                    await _requestLogService.UpdateErrorAsync(gwReqId,
                        new List<ErrorV3Response> { new() { Error = request.Error } },
                        RequestStatus.CONSENT_NOTIFY_ERROR);
                }
            }

            _logger.LogError($"Something went wrong while executing hiu notify. Notification is null: {request?.Notification == null}. Error is null: {request?.Error == null}");
            return 400;
        }

        var notification = request.Notification;
        var gatewayRequestId = await _consentRequestMappingService.GetGatewayRequestIdAsync(
            notification.ConsentRequestId);

        if (notification.Status.Equals("DENIED", StringComparison.OrdinalIgnoreCase))
        {
            if (gatewayRequestId != null)
            {
                await _requestLogService.UpdateConsentResponseAsync(gatewayRequestId,
                    "CONSENT_ON_NOTIFY_RESPONSE",
                    RequestStatus.CONSENT_ON_NOTIFY_RESPONSE_RECEIVED,
                    request);
            }
        }
        else if (!notification.Status.Equals("GRANTED", StringComparison.OrdinalIgnoreCase))
        {
            // REVOKED / EXPIRED – update consent status on patient
            foreach (var artefact in notification.ConsentArtefacts)
            {
                var mapping = await _consentPatientService.FindMappingByConsentIdAsync(artefact.Id, "HIU", hiuId);
                if (mapping?.AbhaAddress != null)
                {
                    await _patientService.UpdatePatientConsentAsync(
                        mapping.AbhaAddress, artefact.Id, notification.Status,
                        request.Timestamp, hiuId);
                }
            }
        }
        else
        {
            // GRANTED – fetch full consent artifacts from gateway
            if (gatewayRequestId != null)
            {
                var consentId = "CONSENT_ON_NOTIFY_RESPONSE";
                if (notification.ConsentArtefacts != null && notification.ConsentArtefacts.Count > 0)
                {
                    consentId = notification.ConsentArtefacts[0].Id;
                }
                await _requestLogService.UpdateConsentResponseAsync(gatewayRequestId,
                    consentId,
                    RequestStatus.CONSENT_ON_NOTIFY_RESPONSE_RECEIVED,
                    request);
            }

            // Trigger fetch-consent for each artefact (fire & forget style)
            foreach (var artefact in notification.ConsentArtefacts)
            {
                _ = FetchConsentFromGatewayAsync(artefact.Id, hiuId, incomingRequestId);
            }
        }

        // Acknowledge the gateway notification
        var onNotifyReq = new ConsentOnNotifyV3Request
        {
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = Utils.GetCurrentTimeStamp(),
            Response = new RespRequest { RequestId = incomingRequestId },
            Acknowledgement = notification.ConsentArtefacts.ConvertAll(a =>
                new ConsentAcknowledgementItem { Status = "OK", ConsentId = a.Id })
        };

        await _gatewayClient.PostToGatewayAsync(
            _config.Gateway?.ConsentHiuOnNotifyPath ?? "api/v3/hiu/consent/request/on-notify",
            onNotifyReq,
            new Dictionary<string, string>
            {
                ["X-HIU-ID"] = hiuId,
                ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
            });

        return 202;
    }

    /// <summary>
    /// Handles Gateway callback: /v3/hiu/consent/on-fetch
    /// Saves the full consent detail to patient record.
    /// </summary>
    public async Task<int> ConsentOnFetchAsync(OnFetchV3Request request, string hiuId)
    {
        if (request?.Consent?.ConsentDetail == null)
        {
            return 400;
        }

        try
        {
            var patientId = request.Consent.ConsentDetail.Patient?.Id ?? string.Empty;
            var consentId = request.Consent.ConsentDetail.ConsentId;

            var consent = new Consent
            {
                LastUpdatedOn = Utils.GetCurrentTimeStamp(),
                Status = request.Consent.Status,
                ConsentDetail = request.Consent.ConsentDetail,
                GrantedOn = Utils.GetCurrentTimeStamp()
            };

            await _patientService.AddConsentAsync(patientId, consent, hiuId);

            await _consentPatientService.SaveConsentPatientMappingAsync(
                consentId, patientId, "HIU", hiuId);

            return 202;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing consent on-fetch");
            if (request.Response != null)
            {
                await _requestLogService.UpdateErrorAsync(
                    request.Response.RequestId,
                    new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = ex.Message } } },
                    RequestStatus.CONSENT_ON_FETCH_ERROR);
            }
            return 400;
        }
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private async Task FetchConsentFromGatewayAsync(string consentId, string hiuId, string incomingRequestId)
    {
        try
        {
            var fetchRequest = new { consentId };
            await _gatewayClient.PostToGatewayAsync(
                _config.Gateway?.FetchConsentPath ?? "api/v3/consent/fetch",
                fetchRequest,
                new Dictionary<string, string>
                {
                    ["X-HIU-ID"] = hiuId,
                    ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching consent {consentId} from gateway");
        }
    }
}
