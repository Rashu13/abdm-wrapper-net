using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class PatientV3Service : IPatientV3Service
{
    private readonly MongoDbContext _context;
    private readonly ILogger<PatientV3Service> _logger;

    public PatientV3Service(MongoDbContext context, ILogger<PatientV3Service> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GetPatientReferenceAsync(string abhaAddress, string hipId)
    {
        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.AbhaAddress, abhaAddress),
            Builders<Patient>.Filter.Eq(p => p.HipId, hipId)
        );
        var patient = await _context.Patients.Find(filter).FirstOrDefaultAsync();
        return patient != null ? patient.PatientReference : string.Empty;
    }

    public async Task<string> GetPatientDisplayAsync(string abhaAddress, string hipId)
    {
        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.AbhaAddress, abhaAddress),
            Builders<Patient>.Filter.Eq(p => p.HipId, hipId)
        );
        var patient = await _context.Patients.Find(filter).FirstOrDefaultAsync();
        return patient != null ? patient.PatientDisplay : string.Empty;
    }

    public async Task<string> GetAbhaAddressAsync(string patientReference, string hipId)
    {
        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.PatientReference, patientReference),
            Builders<Patient>.Filter.Eq(p => p.HipId, hipId)
        );
        var patient = await _context.Patients.Find(filter).FirstOrDefaultAsync();
        return patient != null ? patient.AbhaAddress : string.Empty;
    }

    public async Task UpdateCareContextAsync(string patientReference, List<CareContext> careContexts, string hipId)
    {
        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.PatientReference, patientReference),
            Builders<Patient>.Filter.Eq(p => p.HipId, hipId)
        );

        var patient = await _context.Patients.Find(filter).FirstOrDefaultAsync();
        if (patient == null)
        {
            throw new Exception($"Patient not found in database: {patientReference}");
        }

        if (patient.CareContexts == null)
        {
            var update = Builders<Patient>.Update.Set(p => p.CareContexts, careContexts);
            await _context.Patients.UpdateOneAsync(filter, update);
            return;
        }

        var updateAddToSet = Builders<Patient>.Update.AddToSetEach(p => p.CareContexts, careContexts);
        await _context.Patients.UpdateOneAsync(filter, updateAddToSet);
    }

    public async Task UpdateCareContextStatusAsync(string patientReference, List<CareContext> careContexts, string hipId)
    {
        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.PatientReference, patientReference),
            Builders<Patient>.Filter.Eq(p => p.HipId, hipId)
        );

        var patient = await _context.Patients.Find(filter).FirstOrDefaultAsync();
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

        var update = Builders<Patient>.Update.Set(p => p.CareContexts, existingCareContexts);
        await _context.Patients.UpdateOneAsync(filter, update);
    }

    public async Task AddPatientCareContextsAsync(LinkRecordsV3Request linkRecordsRequest)
    {
        var abhaAddress = linkRecordsRequest.AbhaAddress;
        try
        {
            var filter = Builders<Patient>.Filter.And(
                Builders<Patient>.Filter.Eq(p => p.AbhaAddress, abhaAddress),
                Builders<Patient>.Filter.Eq(p => p.HipId, linkRecordsRequest.RequesterId)
            );

            var existingRecord = await _context.Patients.Find(filter).FirstOrDefaultAsync();
            if (existingRecord == null)
            {
                _logger.LogError("Adding patient failed -> Patient not found");
            }
            else
            {
                var modifiedCareContexts = linkRecordsRequest.CareContexts.Select(cc => new CareContext
                {
                    ReferenceNumber = cc.ReferenceNumber,
                    Display = cc.Display,
                    HiType = cc.HiType,
                    IsLinked = true
                }).ToList();

                var update = Builders<Patient>.Update.AddToSetEach(p => p.CareContexts, modifiedCareContexts);
                await _context.Patients.UpdateOneAsync(filter, update);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding patient care contexts");
        }
    }

    public async Task AddConsentAsync(string abhaAddress, Consent consent, string hipId)
    {
        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.AbhaAddress, abhaAddress),
            Builders<Patient>.Filter.Eq(p => p.HipId, hipId)
        );

        var patient = await _context.Patients.Find(filter).FirstOrDefaultAsync();
        if (patient == null)
        {
            _logger.LogWarning($"Patient not found in database: {abhaAddress}. Creating stub patient.");
            patient = await GetPatientAsync(abhaAddress, hipId);
            if (patient == null)
            {
                patient = new Patient
                {
                    AbhaAddress = abhaAddress,
                    HipId = hipId
                };
                await _context.Patients.InsertOneAsync(patient);
            }
        }

        var consents = patient.Consents ?? new List<Consent>();
        if (consent.ConsentDetail != null)
        {
            var exists = consents.Any(c => c.ConsentDetail != null && c.ConsentDetail.ConsentId == consent.ConsentDetail.ConsentId);
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

        // Update existing care contexts in MongoDB to set isLinked = true where context is in consent
        if (consent.ConsentDetail?.CareContexts != null && consent.ConsentDetail.CareContexts.Count > 0)
        {
            var consentRefNumbers = consent.ConsentDetail.CareContexts
                .Select(cc => cc.CareContextReference)
                .Where(r => r != null)
                .ToList();

            // C# Driver equivalent of arrayFilters to update matched elements in sub-array
            var patientWithUnlinkedMatch = await _context.Patients.Find(filter).FirstOrDefaultAsync();
            if (patientWithUnlinkedMatch != null && patientWithUnlinkedMatch.CareContexts != null)
            {
                var updatedContexts = patientWithUnlinkedMatch.CareContexts.Select(cc =>
                {
                    if (consentRefNumbers.Contains(cc.ReferenceNumber) && !cc.IsLinked)
                    {
                        cc.IsLinked = true;
                    }
                    return cc;
                }).ToList();

                var updateUnlinked = Builders<Patient>.Update.Set(p => p.CareContexts, updatedContexts);
                await _context.Patients.UpdateOneAsync(filter, updateUnlinked);
            }
        }

        // Add the new consent and any new careContexts
        var updateConsents = Builders<Patient>.Update.AddToSet(p => p.Consents, consent);
        if (newCareContexts.Count > 0)
        {
            updateConsents = Builders<Patient>.Update.Combine(
                updateConsents,
                Builders<Patient>.Update.AddToSetEach(p => p.CareContexts, newCareContexts)
            );
        }

        await _context.Patients.UpdateOneAsync(filter, updateConsents);
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
        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.AbhaAddress, abhaAddress),
            Builders<Patient>.Filter.Eq(p => p.HipId, hipId)
        );
        return await _context.Patients.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<FacadeV3Response> UpsertPatientsAsync(List<Patient> patients)
    {
        if (patients == null || patients.Count == 0)
        {
            return new FacadeV3Response { Message = "No updates were performed" };
        }

        var writeModels = new List<WriteModel<Patient>>();

        foreach (var patient in patients)
        {
            var filter = Builders<Patient>.Filter.And(
                Builders<Patient>.Filter.Eq(p => p.AbhaAddress, patient.AbhaAddress),
                Builders<Patient>.Filter.Eq(p => p.HipId, patient.HipId)
            );

            var updateList = new List<UpdateDefinition<Patient>>();

            if (!string.IsNullOrEmpty(patient.Name))
                updateList.Add(Builders<Patient>.Update.Set(p => p.Name, patient.Name));
            if (!string.IsNullOrEmpty(patient.Gender))
                updateList.Add(Builders<Patient>.Update.Set(p => p.Gender, patient.Gender));
            if (!string.IsNullOrEmpty(patient.DateOfBirth))
                updateList.Add(Builders<Patient>.Update.Set(p => p.DateOfBirth, patient.DateOfBirth));
            if (!string.IsNullOrEmpty(patient.PatientReference))
                updateList.Add(Builders<Patient>.Update.Set(p => p.PatientReference, patient.PatientReference));
            if (!string.IsNullOrEmpty(patient.PatientDisplay))
                updateList.Add(Builders<Patient>.Update.Set(p => p.PatientDisplay, patient.PatientDisplay));
            if (!string.IsNullOrEmpty(patient.PatientMobile))
                updateList.Add(Builders<Patient>.Update.Set(p => p.PatientMobile, patient.PatientMobile));

            updateList.Add(Builders<Patient>.Update.Set(p => p.AbhaAddress, patient.AbhaAddress));
            updateList.Add(Builders<Patient>.Update.Set(p => p.HipId, patient.HipId));

            var combinedUpdate = Builders<Patient>.Update.Combine(updateList);

            if (patient.CareContexts != null && patient.CareContexts.Count > 0)
            {
                combinedUpdate = Builders<Patient>.Update.Combine(
                    combinedUpdate,
                    Builders<Patient>.Update.AddToSetEach(p => p.CareContexts, patient.CareContexts)
                );
            }

            var model = new UpdateOneModel<Patient>(filter, combinedUpdate) { IsUpsert = true };
            writeModels.Add(model);
        }

        if (writeModels.Count > 0)
        {
            var result = await _context.Patients.BulkWriteAsync(writeModels);
            var affectedCount = result.Upserts.Count > 0 ? result.Upserts.Count : result.ModifiedCount;
            return new FacadeV3Response 
            { 
                Message = $"Successfully upserted {affectedCount} patients" 
            };
        }

        return new FacadeV3Response { Message = "No updates were performed" };
    }

    public async Task UpdatePatientConsentAsync(string abhaAddress, string consentId, string consentStatus, string lastUpdated, string hipId)
    {
        var filter = Builders<Patient>.Filter.And(
            Builders<Patient>.Filter.Eq(p => p.AbhaAddress, abhaAddress),
            Builders<Patient>.Filter.Eq("consents.consentDetail.consentId", consentId),
            Builders<Patient>.Filter.Eq(p => p.HipId, hipId)
        );

        var patient = await _context.Patients.Find(filter).FirstOrDefaultAsync();
        if (patient != null && patient.Consents != null)
        {
            foreach (var consent in patient.Consents)
            {
                if (consent.ConsentDetail != null && consent.ConsentDetail.ConsentId == consentId)
                {
                    consent.Status = consentStatus;
                    consent.LastUpdatedOn = lastUpdated;
                }
            }

            var update = Builders<Patient>.Update.Set(p => p.Consents, patient.Consents);
            var result = await _context.Patients.UpdateOneAsync(filter, update);
            _logger.LogDebug($"Consent update result: {result.ModifiedCount} modified");
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
        // Simple fallback since HIP Client calls are stubs for now
        return await GetPatientDetailsAsync(abhaAddress, hipId);
    }
}
