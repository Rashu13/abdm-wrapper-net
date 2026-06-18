using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;

namespace AbdmWrapperNet.Controllers;

[ApiController]
[Route("v3/patient")]
public class PatientV3Controller : ControllerBase
{
    private readonly ILogger<PatientV3Controller> _logger;
    private readonly IPatientV3Service _patientService;

    public PatientV3Controller(
        ILogger<PatientV3Controller> logger,
        IPatientV3Service patientService)
    {
        _logger = logger;
        _patientService = patientService;
    }

    /// <summary>
    /// This controller is used to fetch all the details of the patient which includes careContext+consent
    /// </summary>
    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetPatientDetails(
        [FromRoute] string patientId,
        [FromQuery] string hipId)
    {
        _logger.LogInformation($"Request to get patient details for {patientId} in hip {hipId}");
        
        if (string.IsNullOrEmpty(hipId))
        {
            return BadRequest(new FacadeV3Response
            {
                Message = "hipId is mandatory",
                HttpStatusCode = StatusCodes.Status400BadRequest
            });
        }

        var patient = await _patientService.GetPatientDetailsAsync(patientId, hipId);
        if (patient == null)
        {
            var facadeV3Response = new FacadeV3Response
            {
                HttpStatusCode = StatusCodes.Status404NotFound,
                Message = "No Patient found",
                Errors = new List<ErrorResponse>
                {
                    new ErrorResponse
                    {
                        Code = "1000",
                        Message = $"{patientId} not found in {hipId} facility"
                    }
                }
            };
            return NotFound(facadeV3Response);
        }

        return Ok(patient);
    }

    /// <summary>
    /// This controller is used by HIS/WinForms to save Health Data Record (Prescription) JSON
    /// </summary>
    [HttpPost("health-data")]
    public async Task<IActionResult> SaveHealthData([FromBody] HealthDataRecord record)
    {
        _logger.LogInformation($"Request to save health data for care context {record.CareContextReference} and ABHA {record.AbhaAddress}");
        
        if (string.IsNullOrEmpty(record.AbhaAddress) || string.IsNullOrEmpty(record.CareContextReference) || string.IsNullOrEmpty(record.FhirJsonPayload))
        {
            return BadRequest(new { Message = "AbhaAddress, CareContextReference, and FhirJsonPayload are mandatory" });
        }

        await _patientService.AddHealthDataRecordAsync(record);
        
        return Ok(new { Message = "Health data record saved successfully." });
    }
}
