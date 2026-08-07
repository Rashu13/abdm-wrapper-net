using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;
using MongoDB.Bson;

namespace AbdmWrapperNet.Controllers;

/// <summary>
/// HIU Facade Consent Controller
/// Exposes endpoints for the HIU application to:
///  - Initiate consent requests to ABDM
///  - Poll for consent status
///  - List all consent requests
/// Base path: /v3
/// </summary>
[ApiController]
[Route("v3")]
public class HIUFacadeConsentV3Controller : ControllerBase
{
    private readonly HIUConsentV3Service _consentService;
    private readonly AppDbContext _db;
    private readonly ILogger<HIUFacadeConsentV3Controller> _logger;

    public HIUFacadeConsentV3Controller(
        HIUConsentV3Service consentService,
        AppDbContext db,
        ILogger<HIUFacadeConsentV3Controller> logger)
    {
        _consentService = consentService;
        _db = db;
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

    private static object? BsonToJsonElement(MongoDB.Bson.BsonDocument? doc)
    {
        if (doc == null || doc.ElementCount == 0) return null;
        try
        {
            var json = doc.ToJson(new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.CanonicalExtendedJson });
            return System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonNode>(json);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Returns all HIU Consent Requests for the frontend dashboard.
    /// GET /v3/consent/list
    /// </summary>
    [HttpGet("consent/list")]
    public IActionResult GetConsentList()
    {
        var logs = _db.RequestLogs
            .Where(r => r.Module == "HIU_CONSENT")
            .OrderByDescending(r => r.CreatedOn)
            .Take(15)
            .ToList()
            .Select(r => new
            {
                id = r.Id,
                clientRequestId = r.ClientRequestId,
                transactionId = r.TransactionId,
                status = r.Status,
                gatewayRequestId = r.GatewayRequestId,
                createdAt = r.CreatedOn,
                lastUpdated = r.LastUpdated,
                requestDetails = BsonToJsonElement(r.RequestDetails),
                responseDetails = BsonToJsonElement(r.ResponseDetails),
                consentId = r.ConsentId,
            });

        return Ok(logs);
    }
}
