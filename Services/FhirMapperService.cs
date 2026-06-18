using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace AbdmWrapperNet.Services;

public class FhirMapperService : IFhirMapperService
{
    public Task<string> GeneratePrescriptionBundleAsync(string fhirJsonPayload)
    {
        // Parse the simple input JSON
        using var document = JsonDocument.Parse(fhirJsonPayload);
        var root = document.RootElement;

        var careContextReference = GetString(root, "careContextReference") ?? Guid.NewGuid().ToString();
        var authoredOn = GetString(root, "authoredOn") ?? DateTime.UtcNow.ToString("o");
        var patientElement = root.GetProperty("patient");
        var patientName = GetString(patientElement, "name") ?? "Unknown";
        var patientRef = GetString(patientElement, "patientReference") ?? "Patient-1";
        
        var practitionersElement = root.GetProperty("practitioners");
        var orgElement = root.GetProperty("organisation");
        var prescriptionsElement = root.GetProperty("prescriptions");

        // 1. Create Patient
        var patient = new Patient
        {
            Id = patientRef,
            Name = new List<HumanName> { new HumanName { Text = patientName } }
        };

        var genderStr = GetString(patientElement, "gender");
        if (Enum.TryParse<AdministrativeGender>(genderStr, true, out var gender))
        {
            patient.Gender = gender;
        }

        var birthDateStr = GetString(patientElement, "birthDate");
        if (!string.IsNullOrEmpty(birthDateStr))
        {
            patient.BirthDate = birthDateStr;
        }

        // 2. Create Practitioner
        var practitionerId = "PR-1";
        var practitionerName = "Doctor";
        if (practitionersElement.GetArrayLength() > 0)
        {
            var p = practitionersElement[0];
            practitionerName = GetString(p, "name") ?? "Doctor";
            practitionerId = GetString(p, "practitionerId") ?? "PR-1";
        }
        var practitioner = new Practitioner
        {
            Id = practitionerId,
            Name = new List<HumanName> { new HumanName { Text = practitionerName } }
        };

        // 3. Create Organization
        var orgName = GetString(orgElement, "facilityName") ?? "Hospital";
        var orgId = GetString(orgElement, "facilityId") ?? "IN-1";
        var organization = new Organization
        {
            Id = orgId,
            Name = orgName,
            Identifier = new List<Identifier>
            {
                new Identifier { System = "https://facility.abdm.gov.in", Value = orgId }
            }
        };

        // 4. Create Encounter
        var encounter = new Encounter
        {
            Id = "Encounter-1",
            Status = Encounter.EncounterStatus.Finished,
            Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "AMB", "ambulatory"),
            Subject = new ResourceReference($"Patient/{patient.Id}"),
            Period = new Period { Start = authoredOn }
        };

        // 5. Create Composition
        var composition = new Composition
        {
            Id = "Composition-1",
            Identifier = new Identifier { System = "https://ndhm.in", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept("http://snomed.info/sct", "440545006", "Prescription record"),
            Subject = new ResourceReference($"Patient/{patient.Id}"),
            Encounter = new ResourceReference($"Encounter/{encounter.Id}"),
            DateElement = new FhirDateTime(authoredOn),
            Author = new List<ResourceReference> { new ResourceReference($"Practitioner/{practitioner.Id}") },
            Title = "Prescription"
        };

        var medicationSection = new Composition.SectionComponent
        {
            Title = "Outpatient Medication",
            Code = new CodeableConcept("http://snomed.info/sct", "721912009", "Medication summary document")
        };

        // 6. Create MedicationRequest entries
        var entries = new List<Bundle.EntryComponent>
        {
            new Bundle.EntryComponent { FullUrl = $"Composition/{composition.Id}", Resource = composition },
            new Bundle.EntryComponent { FullUrl = $"Patient/{patient.Id}", Resource = patient },
            new Bundle.EntryComponent { FullUrl = $"Practitioner/{practitioner.Id}", Resource = practitioner },
            new Bundle.EntryComponent { FullUrl = $"Organization/{organization.Id}", Resource = organization },
            new Bundle.EntryComponent { FullUrl = $"Encounter/{encounter.Id}", Resource = encounter }
        };

        int medIndex = 1;
        foreach (var p in prescriptionsElement.EnumerateArray())
        {
            var medName = GetString(p, "medicine") ?? "Unknown Medicine";
            var dosage = GetString(p, "dosage") ?? "";

            var medication = new Medication
            {
                Id = $"Medication-{medIndex}",
                Code = new CodeableConcept("http://snomed.info/sct", "0000", medName) // Using mock SNOMED for now
            };

            var medReq = new MedicationRequest
            {
                Id = $"MedicationRequest-{medIndex}",
                Status = MedicationRequest.MedicationrequestStatus.Active,
                Intent = MedicationRequest.MedicationRequestIntent.Order,
                Medication = new ResourceReference($"Medication/{medication.Id}"),
                Subject = new ResourceReference($"Patient/{patient.Id}"),
                AuthoredOnElement = new FhirDateTime(authoredOn),
                Requester = new ResourceReference($"Practitioner/{practitioner.Id}")
            };

            if (!string.IsNullOrEmpty(dosage))
            {
                medReq.DosageInstruction = new List<Dosage>
                {
                    new Dosage { Text = dosage }
                };
            }

            medicationSection.Entry.Add(new ResourceReference($"MedicationRequest/{medReq.Id}"));

            entries.Add(new Bundle.EntryComponent { FullUrl = $"Medication/{medication.Id}", Resource = medication });
            entries.Add(new Bundle.EntryComponent { FullUrl = $"MedicationRequest/{medReq.Id}", Resource = medReq });

            medIndex++;
        }

        composition.Section.Add(medicationSection);

        // 7. Assemble Bundle
        var bundle = new Bundle
        {
            Id = careContextReference,
            Identifier = new Identifier { System = "https://ndhm.in", Value = careContextReference },
            Type = Bundle.BundleType.Document,
            Timestamp = DateTimeOffset.UtcNow,
            Entry = entries
        };

        // 8. Serialize
        var serializer = new FhirJsonSerializer();
        var fhirJson = serializer.SerializeToString(bundle);

        return System.Threading.Tasks.Task.FromResult(fhirJson);
    }

    private string? GetString(JsonElement element, string propName)
    {
        if (element.TryGetProperty(propName, out var prop))
        {
            return prop.GetString();
        }
        return null;
    }
}
