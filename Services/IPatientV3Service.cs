using System.Collections.Generic;
using System.Threading.Tasks;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public interface IPatientV3Service
{
    Task<string> GetPatientReferenceAsync(string abhaAddress, string hipId);
    Task<string> GetPatientDisplayAsync(string abhaAddress, string hipId);
    Task<string> GetAbhaAddressAsync(string patientReference, string hipId);
    Task UpdateCareContextAsync(string patientReference, List<CareContext> careContexts, string hipId);
    Task UpdateCareContextStatusAsync(string patientReference, List<CareContext> careContexts, string hipId);
    Task AddPatientCareContextsAsync(LinkRecordsV3Request linkRecordsRequest);
    Task AddConsentAsync(string abhaAddress, Consent consent, string hipId);
    Task<Consent?> GetConsentDetailsAsync(string abhaAddress, string consentId, string hipId);
    Task<Patient?> GetPatientDetailsAsync(string abhaAddress, string hipId);
    Task UpdatePatientConsentAsync(string abhaAddress, string consentId, string consentStatus, string lastUpdated, string hipId);
    Task<bool> CheckCareContextsAsync(string abhaAddress, string hipId, List<CareContext> careContexts);
    Task<List<CareContext>> GetSameCareContextsAsync(string abhaAddress, string hipId, List<CareContext> careContexts);
    Task<HealthDataRecord?> GetHealthDataRecordAsync(string abhaAddress, string careContextReference);
    Task AddHealthDataRecordAsync(HealthDataRecord record);
    Task<bool> IsConsentValidAsync(string abhaAddress, string consentId, string hipId);
    Task<Patient?> GetPatientAsync(string abhaAddress, string hipId);
    Task<FacadeV3Response> UpsertPatientsAsync(List<Patient> patients);
    Task<Patient?> GetPatientByConsentIdAsync(string consentId);
}
