using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

/// <summary>
/// Service for Milestone 3 (HIU) Subscription Flow operations.
/// </summary>
public class HIUSubscriptionV3Service
{
    private readonly IGatewayClient _gatewayClient;
    private readonly IRequestLogV3Service _requestLogService;
    private readonly AbdmConfig _config;
    private readonly ILogger<HIUSubscriptionV3Service> _logger;

    // In-memory mapping to map subscriptionRequestId (received in notify callback) back to original gatewayRequestId
    private static readonly ConcurrentDictionary<string, string> _subscriptionRequestMapping = new();

    public HIUSubscriptionV3Service(
        IGatewayClient gatewayClient,
        IRequestLogV3Service requestLogService,
        IOptions<AbdmConfig> config,
        ILogger<HIUSubscriptionV3Service> logger)
    {
        _gatewayClient = gatewayClient;
        _requestLogService = requestLogService;
        _config = config.Value;
        _logger = logger;
    }

    /// <summary>
    /// Initiates a subscription request to the ABDM Gateway.
    /// </summary>
    public async Task<FacadeV3Response> InitiateSubscriptionRequestAsync(SubscriptionInitRequest request)
    {
        try
        {
            _logger.LogInformation($"Initiating subscription request: {request.RequestId}");

            var headers = new Dictionary<string, string>
            {
                ["X-HIU-ID"] = request.Subscription?.Hiu?.Id ?? _config.HiuSetup?.HiuId ?? string.Empty,
                ["REQUEST-ID"] = request.RequestId,
                ["TIMESTAMP"] = Utils.GetCurrentTimeStamp()
            };

            var response = await _gatewayClient.PostToGatewayAsync(
                "api/hiecm/subscription-requests/v3/init",
                request, headers);

            bool success = response?.HttpStatus?.StartsWith("2") == true
                           || response?.HttpStatus == "OK"
                           || response?.HttpStatus == "Accepted";

            if (success)
            {
                await _requestLogService.UpdateStatusAsync(request.RequestId, RequestStatus.INITIATING);
                return new FacadeV3Response
                {
                    ClientRequestId = request.RequestId,
                    Message = "Subscription request successfully accepted by gateway.",
                    HttpStatusCode = 202
                };
            }
            else
            {
                var errorMsg = response?.Message ?? "Error from gateway while initiating subscription request";
                _logger.LogError(errorMsg);
                await _requestLogService.UpdateErrorAsync(request.RequestId,
                    new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = errorMsg } } },
                    "SUBSCRIPTION_INIT_ERROR");
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
            _logger.LogError(ex, "Exception while initiating subscription request");
            await _requestLogService.UpdateErrorAsync(request.RequestId,
                new List<ErrorV3Response> { new() { Error = new ErrorResponse { Code = "1000", Message = ex.Message } } },
                "SUBSCRIPTION_INIT_ERROR");
            return new FacadeV3Response
            {
                ClientRequestId = request.RequestId,
                Message = ex.Message,
                HttpStatusCode = 500
            };
        }
    }

    /// <summary>
    /// Handles callback /v3/hiu/subscription-requests/on-init (or with /hiecm)
    /// </summary>
    public async Task<int> OnInitSubscriptionAsync(SubscriptionOnInitV3Request request)
    {
        if (request?.SubscriptionRequest != null && request.Response != null)
        {
            // Map subscriptionRequestId -> original gatewayRequestId for notify callbacks
            _subscriptionRequestMapping[request.SubscriptionRequest.Id] = request.Response.RequestId;

            await _requestLogService.UpdateConsentResponseAsync(
                request.Response.RequestId,
                "SUBSCRIPTION_ON_INIT_RESPONSE",
                RequestStatus.CONSENT_ON_INIT_RESPONSE_RECEIVED, // Reuses status for generic mapping
                request.SubscriptionRequest.Id);

            return 202;
        }
        else if (request?.Error != null && request.Response != null)
        {
            await _requestLogService.UpdateErrorAsync(
                request.Response.RequestId,
                new List<ErrorV3Response> { new() { Error = request.Error } },
                "SUBSCRIPTION_ON_INIT_ERROR");
        }

        return 202;
    }

    /// <summary>
    /// Handles patient approve/deny notify callback: /v3/hiu/subscription-requests/hiu/notify
    /// </summary>
    public async Task<int> OnNotifySubscriptionAsync(SubscriptionNotifyHIURequest request, string hiuId, string incomingRequestId)
    {
        if (request?.Notification == null)
        {
            _logger.LogError("Notify subscription callback request body or notification is null.");
            return 400;
        }

        var notification = request.Notification;
        _subscriptionRequestMapping.TryGetValue(notification.SubscriptionRequestId, out var gatewayRequestId);

        if (gatewayRequestId != null)
        {
            await _requestLogService.UpdateConsentResponseAsync(
                gatewayRequestId,
                "SUBSCRIPTION_HIU_NOTIFY",
                RequestStatus.CONSENT_ON_NOTIFY_RESPONSE_RECEIVED,
                request);
        }

        // Send acknowledgement back to CM (/api/hiecm/subscription-requests/v3/hiu/on-notify)
        var ack = new SubscriptionOnNotifyV3Request
        {
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = Utils.GetCurrentTimeStamp(),
            Response = new RespRequest { RequestId = incomingRequestId },
            Acknowledgement = new SubscriptionAcknowledgement
            {
                Status = "OK",
                SubscriptionRequestId = notification.SubscriptionRequestId
            }
        };

        var headers = new Dictionary<string, string>
        {
            ["X-CM-ID"] = _config.Environment == "sbx" ? "sbx" : "abdm",
            ["REQUEST-ID"] = ack.RequestId,
            ["TIMESTAMP"] = ack.Timestamp,
            ["X-HIU-ID"] = hiuId
        };

        await _gatewayClient.PostToGatewayAsync(
            "api/hiecm/subscription-requests/v3/hiu/on-notify",
            ack, headers);

        return 202;
    }

    /// <summary>
    /// Handles event notify callback: /v3/hiu/subscription/notify
    /// </summary>
    public async Task<int> OnEventNotifySubscriptionAsync(SubscriptionEventNotifyRequest request, string hiuId, string incomingRequestId)
    {
        if (request?.Event == null)
        {
            _logger.LogError("Event notify subscription request body or event is null.");
            return 400;
        }

        var eventDetail = request.Event;
        _logger.LogInformation($"Received subscription event notify: id={eventDetail.Id}, category={eventDetail.Category}, subscriptionId={eventDetail.SubscriptionId}");

        // Save event details or process them (e.g. notify HMS/local Care contexts update)
        // Send acknowledgement back to CM (/api/hiecm/subscription-requests/v3/hiu/care-context/on-notify)
        var ack = new SubscriptionEventAcknowledgementRequest
        {
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = Utils.GetCurrentTimeStamp(),
            Response = new RespRequest { RequestId = incomingRequestId },
            Acknowledgement = new SubscriptionEventAcknowledgementDetail
            {
                Status = "OK",
                EventId = eventDetail.Id
            }
        };

        var headers = new Dictionary<string, string>
        {
            ["X-CM-ID"] = _config.Environment == "sbx" ? "sbx" : "abdm",
            ["REQUEST-ID"] = ack.RequestId,
            ["TIMESTAMP"] = ack.Timestamp,
            ["X-HIU-ID"] = hiuId
        };

        await _gatewayClient.PostToGatewayAsync(
            "api/hiecm/subscription-requests/v3/hiu/care-context/on-notify",
            ack, headers);

        return 202;
    }

    /// <summary>
    /// Retrieves current status of subscription initiation
    /// </summary>
    public async Task<RequestStatusV3Response> GetSubscriptionStatusAsync(string clientRequestId)
    {
        return await _requestLogService.GetStatusAsync(clientRequestId);
    }
}
