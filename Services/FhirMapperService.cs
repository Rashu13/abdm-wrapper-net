using System;
using System.Collections.Generic;
using System.Text.Json;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace AbdmWrapperNet.Services;

public class FhirMapperService : IFhirMapperService
{
    private const string PROFILE_DOCUMENT_BUNDLE = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/DocumentBundle";
    private const string PROFILE_CONFIDENTIALITY = "http://terminology.hl7.org/CodeSystem/v3-Confidentiality";
    private const string PROFILE_PATIENT = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/Patient";
    private const string PROFILE_PRACTITIONER = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/Practitioner";
    private const string PROFILE_ORGANIZATION = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/Organization";
    private const string PROFILE_ENCOUNTER = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/Encounter";
    private const string PROFILE_PRESCRIPTION_RECORD = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/PrescriptionRecord";
    private const string PROFILE_OP_CONSULTATION_RECORD = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/OPConsultationRecord";
    private const string PROFILE_HEALTH_DOCUMENT_RECORD = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/HealthDocumentRecord";
    private const string PROFILE_MEDICATION_REQUEST = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/MedicationRequest";
    private const string PROFILE_OBSERVATION = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/Observation";
    private const string PROFILE_DOCUMENT_REFERENCE = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/DocumentReference";
    private const string PROFILE_BINARY = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/Binary";

    private const string IDENTIFIER_TYPE_SYSTEM = "http://terminology.hl7.org/CodeSystem/v2-0203";
    private const string SNOMED_URL = "http://snomed.info/sct";

    private Meta CreateMeta(string profileUrl)
    {
        return new Meta
        {
            VersionId = "1",
            LastUpdated = DateTimeOffset.UtcNow,
            Profile = new[] { profileUrl }
        };
    }

    private AdministrativeGender ParseGender(string? genderStr)
    {
        if (string.IsNullOrEmpty(genderStr)) return AdministrativeGender.Unknown;
        var clean = genderStr.Trim().ToLowerInvariant();
        if (clean == "m" || clean == "male") return AdministrativeGender.Male;
        if (clean == "f" || clean == "female") return AdministrativeGender.Female;
        if (clean == "other" || clean == "o") return AdministrativeGender.Other;
        return AdministrativeGender.Unknown;
    }

    public System.Threading.Tasks.Task<string> GeneratePrescriptionBundleAsync(string fhirJsonPayload)
    {
        using var document = JsonDocument.Parse(fhirJsonPayload);
        var root = document.RootElement;

        var careContextReference = GetString(root, "careContextReference") ?? Guid.NewGuid().ToString();
        var authoredOn = GetString(root, "authoredOn") ?? DateTime.UtcNow.ToString("o");
        
        var patientElement = GetProperty(root, "patient");
        var patientName = GetString(patientElement, "name") ?? "Unknown";
        var patientRef = GetString(patientElement, "patientReference") ?? "Patient-1";
        
        var practitionersElement = GetProperty(root, "practitioners");
        var orgElement = GetProperty(root, "organisation");
        var prescriptionsElement = GetProperty(root, "prescriptions");

        // 1. Create Patient
        var patient = new Patient
        {
            Id = patientRef,
            Meta = CreateMeta(PROFILE_PATIENT),
            Name = new List<HumanName> { new HumanName { Text = patientName } },
            Gender = ParseGender(GetString(patientElement, "gender"))
        };
        patient.Identifier.Add(new Identifier
        {
            System = "https://healthid.abdm.gov.in",
            Value = patientRef,
            Type = new CodeableConcept
            {
                Text = "Medical record number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "MR", "Medical record number")
                }
            }
        });

        var birthDateStr = GetString(patientElement, "birthDate");
        if (!string.IsNullOrEmpty(birthDateStr))
        {
            patient.BirthDate = NormalizeBirthDate(birthDateStr);
        }

        // 2. Create Practitioner
        var practitionerId = "PR-1";
        var practitionerName = "Doctor";
        if (practitionersElement.ValueKind == JsonValueKind.Array && practitionersElement.GetArrayLength() > 0)
        {
            var p = practitionersElement[0];
            practitionerName = GetString(p, "name") ?? "Doctor";
            practitionerId = GetString(p, "practitionerId") ?? "PR-1";
        }
        var practitioner = new Practitioner
        {
            Id = practitionerId,
            Meta = CreateMeta(PROFILE_PRACTITIONER),
            Name = new List<HumanName> { new HumanName { Text = practitionerName } }
        };
        practitioner.Identifier.Add(new Identifier
        {
            System = "https://doctor.abdm.gov.in",
            Value = practitionerId,
            Type = new CodeableConcept
            {
                Text = "Medical record number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "MD", "Medical record number")
                }
            }
        });

        // 3. Create Organization
        var orgName = GetString(orgElement, "facilityName") ?? "Hospital";
        var orgId = GetString(orgElement, "facilityId") ?? "IN-1";
        var organization = new Organization
        {
            Id = orgId,
            Meta = CreateMeta(PROFILE_ORGANIZATION),
            Name = orgName
        };
        organization.Identifier.Add(new Identifier
        {
            System = "https://facility.abdm.gov.in",
            Value = orgId,
            Type = new CodeableConcept
            {
                Text = "Provider number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "PRN", "Provider number")
                }
            }
        });

        // 4. Create Encounter
        var encounter = new Encounter
        {
            Id = "Encounter-1",
            Meta = CreateMeta(PROFILE_ENCOUNTER),
            Status = Encounter.EncounterStatus.InProgress,
            Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-Confidentiality", "AMB", "Ambulatory"),
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Period = new Period { Start = authoredOn }
        };

        // 5. Create Composition
        var composition = new Composition
        {
            Id = "Composition-1",
            Meta = CreateMeta(PROFILE_PRESCRIPTION_RECORD),
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept
            {
                Text = "Prescription record",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "440545006", "Prescription record")
                }
            },
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Encounter = new ResourceReference($"Encounter/{encounter.Id}") { Display = "Ambulatory" },
            DateElement = new FhirDateTime(authoredOn),
            Custodian = new ResourceReference($"Organisation/{organization.Id}") { Display = orgName },
            Title = "Prescription record"
        };
        composition.Author.Add(new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName });

        var medicationSection = new Composition.SectionComponent
        {
            Title = "Medications",
            Code = new CodeableConcept
            {
                Text = "Prescription record",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "440545006", "Prescription record")
                }
            }
        };

        // 6. Assemble Bundle entries list
        var entries = new List<Bundle.EntryComponent>
        {
            new Bundle.EntryComponent { FullUrl = $"Composition/{composition.Id}", Resource = composition },
            new Bundle.EntryComponent { FullUrl = $"Patient/{patient.Id}", Resource = patient },
            new Bundle.EntryComponent { FullUrl = $"Practitioner/{practitioner.Id}", Resource = practitioner },
            new Bundle.EntryComponent { FullUrl = $"Organisation/{organization.Id}", Resource = organization },
            new Bundle.EntryComponent { FullUrl = $"Encounter/{encounter.Id}", Resource = encounter }
        };

        // 7. Create MedicationRequest entries
        int medIndex = 1;
        if (prescriptionsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var p in prescriptionsElement.EnumerateArray())
            {
                var medName = GetString(p, "medicine") ?? "Unknown Medicine";
                var dosage = GetString(p, "dosage") ?? "";

                var medReq = new MedicationRequest
                {
                    Id = $"MedicationRequest-{medIndex}",
                    Meta = CreateMeta(PROFILE_MEDICATION_REQUEST),
                    Status = MedicationRequest.MedicationrequestStatus.Completed,
                    Intent = MedicationRequest.MedicationRequestIntent.Order,
                    Medication = new CodeableConcept
                    {
                        Text = medName,
                        Coding = new List<Coding>
                        {
                            new Coding(SNOMED_URL, "261665006", medName)
                        }
                    },
                    Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
                    AuthoredOnElement = new FhirDateTime(authoredOn),
                    Requester = new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName }
                };

                if (!string.IsNullOrEmpty(dosage))
                {
                    medReq.DosageInstruction = new List<Dosage>
                    {
                        new Dosage { Text = dosage }
                    };
                }

                medicationSection.Entry.Add(new ResourceReference($"MedicationRequest/{medReq.Id}"));
                entries.Add(new Bundle.EntryComponent { FullUrl = $"MedicationRequest/{medReq.Id}", Resource = medReq });

                medIndex++;
            }
        }

        // 8. Handle Scanned/Attached Documents as Binary resources
        var documentsElement = GetProperty(root, "documents");
        int binaryIndex = 1;
        if (documentsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var d in documentsElement.EnumerateArray())
            {
                var contentType = GetString(d, "contentType") ?? "application/pdf";
                var dataBase64 = GetString(d, "data") ?? "";

                byte[]? rawData = null;
                if (!string.IsNullOrEmpty(dataBase64))
                {
                    try
                    {
                        rawData = Convert.FromBase64String(dataBase64);
                    }
                    catch { }
                }

                if (rawData != null)
                {
                    var binary = new Binary
                    {
                        Id = $"Binary-{binaryIndex}",
                        Meta = CreateMeta(PROFILE_BINARY),
                        ContentType = contentType,
                        Data = rawData
                    };

                    medicationSection.Entry.Add(new ResourceReference($"Binary/{binary.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"Binary/{binary.Id}", Resource = binary });
                    binaryIndex++;
                }
            }
        }

        composition.Section.Add(medicationSection);

        // 9. Assemble Bundle with Meta/Security matching Java
        var bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString(),
            Meta = new Meta
            {
                Profile = new[] { PROFILE_DOCUMENT_BUNDLE },
                VersionId = "1",
                LastUpdated = DateTimeOffset.UtcNow
            },
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = careContextReference },
            Type = Bundle.BundleType.Document,
            Timestamp = DateTimeOffset.UtcNow,
            Entry = entries
        };
        bundle.Meta.Security.Add(new Coding(PROFILE_CONFIDENTIALITY, "V", "very restricted"));

        // 10. Serialize
        var serializer = new FhirJsonSerializer();
        var fhirJson = serializer.SerializeToString(bundle);

        return System.Threading.Tasks.Task.FromResult(fhirJson);
    }

    public System.Threading.Tasks.Task<string> GenerateOPConsultationBundleAsync(string fhirJsonPayload)
    {
        using var document = JsonDocument.Parse(fhirJsonPayload);
        var root = document.RootElement;

        var careContextReference = GetString(root, "careContextReference") ?? Guid.NewGuid().ToString();
        var authoredOn = GetString(root, "authoredOn") ?? DateTime.UtcNow.ToString("o");
        
        var patientElement = GetProperty(root, "patient");
        var patientName = GetString(patientElement, "name") ?? "Unknown";
        var patientRef = GetString(patientElement, "patientReference") ?? "Patient-1";
        
        var practitionersElement = GetProperty(root, "practitioners");
        var orgElement = GetProperty(root, "organisation");

        // 1. Create Patient
        var patient = new Patient
        {
            Id = patientRef,
            Meta = CreateMeta(PROFILE_PATIENT),
            Name = new List<HumanName> { new HumanName { Text = patientName } },
            Gender = ParseGender(GetString(patientElement, "gender"))
        };
        patient.Identifier.Add(new Identifier
        {
            System = "https://healthid.abdm.gov.in",
            Value = patientRef,
            Type = new CodeableConcept
            {
                Text = "Medical record number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "MR", "Medical record number")
                }
            }
        });

        var birthDateStr = GetString(patientElement, "birthDate");
        if (!string.IsNullOrEmpty(birthDateStr))
        {
            patient.BirthDate = NormalizeBirthDate(birthDateStr);
        }

        // 2. Create Practitioner
        var practitionerId = "PR-1";
        var practitionerName = "Doctor";
        if (practitionersElement.ValueKind == JsonValueKind.Array && practitionersElement.GetArrayLength() > 0)
        {
            var p = practitionersElement[0];
            practitionerName = GetString(p, "name") ?? "Doctor";
            practitionerId = GetString(p, "practitionerId") ?? "PR-1";
        }
        var practitioner = new Practitioner
        {
            Id = practitionerId,
            Meta = CreateMeta(PROFILE_PRACTITIONER),
            Name = new List<HumanName> { new HumanName { Text = practitionerName } }
        };
        practitioner.Identifier.Add(new Identifier
        {
            System = "https://doctor.abdm.gov.in",
            Value = practitionerId,
            Type = new CodeableConcept
            {
                Text = "Medical record number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "MD", "Medical record number")
                }
            }
        });

        // 3. Create Organization
        var orgName = GetString(orgElement, "facilityName") ?? "Hospital";
        var orgId = GetString(orgElement, "facilityId") ?? "IN-1";
        var organization = new Organization
        {
            Id = orgId,
            Meta = CreateMeta(PROFILE_ORGANIZATION),
            Name = orgName
        };
        organization.Identifier.Add(new Identifier
        {
            System = "https://facility.abdm.gov.in",
            Value = orgId,
            Type = new CodeableConcept
            {
                Text = "Provider number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "PRN", "Provider number")
                }
            }
        });

        // 4. Create Encounter
        var encounter = new Encounter
        {
            Id = "Encounter-1",
            Meta = CreateMeta(PROFILE_ENCOUNTER),
            Status = Encounter.EncounterStatus.InProgress,
            Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-Confidentiality", "AMB", "Ambulatory"),
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Period = new Period { Start = authoredOn }
        };

        // 5. Create Composition for OP Consultation
        var composition = new Composition
        {
            Id = "Composition-1",
            Meta = CreateMeta(PROFILE_OP_CONSULTATION_RECORD),
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept
            {
                Text = "Clinical consultation report",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "371530004", "Clinical consultation report")
                }
            },
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Encounter = new ResourceReference($"Encounter/{encounter.Id}") { Display = "Ambulatory" },
            DateElement = new FhirDateTime(authoredOn),
            Custodian = new ResourceReference($"Organisation/{organization.Id}") { Display = orgName },
            Title = "Clinical consultation report"
        };
        composition.Author.Add(new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName });

        var entries = new List<Bundle.EntryComponent>
        {
            new Bundle.EntryComponent { FullUrl = $"Composition/{composition.Id}", Resource = composition },
            new Bundle.EntryComponent { FullUrl = $"Patient/{patient.Id}", Resource = patient },
            new Bundle.EntryComponent { FullUrl = $"Practitioner/{practitioner.Id}", Resource = practitioner },
            new Bundle.EntryComponent { FullUrl = $"Organisation/{organization.Id}", Resource = organization },
            new Bundle.EntryComponent { FullUrl = $"Encounter/{encounter.Id}", Resource = encounter }
        };

        // 6. Add Observation for Notes
        var notes = "OP Consultation notes";
        if (root.TryGetProperty("clinicalNotes", out var notesProp) && notesProp.ValueKind == JsonValueKind.String)
        {
            notes = notesProp.GetString() ?? notes;
        }

        var observation = new Observation
        {
            Id = "Observation-1",
            Meta = CreateMeta(PROFILE_OBSERVATION),
            Status = ObservationStatus.Final,
            Code = new CodeableConcept
            {
                Text = "Consultation",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "11429006", "Consultation")
                }
            },
            Subject = new ResourceReference($"Patient/{patient.Id}"),
            Value = new FhirString(notes)
        };

        var notesSection = new Composition.SectionComponent
        {
            Title = "Clinical finding",
            Code = new CodeableConcept
            {
                Text = "Clinical finding",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "404684003", "Clinical finding")
                }
            }
        };
        notesSection.Entry.Add(new ResourceReference($"Observation/{observation.Id}"));
        composition.Section.Add(notesSection);
        entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{observation.Id}", Resource = observation });

        // 7. Add Medication Requests if prescriptions exist
        var prescriptionsElement = GetProperty(root, "prescriptions");
        if (prescriptionsElement.ValueKind == JsonValueKind.Array && prescriptionsElement.GetArrayLength() > 0)
        {
            var medSection = new Composition.SectionComponent
            {
                Title = "Medication summary document",
                Code = new CodeableConcept
                {
                    Text = "Medication summary document",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "721912009", "Medication summary document")
                    }
                }
            };

            int medIndex = 1;
            foreach (var p in prescriptionsElement.EnumerateArray())
            {
                var medName = GetString(p, "medicine") ?? "Unknown Medicine";
                var dosage = GetString(p, "dosage") ?? "";

                var medReq = new MedicationRequest
                {
                    Id = $"MedicationRequest-{medIndex}",
                    Meta = CreateMeta(PROFILE_MEDICATION_REQUEST),
                    Status = MedicationRequest.MedicationrequestStatus.Completed,
                    Intent = MedicationRequest.MedicationRequestIntent.Order,
                    Medication = new CodeableConcept
                    {
                        Text = medName,
                        Coding = new List<Coding>
                        {
                            new Coding(SNOMED_URL, "261665006", medName)
                        }
                    },
                    Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
                    AuthoredOnElement = new FhirDateTime(authoredOn),
                    Requester = new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName }
                };

                if (!string.IsNullOrEmpty(dosage))
                {
                    medReq.DosageInstruction = new List<Dosage>
                    {
                        new Dosage { Text = dosage }
                    };
                }

                // FamilyHistory reference prefix due to Java wrapper typo
                medSection.Entry.Add(new ResourceReference($"FamilyHistory/{medReq.Id}"));
                entries.Add(new Bundle.EntryComponent { FullUrl = $"MedicationRequest/{medReq.Id}", Resource = medReq });

                medIndex++;
            }
            composition.Section.Add(medSection);
        }

        // 8. Handle Scanned/Attached Documents as DocumentReference resources
        var documentsElement = GetProperty(root, "documents");
        if (documentsElement.ValueKind == JsonValueKind.Array && documentsElement.GetArrayLength() > 0)
        {
            var docSection = new Composition.SectionComponent
            {
                Title = "Clinical consultation report",
                Code = new CodeableConcept
                {
                    Text = "Clinical consultation report",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "371530004", "Clinical consultation report")
                    }
                }
            };

            int docIndex = 1;
            foreach (var d in documentsElement.EnumerateArray())
            {
                var contentType = GetString(d, "contentType") ?? "application/pdf";
                var typeStr = GetString(d, "type") ?? "Prescription";
                var dataBase64 = GetString(d, "data") ?? "";

                byte[]? rawData = null;
                if (!string.IsNullOrEmpty(dataBase64))
                {
                    try
                    {
                        rawData = Convert.FromBase64String(dataBase64);
                    }
                    catch { }
                }

                if (rawData != null)
                {
                    var attachment = new Attachment
                    {
                        ContentType = contentType,
                        Data = rawData,
                        Title = typeStr,
                        CreationElement = new FhirDateTime(DateTimeOffset.UtcNow)
                    };

                    var docRef = new DocumentReference
                    {
                        Id = $"DocumentReference-{docIndex}",
                        Meta = CreateMeta(PROFILE_DOCUMENT_REFERENCE),
                        Status = DocumentReferenceStatus.Current,
                        DocStatus = CompositionStatus.Final,
                        Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
                        Content = new List<DocumentReference.ContentComponent>
                        {
                            new DocumentReference.ContentComponent { Attachment = attachment }
                        }
                    };

                    docRef.Identifier.Add(new Identifier
                    {
                        System = "https://facility.abdm.gov.in",
                        Value = organization.Id,
                        Type = new CodeableConcept
                        {
                            Text = typeStr,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "371530004", "Clinical consultation report")
                            }
                        }
                    });

                    docSection.Entry.Add(new ResourceReference($"DocumentReference/{docRef.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"DocumentReference/{docRef.Id}", Resource = docRef });
                    docIndex++;
                }
            }
            composition.Section.Add(docSection);
        }

        // 9. Assemble Bundle with Meta/Security
        var bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString(),
            Meta = new Meta
            {
                Profile = new[] { PROFILE_DOCUMENT_BUNDLE },
                VersionId = "1",
                LastUpdated = DateTimeOffset.UtcNow
            },
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = careContextReference },
            Type = Bundle.BundleType.Document,
            Timestamp = DateTimeOffset.UtcNow,
            Entry = entries
        };
        bundle.Meta.Security.Add(new Coding(PROFILE_CONFIDENTIALITY, "V", "very restricted"));

        // 10. Serialize
        var serializer = new FhirJsonSerializer();
        var fhirJson = serializer.SerializeToString(bundle);

        return System.Threading.Tasks.Task.FromResult(fhirJson);
    }

    public System.Threading.Tasks.Task<string> GenerateHealthDocumentBundleAsync(string fhirJsonPayload)
    {
        using var document = JsonDocument.Parse(fhirJsonPayload);
        var root = document.RootElement;

        var careContextReference = GetString(root, "careContextReference") ?? Guid.NewGuid().ToString();
        var authoredOn = GetString(root, "authoredOn") ?? DateTime.UtcNow.ToString("o");
        
        var patientElement = GetProperty(root, "patient");
        var patientName = GetString(patientElement, "name") ?? "Unknown";
        var patientRef = GetString(patientElement, "patientReference") ?? "Patient-1";
        
        var practitionersElement = GetProperty(root, "practitioners");
        var orgElement = GetProperty(root, "organisation");

        // 1. Create Patient
        var patient = new Patient
        {
            Id = patientRef,
            Meta = CreateMeta(PROFILE_PATIENT),
            Name = new List<HumanName> { new HumanName { Text = patientName } },
            Gender = ParseGender(GetString(patientElement, "gender"))
        };
        patient.Identifier.Add(new Identifier
        {
            System = "https://healthid.abdm.gov.in",
            Value = patientRef,
            Type = new CodeableConcept
            {
                Text = "Medical record number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "MR", "Medical record number")
                }
            }
        });

        var birthDateStr = GetString(patientElement, "birthDate");
        if (!string.IsNullOrEmpty(birthDateStr))
        {
            patient.BirthDate = NormalizeBirthDate(birthDateStr);
        }

        // 2. Create Practitioner
        var practitionerId = "PR-1";
        var practitionerName = "Doctor";
        if (practitionersElement.ValueKind == JsonValueKind.Array && practitionersElement.GetArrayLength() > 0)
        {
            var p = practitionersElement[0];
            practitionerName = GetString(p, "name") ?? "Doctor";
            practitionerId = GetString(p, "practitionerId") ?? "PR-1";
        }
        var practitioner = new Practitioner
        {
            Id = practitionerId,
            Meta = CreateMeta(PROFILE_PRACTITIONER),
            Name = new List<HumanName> { new HumanName { Text = practitionerName } }
        };
        practitioner.Identifier.Add(new Identifier
        {
            System = "https://doctor.abdm.gov.in",
            Value = practitionerId,
            Type = new CodeableConcept
            {
                Text = "Medical record number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "MD", "Medical record number")
                }
            }
        });

        // 3. Create Organization
        var orgName = GetString(orgElement, "facilityName") ?? "Hospital";
        var orgId = GetString(orgElement, "facilityId") ?? "IN-1";
        var organization = new Organization
        {
            Id = orgId,
            Meta = CreateMeta(PROFILE_ORGANIZATION),
            Name = orgName
        };
        organization.Identifier.Add(new Identifier
        {
            System = "https://facility.abdm.gov.in",
            Value = orgId,
            Type = new CodeableConcept
            {
                Text = "Provider number",
                Coding = new List<Coding>
                {
                    new Coding(IDENTIFIER_TYPE_SYSTEM, "PRN", "Provider number")
                }
            }
        });

        // 4. Create Encounter
        var encounter = new Encounter
        {
            Id = "Encounter-1",
            Meta = CreateMeta(PROFILE_ENCOUNTER),
            Status = Encounter.EncounterStatus.InProgress,
            Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-Confidentiality", "AMB", "Ambulatory"),
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Period = new Period { Start = authoredOn }
        };

        // 5. Create Composition for Health Document Record
        var composition = new Composition
        {
            Id = "Composition-1",
            Meta = CreateMeta(PROFILE_HEALTH_DOCUMENT_RECORD),
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept
            {
                Text = "Record artifact",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "419891008", "Record artifact")
                }
            },
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Encounter = new ResourceReference($"Encounter/{encounter.Id}") { Display = "Ambulatory" },
            DateElement = new FhirDateTime(authoredOn),
            Custodian = new ResourceReference($"Organisation/{organization.Id}") { Display = orgName },
            Title = "Health Document"
        };
        composition.Author.Add(new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName });

        var docSection = new Composition.SectionComponent
        {
            Title = "Record artifact",
            Code = new CodeableConcept
            {
                Text = "Record artifact",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "419891008", "Record artifact")
                }
            }
        };

        var entries = new List<Bundle.EntryComponent>
        {
            new Bundle.EntryComponent { FullUrl = $"Composition/{composition.Id}", Resource = composition },
            new Bundle.EntryComponent { FullUrl = $"Patient/{patient.Id}", Resource = patient },
            new Bundle.EntryComponent { FullUrl = $"Practitioner/{practitioner.Id}", Resource = practitioner },
            new Bundle.EntryComponent { FullUrl = $"Organisation/{organization.Id}", Resource = organization },
            new Bundle.EntryComponent { FullUrl = $"Encounter/{encounter.Id}", Resource = encounter }
        };

        // 6. Create DocumentReference entries if present
        var documentsElement = GetProperty(root, "documents");
        int docIndex = 1;
        if (documentsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var d in documentsElement.EnumerateArray())
            {
                var contentType = GetString(d, "contentType") ?? "application/pdf";
                var typeStr = GetString(d, "type") ?? "Prescription";
                var dataBase64 = GetString(d, "data") ?? "";

                byte[]? rawData = null;
                if (!string.IsNullOrEmpty(dataBase64))
                {
                    try
                    {
                        rawData = Convert.FromBase64String(dataBase64);
                    }
                    catch { }
                }

                if (rawData != null)
                {
                    var attachment = new Attachment
                    {
                        ContentType = contentType,
                        Data = rawData,
                        Title = typeStr,
                        CreationElement = new FhirDateTime(DateTimeOffset.UtcNow)
                    };

                    var docRef = new DocumentReference
                    {
                        Id = $"DocumentReference-{docIndex}",
                        Meta = CreateMeta(PROFILE_DOCUMENT_REFERENCE),
                        Status = DocumentReferenceStatus.Current,
                        DocStatus = CompositionStatus.Final,
                        Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
                        Content = new List<DocumentReference.ContentComponent>
                        {
                            new DocumentReference.ContentComponent { Attachment = attachment }
                        }
                    };

                    docRef.Identifier.Add(new Identifier
                    {
                        System = "https://facility.abdm.gov.in",
                        Value = organization.Id,
                        Type = new CodeableConcept
                        {
                            Text = typeStr,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "423876004", "Health Document")
                            }
                        }
                    });

                    docSection.Entry.Add(new ResourceReference($"DocumentReference/{docRef.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"DocumentReference/{docRef.Id}", Resource = docRef });
                    docIndex++;
                }
            }
        }

        composition.Section.Add(docSection);

        // 7. Assemble Bundle with Meta/Security
        var bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString(),
            Meta = new Meta
            {
                Profile = new[] { PROFILE_DOCUMENT_BUNDLE },
                VersionId = "1",
                LastUpdated = DateTimeOffset.UtcNow
            },
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = careContextReference },
            Type = Bundle.BundleType.Document,
            Timestamp = DateTimeOffset.UtcNow,
            Entry = entries
        };
        bundle.Meta.Security.Add(new Coding(PROFILE_CONFIDENTIALITY, "V", "very restricted"));

        // 8. Serialize
        var serializer = new FhirJsonSerializer();
        var fhirJson = serializer.SerializeToString(bundle);

        return System.Threading.Tasks.Task.FromResult(fhirJson);
    }

    private string? GetString(JsonElement element, string propName)
    {
        if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propName, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            return prop.GetString();
        }
        return null;
    }

    private JsonElement GetProperty(JsonElement element, string propName)
    {
        if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propName, out var prop))
        {
            return prop;
        }
        return default;
    }

    private string? NormalizeBirthDate(string? dateStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return null;

        string[] formats = {
            "yyyy-MM-dd",
            "yyyy-dd-MM",
            "dd-MM-yyyy",
            "yyyy/MM/dd",
            "yyyy/dd/MM",
            "dd/MM/yyyy",
            "yyyy.MM.dd",
            "yyyy.dd.MM",
            "dd.MM.yyyy"
        };

        if (DateTime.TryParseExact(dateStr, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt))
        {
            return dt.ToString("yyyy-MM-dd");
        }

        if (DateTime.TryParse(dateStr, out var parsedDt))
        {
            return parsedDt.ToString("yyyy-MM-dd");
        }

        return dateStr;
    }
}
