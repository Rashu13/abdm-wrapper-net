using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Constants;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;

using AbdmWrapperNet.Filters;

namespace AbdmWrapperNet.Controllers;

[ApiController]
[TypeFilter(typeof(GatewaySignatureValidationFilter))]
public class GatewayCallbackV3Controller : ControllerBase
{
    private readonly IWorkflowV3Manager _workflowV3Manager;
    private readonly IRequestLogV3Service _requestLogV3Service;
    private readonly ILogger<GatewayCallbackV3Controller> _logger;

    public GatewayCallbackV3Controller(
        IWorkflowV3Manager workflowV3Manager,
        IRequestLogV3Service requestLogV3Service,
        ILogger<GatewayCallbackV3Controller> logger)
    {
        _workflowV3Manager = workflowV3Manager;
        _requestLogV3Service = requestLogV3Service;
        _logger = logger;
    }

    [HttpPost(GatewayUrl.DiscoverPath)]
    public async Task<IActionResult> Discover([FromBody] DiscoverRequest discoverRequest)
    {
        if (discoverRequest != null && discoverRequest.Error == null)
        {
            Request.Headers.TryGetValue(GatewayConstants.XHipId, out var hipId);
            discoverRequest.HipId = hipId.ToString();
            
            var result = await _workflowV3Manager.DiscoverAsync(discoverRequest, Request.Headers);
            return StatusCode(StatusCodes.Status200OK, result);
        }
        
        _logger.LogError($"{GatewayUrl.DiscoverPath}: {discoverRequest?.Error?.Message}");
        return BadRequest();
    }

    [HttpPost(GatewayUrl.InitLinkingPath)]
    public async Task<IActionResult> InitCall([FromBody] InitV3Response initResponse)
    {
        if (initResponse != null)
        {
            _logger.LogInformation($"{GatewayUrl.InitLinkingPath}: {initResponse}");
            await _workflowV3Manager.InitiateOnInitAsync(initResponse, Request.Headers);
            return Accepted();
        }

        _logger.LogError($"{GatewayUrl.InitLinkingPath}: ERROR");
        return BadRequest();
    }

    [HttpPost(GatewayUrl.ConfirmLinkingPath)]
    public async Task<IActionResult> ConfirmCall([FromBody] ConfirmResponse confirmResponse)
    {
        if (confirmResponse != null && confirmResponse.Error == null)
        {
            _logger.LogInformation($"{GatewayUrl.ConfirmLinkingPath}: {confirmResponse}");
            await _workflowV3Manager.InitiateOnConfirmCallAsync(confirmResponse, Request.Headers);
            return Accepted();
        }

        _logger.LogError($"{GatewayUrl.ConfirmLinkingPath}: Error {confirmResponse?.Error?.Message}");
        return BadRequest();
    }

    [HttpPost(GatewayUrl.HipConsentNotifyPath)]
    public async Task<IActionResult> HipNotify([FromBody] HIPNotifyRequest hipNotifyRequest)
    {
        if (hipNotifyRequest != null)
        {
            await _workflowV3Manager.HipNotifyAsync(hipNotifyRequest, Request.Headers);
            return Accepted();
        }

        _logger.LogDebug("Error in response of Consent Notify");
        return BadRequest();
    }

    [HttpPost(GatewayUrl.HipHealthInformationRequestPath)]
    public async Task<IActionResult> HealthInformation([FromBody] HIPHealthInformationRequest hipHealthInformationRequest)
    {
        if (hipHealthInformationRequest != null)
        {
            await _workflowV3Manager.HealthInformationAsync(hipHealthInformationRequest, Request.Headers);
            return Accepted();
        }

        _logger.LogDebug("Invalid Data request response");
        return BadRequest();
    }

    [HttpPost(GatewayUrl.ProfileSharePath)]
    public async Task<IActionResult> ProfileShare([FromBody] ProfileShareV3Request profileShare)
    {
        if (profileShare != null)
        {
            await _workflowV3Manager.ProfileShareAsync(profileShare, Request.Headers);
            return Accepted();
        }

        _logger.LogDebug("Invalid profile share request");
        return BadRequest();
    }

    [HttpPost(GatewayUrl.LinkContextOnNotifyPath)]
    public IActionResult ContextOnNotify([FromBody] StatusResponse statusResponse)
    {
        _logger.LogInformation(statusResponse.ToString());
        return Accepted();
    }

    [HttpPost(GatewayUrl.DeepLinkingOnNotifyPath)]
    public IActionResult DeepLinkingOnNotify([FromBody] StatusResponse statusResponse)
    {
        _logger.LogInformation(statusResponse.ToString());
        return Accepted();
    }

    [HttpPost(GatewayUrl.OnAddCareContextPath)]
    public async Task<IActionResult> OnAddCareContext([FromBody] LinkOnAddCareContextsV3Response response)
    {
        if (response != null && response.Error != null)
        {
            await _requestLogV3Service.UpdateErrorAsync(
                response.Response?.RequestId ?? string.Empty,
                response.Error,
                "AUTH_ON_ADD_CARE_CONTEXT_ERROR"
            );
            return Accepted();
        }
        else if (response != null)
        {
            _logger.LogDebug(response.ToString());
            await _requestLogV3Service.SetHipOnAddCareContextResponseAsync(response);
            return Accepted();
        }

        var error = "Got Error in onAddCareContext callback: gateway response was null";
        return BadRequest(new GatewayCallbackResponse 
        { 
            Error = new ErrorResponse { Code = GatewayConstants.ErrorCode, Message = error } 
        });
    }

    [HttpPost(GatewayUrl.OnGenerateLinkTokenPath)]
    public async Task<IActionResult> OnGenerateToken([FromBody] OnGenerateTokenResponse response)
    {
        if (response != null && response.Error != null)
        {
            _logger.LogInformation("Updating error");
            await _requestLogV3Service.LinkTokenUpdateErrorAsync(
                response.Response?.RequestId ?? string.Empty,
                response.Error,
                "LINK_TOKEN_REQUEST_ERROR"
            );
            return Accepted();
        }
        else if (response != null)
        {
            await _workflowV3Manager.HandleAddCareContextsAsync(response, Request.Headers);
            return Accepted();
        }

        var error = "Got Error in on-generate-token callback: gateway response was null";
        return BadRequest(new GatewayCallbackResponse 
        { 
            Error = new ErrorResponse { Code = GatewayConstants.ErrorCode, Message = error } 
        });
    }
}
