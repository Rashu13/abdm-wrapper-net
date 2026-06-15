using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;

namespace AbdmWrapperNet.Controllers;

/// <summary>
/// HIU Facade Subscription Controller
/// Exposes endpoints for the HIU application to:
///  - Initiate subscription requests to ABDM CM
///  - Poll for subscription request status
/// Base path: /v3
/// </summary>
[ApiController]
[Route("v3")]
public class HIUFacadeSubscriptionV3Controller : ControllerBase
{
    private readonly HIUSubscriptionV3Service _subscriptionService;
    private readonly ILogger<HIUFacadeSubscriptionV3Controller> _logger;

    public HIUFacadeSubscriptionV3Controller(
        HIUSubscriptionV3Service subscriptionService,
        ILogger<HIUFacadeSubscriptionV3Controller> logger)
    {
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    /// <summary>
    /// Initiates a subscription request to the ABDM Gateway/CM.
    /// POST /v3/subscription-init
    /// </summary>
    [HttpPost("subscription-init")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(FacadeV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(FacadeV3Response))]
    public async Task<IActionResult> InitiateSubscriptionRequest([FromBody] SubscriptionInitRequest request)
    {
        _logger.LogInformation("Facade: initiating subscription request");
        var response = await _subscriptionService.InitiateSubscriptionRequestAsync(request);
        return StatusCode(response.HttpStatusCode > 0 ? response.HttpStatusCode : 202, response);
    }

    /// <summary>
    /// Gets the current status of a subscription request.
    /// GET /v3/subscription-status/{requestId}
    /// </summary>
    [HttpGet("subscription-status/{requestId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RequestStatusV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RequestStatusV3Response))]
    public async Task<IActionResult> GetSubscriptionStatus([FromRoute] string requestId)
    {
        _logger.LogInformation($"Facade: fetching subscription status for {requestId}");
        var response = await _subscriptionService.GetSubscriptionStatusAsync(requestId);
        if (response.Errors != null && response.Errors.Count > 0)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
