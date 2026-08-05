using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;

namespace AbdmWrapperNet.Controllers;

[ApiController]
[Route("v3")]
public class HIPFacadeLinkV3Controller : ControllerBase
{
    private readonly ILogger<HIPFacadeLinkV3Controller> _logger;
    private readonly IWorkflowV3Manager _workflowV3Manager;

    public HIPFacadeLinkV3Controller(
        ILogger<HIPFacadeLinkV3Controller> logger,
        IWorkflowV3Manager workflowV3Manager)
    {
        _logger = logger;
        _workflowV3Manager = workflowV3Manager;
    }

    /// <summary>
    /// Facade POST method to facade for linking careContexts i.e. hipInitiatedLinking.
    /// </summary>
    [HttpPost("link-carecontexts")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(FacadeV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(FacadeV3Response))]
    public async Task<IActionResult> LinkRecords([FromBody] LinkRecordsV3Request linkRecordsV3Request)
    {
        _logger.LogInformation("Facade request to link care contexts received.");
        var facadeResponse = await _workflowV3Manager.InitiateCareContextLinkingAsync(linkRecordsV3Request);
        if (facadeResponse.Errors == null || facadeResponse.Errors.Count == 0)
        {
            return Accepted(facadeResponse);
        }
        return BadRequest(facadeResponse);
    }

    /// <summary>
    /// Facade GET method to facade for checking status of hipInitiatedLinking.
    /// </summary>
    [HttpGet("link-status/{requestId}")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RequestStatusV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RequestStatusV3Response))]
    public async Task<IActionResult> FetchCareContextStatus([FromRoute] string requestId)
    {
        _logger.LogInformation($"Facade request to fetch care context status for request: {requestId}");
        var statusResponse = await _workflowV3Manager.GetCareContextRequestStatusAsync(requestId);
        if (statusResponse.Errors == null || statusResponse.Errors.Count == 0)
        {
            return Accepted(statusResponse);
        }
        return BadRequest(statusResponse);
    }

    /// <summary>
    /// Facade PUT method for adding or modifying patients in database.
    /// </summary>
    [HttpPut("add-patients")]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(FacadeV3Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(FacadeV3Response))]
    public async Task<IActionResult> AddPatients([FromBody] List<Patient> patients)
    {
        _logger.LogInformation("Facade request to add or modify patients in database.");
        var facadeResponse = await _workflowV3Manager.AddPatientsAsync(patients);
        if (facadeResponse.Errors == null || facadeResponse.Errors.Count == 0)
        {
            return Accepted(facadeResponse);
        }
        return BadRequest(facadeResponse);
    }

    /// <summary>
    /// Send Deep Linking SMS to patient mobile.
    /// </summary>
    [HttpPost("sms/notify")]
    public async Task<IActionResult> DeepLinking([FromBody] DeepLinkingRequest deepLinkingRequest)
    {
        _logger.LogInformation("Facade request to send deep linking SMS notify.");
        var facadeResponse = await _workflowV3Manager.SendDeepLinkingSmsAsync(deepLinkingRequest);
        return StatusCode(facadeResponse.HttpStatusCode > 0 ? facadeResponse.HttpStatusCode : 202, facadeResponse);
    }
}
