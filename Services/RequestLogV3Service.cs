using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class RequestLogV3Service : IRequestLogV3Service
{
    private readonly MongoDbContext _context;
    private readonly IPatientV3Service _patientService;
    private readonly ILogger<RequestLogV3Service> _logger;

    public RequestLogV3Service(MongoDbContext context, IPatientV3Service patientService, ILogger<RequestLogV3Service> logger)
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
        var filter = Builders<RequestLog>.Filter.Eq(r => r.GatewayRequestId, requestId);
        var update = Builders<RequestLog>.Update
            .Set(r => r.Status, status.ToString())
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        var result = await _context.RequestLogs.UpdateOneAsync(filter, update);
        if (result.MatchedCount == 0)
        {
            throw new Exception($"Request not found for request id: {requestId}");
        }
    }

    public async Task UpdateErrorAsync(string requestId, object error, RequestStatus requestStatus)
    {
        await UpdateErrorAsync(requestId, error, requestStatus.ToString());
    }

    public async Task UpdateErrorAsync(string requestId, object error, string status)
    {
        var filter = Builders<RequestLog>.Filter.Eq(r => r.GatewayRequestId, requestId);
        var update = Builders<RequestLog>.Update
            .Set(r => r.Error, ToBsonValue(error))
            .Set(r => r.Status, status)
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        await _context.RequestLogs.UpdateOneAsync(filter, update);
    }

    public async Task UpdateErrorLinkErrorAsync(string linkTokenRequestId, object error, RequestStatus requestStatus)
    {
        var filter = Builders<RequestLog>.Filter.Eq(r => r.LinkTokenRequestId, linkTokenRequestId);
        var update = Builders<RequestLog>.Update
            .Set(r => r.Error, ToBsonValue(error))
            .Set(r => r.Status, requestStatus.ToString())
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        await _context.RequestLogs.UpdateOneAsync(filter, update);
    }

    public async Task LinkTokenUpdateErrorAsync(string requestId, object error, RequestStatus requestStatus)
    {
        await LinkTokenUpdateErrorAsync(requestId, error, requestStatus.ToString());
    }

    public async Task LinkTokenUpdateErrorAsync(string requestId, object error, string status)
    {
        _logger.LogInformation($"LinkTokenUpdateError: {requestId}");
        var filter = Builders<RequestLog>.Filter.Eq(r => r.GatewayRequestId, requestId);
        var update = Builders<RequestLog>.Update
            .Set(r => r.Error, ToBsonValue(error))
            .Set(r => r.Status, status)
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        await _context.RequestLogs.UpdateOneAsync(filter, update);
    }

    public async Task UpdateStatusAsync(string requestId, RequestStatus requestStatus)
    {
        _logger.LogInformation($"GatewayRequestId: {requestId} requestStatus: {requestStatus}");
        var filter = Builders<RequestLog>.Filter.Eq(r => r.GatewayRequestId, requestId);
        var update = Builders<RequestLog>.Update
            .Set(r => r.Status, requestStatus.ToString())
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        await _context.RequestLogs.UpdateOneAsync(filter, update);
    }

    public async Task UpdateConsentResponseAsync<T>(string requestId, string identifier, RequestStatus requestStatus, T consentDetails)
    {
        var filter = Builders<RequestLog>.Filter.Eq(r => r.GatewayRequestId, requestId);
        var requestLog = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();
        if (requestLog == null)
        {
            throw new Exception($"Request not found for request id: {requestId}");
        }

        var responseDetails = requestLog.ResponseDetails ?? new BsonDocument();
        responseDetails[identifier] = ToBsonValue(consentDetails);

        var update = Builders<RequestLog>.Update
            .Set(r => r.ResponseDetails, responseDetails)
            .Set(r => r.Status, requestStatus.ToString())
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        await _context.RequestLogs.UpdateOneAsync(filter, update);
    }

    public async Task<RequestLog?> FindRequestLogByTransactionIdAsync(string transactionId)
    {
        var filter = Builders<RequestLog>.Filter.Eq(r => r.TransactionId, transactionId);
        return await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();
    }

    public async Task PersistHipLinkRequestAsync(LinkRecordsV3Request linkRecordsV3Request, RequestStatus status, object? errors)
    {
        if (linkRecordsV3Request == null) return;

        var filter = Builders<RequestLog>.Filter.Eq(r => r.ClientRequestId, linkRecordsV3Request.RequestId);
        var existingLog = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();

        if (existingLog == null)
        {
            var requestLog = new RequestLog
            {
                AbhaAddress = linkRecordsV3Request.AbhaAddress,
                Module = "HIP_INITIATED_LINKING",
                CreatedOn = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                ClientRequestId = linkRecordsV3Request.RequestId,
                GatewayRequestId = linkRecordsV3Request.RequestId,
                HipId = linkRecordsV3Request.RequesterId,
                Status = status.ToString(),
                RequestDetails = new BsonDocument { { "linkRecordsRequest", ToBsonDocument(linkRecordsV3Request) } }
            };

            if (errors != null)
            {
                requestLog.Error = ToBsonValue(errors);
            }

            await _context.RequestLogs.InsertOneAsync(requestLog);
            return;
        }

        var requestDetails = existingLog.RequestDetails ?? new BsonDocument();
        requestDetails["linkRecordsRequest"] = ToBsonDocument(linkRecordsV3Request);

        var update = Builders<RequestLog>.Update
            .Set(r => r.RequestDetails, requestDetails)
            .Set(r => r.Status, status.ToString())
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        await _context.RequestLogs.UpdateOneAsync(filter, update);
    }

    public async Task<RequestStatusV3Response> GetStatusAsync(string requestId)
    {
        var filter = Builders<RequestLog>.Filter.Eq(r => r.ClientRequestId, requestId);
        var requestLog = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();

        if (requestLog != null)
        {
            var response = new RequestStatusV3Response
            {
                RequestId = requestId,
                Status = RequestStatusExtensions.GetDescription(requestLog.Status)
            };

            if (requestLog.Error != null)
            {
                // Simple representation of errors list
                response.Errors = new List<ErrorResponse>
                {
                    new ErrorResponse { Message = requestLog.Error.ToString() ?? string.Empty }
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
        var filter = Builders<RequestLog>.Filter.Eq(r => r.ConsentId, consentId);
        var requestLog = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();

        if (requestLog == null)
        {
            throw new Exception($"Request not found for consentId: {consentId}");
        }

        var requestDetails = requestLog.RequestDetails ?? new BsonDocument();
        requestDetails["healthInformationRequest"] = ToBsonDocument(hipHealthInformationRequest);

        var update = Builders<RequestLog>.Update
            .Set(r => r.RequestDetails, requestDetails)
            .Set(r => r.Status, requestStatus.GetValue())
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        await _context.RequestLogs.UpdateOneAsync(filter, update);
    }

    public async Task<RequestLog?> FindByConsentIdAsync(string consentId, string entityType)
    {
        var filter = Builders<RequestLog>.Filter.And(
            Builders<RequestLog>.Filter.Eq(r => r.ConsentId, consentId),
            Builders<RequestLog>.Filter.Eq(r => r.EntityType, entityType)
        );
        return await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<RequestLog?> FindByConsentIdAsync(string consentId, string entityType, string hipId)
    {
        var filter = Builders<RequestLog>.Filter.And(
            Builders<RequestLog>.Filter.Eq(r => r.ConsentId, consentId),
            Builders<RequestLog>.Filter.Eq(r => r.EntityType, entityType),
            Builders<RequestLog>.Filter.Eq(r => r.HipId, hipId)
        );
        return await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();
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
            requestLog.Error = ToBsonValue(hipOnNotifyRequest.Error);
        }

        await _context.RequestLogs.InsertOneAsync(requestLog);
    }

    public async Task<string> GetPatientIdAsync(string linkRefNumber)
    {
        var filter = Builders<RequestLog>.Filter.Eq(r => r.LinkRefNumber, linkRefNumber);
        var requestLog = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();
        if (requestLog == null) return string.Empty;

        if (requestLog.RequestDetails.TryGetValue("initResponse", out var initResponseBson))
        {
            return initResponseBson.AsBsonDocument.GetValue("abhaAddress", "").AsString;
        }

        return string.Empty;
    }

    public async Task<string> GetPatientReferenceAsync(string linkRefNumber)
    {
        var filter = Builders<RequestLog>.Filter.Eq(r => r.LinkRefNumber, linkRefNumber);
        var requestLog = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();
        if (requestLog == null) return string.Empty;

        if (requestLog.RequestDetails.TryGetValue("initResponse", out var initResponseBson))
        {
            var abhaAddress = initResponseBson.AsBsonDocument.GetValue("abhaAddress", "").AsString;
            return await _patientService.GetPatientReferenceAsync(abhaAddress, requestLog.HipId);
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

        await _context.RequestLogs.InsertOneAsync(newRecord);
    }

    public async Task SetLinkResponseAsync(InitV3Response initResponse, string requestId, string referenceNumber, string hipId, string clientRequestId)
    {
        if (initResponse == null) return;

        var filter = Builders<RequestLog>.Filter.Eq(r => r.TransactionId, initResponse.TransactionId);
        var existingRecord = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();

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

            await _context.RequestLogs.InsertOneAsync(newRecord);
        }
        else
        {
            var requestDetails = existingRecord.RequestDetails ?? new BsonDocument();
            requestDetails["initResponse"] = ToBsonDocument(initResponse);

            var update = Builders<RequestLog>.Update
                .Set(r => r.ClientRequestId, clientRequestId)
                .Set(r => r.GatewayRequestId, requestId)
                .Set(r => r.LinkRefNumber, referenceNumber)
                .Set(r => r.RequestDetails, requestDetails)
                .Set(r => r.LastUpdated, DateTime.UtcNow);

            await _context.RequestLogs.UpdateOneAsync(filter, update);
        }
    }

    public async Task<List<CareContext>?> GetSelectedCareContextsAsync(string abhaAddress, string linkRefNumber)
    {
        var filter = Builders<RequestLog>.Filter.Eq(r => r.LinkRefNumber, linkRefNumber);
        var requestLog = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();

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
        var filter = Builders<RequestLog>.Filter.Eq(r => r.GatewayRequestId, requestId);
        var update = Builders<RequestLog>.Update
            .Set(r => r.TransactionId, transactionId)
            .Set(r => r.LastUpdated, DateTime.UtcNow);

        await _context.RequestLogs.UpdateOneAsync(filter, update);
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
            requestLog.Error = ToBsonValue(errors);
        }

        await _context.RequestLogs.InsertOneAsync(requestLog);
    }

    public async Task SetHipOnAddCareContextResponseAsync(LinkOnAddCareContextsV3Response response)
    {
        if (response?.Response == null) return;

        var gatewayRequestId = response.Response.RequestId;
        var filter = Builders<RequestLog>.Filter.Eq(r => r.GatewayRequestId, gatewayRequestId);
        var requestLog = await _context.RequestLogs.Find(filter).FirstOrDefaultAsync();

        if (requestLog == null)
        {
            throw new Exception($"Request not found in database for: {gatewayRequestId}");
        }

        var requestDetails = requestLog.RequestDetails ?? new BsonDocument();
        requestDetails["hipOnAddCareContextResponse"] = ToBsonDocument(response);

        var update = Builders<RequestLog>.Update.Set(r => r.RequestDetails, requestDetails);

        if (response.Error != null)
        {
            update = Builders<RequestLog>.Update.Combine(
                update,
                Builders<RequestLog>.Update.Set(r => r.Error, ToBsonValue(response.Error))
            );
        }
        else
        {
            update = Builders<RequestLog>.Update.Combine(
                update,
                Builders<RequestLog>.Update.Set(r => r.Status, RequestStatus.CARE_CONTEXT_LINKED.GetValue())
            );

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

        await _context.RequestLogs.UpdateOneAsync(filter, update);
    }

    public async Task<RequestLog?> GetLogsByAbhaAddressAsync(string abhaAddress, string hipId)
    {
        var filterToken = Builders<LinkToken>.Filter.And(
            Builders<LinkToken>.Filter.Eq(lt => lt.AbhaAddress, abhaAddress),
            Builders<LinkToken>.Filter.Eq(lt => lt.HipId, hipId)
        );

        var linkToken = await _context.LinkTokens.Find(filterToken).FirstOrDefaultAsync();
        if (linkToken == null || string.IsNullOrEmpty(linkToken.LinkTokenRequestId)) return null;

        var filterLog = Builders<RequestLog>.Filter.Eq(r => r.LinkTokenRequestId, linkToken.LinkTokenRequestId);
        return await _context.RequestLogs.Find(filterLog).FirstOrDefaultAsync();
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

        await _context.RequestLogs.InsertOneAsync(requestLog);
    }
}
