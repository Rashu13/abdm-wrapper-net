using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class SqlServerRequestLogV3Service : IRequestLogV3Service
{
    private readonly AppDbContext _context;
    private readonly IPatientV3Service _patientService;
    private readonly ILogger<SqlServerRequestLogV3Service> _logger;

    public SqlServerRequestLogV3Service(AppDbContext context, IPatientV3Service patientService, ILogger<SqlServerRequestLogV3Service> logger)
    {
        _context = context;
        _patientService = patientService;
        _logger = logger;
    }

    private BsonDocument ToBsonDocument(object? obj)
    {
        if (obj == null) return new BsonDocument();
        if (obj is BsonDocument doc) return doc;
        var json = JsonSerializer.Serialize(obj);
        return BsonSerializer.Deserialize<BsonDocument>(json);
    }

    private BsonValue ToBsonValue(object? obj)
    {
        if (obj == null) return BsonNull.Value;
        if (obj is BsonValue value) return value;
        var json = JsonSerializer.Serialize(obj);
        return BsonSerializer.Deserialize<BsonValue>(json);
    }

    public async Task UpdateConsentStatusAsync(string requestId, RequestStatus status)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.GatewayRequestId == requestId);
        if (log == null)
        {
            throw new Exception($"Request not found for request id: {requestId}");
        }

        log.Status = status.ToString();
        log.LastUpdated = DateTime.UtcNow;
        _context.RequestLogs.Update(log);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateErrorAsync(string requestId, object error, RequestStatus requestStatus)
    {
        await UpdateErrorAsync(requestId, error, requestStatus.ToString());
    }

    public async Task UpdateErrorAsync(string requestId, object error, string status)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.GatewayRequestId == requestId);
        if (log != null)
        {
            log.Error = error;
            log.Status = status;
            log.LastUpdated = DateTime.UtcNow;
            _context.RequestLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateErrorLinkErrorAsync(string linkTokenRequestId, object error, RequestStatus requestStatus)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.LinkTokenRequestId == linkTokenRequestId);
        if (log != null)
        {
            log.Error = error;
            log.Status = requestStatus.ToString();
            log.LastUpdated = DateTime.UtcNow;
            _context.RequestLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }

    public async Task LinkTokenUpdateErrorAsync(string requestId, object error, RequestStatus requestStatus)
    {
        await LinkTokenUpdateErrorAsync(requestId, error, requestStatus.ToString());
    }

    public async Task LinkTokenUpdateErrorAsync(string requestId, object error, string status)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.LinkTokenRequestId == requestId);
        if (log != null)
        {
            log.Error = error;
            log.Status = status;
            log.LastUpdated = DateTime.UtcNow;
            _context.RequestLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateStatusAsync(string requestId, RequestStatus requestStatus)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.GatewayRequestId == requestId);
        if (log != null)
        {
            log.Status = requestStatus.GetValue();
            log.LastUpdated = DateTime.UtcNow;
            _context.RequestLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateConsentResponseAsync<T>(string requestId, string identifier, RequestStatus requestStatus, T consentDetails)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.GatewayRequestId == requestId);

        if (log == null)
        {
            log = new RequestLog
            {
                GatewayRequestId = requestId,
                Module = "HIP_CONSENT",
                CreatedOn = DateTime.UtcNow
            };
            await _context.RequestLogs.AddAsync(log);
        }

        log.Status = requestStatus.GetValue();
        log.ConsentId = identifier;
        log.LastUpdated = DateTime.UtcNow;

        var requestDetails = log.RequestDetails ?? new BsonDocument();
        requestDetails["consentDetails"] = ToBsonValue(consentDetails);
        log.RequestDetails = requestDetails;

        _context.RequestLogs.Update(log);
        await _context.SaveChangesAsync();
    }

    public async Task<RequestLog?> FindRequestLogByTransactionIdAsync(string transactionId)
    {
        return await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.TransactionId == transactionId);
    }

    public async Task PersistHipLinkRequestAsync(LinkRecordsV3Request linkRecordsRequest, RequestStatus status, object? errors)
    {
        var log = new RequestLog
        {
            Module = "HIP_LINKING",
            AbhaAddress = linkRecordsRequest.AbhaAddress,
            ClientRequestId = linkRecordsRequest.RequestId,
            GatewayRequestId = linkRecordsRequest.RequestId,
            CreatedOn = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            Status = status.GetValue(),
            RequestDetails = new BsonDocument { { "linkRecordsRequest", ToBsonDocument(linkRecordsRequest) } }
        };

        if (errors != null)
        {
            log.Error = errors;
        }

        await _context.RequestLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<RequestStatusV3Response> GetStatusAsync(string requestId)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.ClientRequestId == requestId);

        if (log != null)
        {
            var response = new RequestStatusV3Response
            {
                RequestId = requestId,
                Status = RequestStatusExtensions.GetDescription(log.Status)
            };

            if (log.Error != null)
            {
                response.Errors = new List<ErrorResponse>
                {
                    new ErrorResponse { Message = log.Error.ToString() ?? string.Empty }
                };
            }

            return response;
        }

        throw new Exception($"Request not found in database for: {requestId}");
    }

    public async Task SaveHealthInformationRequestAsync(HIPHealthInformationRequest hipHealthInformationRequest, RequestStatus requestStatus)
    {
        if (hipHealthInformationRequest?.HiRequest?.Consent == null)
        {
            throw new ArgumentException("Invalid health information request: missing consent details");
        }

        var consentId = hipHealthInformationRequest.HiRequest.Consent.Id;
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.ConsentId == consentId);

        if (log == null)
        {
            log = new RequestLog
            {
                Module = "HIP_DATA_FLOW",
                CreatedOn = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                GatewayRequestId = hipHealthInformationRequest.RequestId,
                TransactionId = hipHealthInformationRequest.TransactionId,
                ConsentId = consentId,
                Status = requestStatus.GetValue(),
                RequestDetails = new BsonDocument { { "healthInformationRequest", ToBsonDocument(hipHealthInformationRequest) } }
            };
            await _context.RequestLogs.AddAsync(log);
        }
        else
        {
            var requestDetails = log.RequestDetails ?? new BsonDocument();
            requestDetails["healthInformationRequest"] = ToBsonDocument(hipHealthInformationRequest);
            log.RequestDetails = requestDetails;
            log.Status = requestStatus.GetValue();
            log.LastUpdated = DateTime.UtcNow;
            _context.RequestLogs.Update(log);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<RequestLog?> FindByConsentIdAsync(string consentId, string entityType)
    {
        return await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.ConsentId == consentId && r.EntityType == entityType);
    }

    public async Task<RequestLog?> FindByConsentIdAsync(string consentId, string entityType, string hipId)
    {
        return await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.ConsentId == consentId && r.EntityType == entityType && r.HipId == hipId);
    }

    public async Task DataTransferNotifyAsync(HIPNotifyRequest hipNotifyRequest, RequestStatus requestStatus, HIPOnNotifyRequest hipOnNotifyRequest, string hipId)
    {
        if (hipNotifyRequest?.Notification == null) return;

        var requestLog = new RequestLog
        {
            Module = "HIP_CONSENT",
            GatewayRequestId = hipOnNotifyRequest.RequestId,
            Status = requestStatus.GetValue(),
            ConsentId = hipNotifyRequest.Notification.ConsentId,
            EntityType = "HIP",
            HipId = hipId,
            RequestDetails = new BsonDocument { { "hipNotifyRequest", ToBsonDocument(hipNotifyRequest) } },
            CreatedOn = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        if (hipOnNotifyRequest.Error != null)
        {
            requestLog.Error = hipOnNotifyRequest.Error;
        }

        await _context.RequestLogs.AddAsync(requestLog);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GetPatientIdAsync(string linkRefNumber)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.LinkRefNumber == linkRefNumber);

        if (log != null && log.RequestDetails != null)
        {
            if (log.RequestDetails.TryGetValue("initResponse", out var initResponseBson))
            {
                var abhaAddress = initResponseBson.AsBsonDocument.GetValue("abhaAddress", "").AsString;
                return abhaAddress;
            }
        }
        return string.Empty;
    }

    public async Task<string> GetPatientReferenceAsync(string linkRefNumber)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.LinkRefNumber == linkRefNumber);

        if (log == null) return string.Empty;

        if (log.RequestDetails.TryGetValue("initResponse", out var initResponseBson))
        {
            var abhaAddress = initResponseBson.AsBsonDocument.GetValue("abhaAddress", "").AsString;
            return await _patientService.GetPatientReferenceAsync(abhaAddress, log.HipId);
        }

        return string.Empty;
    }

    public async Task SetDiscoverResponseAsync(DiscoverRequest discoverRequest, object onDiscoverV3Request, string patientReference)
    {
        if (discoverRequest == null) return;

        var abhaAddress = await _patientService.GetAbhaAddressAsync(patientReference, discoverRequest.HipId);

        var newRecord = new RequestLog
        {
            ClientRequestId = discoverRequest.RequestId,
            Module = "HIP_DISCOVERY",
            TransactionId = discoverRequest.TransactionId,
            HipId = discoverRequest.HipId,
            AbhaAddress = abhaAddress,
            RequestDetails = new BsonDocument 
            { 
                { "discoverRequest", ToBsonDocument(discoverRequest) },
                { "onDiscoverResponse", ToBsonDocument(onDiscoverV3Request) }
            },
            CreatedOn = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await _context.RequestLogs.AddAsync(newRecord);
        await _context.SaveChangesAsync();
    }

    public async Task SetLinkResponseAsync(InitV3Response initResponse, string requestId, string referenceNumber, string hipId, string clientRequestId)
    {
        if (initResponse == null) return;

        var existingRecord = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.TransactionId == initResponse.TransactionId);

        if (existingRecord == null)
        {
            var newRecord = new RequestLog
            {
                HipId = hipId,
                ClientRequestId = clientRequestId,
                GatewayRequestId = requestId,
                AbhaAddress = initResponse.AbhaAddress,
                TransactionId = initResponse.TransactionId,
                Status = RequestStatus.USER_INIT_REQUEST_RECEIVED_BY_WRAPPER.GetValue(),
                RequestDetails = new BsonDocument { { "initResponse", ToBsonDocument(initResponse) } },
                CreatedOn = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            await _context.RequestLogs.AddAsync(newRecord);
        }
        else
        {
            var requestDetails = existingRecord.RequestDetails ?? new BsonDocument();
            requestDetails["initResponse"] = ToBsonDocument(initResponse);

            existingRecord.ClientRequestId = clientRequestId;
            existingRecord.GatewayRequestId = requestId;
            existingRecord.LinkRefNumber = referenceNumber;
            existingRecord.RequestDetails = requestDetails;
            existingRecord.LastUpdated = DateTime.UtcNow;

            _context.RequestLogs.Update(existingRecord);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<CareContext>?> GetSelectedCareContextsAsync(string abhaAddress, string linkRefNumber)
    {
        var requestLog = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.LinkRefNumber == linkRefNumber);

        if (requestLog != null && requestLog.RequestDetails != null)
        {
            if (requestLog.RequestDetails.TryGetValue("initResponse", out var initResponseBson))
            {
                var json = initResponseBson.ToString();
                var initResponse = JsonSerializer.Deserialize<InitV3Response>(json);
                if (initResponse?.Patient != null)
                {
                    var selectedCareContexts = new List<CareContext>();
                    foreach (var patient in initResponse.Patient)
                    {
                        if (patient.CareContexts != null)
                        {
                            selectedCareContexts.AddRange(patient.CareContexts);
                        }
                    }

                    var patientDetails = await _patientService.GetPatientDetailsAsync(abhaAddress, requestLog.HipId);
                    var existingCareContexts = patientDetails?.CareContexts;
                    if (existingCareContexts == null) return null;

                    var selectedRefNumbers = selectedCareContexts.Select(cc => cc.ReferenceNumber).ToHashSet();
                    return existingCareContexts.Where(cc => selectedRefNumbers.Contains(cc.ReferenceNumber)).ToList();
                }
            }
        }

        return null;
    }

    public async Task UpdateTransactionIdAsync(string requestId, string transactionId)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.GatewayRequestId == requestId);
        if (log != null)
        {
            log.TransactionId = transactionId;
            log.LastUpdated = DateTime.UtcNow;
            _context.RequestLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveLinkTokenRequestAsync(LinkRecordsV3Request linkRecordsV3Request, string linkTokenRequestId, RequestStatus status, List<ErrorV3Response>? errors)
    {
        if (linkRecordsV3Request == null) return;

        var requestLog = new RequestLog
        {
            Module = "HIP_INITIATED_LINKING",
            AbhaAddress = linkRecordsV3Request.AbhaAddress,
            CreatedOn = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            ClientRequestId = linkRecordsV3Request.RequestId,
            GatewayRequestId = linkRecordsV3Request.RequestId,
            LinkTokenRequestId = linkTokenRequestId,
            HipId = linkRecordsV3Request.RequesterId,
            Status = status.GetValue(),
            RequestDetails = new BsonDocument { { "linkRecordsRequest", ToBsonDocument(linkRecordsV3Request) } }
        };

        if (errors != null)
        {
            requestLog.Error = errors;
        }

        await _context.RequestLogs.AddAsync(requestLog);
        await _context.SaveChangesAsync();
    }

    public async Task SetHipOnAddCareContextResponseAsync(LinkOnAddCareContextsV3Response response)
    {
        if (response?.Response == null) return;

        var gatewayRequestId = response.Response.RequestId;
        var requestLog = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.GatewayRequestId == gatewayRequestId);

        if (requestLog == null)
        {
            throw new Exception($"Request not found in database for: {gatewayRequestId}");
        }

        var requestDetails = requestLog.RequestDetails ?? new BsonDocument();
        requestDetails["hipOnAddCareContextResponse"] = ToBsonDocument(response);
        requestLog.RequestDetails = requestDetails;

        if (response.Error != null)
        {
            requestLog.Error = response.Error;
        }
        else
        {
            requestLog.Status = RequestStatus.CARE_CONTEXT_LINKED.GetValue();

            if (requestLog.RequestDetails.TryGetValue("linkRecordsRequest", out var linkRecordsBson))
            {
                var json = linkRecordsBson.ToString();
                var linkRecordsRequest = JsonSerializer.Deserialize<LinkRecordsV3Request>(json);
                if (linkRecordsRequest != null)
                {
                    await _patientService.AddPatientCareContextsAsync(linkRecordsRequest);
                }
            }
        }

        _context.RequestLogs.Update(requestLog);
        await _context.SaveChangesAsync();
    }

    public async Task<RequestLog?> GetLogsByAbhaAddressAsync(string abhaAddress, string hipId)
    {
        var linkToken = await _context.LinkTokens
            .FirstOrDefaultAsync(lt => lt.AbhaAddress == abhaAddress && lt.HipId == hipId);
        
        if (linkToken == null || string.IsNullOrEmpty(linkToken.LinkTokenRequestId)) return null;

        return await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.LinkTokenRequestId == linkToken.LinkTokenRequestId);
    }

    public async Task SaveScanAndShareDetailsAsync(ProfileShareV3Request profileShareV3Request, object onShareV3Request)
    {
        if (profileShareV3Request?.Profile?.Patient == null) return;

        var requestLog = new RequestLog
        {
            Module = "HIP_SCAN_AND_SHARE",
            AbhaAddress = profileShareV3Request.Profile.Patient.AbhaAddress,
            HipId = profileShareV3Request.MetaData?.HipId ?? string.Empty,
            CreatedOn = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            RequestDetails = new BsonDocument
            {
                { "shareProfileRequest", ToBsonDocument(profileShareV3Request) },
                { "shareProfileResponse", ToBsonDocument(onShareV3Request) }
            }
        };

        await _context.RequestLogs.AddAsync(requestLog);
        await _context.SaveChangesAsync();
    }

    public async Task SaveConsentRequestAsync(InitConsentRequest request, RequestStatus status)
    {
        if (request == null) return;

        var log = new RequestLog
        {
            Module = "HIU_CONSENT",
            AbhaAddress = request.Consent?.Patient?.Id ?? string.Empty,
            ClientRequestId = request.RequestId,
            GatewayRequestId = request.RequestId,
            CreatedOn = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            Status = status.ToString(),
            RequestDetails = new BsonDocument { { "consentRequest", ToBsonDocument(request) } }
        };

        await _context.RequestLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<RequestLog?> FindByClientRequestIdAsync(string clientRequestId)
    {
        return await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.ClientRequestId == clientRequestId);
    }

    public async Task SaveResponseDetailsAsync(string transactionId, BsonDocument responseDetails)
    {
        var log = await _context.RequestLogs
            .FirstOrDefaultAsync(r => r.TransactionId == transactionId);
        if (log != null)
        {
            log.ResponseDetails = responseDetails;
            log.LastUpdated = DateTime.UtcNow;
            _context.RequestLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveHiuHealthInformationRequestAsync(string clientRequestId, string consentId, string entityType, string hiuId, string status, BsonDocument requestDetails)
    {
        var log = new RequestLog
        {
            Module = "HIU_HEALTH_INFORMATION",
            ClientRequestId = clientRequestId,
            GatewayRequestId = clientRequestId,
            ConsentId = consentId,
            EntityType = entityType,
            HipId = hiuId,
            Status = status,
            CreatedOn = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            RequestDetails = requestDetails
        };

        await _context.RequestLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
