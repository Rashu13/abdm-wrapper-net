using System.Threading.Tasks;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public interface IConsentPatientV3Service
{
    Task SaveConsentPatientMappingAsync(string consentId, string patientAbhaAddress, string entityType, string hipId);
    Task<ConsentPatient?> FindMappingByConsentIdAsync(string consentId, string entityType, string hipId);
}
