using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ABDM.Api;
using ABDM.Models;

namespace AbdmWrapperNet.Controllers;

[ApiController]
[Route("api/v3/m1")]
public class AbdmM1Controller : ControllerBase
{
    private readonly AbdmApiClient _client;
    private readonly ILogger<AbdmM1Controller> _logger;

    public AbdmM1Controller(AbdmApiClient client, ILogger<AbdmM1Controller> logger)
    {
        _client = client;
        _logger = logger;
    }

    [HttpPost("generate-otp")]
    public async Task<IActionResult> GenerateOtp([FromBody] AbdmGenerateOtpRequest request)
    {
        _logger.LogInformation("M1: Generating OTP request received.");
        var response = await _client.GenerateOtpAsync(request);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] AbdmVerifyOtpRequest request)
    {
        _logger.LogInformation("M1: Verifying OTP request received.");
        var response = await _client.VerifyOtpAsync(request);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("login-otp")]
    public async Task<IActionResult> LoginOtp([FromBody] AbdmGenerateOtpRequest request)
    {
        _logger.LogInformation("M1: Login request OTP received.");
        var response = await _client.LoginRequestOtpAsync(request);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("login-verify")]
    public async Task<IActionResult> LoginVerify([FromBody] AbdmVerifyOtpRequest request)
    {
        _logger.LogInformation("M1: Login verify OTP received.");
        var response = await _client.LoginVerifyOtpAsync(request);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("suggestions/{txnId}")]
    public async Task<IActionResult> GetSuggestions([FromRoute] string txnId)
    {
        _logger.LogInformation($"M1: Fetching ABHA suggestions for txnId: {txnId}");
        var response = await _client.GetAbhaSuggestionsAsync(txnId);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("create-abha")]
    public async Task<IActionResult> CreateAbhaAddress([FromQuery] string txnId, [FromQuery] string abhaAddress, [FromHeader(Name = "X-Token")] string? userToken = null)
    {
        _logger.LogInformation($"M1: Creating ABHA address {abhaAddress} for txnId: {txnId}");
        var response = await _client.CreateAbhaAddressAsync(txnId, abhaAddress, userToken);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile([FromHeader(Name = "X-Token")] string userToken)
    {
        _logger.LogInformation("M1: Fetching ABHA profile.");
        if (string.IsNullOrEmpty(userToken))
        {
            return BadRequest(new { success = false, message = "X-Token header is mandatory" });
        }
        var response = await _client.GetAbhaProfileAsync(userToken);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("card")]
    public async Task<IActionResult> GetCard([FromHeader(Name = "X-Token")] string userToken)
    {
        _logger.LogInformation("M1: Fetching ABHA card.");
        if (string.IsNullOrEmpty(userToken))
        {
            return BadRequest(new { success = false, message = "X-Token header is mandatory" });
        }
        var response = await _client.GetAbhaCardAsync(userToken);
        if (response.Success)
        {
            try
            {
                byte[] cardBytes = Convert.FromBase64String(response.Data.Content);
                return File(cardBytes, response.Data.ContentType ?? "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error parsing card: " + ex.Message });
            }
        }
        return BadRequest(response);
    }
}
