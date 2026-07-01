using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;
using MongoDB.Bson;

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
            if (string.IsNullOrEmpty(hiuId))
            {
                hiuId = !string.IsNullOrEmpty(_config.HiuSetup?.HiuId) ? _config.HiuSetup.HiuId : _config.HipId;
            }

            _logger.LogInformation($"Initiating health information request for consent: {clientRequest.ConsentId} with HIU-ID: {hiuId}");

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

            // Save key material in request details log so we can decrypt pushed data later
            var requestLogDetails = new BsonDocument
            {
                { "privateKey", hiuKeys.PrivateKey },
                { "publicKey", hiuKeys.PublicKey },
                { "nonce", hiuKeys.Nonce }
            };
            await _requestLogService.SaveHiuHealthInformationRequestAsync(
                clientRequest.RequestId,
                clientRequest.ConsentId,
                "HIU",
                hiuId,
                RequestStatus.INITIATING.ToString(),
                requestLogDetails);

            string dataPushUrl = _config.HiuSetup?.DataPushUrl ?? string.Empty;
            if (string.IsNullOrEmpty(dataPushUrl) || dataPushUrl.Contains("localhost") || dataPushUrl.Contains("127.0.0.1") || !dataPushUrl.StartsWith("https://"))
            {
                dataPushUrl = "https://sbx.wati.digital/v3/hiu/health-information/transfer";
            }

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
                    dataPushUrl = dataPushUrl,
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
                _config.Gateway?.HealthInformationConsentManagerPath ?? "api/v3/health-information/request",
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
            var log = await _requestLogService.FindByClientRequestIdAsync(requestId);
            if (log == null)
            {
                return new HealthInformationV3Response
                {
                    Errors = new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = $"Request not found in database for: {requestId}" } } },
                    HttpStatusCode = 404
                };
            }

            var statusResponse = await _requestLogService.GetStatusAsync(requestId);
            var response = new HealthInformationV3Response
            {
                Status = statusResponse.Status,
                Errors = statusResponse.Errors != null
                    ? statusResponse.Errors.ConvertAll(e => new ErrorV3Response { Error = e })
                    : null,
                HttpStatusCode = statusResponse.Errors != null ? 400 : 200
            };

            // Check if we have received encrypted health data
            if (log.ResponseDetails != null && log.ResponseDetails.Contains("entries"))
            {
                var pushRequestJson = log.ResponseDetails.ToString();
                var pushRequest = System.Text.Json.JsonSerializer.Deserialize<HealthInformationPushRequest>(pushRequestJson);

                if (pushRequest != null && pushRequest.Entries != null && pushRequest.KeyMaterial != null)
                {
                    string? hiuPrivateKey = null;
                    string? hiuNonce = null;
                    if (log.RequestDetails != null)
                    {
                        if (log.RequestDetails.Contains("privateKey")) hiuPrivateKey = log.RequestDetails["privateKey"].AsString;
                        if (log.RequestDetails.Contains("nonce")) hiuNonce = log.RequestDetails["nonce"].AsString;
                    }

                    string? hipPublicKey = pushRequest.KeyMaterial.DhPublicKey?.KeyValue;
                    string? hipNonce = pushRequest.KeyMaterial.Nonce;

                    if (!string.IsNullOrEmpty(hiuPrivateKey) && !string.IsNullOrEmpty(hiuNonce) &&
                        !string.IsNullOrEmpty(hipPublicKey) && !string.IsNullOrEmpty(hipNonce))
                    {
                        var cryptoService = new CryptographyService(Microsoft.Extensions.Logging.Abstractions.NullLogger<CryptographyService>.Instance);
                        var decryptedEntries = new List<DecryptedEntry>();

                        foreach (var entry in pushRequest.Entries)
                        {
                            try
                            {
                                var decryptedContent = cryptoService.Decrypt(hipNonce, hiuNonce, hiuPrivateKey, hipPublicKey, entry.Content);
                                
                                object? fhirBundleObj = null;
                                try
                                {
                                    fhirBundleObj = System.Text.Json.JsonSerializer.Deserialize<object>(decryptedContent);
                                }
                                catch
                                {
                                    fhirBundleObj = decryptedContent;
                                }

                                decryptedEntries.Add(new DecryptedEntry
                                {
                                    CareContextReference = entry.CareContextReference,
                                    FhirBundle = fhirBundleObj
                                });
                            }
                            catch (Exception dex)
                            {
                                _logger.LogError(dex, $"Failed to decrypt entry for CareContext: {entry.CareContextReference}");
                            }
                        }

                        response.DecryptedHealthInformation = decryptedEntries;
                    }
                    else
                    {
                        _logger.LogWarning($"Decryption keys or nonces missing for requestId: {requestId}");
                    }
                }
            }

            return response;
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

        // Save the pushed encrypted health data into ResponseDetails
        var jsonStr = System.Text.Json.JsonSerializer.Serialize(pushRequest);
        var bsonDoc = BsonDocument.Parse(jsonStr);
        await _requestLogService.SaveResponseDetailsAsync(pushRequest.TransactionId, bsonDoc);

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
                _config.Gateway?.HealthInformationPushNotificationPath ?? "api/v3/health-information/notify",
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
