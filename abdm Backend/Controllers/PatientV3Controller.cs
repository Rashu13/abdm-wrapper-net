using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;
using AbdmWrapperNet.Services;

namespace AbdmWrapperNet.Controllers;

[ApiController]
[Route("v3/patient")]
public class PatientV3Controller : ControllerBase
{
    private readonly ILogger<PatientV3Controller> _logger;
    private readonly IPatientV3Service _patientService;
    private readonly AbdmConfig _abdmConfig;

    public PatientV3Controller(
        ILogger<PatientV3Controller> logger,
        IPatientV3Service patientService,
        IOptions<AbdmConfig> abdmConfig)
    {
        _logger = logger;
        _patientService = patientService;
        _abdmConfig = abdmConfig.Value;
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
        
        // Handle patient registration without ABHA address using MobileNo + PatientName
        if (string.IsNullOrEmpty(record.AbhaAddress) || record.AbhaAddress.StartsWith("mobile-"))
        {
            if (string.IsNullOrEmpty(record.AbhaAddress))
            {
                if (!string.IsNullOrEmpty(record.MobileNo))
                {
                    record.AbhaAddress = $"mobile-{record.MobileNo}@sbx";
                }
            }
            else if (record.AbhaAddress.StartsWith("mobile-"))
            {
                record.MobileNo = record.AbhaAddress.Replace("mobile-", "").Replace("@sbx", "");
            }

            if (!string.IsNullOrEmpty(record.MobileNo))
            {
                var dummyAbha = record.AbhaAddress;

                // Read direct parameters sent by the UI
                string patientGender = record.Gender ?? string.Empty;
                string patientDob = record.DateOfBirth ?? string.Empty;

                // Fallback to parsing from FhirJsonPayload if parameters are missing
                if (string.IsNullOrEmpty(patientGender) || string.IsNullOrEmpty(patientDob) || string.IsNullOrEmpty(record.PatientName))
                {
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(record.FhirJsonPayload);
                        if (doc.RootElement.TryGetProperty("patient", out var patientProp))
                        {
                            if (string.IsNullOrEmpty(record.PatientName) && patientProp.TryGetProperty("name", out var nameProp))
                            {
                                record.PatientName = nameProp.GetString();
                            }
                            if (string.IsNullOrEmpty(patientGender) && patientProp.TryGetProperty("gender", out var genderProp))
                            {
                                patientGender = genderProp.GetString() ?? string.Empty;
                            }
                            if (string.IsNullOrEmpty(patientDob) && patientProp.TryGetProperty("birthDate", out var dobProp))
                            {
                                patientDob = dobProp.GetString() ?? string.Empty;
                            }
                        }
                    }
                    catch {}
                }

                // Standardize gender code for gateway compatibility
                if (!string.IsNullOrEmpty(patientGender))
                {
                    patientGender = patientGender.Trim().ToUpper();
                    if (patientGender == "MALE" || patientGender == "M") patientGender = "M";
                    else if (patientGender == "FEMALE" || patientGender == "F") patientGender = "F";
                    else if (patientGender == "OTHER" || patientGender == "O") patientGender = "O";
                }

                // If gender or dateOfBirth remain completely unspecified, return a validation bad request
                if (string.IsNullOrEmpty(patientGender) || string.IsNullOrEmpty(patientDob))
                {
                    return BadRequest(new { Message = "Gender and DateOfBirth are mandatory for patients without an ABHA address." });
                }

                var existingPatient = await _patientService.GetPatientAsync(dummyAbha, _abdmConfig.HipId);
                if (existingPatient == null)
                {
                    var newPatient = new Patient
                    {
                        AbhaAddress = dummyAbha,
                        PatientMobile = record.MobileNo,
                        Name = record.PatientName ?? "Unknown Patient",
                        Gender = patientGender,
                        DateOfBirth = patientDob,
                        PatientReference = record.CareContextReference ?? Guid.NewGuid().ToString(),
                        PatientDisplay = record.PatientName ?? "Unknown Patient",
                        HipId = _abdmConfig.HipId,
                        CareContexts = new List<CareContext>
                        {
                            new CareContext
                            {
                                ReferenceNumber = record.CareContextReference,
                                Display = record.RecordType == "DischargeSummary" ? "Discharge Summary" : "Prescription Record",
                                HiType = record.RecordType,
                                IsLinked = false
                            }
                        }
                    };
                    await _patientService.UpsertPatientsAsync(new List<Patient> { newPatient });
                }
                else
                {
                    bool needsUpdate = false;
                    if (existingPatient.Gender != patientGender)
                    {
                        existingPatient.Gender = patientGender;
                        needsUpdate = true;
                    }
                    if (existingPatient.DateOfBirth != patientDob)
                    {
                        existingPatient.DateOfBirth = patientDob;
                        needsUpdate = true;
                    }

                    if (existingPatient.CareContexts == null) existingPatient.CareContexts = new List<CareContext>();
                    if (!existingPatient.CareContexts.Any(c => c.ReferenceNumber == record.CareContextReference))
                    {
                        existingPatient.CareContexts.Add(new CareContext
                        {
                            ReferenceNumber = record.CareContextReference,
                            Display = record.RecordType == "DischargeSummary" ? "Discharge Summary" : "Prescription Record",
                            HiType = record.RecordType,
                            IsLinked = false
                        });
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        await _patientService.UpsertPatientsAsync(new List<Patient> { existingPatient });
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(record.AbhaAddress) || string.IsNullOrEmpty(record.CareContextReference) || string.IsNullOrEmpty(record.FhirJsonPayload))
        {
            return BadRequest(new { Message = "AbhaAddress (or MobileNo), CareContextReference, and FhirJsonPayload are mandatory" });
        }

        record.CreatedAt = DateTime.UtcNow;
        await _patientService.AddHealthDataRecordAsync(record);
        
        return Ok(new { Message = "Health data record saved successfully." });
    }

    /// <summary>
    /// Fetches all saved health data records for a specific ABHA address from local database
    /// </summary>
    [HttpGet("health-data")]
    public async Task<IActionResult> GetHealthDataByAbha([FromQuery] string abhaAddress)
    {
        if (string.IsNullOrEmpty(abhaAddress))
        {
            return BadRequest(new { Message = "AbhaAddress query parameter is required" });
        }

        var records = await _patientService.GetHealthDataRecordsByAbhaAsync(abhaAddress);
        return Ok(records);
    }

    /// <summary>
    /// Returns all registered patients for the current HIP from local database.
    /// Used by the Patient Registry dashboard in the frontend.
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> GetAllPatients([FromQuery] string? hipId = null)
    {
        var resolvedHipId = hipId ?? _abdmConfig.HipId;
        _logger.LogInformation($"Fetching all patients for HIP: {resolvedHipId}");

        var patients = await _patientService.GetAllPatientsByHipIdAsync(resolvedHipId);
        
        var result = patients.Select(p => new
        {
            abhaAddress      = p.AbhaAddress,
            abhaNumber       = p.AbhaNumber,
            pincode          = p.Pincode,
            name             = p.Name,
            patientReference = p.PatientReference,
            patientDisplay   = p.PatientDisplay,
            gender           = p.Gender,
            dateOfBirth      = p.DateOfBirth,
            mobile           = p.PatientMobile,
            hipId            = p.HipId,
            careContextCount = p.CareContexts?.Count ?? 0,
        });

        return Ok(result);
    }

    /// <summary>
    /// Returns all care contexts registered across all patients for the current HIP from local database.
    /// Used to show the linked records feed in the Care Context tab.
    /// </summary>
    [HttpGet("care-contexts")]
    public async Task<IActionResult> GetAllCareContexts([FromQuery] string? hipId = null)
    {
        var resolvedHipId = hipId ?? _abdmConfig.HipId;
        _logger.LogInformation($"Fetching all care contexts for HIP: {resolvedHipId}");

        var patients = await _patientService.GetAllPatientsByHipIdAsync(resolvedHipId);
        var list = new List<object>();

        foreach (var p in patients)
        {
            if (p.CareContexts != null)
            {
                foreach (var cc in p.CareContexts)
                {
                    list.Add(new
                    {
                        referenceNumber = cc.ReferenceNumber,
                        display = $"{cc.Display} - {p.Name} ({p.AbhaAddress})",
                        type = cc.HiType,
                        date = DateTime.UtcNow.ToString("yyyy-MM-dd")
                    });
                }
            }
        }

        return Ok(list);
    }
}
