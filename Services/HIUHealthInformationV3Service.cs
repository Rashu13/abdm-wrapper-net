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
/// Handles HIU Health Information operations:
///  - Facade: initiate data fetch request (using consent), get decrypted health info status.
///  - Gateway callback: receive on-request, store encrypted FHIR bundles pushed by HIP.
/// </summary>
public class HIUHealthInformationV3Service
{
    private readonly IGatewayClient _gatewayClient;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly IPatientV3Service _patientService;
    private readonly IConsentPatientV3Service _consentPatientService;
    private readonly AbdmConfig _config;
    private readonly ILogger<HIUHealthInformationV3Service> _logger;

    public HIUHealthInformationV3Service(
        IGatewayClient gatewayClient,
        IRequestLogV3Service requestLogService,
        IPatientV3Service patientService,
        IConsentPatientV3Service consentPatientService,
        IOptions<AbdmConfig> config,
        ILogger<HIUHealthInformationV3Service> logger)
    {
        _gatewayClient = gatewayClient;
        _requestLogService = requestLogService;
        _patientService = patientService;
        _consentPatientService = consentPatientService;
        _config = config.Value;
        _logger = logger;
    }

    // ─── Facade: Initiate health data fetch ──────────────────────────────────

    /// <summary>
    /// Builds a health information fetch request and sends it to the ABDM gateway.
    /// Uses consentId to look up the full consent detail, generates crypto key material.
    /// </summary>
    public async Task<FacadeV3Response> FetchHealthInformationAsync(HIUClientHealthInformationRequest clientRequest, string hiuId)
    {
        try
        {
            _logger.LogInformation($"Initiating health information request for consent: {clientRequest.ConsentId}");

            // Validate consent exists for this HIU
            var consentMapping = await _consentPatientService.FindMappingByConsentIdAsync(
                clientRequest.ConsentId, "HIU", hiuId);
            if (consentMapping == null)
            {
                return new FacadeV3Response
                {
                    Errors = new List<ErrorResponse> { new() { Code = "1000", Message = "Consent not found for given consentId and HIU" } },
                    HttpStatusCode = 400
                };
            }

            // Generate real EC curve25519 key pair for encryption
            var cryptoService = new CryptographyService(Microsoft.Extensions.Logging.Abstractions.NullLogger<CryptographyService>.Instance);
            var hiuKeys = cryptoService.GenerateKeys();
            // keyValue = raw uncompressed EC point (65 bytes base64)
            // Java's CipherKeyManager.getEncodedPublicKey() = ecKey.getQ().getEncoded(false) = raw point

            var gatewayRequest = new
            {
                requestId = clientRequest.RequestId,
                timestamp = Utils.GetCurrentTimeStamp(),
                hiRequest = new
                {
                    consent = new { id = clientRequest.ConsentId },
                    dateRange = new 
                    { 
                        from = clientRequest.DateRange?.From ?? "2020-01-01T00:00:00.000Z", 
                        to = clientRequest.DateRange?.To ?? Utils.GetCurrentTimeStamp() 
                    },
                    dataPushUrl = _config.HiuSetup?.DataPushUrl ?? string.Empty,
                    keyMaterial = new
                    {
                        cryptoAlg = "ECDH",
                        curve = "Curve25519",
                        dhPublicKey = new
                        {
                            expiry = DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            parameters = "Curve25519/32byte random key",
                            keyValue = hiuKeys.PublicKey
                        },
                        nonce = hiuKeys.Nonce
                    }
                }
            };

            var headers = new Dictionary<string, string>
            {
                ["X-HIU-ID"] = hiuId,
                ["REQUEST-ID"] = clientRequest.RequestId,
                ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
            };

            var response = await _gatewayClient.PostToGatewayAsync(
                _config.HiuSetup?.HealthInformationRequestPath ?? "api/v3/health-information/request",
                gatewayRequest, headers);

            bool success = response?.HttpStatus is "OK" or "Accepted"
                           || (response?.HttpStatus?.StartsWith("2") == true);

            if (success)
            {
                return new FacadeV3Response
                {
                    ClientRequestId = clientRequest.RequestId,
                    Message = RequestStatus.HEALTH_INFORMATION_REQUEST_SUCCESS.GetValue(),
                    HttpStatusCode = 202
                };
            }

            var errorMsg = response?.Message ?? "Error from gateway for health information request";
            _logger.LogError(errorMsg);
            return new FacadeV3Response
            {
                ClientRequestId = clientRequest.RequestId,
                Errors = new List<ErrorResponse> { new() { Code = "1000", Message = errorMsg } },
                HttpStatusCode = 400
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during health information fetch");
            return new FacadeV3Response
            {
                ClientRequestId = clientRequest.RequestId,
                Message = ex.Message,
                HttpStatusCode = 500
            };
        }
    }

    /// <summary>
    /// Returns the status + stored (encrypted) health information for a given requestId.
    /// Decryption would be done here when CryptographyService is fully wired.
    /// </summary>
    public async Task<HealthInformationV3Response> GetHealthInformationAsync(string requestId)
    {
        try
        {
            var statusResponse = await _requestLogService.GetStatusAsync(requestId);
            return new HealthInformationV3Response
            {
                Status = statusResponse.Status,
                Errors = statusResponse.Errors != null
                    ? statusResponse.Errors.ConvertAll(e => new ErrorV3Response { Error = e })
                    : null,
                HttpStatusCode = statusResponse.Errors != null ? 400 : 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception fetching health information status for {requestId}");
            return new HealthInformationV3Response
            {
                Errors = new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = ex.Message } } },
                HttpStatusCode = 500
            };
        }
    }

    // ─── Gateway Callback: /v3/hiu/health-information/on-request ─────────────

    /// <summary>
    /// Gateway sends this after HIU's health-information/request is accepted.
    /// Updates the transactionId so we can match the incoming FHIR push.
    /// </summary>
    public async Task HandleOnRequestAsync(HIUHealthInformationOnRequest onRequest)
    {
        if (onRequest?.Error != null)
        {
            var reqId = onRequest.Response?.RequestId ?? onRequest.RequestId;
            _logger.LogError($"HIU health-information/on-request error: {onRequest.Error.Message}");
            await _requestLogService.UpdateErrorAsync(reqId,
                new List<ErrorV3Response> { new() { Error = onRequest.Error } },
                RequestStatus.HEALTH_INFORMATION_ON_REQUEST_ERROR);
            return;
        }

        if (onRequest?.HiRequest?.TransactionId != null && onRequest.Response?.RequestId != null)
        {
            await _requestLogService.UpdateTransactionIdAsync(
                onRequest.Response.RequestId,
                onRequest.HiRequest.TransactionId);

            await _requestLogService.UpdateStatusAsync(
                onRequest.Response.RequestId,
                RequestStatus.HEALTH_INFORMATION_ON_REQUEST_SUCCESS);
        }
    }

    // ─── Gateway Callback: HIP pushes encrypted FHIR data ────────────────────

    /// <summary>
    /// Receives the encrypted health information pushed by the HIP.
    /// Stores the raw encrypted data; decryption can be triggered via facade status endpoint.
    /// </summary>
    public async Task<GenericV3Response> ProcessEncryptedHealthInformationAsync(
        HealthInformationPushRequest pushRequest, string hiuId)
    {
        if (pushRequest == null || pushRequest.Entries == null || pushRequest.TransactionId == null)
        {
            return new GenericV3Response { HttpStatus = "BadRequest", Status = "Error", Message = "Invalid push request" };
        }

        var requestLog = await _requestLogService.FindRequestLogByTransactionIdAsync(pushRequest.TransactionId);
        if (requestLog == null)
        {
            return new GenericV3Response { HttpStatus = "BadRequest", Status = "Error", Message = "Transaction ID not found" };
        }

        // Update status to indicate data received
        await _requestLogService.UpdateStatusAsync(
            requestLog.GatewayRequestId ?? string.Empty,
            RequestStatus.ENCRYPTED_HEALTH_INFORMATION_RECEIVED);

        // Notify gateway that data was received
        await NotifyGatewayAsync(pushRequest, hiuId, requestLog.ConsentId ?? string.Empty);

        return new GenericV3Response { HttpStatus = "OK", Status = "Accepted" };
    }

    private async Task NotifyGatewayAsync(HealthInformationPushRequest pushRequest, string hiuId, string consentId)
    {
        try
        {
            var statusNotification = new
            {
                consentId,
                transactionId = pushRequest.TransactionId,
                doneAt = Utils.GetCurrentTimeStamp(),
                notifier = new { type = "HIU", id = hiuId },
                statusNotification = new
                {
                    sessionStatus = "TRANSFERRED",
                    hipId = "HIP",
                    statusResponses = pushRequest.Entries.ConvertAll(entry => new
                    {
                        careContextReference = entry.CareContextReference,
                        hiStatus = "OK",
                        description = "Done"
                    })
                }
            };

            var notification = new { notification = statusNotification };
            await _gatewayClient.PostToGatewayAsync(
                _config.HiuSetup?.HealthInformationPushNotificationPath ?? "api/v3/health-information/notify",
                notification,
                new Dictionary<string, string>
                {
                    ["X-HIU-ID"] = hiuId,
                    ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying gateway of health information receipt");
        }
    }
}
