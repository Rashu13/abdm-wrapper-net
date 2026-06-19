using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ABDM.Api;
using ABDM.Models;
using MongoDB.Driver;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AbdmWrapperNet.Data;

namespace AbdmWrapperNet.Controllers;

[ApiController]
[Route("api/v3/m1")]
public class AbdmM1Controller : ControllerBase
{
    private readonly AbdmApiClient _client;
    private readonly IPatientV3Service _patientService;
    private readonly ILogger<AbdmM1Controller> _logger;

    public AbdmM1Controller(AbdmApiClient client, IPatientV3Service patientService, ILogger<AbdmM1Controller> logger)
    {
        _client = client;
        _patientService = patientService;
        _logger = logger;
    }

    [HttpPost("generate-otp")]
    public async Task<IActionResult> GenerateOtp([FromBody] AbdmGenerateOtpRequest request)
    {
        _logger.LogInformation("M1: Generating OTP request received.");

        if (request.ConsentTimestamp != null || request.Chk1 == true)
        {
            try
            {
                var consentLog = new AbhaConsentLog
                {
                    LoginId = request.LoginId ?? string.Empty,
                    ConsentTimestamp = (request.ConsentTimestamp ?? DateTime.UtcNow).ToUniversalTime(),
                    OperatorName = request.OperatorName ?? string.Empty,
                    BeneficiaryName = request.BeneficiaryName ?? string.Empty,
                    Chk1 = request.Chk1 ?? false,
                    Chk2 = request.Chk2 ?? false,
                    Chk3 = request.Chk3 ?? false,
                    Chk4 = request.Chk4 ?? false,
                    Chk5 = request.Chk5 ?? false,
                    Chk6 = request.Chk6 ?? false,
                    Chk7 = request.Chk7 ?? false,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty
                };

                // Check SQL DbContext
                var sqlDb = HttpContext.RequestServices.GetService(typeof(AbdmWrapperNet.Data.AppDbContext)) as AbdmWrapperNet.Data.AppDbContext;
                if (sqlDb != null)
                {
                    sqlDb.AbhaConsentLogs.Add(consentLog);
                    await sqlDb.SaveChangesAsync();
                }

                // Check Mongo DbContext
                var mongoDb = HttpContext.RequestServices.GetService(typeof(AbdmWrapperNet.Data.MongoDbContext)) as AbdmWrapperNet.Data.MongoDbContext;
                if (mongoDb != null)
                {
                    await mongoDb.AbhaConsentLogs.InsertOneAsync(consentLog);
                }
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Failed to log ABHA nudge consent to database.");
            }
        }

        var response = await _client.GenerateOtpAsync(request);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] AbdmVerifyOtpRequest request)
    {
        _logger.LogInformation("M1: Verifying OTP request received.");
        var response = await _client.VerifyOtpAsync(request);
        if (response.Success && response.Data != null)
        {
            await SavePatientFromProfileAsync(response.Data);
            return Ok(response);
        }
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
        if (response.Success && response.Data != null)
        {
            if (response.Data.Accounts != null)
            {
                foreach (var acc in response.Data.Accounts)
                {
                    await SavePatientFromProfileAsync(acc);
                }
            }
            return Ok(response);
        }
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
        if (response.Success)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                var profileResp = await _client.GetAbhaProfileAsync(userToken);
                if (profileResp.Success && profileResp.Data != null)
                {
                    await SavePatientFromProfileAsync(profileResp.Data);
                }
            }
            else if (response.Data != null)
            {
                var hipId = _client.Settings?.HipId ?? "MIDHA HOSPITAL";
                var patient = new Patient
                {
                    AbhaAddress = response.Data.AbhaAddress ?? abhaAddress,
                    PatientReference = response.Data.HealthIdNumber ?? string.Empty,
                    HipId = hipId,
                    PatientDisplay = abhaAddress
                };
                await _patientService.UpsertPatientsAsync(new List<Patient> { patient });
            }
            return Ok(response);
        }
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
        if (response.Success && response.Data != null)
        {
            await SavePatientFromProfileAsync(response.Data);
            return Ok(response);
        }
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

    [HttpGet("scan-share-requests")]
    public async Task<IActionResult> GetScanShareRequests()
    {
        var list = new List<object>();
        try
        {
            var sqlDb = HttpContext.RequestServices.GetService(typeof(AppDbContext)) as AppDbContext;
            if (sqlDb != null)
            {
                var logs = await sqlDb.RequestLogs
                    .Where(l => l.Module == "HIP_SCAN_AND_SHARE")
                    .OrderByDescending(l => l.CreatedOn)
                    .Take(50)
                    .ToListAsync();
                
                foreach (var log in logs)
                {
                    list.Add(new
                    {
                        Id = log.Id,
                        AbhaAddress = log.AbhaAddress,
                        CreatedOn = log.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss"),
                        Details = log.RequestDetails?.ToString() ?? "{}"
                    });
                }
            }
            else
            {
                var mongoDb = HttpContext.RequestServices.GetService(typeof(MongoDbContext)) as MongoDbContext;
                if (mongoDb != null)
                {
                    var logs = await mongoDb.RequestLogs
                        .Find(l => l.Module == "HIP_SCAN_AND_SHARE")
                        .SortByDescending(l => l.CreatedOn)
                        .Limit(50)
                        .ToListAsync();
                    
                    foreach (var log in logs)
                    {
                        list.Add(new
                        {
                            Id = log.Id,
                            AbhaAddress = log.AbhaAddress,
                            CreatedOn = log.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss"),
                            Details = log.RequestDetails?.ToString() ?? "{}"
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get scan share requests");
        }
        return Ok(list);
    }

    [HttpPost("mobile-verify-otp")]
    public async Task<IActionResult> MobileVerifyOtp([FromBody] MobileUpdateOtpRequest request, [FromHeader(Name = "X-Token")] string? userToken = null)
    {
        _logger.LogInformation("M1: Request to send mobile verification OTP.");
        if (string.IsNullOrEmpty(request.UserToken) && !string.IsNullOrEmpty(userToken))
        {
            request.UserToken = userToken;
        }
        var response = await _client.MobileUpdateSendOtpAsync(request);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("mobile-verify-confirm")]
    public async Task<IActionResult> MobileVerifyConfirm([FromBody] MobileUpdateVerifyRequest request, [FromHeader(Name = "X-Token")] string? userToken = null)
    {
        _logger.LogInformation("M1: Request to verify mobile OTP.");
        if (string.IsNullOrEmpty(request.UserToken) && !string.IsNullOrEmpty(userToken))
        {
            request.UserToken = userToken;
        }
        var response = await _client.MobileUpdateVerifyOtpAsync(request);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("email-verify-link")]
    public async Task<IActionResult> EmailVerifyLink([FromBody] EmailVerificationLinkRequest request, [FromHeader(Name = "X-Token")] string? userToken = null)
    {
        _logger.LogInformation("M1: Request for email verification link.");
        if (string.IsNullOrEmpty(request.UserToken) && !string.IsNullOrEmpty(userToken))
        {
            request.UserToken = userToken;
        }
        var response = await _client.RequestEmailVerificationLinkAsync(request);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("re-kyc-otp")]
    public async Task<IActionResult> RequestReKycOtp([FromQuery] string abhaNumber, [FromQuery] string? abhaAddress = null, [FromHeader(Name = "X-Token")] string? userToken = null)
    {
        _logger.LogInformation("M1: Requesting Re-KYC OTP.");
        if (string.IsNullOrEmpty(userToken))
        {
            return BadRequest(new { success = false, message = "X-Token header is mandatory" });
        }
        var response = await _client.RequestReKycOtpAsync(userToken, abhaNumber, abhaAddress);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("re-kyc-verify")]
    public async Task<IActionResult> VerifyReKycOtp([FromQuery] string transactionId, [FromQuery] string otp, [FromHeader(Name = "X-Token")] string? userToken = null)
    {
        _logger.LogInformation("M1: Verifying Re-KYC OTP.");
        if (string.IsNullOrEmpty(userToken))
        {
            return BadRequest(new { success = false, message = "X-Token header is mandatory" });
        }
        var response = await _client.VerifyReKycOtpAsync(userToken, transactionId, otp);
        if (response.Success) return Ok(response);
        return BadRequest(response);
    }

    private async Task SavePatientFromProfileAsync(AbhaProfile profile)
    {
        if (profile == null || string.IsNullOrEmpty(profile.AbhaAddress))
        {
            _logger.LogWarning("M1 Auto-Save: Cannot save patient, profile or ABHA address is null.");
            return;
        }

        try
        {
            var hipId = _client.Settings?.HipId ?? "MIDHA HOSPITAL";
            var patient = new Patient
            {
                AbhaAddress = profile.AbhaAddress,
                Name = profile.Name ?? $"{profile.FirstName} {profile.MiddleName} {profile.LastName}".Trim(),
                Gender = profile.Gender ?? string.Empty,
                DateOfBirth = profile.Dob ?? profile.YearOfBirth ?? string.Empty,
                PatientMobile = profile.Mobile ?? string.Empty,
                HipId = hipId,
                PatientReference = profile.HealthIdNumber ?? profile.AbhaAddress,
                PatientDisplay = profile.Name ?? $"{profile.FirstName} {profile.MiddleName} {profile.LastName}".Trim(),
                CareContexts = new List<CareContext>()
            };

            _logger.LogInformation($"M1 Auto-Save: Upserting patient {patient.AbhaAddress} in database.");
            await _patientService.UpsertPatientsAsync(new List<Patient> { patient });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"M1 Auto-Save: Failed to save patient {profile.AbhaAddress}");
        }
    }
}
