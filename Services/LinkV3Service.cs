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
/// User-Initiated Linking — migrated from Java LinkV3Service.java
/// Flow:
///   on-init  → send OTP to patient mobile → POST /link/on-init to gateway
///   on-confirm → verify OTP → POST /link/on-confirm with care contexts
/// </summary>
public class LinkV3Service : ILinkV3Service
{
    private readonly IGatewayClient _gateway;
    private readonly IPatientV3Service _patientService;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly AbdmConfig _config;
    private readonly ILogger<LinkV3Service> _logger;

    public LinkV3Service(
        IGatewayClient gateway,
        IPatientV3Service patientService,
        IRequestLogV3Service requestLogService,
        IOptions<AbdmConfig> config,
        ILogger<LinkV3Service> logger)
    {
        _gateway = gateway;
        _patientService = patientService;
        _requestLogService = requestLogService;
        _config = config.Value;
        _logger = logger;
    }

    /// <summary>
    /// Gateway sends init with patient + careContexts → HIP sends OTP → reply on-init to gateway
    /// </summary>
    public async Task OnInitAsync(InitV3Response initResponse, IHeaderDictionary headers)
    {
        headers.TryGetValue("REQUEST-ID", out var incomingRequestId);
        headers.TryGetValue("X-HIP-ID", out var hipId);

        string linkRefNumber = Guid.NewGuid().ToString();
        string requestId = Guid.NewGuid().ToString();

        try
        {
            // Get patient mobile from DB
            var patient = await _patientService.GetPatientAsync(initResponse.AbhaAddress, hipId.ToString());
            string mobile = patient?.PatientMobile ?? "UNKNOWN";

            // OTP expiry = 10 minutes from now
            string otpExpiry = DateTime.UtcNow.AddMinutes(10).ToString("o");

            var onInitBody = new
            {
                requestId,
                timestamp = Utils.GetCurrentTimeStamp(),
                transactionId = initResponse.TransactionId,
                link = new
                {
                    referenceNumber = linkRefNumber,
                    authenticationType = "MEDIATE",
                    meta = new
                    {
                        communicationMedium = "MOBILE",
                        communicationHint = mobile,
                        communicationExpiry = otpExpiry
                    }
                },
                response = new { requestId = incomingRequestId.ToString() }
            };

            _logger.LogInformation($"Sending on-init for {initResponse.AbhaAddress}, linkRef: {linkRefNumber}");
            await _gateway.PostToGatewayAsync(
                _config.Gateway.OnInitLinkPath ?? "api/v3/hip/link/care-context/on-init",
                onInitBody,
                new Dictionary<string, string>
                {
                    ["X-HIP-ID"] = hipId.ToString(),
                    ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });

            // Persist the link response
            await _requestLogService.SetLinkResponseAsync(
                initResponse, requestId, linkRefNumber,
                hipId.ToString(), incomingRequestId.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnInitAsync");
        }
    }

    /// <summary>
    /// Gateway sends OTP token entered by patient → verify OTP → reply on-confirm with care contexts
    /// </summary>
    public async Task OnConfirmAsync(ConfirmResponse confirmResponse, IHeaderDictionary headers)
    {
        headers.TryGetValue("REQUEST-ID", out var incomingRequestId);
        headers.TryGetValue("X-HIP-ID", out var hipId);

        string linkRefNumber = confirmResponse.Confirmation?.LinkRefNumber ?? string.Empty;

        try
        {
            string abhaAddress = await _requestLogService.GetPatientIdAsync(linkRefNumber);
            string patientRef = await _patientService.GetPatientReferenceAsync(abhaAddress, hipId.ToString());
            var patient = await _patientService.GetPatientAsync(abhaAddress, hipId.ToString());
            string display = patient?.PatientDisplay ?? patientRef;

            var careContexts = await _requestLogService.GetSelectedCareContextsAsync(abhaAddress, linkRefNumber);

            object onConfirmBody;

            if (careContexts == null || careContexts.Count == 0)
            {
                // Care contexts not found → error
                onConfirmBody = new
                {
                    requestId = Guid.NewGuid().ToString(),
                    timestamp = Utils.GetCurrentTimeStamp(),
                    error = new { code = "HIP-1000", message = "CareContexts don't match" },
                    response = new { requestId = incomingRequestId.ToString() }
                };
            }
            else
            {
                // Group by HI type
                var grouped = careContexts
                    .GroupBy(c => string.IsNullOrEmpty(c.HiType) ? "HealthDocumentRecord" : c.HiType)
                    .Select(g => new
                    {
                        referenceNumber = patientRef,
                        display,
                        hiType = g.Key,
                        count = g.Count(),
                        careContexts = g.Select(cc => new { display = cc.Display, referenceNumber = cc.ReferenceNumber }).ToList()
                    }).ToList();

                onConfirmBody = new
                {
                    requestId = Guid.NewGuid().ToString(),
                    timestamp = Utils.GetCurrentTimeStamp(),
                    patient = grouped,
                    response = new { requestId = incomingRequestId.ToString() }
                };

                // Mark care contexts as linked
                await _patientService.UpdateCareContextStatusAsync(abhaAddress, careContexts, hipId.ToString());
            }

            _logger.LogInformation($"Sending on-confirm for abha: {abhaAddress}");
            await _gateway.PostToGatewayAsync(
                _config.Gateway.OnConfirmLinkPath ?? "api/v3/hip/link/care-context/on-confirm",
                onConfirmBody,
                new Dictionary<string, string>
                {
                    ["X-HIP-ID"] = hipId.ToString(),
                    ["REQUEST-ID"] = Guid.NewGuid().ToString(),
                    ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnConfirmAsync");
        }
    }
}
