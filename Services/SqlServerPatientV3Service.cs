using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class SqlServerPatientV3Service : IPatientV3Service
{
    private readonly AppDbContext _context;
    private readonly ILogger<SqlServerPatientV3Service> _logger;

    public SqlServerPatientV3Service(AppDbContext context, ILogger<SqlServerPatientV3Service> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GetPatientReferenceAsync(string abhaAddress, string hipId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.AbhaAddress == abhaAddress && p.HipId == hipId);
        return patient != null ? patient.PatientReference : string.Empty;
    }

    public async Task<string> GetPatientDisplayAsync(string abhaAddress, string hipId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.AbhaAddress == abhaAddress && p.HipId == hipId);
        return patient != null ? patient.PatientDisplay : string.Empty;
    }

    public async Task<string> GetAbhaAddressAsync(string patientReference, string hipId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientReference == patientReference && p.HipId == hipId);
        return patient != null ? patient.AbhaAddress : string.Empty;
    }

    public async Task UpdateCareContextAsync(string patientReference, List<CareContext> careContexts, string hipId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientReference == patientReference && p.HipId == hipId);
        if (patient == null)
        {
            throw new Exception($"Patient not found in database: {patientReference}");
        }

        if (patient.CareContexts == null)
        {
            patient.CareContexts = new List<CareContext>();
        }

        foreach (var cc in careContexts)
        {
            if (!patient.CareContexts.Any(existing => existing.ReferenceNumber == cc.ReferenceNumber))
            {
                patient.CareContexts.Add(cc);
            }
        }

        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCareContextStatusAsync(string patientReference, List<CareContext> careContexts, string hipId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientReference == patientReference && p.HipId == hipId);
        if (patient == null)
        {
            _logger.LogError($"Patient not found with reference: {patientReference} and facility: {hipId}");
            return;
        }

        var existingCareContexts = patient.CareContexts;
        if (existingCareContexts == null)
        {
            _logger.LogInformation($"No care contexts found for patient reference: {patientReference}");
            return;
        }

        foreach (var existingContext in existingCareContexts)
        {
            foreach (var careContext in careContexts)
            {
                if (existingContext.ReferenceNumber == careContext.ReferenceNumber)
                {
                    existingContext.IsLinked = true;
                }
            }
        }

        patient.CareContexts = existingCareContexts;
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task AddPatientCareContextsAsync(LinkRecordsV3Request linkRecordsRequest)
    {
        var abhaAddress = linkRecordsRequest.AbhaAddress;
        try
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.AbhaAddress == abhaAddress && p.HipId == linkRecordsRequest.RequesterId);

            if (patient == null)
            {
                _logger.LogError("Adding patient failed -> Patient not found");
            }
            else
            {
                if (patient.CareContexts == null)
                {
                    patient.CareContexts = new List<CareContext>();
                }

                foreach (var cc in linkRecordsRequest.CareContexts)
                {
                    if (!patient.CareContexts.Any(existing => existing.ReferenceNumber == cc.ReferenceNumber))
                    {
                        patient.CareContexts.Add(new CareContext
                        {
                            ReferenceNumber = cc.ReferenceNumber,
                            Display = cc.Display,
                            HiType = cc.HiType,
                            IsLinked = true
                        });
                    }
                    else
                    {
                        var match = patient.CareContexts.First(existing => existing.ReferenceNumber == cc.ReferenceNumber);
                        match.IsLinked = true;
                    }
                }

                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding patient care contexts");
        }
    }

    public async Task AddConsentAsync(string abhaAddress, Consent consent, string hipId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.AbhaAddress == abhaAddress && p.HipId == hipId);
        
        if (patient == null)
        {
            _logger.LogWarning($"Patient not found in database: {abhaAddress}. Creating stub patient.");
            patient = new Patient
            {
                AbhaAddress = abhaAddress,
                HipId = hipId
            };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        if (patient.Consents == null)
        {
            patient.Consents = new List<Consent>();
        }

        if (consent.ConsentDetail != null)
        {
            var exists = patient.Consents.Any(c => c.ConsentDetail != null && c.ConsentDetail.ConsentId == consent.ConsentDetail.ConsentId);
            if (exists)
            {
                _logger.LogWarning($"Consent {consent.ConsentDetail.ConsentId} already exists for patient {abhaAddress}.");
                return;
            }
        }

        var existingCareContexts = (patient.CareContexts ?? new List<CareContext>())
            .Select(cc => cc.ReferenceNumber)
            .Where(refNo => refNo != null)
            .ToHashSet();

        var newCareContexts = new List<CareContext>();
        if (consent.ConsentDetail?.CareContexts != null)
        {
            foreach (var cc in consent.ConsentDetail.CareContexts)
            {
                if (cc.CareContextReference != null && !existingCareContexts.Contains(cc.CareContextReference))
                {
                    newCareContexts.Add(new CareContext
                    {
                        ReferenceNumber = cc.CareContextReference,
                        Display = $"Added careContext from consent: {consent.ConsentDetail.ConsentId}",
                        IsLinked = true,
                        HiType = "HealthDocumentRecord"
                    });
                }
            }
        }

        if (consent.ConsentDetail?.CareContexts != null && consent.ConsentDetail.CareContexts.Count > 0)
        {
            var consentRefNumbers = consent.ConsentDetail.CareContexts
                .Select(cc => cc.CareContextReference)
                .Where(r => r != null)
                .ToList();

            if (patient.CareContexts != null)
            {
                foreach (var cc in patient.CareContexts)
                {
                    if (consentRefNumbers.Contains(cc.ReferenceNumber) && !cc.IsLinked)
                    {
                        cc.IsLinked = true;
                    }
                }
            }
        }

        patient.Consents.Add(consent);
        if (newCareContexts.Count > 0)
        {
            if (patient.CareContexts == null) patient.CareContexts = new List<CareContext>();
            patient.CareContexts.AddRange(newCareContexts);
        }

        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task<Consent?> GetConsentDetailsAsync(string abhaAddress, string consentId, string hipId)
    {
        var patient = await GetPatientDetailsAsync(abhaAddress, hipId);
        if (patient == null)
        {
            throw new Exception($"Patient not found in database: {abhaAddress}");
        }

        return patient.Consents?.FirstOrDefault(c => c.ConsentDetail != null && c.ConsentDetail.ConsentId == consentId);
    }

    public async Task<Patient?> GetPatientDetailsAsync(string abhaAddress, string hipId)
    {
        return await _context.Patients
            .FirstOrDefaultAsync(p => p.AbhaAddress == abhaAddress && p.HipId == hipId);
    }

    public async Task<FacadeV3Response> UpsertPatientsAsync(List<Patient> patients)
    {
        if (patients == null || patients.Count == 0)
        {
            return new FacadeV3Response { Message = "No updates were performed" };
        }

        int affectedCount = 0;
        foreach (var patient in patients)
        {
            var existing = await _context.Patients
                .FirstOrDefaultAsync(p => p.AbhaAddress == patient.AbhaAddress && p.HipId == patient.HipId);

            if (existing == null)
            {
                await _context.Patients.AddAsync(patient);
                affectedCount++;
            }
            else
            {
                if (!string.IsNullOrEmpty(patient.Name)) existing.Name = patient.Name;
                if (!string.IsNullOrEmpty(patient.Gender)) existing.Gender = patient.Gender;
                if (!string.IsNullOrEmpty(patient.DateOfBirth)) existing.DateOfBirth = patient.DateOfBirth;
                if (!string.IsNullOrEmpty(patient.PatientReference)) existing.PatientReference = patient.PatientReference;
                if (!string.IsNullOrEmpty(patient.PatientDisplay)) existing.PatientDisplay = patient.PatientDisplay;
                if (!string.IsNullOrEmpty(patient.PatientMobile)) existing.PatientMobile = patient.PatientMobile;

                if (patient.CareContexts != null && patient.CareContexts.Count > 0)
                {
                    if (existing.CareContexts == null) existing.CareContexts = new List<CareContext>();
                    foreach (var cc in patient.CareContexts)
                    {
                        if (!existing.CareContexts.Any(e => e.ReferenceNumber == cc.ReferenceNumber))
                        {
                            existing.CareContexts.Add(cc);
                        }
                    }
                }

                _context.Patients.Update(existing);
                affectedCount++;
            }
        }

        await _context.SaveChangesAsync();
        return new FacadeV3Response 
        { 
            Message = $"Successfully upserted {affectedCount} patients" 
        };
    }

    public async Task UpdatePatientConsentAsync(string abhaAddress, string consentId, string consentStatus, string lastUpdated, string hipId)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.AbhaAddress == abhaAddress && p.HipId == hipId);

        if (patient != null && patient.Consents != null)
        {
            bool modified = false;
            foreach (var consent in patient.Consents)
            {
                if (consent.ConsentDetail != null && consent.ConsentDetail.ConsentId == consentId)
                {
                    consent.Status = consentStatus;
                    consent.LastUpdatedOn = lastUpdated;
                    modified = true;
                }
            }

            if (modified)
            {
                _context.Patients.Update(patient);
                await _context.SaveChangesAsync();
                _logger.LogDebug($"Consent updated in SQL Server database");
            }
        }
    }

    public async Task<bool> CheckCareContextsAsync(string abhaAddress, string hipId, List<CareContext> careContexts)
    {
        var patient = await GetPatientDetailsAsync(abhaAddress, hipId);
        if (patient == null || patient.CareContexts == null)
        {
            return false;
        }

        return patient.CareContexts.Any(ec => careContexts.Any(cc => cc.ReferenceNumber == ec.ReferenceNumber && cc.HiType == ec.HiType));
    }

    public async Task<List<CareContext>> GetSameCareContextsAsync(string abhaAddress, string hipId, List<CareContext> careContexts)
    {
        var patient = await GetPatientDetailsAsync(abhaAddress, hipId);
        if (patient == null || patient.CareContexts == null)
        {
            return new List<CareContext>();
        }

        bool allNew = careContexts.All(cc => !patient.CareContexts.Any(ec => cc.ReferenceNumber == ec.ReferenceNumber && cc.HiType == ec.HiType && ec.IsLinked));
        if (allNew)
        {
            return new List<CareContext>();
        }

        return patient.CareContexts.Where(ec => careContexts.Any(cc => cc.ReferenceNumber == ec.ReferenceNumber && cc.HiType == ec.HiType && ec.IsLinked)).ToList();
    }

    public async Task<bool> IsConsentValidAsync(string abhaAddress, string consentId, string hipId)
    {
        var consent = await GetConsentDetailsAsync(abhaAddress, consentId, hipId);
        if (consent?.ConsentDetail?.Permission == null)
            return false;

        return !Utils.CheckExpiry(consent.ConsentDetail.Permission.DataEraseAt);
    }

    public async Task<Patient?> GetPatientAsync(string abhaAddress, string hipId)
    {
        return await GetPatientDetailsAsync(abhaAddress, hipId);
    }

    public async Task<HealthDataRecord?> GetHealthDataRecordAsync(string abhaAddress, string careContextReference)
    {
        return await _context.HealthDataRecords
            .FirstOrDefaultAsync(h => h.AbhaAddress == abhaAddress && h.CareContextReference == careContextReference);
    }

    public async Task AddHealthDataRecordAsync(HealthDataRecord record)
    {
        var existing = await _context.HealthDataRecords
            .FirstOrDefaultAsync(h => h.AbhaAddress == record.AbhaAddress && h.CareContextReference == record.CareContextReference);
        
        if (existing != null)
        {
            existing.FhirJsonPayload = record.FhirJsonPayload;
            existing.RecordType = record.RecordType;
            if (existing.CreatedAt.Kind == DateTimeKind.Unspecified)
            {
                existing.CreatedAt = DateTime.SpecifyKind(existing.CreatedAt, DateTimeKind.Utc);
            }
            _context.HealthDataRecords.Update(existing);
        }
        else
        {
            await _context.HealthDataRecords.AddAsync(record);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<Patient?> GetPatientByConsentIdAsync(string consentId)
    {
        // Since Consents is stored as a JSON string via ValueConverter, we can't use .Any() in LINQ to SQL directly.
        // We use string match since consentId is a unique UUID.
        var patients = await _context.Patients.ToListAsync(); // Fallback if EF.Property fails
        return patients.FirstOrDefault(p => p.Consents != null && p.Consents.Any(c => c.ConsentDetail != null && c.ConsentDetail.ConsentId == consentId));
    }
}
