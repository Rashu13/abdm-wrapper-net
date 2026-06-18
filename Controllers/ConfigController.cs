using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Services;

namespace AbdmWrapperNet.Controllers;

[ApiController]
[Route("v3/config")]
public class ConfigController : ControllerBase
{
    private readonly IGatewayClient _gatewayClient;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(IGatewayClient gatewayClient, ILogger<ConfigController> logger)
    {
        _gatewayClient = gatewayClient;
        _logger = logger;
    }

    public class BridgeUrlRequest
    {
        public string url { get; set; } = string.Empty;
    }

    /// <summary>
    /// Update or Register Bridge URL with ABDM Gateway
    /// </summary>
    [HttpPost("register-bridge")]
    public async Task<IActionResult> RegisterBridgeUrl([FromBody] BridgeUrlRequest request)
    {
        _logger.LogInformation($"Registering bridge URL: {request.url}");
        
        // This will automatically fetch the token and use PATCH internally
        var response = await _gatewayClient.PatchToGatewayAsync("hiecm/gateway/v3/bridge/url", request);
        
        if (response != null && response.HttpStatus == "OK")
        {
            return Ok(new { message = "Bridge URL updated successfully!" });
        }

        return BadRequest(response);
    }

    /// <summary>
    /// Check the registered BRIDGE URL and Facilities
    /// </summary>
    [HttpGet("bridge-services")]
    public async Task<IActionResult> CheckBridgeServices()
    {
        _logger.LogInformation("Checking bridge services.");
        
        var responseString = await _gatewayClient.GetFromGatewayAsync("hiecm/gateway/v3/bridge-services");
        
        return Content(responseString, "application/json");
    }
}
