using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Constants;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;

namespace AbdmWrapperNet.Controllers;

/// <summary>
/// HIU Gateway Callback Controller
/// Receives asynchronous callbacks from the ABDM Gateway for:
///  - Consent: on-init, on-status, notify (hiu), on-fetch
///  - Health Information: on-request, and encrypted data push from HIP
/// </summary>
[ApiController]
public class HIUGatewayCallbackV3Controller : ControllerBase
{
    private readonly HIUConsentV3Service _consentService;
    private readonly HIUHealthInformationV3Service _healthInfoService;
    private readonly HIUSubscriptionV3Service _subscriptionService;
    private readonly ILogger<HIUGatewayCallbackV3Controller> _logger;

    public HIUGatewayCallbackV3Controller(
        HIUConsentV3Service consentService,
        HIUHealthInformationV3Service healthInfoService,
        HIUSubscriptionV3Service subscriptionService,
        ILogger<HIUGatewayCallbackV3Controller> logger)
    {
        _consentService = consentService;
        _healthInfoService = healthInfoService;
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    // ─── Consent Callbacks ────────────────────────────────────────────────────

    /// <summary>
    /// Gateway notifies consent init was received.
    /// POST /api/v3/hiu/consent/request/on-init
    /// </summary>
    [HttpPost(GatewayUrl.ConsentOnInitPath)]
    public async Task<IActionResult> OnInitConsent([FromBody] ConsentOnInitV3Request request)
    {
        _logger.LogInformation("Gateway callback: consent/request/on-init");
        Request.Headers.TryGetValue("X-HIU-ID", out var hiuId);
        int statusCode = await _consentService.OnInitConsentAsync(request, hiuId.ToString());
        return StatusCode(statusCode);
    }

    /// <summary>
    /// Gateway notifies consent status changed.
    /// POST /api/v3/hiu/consent/request/on-status
    /// </summary>
    [HttpPost(GatewayUrl.ConsentOnStatusPath)]
    public async Task<IActionResult> ConsentOnStatus([FromBody] HIUConsentOnStatusV3Request request)
    {
        _logger.LogInformation("Gateway callback: consent/request/on-status");
        int statusCode = await _consentService.ConsentOnStatusAsync(request);
        return StatusCode(statusCode);
    }

    /// <summary>
    /// Gateway notifies HIU of consent artefact grant/revoke/expiry.
    /// POST /api/v3/hiu/consent/request/notify
    /// </summary>
    [HttpPost(GatewayUrl.ConsentHiuNotifyPath)]
    public async Task<IActionResult> ConsentNotify([FromBody] NotifyHIURequest request)
    {
        _logger.LogInformation("Gateway callback: hiu/consent/request/notify");
        Request.Headers.TryGetValue("X-HIU-ID", out var hiuId);
        Request.Headers.TryGetValue("REQUEST-ID", out var requestId);
        int statusCode = await _consentService.HiuNotifyAsync(request, hiuId.ToString(), requestId.ToString());
        return StatusCode(statusCode);
    }

    /// <summary>
    /// Gateway returns the full consent artefact details.
    /// POST /api/v3/hiu/consent/on-fetch
    /// </summary>
    [HttpPost(GatewayUrl.ConsentOnFetchPath)]
    public async Task<IActionResult> OnFetchConsent([FromBody] OnFetchV3Request request)
    {
        _logger.LogInformation("Gateway callback: hiu/consent/on-fetch");
        Request.Headers.TryGetValue("X-HIU-ID", out var hiuId);
        int statusCode = await _consentService.ConsentOnFetchAsync(request, hiuId.ToString());
        return StatusCode(statusCode);
    }

    // ─── Health Information Callbacks ─────────────────────────────────────────

    /// <summary>
    /// Gateway confirms health information request was accepted.
    /// POST /api/v3/hiu/health-information/on-request
    /// </summary>
    [HttpPost(GatewayUrl.HiuHealthInformationOnRequestPath)]
    public async Task<IActionResult> HealthInformationOnRequest([FromBody] HIUHealthInformationOnRequest request)
    {
        _logger.LogInformation("Gateway callback: hiu/health-information/on-request");
        await _healthInfoService.HandleOnRequestAsync(request);
        return Accepted();
    }

    /// <summary>
    /// HIP pushes encrypted FHIR bundles directly to HIU.
    /// POST /v3/hiu/health-information/transfer  (dataPushUrl configured in fetch request)
    /// </summary>
    [HttpPost("v3/hiu/health-information/transfer")]
    public async Task<IActionResult> HealthInformationTransfer([FromBody] HealthInformationPushRequest request)
    {
        _logger.LogInformation($"Received encrypted health data push for transactionId: {request?.TransactionId}");
        Request.Headers.TryGetValue("X-HIU-ID", out var hiuId);
        var result = await _healthInfoService.ProcessEncryptedHealthInformationAsync(request!, hiuId.ToString());
        return StatusCode(result.HttpStatus == "OK" ? 202 : 400, result);
    }

    // ─── Subscription Callbacks ───────────────────────────────────────────────

    /// <summary>
    /// Gateway callback when subscription request is initialized.
    /// POST /api/v3/hiu/subscription-requests/on-init
    /// </summary>
    [HttpPost("api/v3/hiu/subscription-requests/on-init")]
    [HttpPost("api/v3/hiu/hiecm/subscription-requests/on-init")]
    public async Task<IActionResult> OnInitSubscription([FromBody] SubscriptionOnInitV3Request request)
    {
        _logger.LogInformation("Gateway callback: subscription-requests/on-init");
        int statusCode = await _subscriptionService.OnInitSubscriptionAsync(request);
        return StatusCode(statusCode);
    }

    /// <summary>
    /// Gateway callback when subscription status changes (approved/denied by patient).
    /// POST /api/v3/hiu/subscription-requests/hiu/notify
    /// </summary>
    [HttpPost("api/v3/hiu/subscription-requests/hiu/notify")]
    public async Task<IActionResult> OnNotifySubscription([FromBody] SubscriptionNotifyHIURequest request)
    {
        _logger.LogInformation("Gateway callback: subscription-requests/hiu/notify");
        Request.Headers.TryGetValue("X-HIU-ID", out var hiuId);
        Request.Headers.TryGetValue("REQUEST-ID", out var requestId);
        int statusCode = await _subscriptionService.OnNotifySubscriptionAsync(request, hiuId.ToString(), requestId.ToString());
        return StatusCode(statusCode);
    }

    /// <summary>
    /// Gateway callback for events matching subscription categories.
    /// POST /api/v3/hiu/subscription/notify
    /// </summary>
    [HttpPost("api/v3/hiu/subscription/notify")]
    public async Task<IActionResult> OnEventNotifySubscription([FromBody] SubscriptionEventNotifyRequest request)
    {
        _logger.LogInformation("Gateway callback: subscription/notify");
        Request.Headers.TryGetValue("X-HIU-ID", out var hiuId);
        Request.Headers.TryGetValue("REQUEST-ID", out var requestId);
        int statusCode = await _subscriptionService.OnEventNotifySubscriptionAsync(request, hiuId.ToString(), requestId.ToString());
        return StatusCode(statusCode);
    }
}
