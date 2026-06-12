using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;

namespace AbdmWrapperNet.Controllers;

/// <summary>
/// HIU Facade Consent Controller
/// Exposes endpoints for the HIU application to:
///  - Initiate consent requests to ABDM
///  - Poll for consent status
/// Base path: /v3
/// </summary>
[ApiController]
[Route("v3")]
public class HIUFacadeConsentV3Controller : ControllerBase
{
    private readonly HIUConsentV3Service _consentService;
    private readonly ILogger<HIUFacadeConsentV3Controller> _logger;

    public HIUFacadeConsentV3Controller(
        HIUConsentV3Service consentService,
        ILogger<HIUFacadeConsentV3Controller> logger)
    {
        _consentService = consentService;
        _logger = logger;
    }

    /// <summary>
    /// Initiates a consent request to the ABDM Gateway.
    /// POST /v3/consent-init
    /// </summary>
    [HttpPost("consent-init")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(FacadeV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(FacadeV3Response))]
    public async Task<IActionResult> InitiateConsentRequest([FromBody] InitConsentRequest request)
    {
        _logger.LogInformation("Facade: initiating consent request");
        var response = await _consentService.InitiateConsentRequestAsync(request);
        return StatusCode(response.HttpStatusCode > 0 ? response.HttpStatusCode : 202, response);
    }

    /// <summary>
    /// Gets the current status of a consent request.
    /// GET /v3/consent-status/{requestId}
    /// </summary>
    [HttpGet("consent-status/{requestId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsentStatusV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ConsentStatusV3Response))]
    public async Task<IActionResult> GetConsentStatus([FromRoute] string requestId)
    {
        _logger.LogInformation($"Facade: fetching consent status for {requestId}");
        var response = await _consentService.GetConsentStatusAsync(requestId);
        return StatusCode(response.HttpStatusCode > 0 ? response.HttpStatusCode : 200, response);
    }
}
