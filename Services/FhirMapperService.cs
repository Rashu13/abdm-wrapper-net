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
    private const string PROFILE_DIAGNOSTIC_REPORT_RECORD = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/DiagnosticReportRecord";
    private const string PROFILE_DISCHARGE_SUMMARY_RECORD = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/DischargeSummaryRecord";
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
                    var dosageInst = new Dosage { Text = dosage };

                    var additionalInstructions = GetString(p, "additionalInstructions");
                    if (!string.IsNullOrEmpty(additionalInstructions))
                    {
                        dosageInst.AdditionalInstruction = new List<CodeableConcept>
                        {
                            new CodeableConcept
                            {
                                Text = additionalInstructions,
                                Coding = new List<Coding>
                                {
                                    new Coding(SNOMED_URL, "1000000570007", additionalInstructions)
                                }
                            }
                        };
                    }

                    var route = GetString(p, "route");
                    if (!string.IsNullOrEmpty(route))
                    {
                        dosageInst.Route = new CodeableConcept
                        {
                            Text = route,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "26643006", route)
                            }
                        };
                    }

                    var method = GetString(p, "method");
                    if (!string.IsNullOrEmpty(method))
                    {
                        dosageInst.Method = new CodeableConcept
                        {
                            Text = method,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "421521004", method)
                            }
                        };
                    }

                    var timing = GetString(p, "timing");
                    if (!string.IsNullOrEmpty(timing))
                    {
                        try
                        {
                            var parts = timing.Split('-');
                            if (parts.Length == 3 && int.TryParse(parts[0], out int freq) && int.TryParse(parts[1], out int period))
                            {
                                var unitStr = parts[2].ToUpperInvariant();
                                Timing.UnitsOfTime? unit = null;
                                if (unitStr == "S") unit = Timing.UnitsOfTime.S;
                                else if (unitStr == "MIN") unit = Timing.UnitsOfTime.Min;
                                else if (unitStr == "H") unit = Timing.UnitsOfTime.H;
                                else if (unitStr == "D") unit = Timing.UnitsOfTime.D;
                                else if (unitStr == "WK") unit = Timing.UnitsOfTime.Wk;
                                else if (unitStr == "MO") unit = Timing.UnitsOfTime.Mo;

                                if (unit.HasValue)
                                {
                                    dosageInst.Timing = new Timing
                                    {
                                        Repeat = new Timing.RepeatComponent
                                        {
                                            Frequency = freq,
                                            Period = (decimal)period,
                                            PeriodUnit = unit.Value
                                        }
                                    };
                                }
                            }
                        }
                        catch { }
                    }

                    medReq.DosageInstruction = new List<Dosage> { dosageInst };
                }

                var reason = GetString(p, "reason");
                if (!string.IsNullOrEmpty(reason))
                {
                    medReq.ReasonCode = new List<CodeableConcept>
                    {
                        new CodeableConcept
                        {
                            Text = reason,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "55607006", reason)
                            }
                        }
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

    private int? GetInt(JsonElement element, string propName)
    {
        if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propName, out var prop))
        {
            if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var val)) return val;
            if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out var sval)) return sval;
        }
        return null;
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

    public System.Threading.Tasks.Task<string> GenerateDiagnosticReportBundleAsync(string fhirJsonPayload)
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

        // 5. Create Composition for Diagnostic Report Record
        var composition = new Composition
        {
            Id = "Composition-1",
            Meta = CreateMeta(PROFILE_DIAGNOSTIC_REPORT_RECORD),
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept
            {
                Text = "Diagnostic Report Note",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "721981007", "Diagnostic Report Note")
                }
            },
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Encounter = new ResourceReference($"Encounter/{encounter.Id}") { Display = "Ambulatory" },
            DateElement = new FhirDateTime(authoredOn),
            Custodian = new ResourceReference($"Organisation/{organization.Id}") { Display = orgName },
            Title = "Diagnostic Report"
        };
        composition.Author.Add(new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName });

        var docSection = new Composition.SectionComponent
        {
            Title = "Diagnostic Report Note",
            Code = new CodeableConcept
            {
                Text = "Diagnostic Report Note",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "721981007", "Diagnostic Report Note")
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
                var typeStr = GetString(d, "type") ?? "DiagnosticReport";
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

    public System.Threading.Tasks.Task<string> GenerateDischargeSummaryBundleAsync(string fhirJsonPayload)
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

        // 5. Create Composition for Discharge Summary Record
        var composition = new Composition
        {
            Id = "Composition-1",
            Meta = CreateMeta(PROFILE_DISCHARGE_SUMMARY_RECORD),
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept
            {
                Text = "Discharge summary",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "373942005", "Discharge summary")
                }
            },
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Encounter = new ResourceReference($"Encounter/{encounter.Id}") { Display = "Ambulatory" },
            DateElement = new FhirDateTime(authoredOn),
            Custodian = new ResourceReference($"Organisation/{organization.Id}") { Display = orgName },
            Title = "Discharge Summary"
        };
        composition.Author.Add(new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName });

        var docSection = new Composition.SectionComponent
        {
            Title = "Discharge summary",
            Code = new CodeableConcept
            {
                Text = "Discharge summary",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "373942005", "Discharge summary")
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
                var typeStr = GetString(d, "type") ?? "DischargeSummary";
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

    public System.Threading.Tasks.Task<string> GenerateImmunizationBundleAsync(string fhirJsonPayload)
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

        // 5. Create Composition for Immunization Record
        var composition = new Composition
        {
            Id = "Composition-1",
            Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/ImmunizationRecord"),
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept
            {
                Text = "Immunization record",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "41000179103", "Immunization record")
                }
            },
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Encounter = new ResourceReference($"Encounter/{encounter.Id}") { Display = "Ambulatory" },
            DateElement = new FhirDateTime(authoredOn),
            Custodian = new ResourceReference($"Organisation/{organization.Id}") { Display = orgName },
            Title = "Immunization Record"
        };
        composition.Author.Add(new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName });

        var immunizationSection = new Composition.SectionComponent
        {
            Title = "Immunization record",
            Code = new CodeableConcept
            {
                Text = "Immunization record",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "41000179103", "Immunization record")
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

        // Parse structured immunizations if present
        var immunizationsElement = GetProperty(root, "immunizations");
        int immIndex = 1;
        if (immunizationsElement.ValueKind == JsonValueKind.Array && immunizationsElement.GetArrayLength() > 0)
        {
            foreach (var immEl in immunizationsElement.EnumerateArray())
            {
                var vaccineName = GetString(immEl, "vaccineName") ?? "Vaccine";
                var dateStr = GetString(immEl, "date") ?? authoredOn;
                var lotNo = GetString(immEl, "lotNumber") ?? "LOT123";
                var doseVal = GetInt(immEl, "doseNumber") ?? 1;

                var immunization = new Immunization
                {
                    Id = $"Immunization-{immIndex}",
                    Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/Immunization"),
                    Status = Immunization.ImmunizationStatusCodes.Completed,
                    Patient = new ResourceReference($"Patient/{patient.Id}"),
                    Occurrence = new FhirDateTime(dateStr),
                    PrimarySource = true,
                    LotNumber = lotNo
                };

                immunization.VaccineCode = new CodeableConcept
                {
                    Text = vaccineName,
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "1119000", vaccineName)
                    }
                };

                immunization.Extension.Add(new Extension
                {
                    Url = "https://nrces.in/ndhm/fhir/r4/StructureDefinition/BrandName",
                    Value = new FhirString(vaccineName)
                });

                immunization.ProtocolApplied.Add(new Immunization.ProtocolAppliedComponent
                {
                    DoseNumber = new PositiveInt(doseVal)
                });

                immunization.Performer.Add(new Immunization.PerformerComponent
                {
                    Actor = new ResourceReference($"Practitioner/{practitioner.Id}")
                });

                immunizationSection.Entry.Add(new ResourceReference($"Immunization/{immunization.Id}"));
                entries.Add(new Bundle.EntryComponent { FullUrl = $"Immunization/{immunization.Id}", Resource = immunization });
                immIndex++;
            }
        }
        // No default mock immunization is added to enforce strictly dynamic data flow.

        // 6. Create DocumentReference entries if present
        var documentsElement = GetProperty(root, "documents");
        int docIndex = 1;
        if (documentsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var d in documentsElement.EnumerateArray())
            {
                var contentType = GetString(d, "contentType") ?? "application/pdf";
                var typeStr = GetString(d, "type") ?? "ImmunizationRecord";
                var dataBase64 = GetString(d, "data") ?? "";

                byte[]? rawData = null;
                if (!string.IsNullOrEmpty(dataBase64))
                {
                    try { rawData = Convert.FromBase64String(dataBase64); } catch { }
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

                    immunizationSection.Entry.Add(new ResourceReference($"DocumentReference/{docRef.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"DocumentReference/{docRef.Id}", Resource = docRef });
                    docIndex++;
                }
            }
        }

        composition.Section.Add(immunizationSection);

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

    public System.Threading.Tasks.Task<string> GenerateWellnessRecordBundleAsync(string fhirJsonPayload)
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

        // 5. Create Composition for Wellness Record
        var composition = new Composition
        {
            Id = "Composition-1",
            Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/WellnessRecord"),
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept
            {
                Text = "Wellness Record"
            },
            Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
            Encounter = new ResourceReference($"Encounter/{encounter.Id}") { Display = "Ambulatory" },
            DateElement = new FhirDateTime(authoredOn),
            Custodian = new ResourceReference($"Organisation/{organization.Id}") { Display = orgName },
            Title = "Wellness Record"
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

        // 6. Create Observations if present
        int obsIndex = 1;
        var vitalSignsElement = GetProperty(root, "vitalSigns");
        var vitalSignsList = ParseObservations(vitalSignsElement, "Vital Signs", patient, practitioner, ref obsIndex);
        
        var bodyMeasurementsElement = GetProperty(root, "bodyMeasurements");
        var bodyMeasurementsList = ParseObservations(bodyMeasurementsElement, "Body Measurement", patient, practitioner, ref obsIndex);
        
        var physicalActivitiesElement = GetProperty(root, "physicalActivities");
        var physicalActivitiesList = ParseObservations(physicalActivitiesElement, "Physical Activity", patient, practitioner, ref obsIndex);
        
        var generalAssessmentsElement = GetProperty(root, "generalAssessments");
        var generalAssessmentsList = ParseObservations(generalAssessmentsElement, "General Assessment", patient, practitioner, ref obsIndex);
        
        var womanHealthsElement = GetProperty(root, "womanHealths");
        var womanHealthsList = ParseObservations(womanHealthsElement, "Women Health", patient, practitioner, ref obsIndex);
        
        var lifeStylesElement = GetProperty(root, "lifeStyles");
        var lifeStylesList = ParseObservations(lifeStylesElement, "Lifestyle", patient, practitioner, ref obsIndex);
        
        var otherObservationsElement = GetProperty(root, "otherObservations");
        var otherObservationsList = ParseObservations(otherObservationsElement, "Other Observations", patient, practitioner, ref obsIndex);

        // No fallback observations are created to enforce strictly dynamic data flow from the UI.

        // Add sections to Composition & resources to entries
        if (vitalSignsList.Count > 0)
        {
            var sec = new Composition.SectionComponent { Title = "Vital Signs" };
            foreach (var obs in vitalSignsList) { sec.Entry.Add(new ResourceReference($"Observation/{obs.Id}")); entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{obs.Id}", Resource = obs }); }
            composition.Section.Add(sec);
        }
        if (bodyMeasurementsList.Count > 0)
        {
            var sec = new Composition.SectionComponent { Title = "Body Measurement" };
            foreach (var obs in bodyMeasurementsList) { sec.Entry.Add(new ResourceReference($"Observation/{obs.Id}")); entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{obs.Id}", Resource = obs }); }
            composition.Section.Add(sec);
        }
        if (physicalActivitiesList.Count > 0)
        {
            var sec = new Composition.SectionComponent { Title = "Physical Activity" };
            foreach (var obs in physicalActivitiesList) { sec.Entry.Add(new ResourceReference($"Observation/{obs.Id}")); entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{obs.Id}", Resource = obs }); }
            composition.Section.Add(sec);
        }
        if (generalAssessmentsList.Count > 0)
        {
            var sec = new Composition.SectionComponent { Title = "General Assessment" };
            foreach (var obs in generalAssessmentsList) { sec.Entry.Add(new ResourceReference($"Observation/{obs.Id}")); entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{obs.Id}", Resource = obs }); }
            composition.Section.Add(sec);
        }
        if (womanHealthsList.Count > 0)
        {
            var sec = new Composition.SectionComponent { Title = "Women Health" };
            foreach (var obs in womanHealthsList) { sec.Entry.Add(new ResourceReference($"Observation/{obs.Id}")); entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{obs.Id}", Resource = obs }); }
            composition.Section.Add(sec);
        }
        if (lifeStylesList.Count > 0)
        {
            var sec = new Composition.SectionComponent { Title = "Lifestyle" };
            foreach (var obs in lifeStylesList) { sec.Entry.Add(new ResourceReference($"Observation/{obs.Id}")); entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{obs.Id}", Resource = obs }); }
            composition.Section.Add(sec);
        }
        if (otherObservationsList.Count > 0)
        {
            var sec = new Composition.SectionComponent { Title = "Other Observations" };
            foreach (var obs in otherObservationsList) { sec.Entry.Add(new ResourceReference($"Observation/{obs.Id}")); entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{obs.Id}", Resource = obs }); }
            composition.Section.Add(sec);
        }

        // 7. Create DocumentReference entries if present
        var documentsElement = GetProperty(root, "documents");
        int docIndex = 1;
        if (documentsElement.ValueKind == JsonValueKind.Array)
        {
            var docSec = new Composition.SectionComponent { Title = "Document Reference" };
            bool hasDocs = false;
            foreach (var d in documentsElement.EnumerateArray())
            {
                var contentType = GetString(d, "contentType") ?? "application/pdf";
                var typeStr = GetString(d, "type") ?? "WellnessRecord";
                var dataBase64 = GetString(d, "data") ?? "";

                byte[]? rawData = null;
                if (!string.IsNullOrEmpty(dataBase64))
                {
                    try { rawData = Convert.FromBase64String(dataBase64); } catch { }
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

                    docSec.Entry.Add(new ResourceReference($"DocumentReference/{docRef.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"DocumentReference/{docRef.Id}", Resource = docRef });
                    docIndex++;
                    hasDocs = true;
                }
            }
            if (hasDocs)
            {
                composition.Section.Add(docSec);
            }
        }

        // 8. Assemble Bundle with Meta/Security
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

        // 9. Serialize
        var serializer = new FhirJsonSerializer();
        var fhirJson = serializer.SerializeToString(bundle);

        return System.Threading.Tasks.Task.FromResult(fhirJson);
    }

    private List<Observation> ParseObservations(JsonElement arrayElement, string sectionTitle, Patient patient, Practitioner practitioner, ref int obsIndex)
    {
        var list = new List<Observation>();
        if (arrayElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in arrayElement.EnumerateArray())
            {
                var codeText = GetString(item, "codeText") ?? GetString(item, "observation") ?? "Observation";
                var valueStr = GetString(item, "value") ?? GetString(item, "result") ?? "";
                
                var observation = new Observation
                {
                    Id = $"Observation-{obsIndex}",
                    Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/Observation"),
                    Status = ObservationStatus.Final,
                    Code = new CodeableConcept { Text = codeText },
                    Subject = new ResourceReference($"Patient/{patient.Id}"),
                    Value = new FhirString(valueStr)
                };
                
                observation.Performer.Add(new ResourceReference($"Practitioner/{practitioner.Id}"));
                list.Add(observation);
                obsIndex++;
            }
        }
        return list;
    }

    public System.Threading.Tasks.Task<string> GenerateInvoiceBundleAsync(string fhirJsonPayload)
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

        // Generate UUIDs for all resources to ensure robust reference resolution in ABDM viewer
        var patientGuid = Guid.NewGuid().ToString();
        var patientUuid = "urn:uuid:" + patientGuid;

        var practitionerGuid = Guid.NewGuid().ToString();
        var practitionerUuid = "urn:uuid:" + practitionerGuid;

        var orgGuid = Guid.NewGuid().ToString();
        var orgUuid = "urn:uuid:" + orgGuid;

        var encounterGuid = Guid.NewGuid().ToString();
        var encounterUuid = "urn:uuid:" + encounterGuid;

        var compositionGuid = Guid.NewGuid().ToString();
        var compositionUuid = "urn:uuid:" + compositionGuid;

        // 1. Create Patient
        var patient = new Patient
        {
            Id = patientGuid,
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
            Id = practitionerGuid,
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
            Id = orgGuid,
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
            Id = encounterGuid,
            Meta = CreateMeta(PROFILE_ENCOUNTER),
            Status = Encounter.EncounterStatus.InProgress,
            Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-Confidentiality", "AMB", "Ambulatory"),
            Subject = new ResourceReference(patientUuid) { Display = patientName },
            Period = new Period { Start = authoredOn }
        };

        var entries = new List<Bundle.EntryComponent>
        {
            new Bundle.EntryComponent { FullUrl = patientUuid, Resource = patient },
            new Bundle.EntryComponent { FullUrl = practitionerUuid, Resource = practitioner },
            new Bundle.EntryComponent { FullUrl = orgUuid, Resource = organization },
            new Bundle.EntryComponent { FullUrl = encounterUuid, Resource = encounter }
        };

        // Determine if this is structured (has lineItems) or unstructured (documents only)
        var lineItemsElement = GetProperty(root, "lineItems");
        var documentsElement = GetProperty(root, "documents");

        bool isStructured = lineItemsElement.ValueKind == JsonValueKind.Array && lineItemsElement.GetArrayLength() > 0;
        bool hasDocuments = documentsElement.ValueKind == JsonValueKind.Array && documentsElement.GetArrayLength() > 0;

        // If neither is present, default to structured (with default mock item) so we always have some invoice content.
        if (!isStructured && !hasDocuments)
        {
            isStructured = true;
        }

        Hl7.Fhir.Model.Invoice? invoice = null;
        var chargeItems = new List<(string Uuid, Hl7.Fhir.Model.ChargeItem Resource)>();
        string? invoiceUuid = null;

        if (isStructured)
        {
            var invoiceGuid = Guid.NewGuid().ToString();
            invoiceUuid = "urn:uuid:" + invoiceGuid;

            invoice = new Hl7.Fhir.Model.Invoice
            {
                Id = invoiceGuid,
                Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/Invoice"),
                Status = Hl7.Fhir.Model.Invoice.InvoiceStatus.Issued,
                Subject = new ResourceReference(patientUuid) { Display = patientName },
                DateElement = new FhirDateTime(authoredOn)
            };

            // Set Invoice Type (Consultation, Pharmacy, IPD, OPD, Others)
            var invoiceTypeStr = GetString(root, "invoiceType") ?? GetString(root, "type") ?? "";
            if (!string.IsNullOrEmpty(invoiceTypeStr))
            {
                string code = "99";
                string display = "Others";
                if (invoiceTypeStr.Equals("Consultation", StringComparison.OrdinalIgnoreCase) || invoiceTypeStr.Equals("00"))
                {
                    code = "00";
                    display = "Consultation";
                }
                else if (invoiceTypeStr.Equals("Pharmacy", StringComparison.OrdinalIgnoreCase) || invoiceTypeStr.Equals("01"))
                {
                    code = "01";
                    display = "Pharmacy";
                }
                else if (invoiceTypeStr.Equals("IPD", StringComparison.OrdinalIgnoreCase) || invoiceTypeStr.Equals("02"))
                {
                    code = "02";
                    display = "IPD";
                }
                else if (invoiceTypeStr.Equals("OPD", StringComparison.OrdinalIgnoreCase) || invoiceTypeStr.Equals("03"))
                {
                    code = "03";
                    display = "OPD";
                }

                invoice.Type = new CodeableConcept
                {
                    Text = display,
                    Coding = new List<Coding>
                    {
                        new Coding("https://nrces.in/ndhm/fhir/r4/CodeSystem/ndhm-billing-codes", code, display)
                    }
                };
            }

            decimal totalNetVal = 0;
            bool hasPrices = false;

            if (lineItemsElement.ValueKind == JsonValueKind.Array && lineItemsElement.GetArrayLength() > 0)
            {
                int seq = 1;
                foreach (var item in lineItemsElement.EnumerateArray())
                {
                    var itemName = GetString(item, "itemName") ?? GetString(item, "chargeItem") ?? "Consultation & Clinical Services";
                    var priceStr = GetString(item, "price") ?? "";
                    
                    var chargeItemGuid = Guid.NewGuid().ToString();
                    var chargeItemUuid = "urn:uuid:" + chargeItemGuid;
                    
                    string catCode = "99";
                    string catDisplay = "Others";
                    string lowerName = itemName.ToLower();
                    if (lowerName.Contains("consult"))
                    {
                        catCode = "00";
                        catDisplay = "Consultation";
                    }
                    else if (lowerName.Contains("pharmacy") || lowerName.Contains("medicine") || lowerName.Contains("drug"))
                    {
                        catCode = "01";
                        catDisplay = "Pharmacy";
                    }
                    else if (lowerName.Contains("ipd") || lowerName.Contains("room") || lowerName.Contains("bed") || lowerName.Contains("rent") || lowerName.Contains("ward"))
                    {
                        catCode = "02";
                        catDisplay = "IPD";
                    }
                    else if (lowerName.Contains("opd"))
                    {
                        catCode = "03";
                        catDisplay = "OPD";
                    }
                    else if (lowerName.Contains("pathology") || lowerName.Contains("lab") || lowerName.Contains("test") || lowerName.Contains("investigation"))
                    {
                        catCode = "04";
                        catDisplay = "Pathology";
                    }
                    else if (lowerName.Contains("nurs"))
                    {
                        catCode = "06";
                        catDisplay = "Nursing Charges";
                    }

                    decimal quantityVal = 1.0m;
                    string quantityUnit = "unit";
                    
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        if (item.TryGetProperty("quantity", out var qtyElem))
                        {
                            if (qtyElem.ValueKind == JsonValueKind.Number && qtyElem.TryGetDecimal(out decimal qVal))
                            {
                                quantityVal = qVal;
                            }
                            else if (qtyElem.ValueKind == JsonValueKind.String && decimal.TryParse(qtyElem.GetString(), out decimal qStrVal))
                            {
                                quantityVal = qStrVal;
                            }
                        }
                        if (item.TryGetProperty("unit", out var unitElem) && unitElem.ValueKind == JsonValueKind.String)
                        {
                            quantityUnit = unitElem.GetString() ?? "unit";
                        }
                    }

                    var chargeItem = new Hl7.Fhir.Model.ChargeItem
                    {
                        Id = chargeItemGuid,
                        Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/ChargeItem"),
                        Status = Hl7.Fhir.Model.ChargeItem.ChargeItemStatus.Billed,
                        Code = new CodeableConcept
                        {
                            Text = catDisplay,
                            Coding = new List<Coding>
                            {
                                new Coding("https://nrces.in/ndhm/fhir/r4/CodeSystem/ndhm-billing-codes", catCode, catDisplay)
                            }
                        },
                        Product = new CodeableConcept
                        {
                            Text = itemName,
                            Coding = new List<Coding>
                            {
                                new Coding("http://snomed.info/sct", "266753000", itemName)
                            }
                        },
                        Subject = new ResourceReference(patientUuid) { Display = patientName },
                        Quantity = new Quantity
                        {
                            Value = quantityVal,
                            Unit = quantityUnit,
                            System = "http://unitsofmeasure.org",
                            Code = quantityUnit == "unit" ? "{unit}" : quantityUnit
                        }
                    };

                    var lineItem = new Hl7.Fhir.Model.Invoice.LineItemComponent
                    {
                        Sequence = seq,
                        ChargeItem = new ResourceReference(chargeItemUuid) { Display = itemName }
                    };

                    lineItem.PriceComponent = new List<Hl7.Fhir.Model.Invoice.PriceComponentComponent>();

                    if (decimal.TryParse(priceStr, out decimal price))
                    {
                        hasPrices = true;
                        totalNetVal += price;
                        chargeItem.PriceOverride = new Money { Value = price, Currency = Hl7.Fhir.Model.Money.Currencies.INR };
                        
                        lineItem.PriceComponent.Add(new Hl7.Fhir.Model.Invoice.PriceComponentComponent
                        {
                            Type = Hl7.Fhir.Model.InvoicePriceComponentType.Base,
                            Code = new CodeableConcept
                            {
                                Text = itemName,
                                Coding = new List<Coding>
                                {
                                    new Coding("http://snomed.info/sct", "266753000", itemName)
                                }
                            },
                            Factor = 1.0m,
                            Amount = new Money { Value = price, Currency = Hl7.Fhir.Model.Money.Currencies.INR }
                        });
                    }
                    else
                    {
                        lineItem.PriceComponent.Add(new Hl7.Fhir.Model.Invoice.PriceComponentComponent
                        {
                            Type = Hl7.Fhir.Model.InvoicePriceComponentType.Base,
                            Factor = 1.0m
                        });
                    }
                    
                    chargeItems.Add((chargeItemUuid, chargeItem));
                    invoice.LineItem.Add(lineItem);
                    seq++;
                }
            }
            else
            {
                var chargeItemGuid = Guid.NewGuid().ToString();
                var chargeItemUuid = "urn:uuid:" + chargeItemGuid;
                
                var chargeItem = new Hl7.Fhir.Model.ChargeItem
                {
                    Id = chargeItemGuid,
                    Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/ChargeItem"),
                    Status = Hl7.Fhir.Model.ChargeItem.ChargeItemStatus.Billed,
                    Code = new CodeableConcept
                    {
                        Text = "Consultation",
                        Coding = new List<Coding>
                        {
                            new Coding("https://nrces.in/ndhm/fhir/r4/CodeSystem/ndhm-billing-codes", "00", "Consultation")
                        }
                    },
                    Product = new CodeableConcept
                    {
                        Text = "Consultation & Clinical Services",
                        Coding = new List<Coding>
                        {
                            new Coding("http://snomed.info/sct", "266753000", "Consultation & Clinical Services")
                        }
                    },
                    Subject = new ResourceReference(patientUuid) { Display = patientName },
                    Quantity = new Quantity
                    {
                        Value = 1.0m,
                        Unit = "unit",
                        System = "http://unitsofmeasure.org",
                        Code = "{unit}"
                    }
                };
                chargeItems.Add((chargeItemUuid, chargeItem));

                invoice.LineItem.Add(new Hl7.Fhir.Model.Invoice.LineItemComponent
                {
                    Sequence = 1,
                    ChargeItem = new ResourceReference(chargeItemUuid) { Display = "Consultation & Clinical Services" }
                });
            }

            if (hasPrices)
            {
                invoice.TotalNet = new Money { Value = totalNetVal, Currency = Hl7.Fhir.Model.Money.Currencies.INR };
                invoice.TotalGross = new Money { Value = totalNetVal, Currency = Hl7.Fhir.Model.Money.Currencies.INR };
            }
        }

        // 5. Create Composition for Invoice Record
        var composition = new Composition
        {
            Id = compositionGuid,
            Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/InvoiceRecord"),
            Identifier = new Identifier { System = "https://ABDM_WRAPPER/bundle", Value = Guid.NewGuid().ToString() },
            Status = CompositionStatus.Final,
            Type = new CodeableConcept
            {
                Text = "Invoice",
                Coding = new List<Coding>
                {
                    new Coding(SNOMED_URL, "423876004", "Invoice Document")
                }
            },
            Subject = new ResourceReference(patientUuid) { Display = patientName },
            Encounter = new ResourceReference(encounterUuid) { Display = "Ambulatory" },
            DateElement = new FhirDateTime(authoredOn),
            Custodian = new ResourceReference(orgUuid) { Display = orgName },
            Title = "Invoice"
        };
        composition.Author.Add(new ResourceReference(practitionerUuid) { Display = practitionerName });

        // Add structured Invoice if present
        if (invoice != null && invoiceUuid != null)
        {
            var invoiceSection = new Composition.SectionComponent
            {
                Title = "Invoice",
                Code = new CodeableConcept
                {
                    Text = "Invoice",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "423876004", "Invoice Document")
                    }
                }
            };
            invoiceSection.Entry.Add(new ResourceReference(invoiceUuid));
            composition.Section.Add(invoiceSection);

            entries.Add(new Bundle.EntryComponent { FullUrl = invoiceUuid, Resource = invoice });
            foreach (var ci in chargeItems)
            {
                entries.Add(new Bundle.EntryComponent { FullUrl = ci.Uuid, Resource = ci.Resource });
            }
        }

        // Add unstructured Documents if present
        if (hasDocuments)
        {
            var docSection = new Composition.SectionComponent
            {
                Title = "Invoice Document",
                Code = new CodeableConcept
                {
                    Text = "Invoice Document",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "423876004", "Invoice Document")
                    }
                }
            };

            foreach (var d in documentsElement.EnumerateArray())
            {
                var contentType = GetString(d, "contentType") ?? "application/pdf";
                var typeStr = GetString(d, "type") ?? "Invoice";
                var dataBase64 = GetString(d, "data") ?? "";

                byte[]? rawData = null;
                if (!string.IsNullOrEmpty(dataBase64))
                {
                    try { rawData = Convert.FromBase64String(dataBase64); } catch {}
                }

                if (rawData != null)
                {
                    var docRefGuid = Guid.NewGuid().ToString();
                    var docRefUuid = "urn:uuid:" + docRefGuid;

                    var attachment = new Attachment
                    {
                        ContentType = contentType,
                        Data = rawData,
                        Title = typeStr,
                        CreationElement = new FhirDateTime(DateTimeOffset.UtcNow)
                    };

                    var docRef = new DocumentReference
                    {
                        Id = docRefGuid,
                        Meta = CreateMeta(PROFILE_DOCUMENT_REFERENCE),
                        Status = DocumentReferenceStatus.Current,
                        DocStatus = CompositionStatus.Final,
                        Subject = new ResourceReference(patientUuid) { Display = patientName },
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
                                new Coding(SNOMED_URL, "423876004", "Invoice Document")
                            }
                        }
                    });

                    docSection.Entry.Add(new ResourceReference(docRefUuid));
                    entries.Add(new Bundle.EntryComponent { FullUrl = docRefUuid, Resource = docRef });
                }
            }
            composition.Section.Add(docSection);
        }

        // Insert Composition at the beginning of the entries list
        entries.Insert(0, new Bundle.EntryComponent { FullUrl = compositionUuid, Resource = composition });

        // 8. Assemble Bundle with Meta/Security
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

        // 9. Serialize
        var serializer = new FhirJsonSerializer();
        var fhirJson = serializer.SerializeToString(bundle);

        return System.Threading.Tasks.Task.FromResult(fhirJson);
    }
}
