using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;

namespace AbdmWrapperNet.Controllers;

/// <summary>
/// HIU Facade Health Information Controller
/// Exposes endpoints for the HIU application to:
///  - Request encrypted health data from HIP (via ABDM gateway)
///  - Fetch the status / decrypted FHIR bundles once data arrives
/// Base path: /v3/health-information
/// </summary>
[ApiController]
[Route("v3/health-information")]
public class HIUFacadeHealthInformationV3Controller : ControllerBase
{
    private readonly HIUHealthInformationV3Service _healthInfoService;
    private readonly ILogger<HIUFacadeHealthInformationV3Controller> _logger;

    public HIUFacadeHealthInformationV3Controller(
        HIUHealthInformationV3Service healthInfoService,
        ILogger<HIUFacadeHealthInformationV3Controller> logger)
    {
        _healthInfoService = healthInfoService;
        _logger = logger;
    }

    /// <summary>
    /// Initiates the health information fetch request to ABDM gateway.
    /// POST /v3/health-information/fetch-records
    /// </summary>
    [HttpPost("fetch-records")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(FacadeV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(FacadeV3Response))]
    public async Task<IActionResult> FetchHealthInformation([FromBody] HIUClientHealthInformationRequest request)
    {
        _logger.LogInformation($"Facade: initiating health information fetch for consent {request.ConsentId}");

        Request.Headers.TryGetValue("X-HIU-ID", out var hiuId);
        var response = await _healthInfoService.FetchHealthInformationAsync(request, hiuId.ToString());
        return StatusCode(response.HttpStatusCode > 0 ? response.HttpStatusCode : 202, response);
    }

    /// <summary>
    /// Gets the status and (if available) decrypted FHIR bundles for a health data request.
    /// GET /v3/health-information/status/{requestId}
    /// </summary>
    [HttpGet("status/{requestId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HealthInformationV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(HealthInformationV3Response))]
    public async Task<IActionResult> GetHealthInformationStatus([FromRoute] string requestId)
    {
        _logger.LogInformation($"Facade: fetching health information status for {requestId}");
        var response = await _healthInfoService.GetHealthInformationAsync(requestId);
        return StatusCode(response.HttpStatusCode > 0 ? response.HttpStatusCode : 200, response);
    }
}
