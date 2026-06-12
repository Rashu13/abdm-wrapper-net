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
/// Patient Discovery Service — migrated from Java DiscoveryV3Service.java
/// Flow: Gateway sends patient demographics → lookup in DB → send on-discover to gateway
/// </summary>
public class DiscoveryV3Service : IDiscoveryV3Service
{
    private readonly IGatewayClient _gateway;
    private readonly IPatientV3Service _patientService;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly AbdmConfig _config;
    private readonly ILogger<DiscoveryV3Service> _logger;

    public DiscoveryV3Service(
        IGatewayClient gateway,
        IPatientV3Service patientService,
        IRequestLogV3Service requestLogService,
        IOptions<AbdmConfig> config,
        ILogger<DiscoveryV3Service> logger)
    {
        _gateway = gateway;
        _patientService = patientService;
        _requestLogService = requestLogService;
        _config = config.Value;
        _logger = logger;
    }

    public async Task<GenericV3Response?> DiscoverAsync(DiscoverRequest discoverRequest, IHeaderDictionary headers)
    {
        if (discoverRequest?.Patient == null)
            return new GenericV3Response { HttpStatus = "BadRequest", Status = "Error" };

        string abhaAddress = discoverRequest.Patient.Id;
        string hipId = discoverRequest.HipId;

        headers.TryGetValue("REQUEST-ID", out var incomingRequestId);

        var patient = await _patientService.GetPatientAsync(abhaAddress, hipId);

        if (patient != null)
        {
            _logger.LogInformation($"Patient found in DB: {patient.Name} / {abhaAddress}");
            _ = SendOnDiscoverAsync(discoverRequest, patient, incomingRequestId.ToString(), hipId);
            return new GenericV3Response { HttpStatus = "OK", Status = "SUCCESS" };
        }

        // Patient not in DB → send on-discover with "not found" error
        _logger.LogWarning($"Patient not found for abhaAddress: {abhaAddress}");
        _ = SendOnDiscoverErrorAsync(discoverRequest, incomingRequestId.ToString(), hipId);
        return new GenericV3Response { HttpStatus = "OK", Status = "NOT_FOUND" };
    }

    private async Task SendOnDiscoverAsync(DiscoverRequest req, Patient patient, string incomingRequestId, string hipId)
    {
        try
        {
            // Get unlinked care contexts
            var unlinked = (patient.CareContexts ?? new List<CareContext>())
                .Where(c => !c.IsLinked)
                .GroupBy(c => string.IsNullOrEmpty(c.HiType) ? "HealthDocumentRecord" : c.HiType)
                .Select(g => new PatientCareContextHIType
                {
                    ReferenceNumber = patient.PatientReference,
                    Display = patient.PatientDisplay,
                    HiType = g.Key,
                    Count = g.Count(),
                    CareContexts = g.Select(cc => new CareContext
                    {
                        Display = cc.Display,
                        ReferenceNumber = cc.ReferenceNumber
                    }).ToList()
                }).ToList();

            var onDiscoverRequest = new
            {
                requestId = Guid.NewGuid().ToString(),
                timestamp = Utils.GetCurrentTimeStamp(),
                transactionId = req.TransactionId,
                patient = unlinked,
                matchedBy = new[] { "ABHA_ADDRESS" },
                response = new { requestId = incomingRequestId }
            };

            await _gateway.PostToGatewayAsync(
                _config.Gateway.OnDiscoverPath ?? "api/v3/care-contexts/on-discover",
                onDiscoverRequest,
                new Dictionary<string, string>
                {
                    ["X-HIP-ID"] = hipId,
                    ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });

            await _requestLogService.SetDiscoverResponseAsync(req, onDiscoverRequest, patient.PatientReference);
            await _patientService.UpdateCareContextAsync(patient.PatientReference, patient.CareContexts ?? new(), hipId);
        }
        catch (Exception ex) { _logger.LogError(ex, "Error sending on-discover"); }
    }

    private async Task SendOnDiscoverErrorAsync(DiscoverRequest req, string incomingRequestId, string hipId)
    {
        try
        {
            var onDiscoverError = new
            {
                requestId = Guid.NewGuid().ToString(),
                timestamp = Utils.GetCurrentTimeStamp(),
                transactionId = req.TransactionId,
                error = new { code = "HIP-1000", message = "Patient not found" },
                response = new { requestId = incomingRequestId }
            };

            await _gateway.PostToGatewayAsync(
                _config.Gateway.OnDiscoverPath ?? "api/v3/care-contexts/on-discover",
                onDiscoverError,
                new Dictionary<string, string>
                {
                    ["X-HIP-ID"] = hipId,
                    ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });
        }
        catch (Exception ex) { _logger.LogError(ex, "Error sending on-discover error"); }
    }
}
