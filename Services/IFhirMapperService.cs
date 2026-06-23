using System.Threading.Tasks;

namespace AbdmWrapperNet.Services;

public interface IFhirMapperService
{
    Task<string> GeneratePrescriptionBundleAsync(string fhirJsonPayload);
    Task<string> GenerateOPConsultationBundleAsync(string fhirJsonPayload);
    Task<string> GenerateHealthDocumentBundleAsync(string fhirJsonPayload);
    Task<string> GenerateDiagnosticReportBundleAsync(string fhirJsonPayload);
    Task<string> GenerateDischargeSummaryBundleAsync(string fhirJsonPayload);
}
