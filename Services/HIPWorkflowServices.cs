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
    private readonly IPatientV3Service _patientService;
    private readonly AbdmConfig _config;
    private readonly ILogger<ConsentV3Service> _logger;

    public ConsentV3Service(
        IGatewayClient gateway,
        IRequestLogV3Service requestLogService,
        IPatientV3Service patientService,
        IOptions<AbdmConfig> config,
        ILogger<ConsentV3Service> logger)
    {
        _gateway = gateway;
        _requestLogService = requestLogService;
        _patientService = patientService;
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

            if (request.Notification != null)
            {
                var consentId = request.Notification.ConsentId;
                var status = request.Notification.Status;
                var abhaAddress = request.Notification.ConsentDetail?.Patient?.Id;

                if (!string.IsNullOrEmpty(abhaAddress))
                {
                    if (status.Equals("GRANTED", StringComparison.OrdinalIgnoreCase))
                    {
                        var consent = new Consent
                        {
                            Status = status,
                            LastUpdatedOn = Utils.GetCurrentTimeStamp(),
                            GrantedOn = Utils.GetCurrentTimeStamp(),
                            ConsentDetail = request.Notification.ConsentDetail,
                            Signature = request.Notification.Signature ?? string.Empty
                        };
                        await _patientService.AddConsentAsync(abhaAddress, consent, hipId.ToString());
                    }
                    else
                    {
                        await _patientService.UpdatePatientConsentAsync(abhaAddress, consentId, status, Utils.GetCurrentTimeStamp(), hipId.ToString());
                    }
                }
            }

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
    private readonly IFhirMapperService _fhirMapperService;
    private readonly ILogger<HIPHealthInformationV3Service> _logger;

    public HIPHealthInformationV3Service(
        IRequestLogV3Service requestLogService,
        IPatientV3Service patientService,
        ICryptographyService cryptographyService,
        IFhirMapperService fhirMapperService,
        ILogger<HIPHealthInformationV3Service> logger)
    {
        _requestLogService = requestLogService;
        _patientService = patientService;
        _cryptographyService = cryptographyService;
        _fhirMapperService = fhirMapperService;
        _logger = logger;
    }

    /// <summary>
    /// Gateway sends health-information request → HIP fetches FHIR data from its HIS and pushes to HIU.
    /// </summary>
    public async Task HealthInformationAsync(HIPHealthInformationRequest request, IHeaderDictionary headers)
    {
        try
        {
            _logger.LogInformation($"Health information request received. TransactionId: {request.TransactionId}");

            // Persist the incoming health information request for tracking
            await _requestLogService.SaveHealthInformationRequestAsync(
                request, RequestStatus.HEALTH_INFORMATION_REQUEST_SUCCESS);

            if (request.HiRequest?.Consent?.Id == null)
            {
                _logger.LogError("Consent ID is missing from the Health Information Request");
                return;
            }

            var consentId = request.HiRequest.Consent.Id;
            var patient = await _patientService.GetPatientByConsentIdAsync(consentId);
            if (patient == null)
            {
                _logger.LogError($"No patient found with granted consent ID: {consentId}");
                return;
            }

            var consent = patient.Consents?.FirstOrDefault(c => c.ConsentDetail != null && c.ConsentDetail.ConsentId == consentId);
            if (consent?.ConsentDetail?.CareContexts == null || !consent.ConsentDetail.CareContexts.Any())
            {
                _logger.LogInformation("No care contexts approved for this consent.");
                return;
            }

            var careContexts = consent.ConsentDetail.CareContexts;
            var healthInformationBundles = new List<HealthInformationBundle>();
            var abhaAddress = patient.AbhaAddress;
            
            using var httpClient = new System.Net.Http.HttpClient();

            foreach (var cc in careContexts)
            {
                var record = await _patientService.GetHealthDataRecordAsync(abhaAddress, cc.CareContextReference ?? "");
                if (record != null && !string.IsNullOrEmpty(record.FhirJsonPayload))
                {
                    try 
                    {
                        // Use native .NET C# FHIR Mapper!
                        string fhirBundleStr;
                        if (record.RecordType != null && (record.RecordType.Equals("OPConsultationRecord", StringComparison.OrdinalIgnoreCase) || record.RecordType.Equals("OPConsultation", StringComparison.OrdinalIgnoreCase)))
                        {
                            fhirBundleStr = await _fhirMapperService.GenerateOPConsultationBundleAsync(record.FhirJsonPayload);
                        }
                        else if (record.RecordType != null && (record.RecordType.Equals("HealthDocumentRecord", StringComparison.OrdinalIgnoreCase) || record.RecordType.Equals("HealthDocument", StringComparison.OrdinalIgnoreCase)))
                        {
                            fhirBundleStr = await _fhirMapperService.GenerateHealthDocumentBundleAsync(record.FhirJsonPayload);
                        }
                        else if (record.RecordType != null && (record.RecordType.Equals("DiagnosticReport", StringComparison.OrdinalIgnoreCase) || record.RecordType.Equals("DiagnosticReportRecord", StringComparison.OrdinalIgnoreCase)))
                        {
                            fhirBundleStr = await _fhirMapperService.GenerateDiagnosticReportBundleAsync(record.FhirJsonPayload);
                        }
                        else if (record.RecordType != null && (record.RecordType.Equals("DischargeSummary", StringComparison.OrdinalIgnoreCase) || record.RecordType.Equals("DischargeSummaryRecord", StringComparison.OrdinalIgnoreCase)))
                        {
                            fhirBundleStr = await _fhirMapperService.GenerateDischargeSummaryBundleAsync(record.FhirJsonPayload);
                        }
                        else
                        {
                            fhirBundleStr = await _fhirMapperService.GeneratePrescriptionBundleAsync(record.FhirJsonPayload);
                        }
                        
                        healthInformationBundles.Add(new HealthInformationBundle
                        {
                            CareContextReference = cc.CareContextReference,
                            BundleContent = fhirBundleStr
                        });
                        _logger.LogInformation($"Successfully generated C# FHIR bundle for {cc.CareContextReference}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Could not map FHIR in .NET Core for {cc.CareContextReference}. Exception: {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogWarning($"No health data record found in DB for care context: {cc.CareContextReference}");
                }
            }

            if (!healthInformationBundles.Any())
            {
                _logger.LogWarning("No FHIR bundles could be generated. Aborting push.");
                return;
            }

            // 3. Encrypt bundles using keyMaterial from the request
            var encryptionResponse = _cryptographyService.Encrypt(request, healthInformationBundles);

            // 4. POST encrypted bundles to HIU's DataPushUrl
            var dataPushPayload = new
            {
                pageNumber = 1,
                pageCount = 1,
                transactionId = request.TransactionId,
                entries = encryptionResponse.HealthInformationBundles.Select(b => new
                {
                    content = b.BundleContent,
                    media = "application/fhir+json",
                    checksum = "string",
                    careContextReference = b.CareContextReference
                }).ToList(),
                keyMaterial = new
                {
                    cryptoAlg = request.HiRequest?.KeyMaterial?.CryptoAlg ?? "ECDH",
                    curve = request.HiRequest?.KeyMaterial?.Curve ?? "Curve25519",
                    dhPublicKey = new
                    {
                        expiry = request.HiRequest?.KeyMaterial?.DhPublicKey?.Expiry,
                        parameters = request.HiRequest?.KeyMaterial?.DhPublicKey?.Parameters ?? "Curve25519",
                        keyValue = encryptionResponse.KeyToShare
                    },
                    nonce = encryptionResponse.SenderNonce
                }
            };

            var dataPushUrl = request.HiRequest?.DataPushUrl;
            if (!string.IsNullOrEmpty(dataPushUrl))
            {
                _logger.LogInformation($"Pushing encrypted data to HIU at {dataPushUrl}");
                var jsonOptions = new System.Text.Json.JsonSerializerOptions 
                { 
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                };
                var pushContent = new System.Net.Http.StringContent(
                    System.Text.Json.JsonSerializer.Serialize(dataPushPayload, jsonOptions), 
                    System.Text.Encoding.UTF8, "application/json");
                    
                var pushResponse = await httpClient.PostAsync(dataPushUrl, pushContent);
                var responseContent = await pushResponse.Content.ReadAsStringAsync();
                _logger.LogInformation($"Data push response: {pushResponse.StatusCode}, Content: {responseContent}");
            }
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

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (string Token, DateTime Expiry)> _tokenCache = new();
    private static int _tokenCounter = 0;
    private static DateTime _lastResetDate = DateTime.Today;

    private string GetOrGenerateToken(string abhaAddress, string hipId, string context, out string expiryStr)
    {
        lock (_tokenCache)
        {
            if (DateTime.Today > _lastResetDate)
            {
                _tokenCounter = 0;
                _tokenCache.Clear();
                _lastResetDate = DateTime.Today;
            }

            string cacheKey = $"{hipId}:{abhaAddress}:{context}";
            if (_tokenCache.TryGetValue(cacheKey, out var cached) && DateTime.UtcNow < cached.Expiry)
            {
                // ABDM requires expiry as duration in seconds (e.g. "1800" for 30 minutes)
                var remaining = (cached.Expiry - DateTime.UtcNow).TotalSeconds;
                expiryStr = Math.Max(60, (int)remaining).ToString();
                return cached.Token;
            }

            _tokenCounter++;
            string tokenNum = _tokenCounter.ToString("D4"); // "0001", "0002" etc.
            DateTime expiry = DateTime.UtcNow.AddMinutes(30);
            _tokenCache[cacheKey] = (tokenNum, expiry);

            // ABDM requires expiry as duration in seconds (e.g. "1800" for 30 minutes)
            expiryStr = "1800";
            return tokenNum;
        }
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

            string context = request.MetaData?.Context ?? "1";
            string abhaAddress = request.Profile?.Patient?.AbhaAddress ?? string.Empty;
            string tokenNum = GetOrGenerateToken(abhaAddress, hipId.ToString(), context, out string expiryStr);

            // Send on-share acknowledgement to gateway conforming to V3 Schema
            var onShare = new
            {
                response = new { requestId = incomingRequestId.ToString() },
                acknowledgement = new
                {
                    status = "SUCCESS",
                    abhaAddress = abhaAddress,
                    profile = new
                    {
                        context = context,
                        tokenNumber = tokenNum,
                        expiry = expiryStr
                    }
                }
            };

            // Persist the scan & share request
            await _requestLogService.SaveScanAndShareDetailsAsync(request, onShare);

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
