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
                var medName = GetString(p, "medicine") ?? GetString(p, "name") ?? "Unknown Medicine";
                var dosage = GetString(p, "dosage") ?? "";
                var explicitSnomed = GetString(p, "snomedCode") ?? GetString(p, "code");
                var snomedCode = ResolveSnomedMedicineCode(medName, explicitSnomed);

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
                            new Coding(SNOMED_URL, snomedCode, medName)
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
                                Coding = new List<Coding>
                                {
                                    new Coding(SNOMED_URL, "1000000570007", additionalInstructions)
                                }
                            }
                        };
                    }

                    var route = GetString(p, "route") ?? "";
                    if (!string.IsNullOrEmpty(route))
                    {
                        string rCode = "26643006"; // Default Oral
                        if (route.Equals("IV", StringComparison.OrdinalIgnoreCase)) rCode = "47625008";
                        else if (route.Equals("IM", StringComparison.OrdinalIgnoreCase)) rCode = "78421000";
                        else if (route.Equals("Topical", StringComparison.OrdinalIgnoreCase)) rCode = "6064005";
                        else if (route.Equals("Inhalation", StringComparison.OrdinalIgnoreCase)) rCode = "418187005";
                        else if (route.Equals("Sublingual", StringComparison.OrdinalIgnoreCase)) rCode = "372473007";

                        dosageInst.Route = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, rCode, route)
                            }
                        };
                    }

                    var method = GetString(p, "method") ?? "";
                    if (!string.IsNullOrEmpty(method))
                    {
                        string mCode = "311504000"; // Default With or after food
                        if (method.Equals("Before Food", StringComparison.OrdinalIgnoreCase) || method.Equals("Before Meal", StringComparison.OrdinalIgnoreCase)) mCode = "252160004";
                        else if (method.Equals("After Food", StringComparison.OrdinalIgnoreCase) || method.Equals("After Meal", StringComparison.OrdinalIgnoreCase)) mCode = "262235003";
                        else if (method.Equals("Empty Stomach", StringComparison.OrdinalIgnoreCase)) mCode = "252161000";

                        if (dosageInst.AdditionalInstruction == null)
                        {
                            dosageInst.AdditionalInstruction = new List<CodeableConcept>();
                        }
                        dosageInst.AdditionalInstruction.Add(new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, mCode, method)
                            }
                        });
                    }

                    // Parse dosagePattern to get the frequency, period, and unit
                    int freq = 1;
                    if (dosage.Contains("1-0-1")) freq = 2;
                    else if (dosage.Contains("1-1-1")) freq = 3;

                    dosageInst.Timing = new Timing
                    {
                        Repeat = new Timing.RepeatComponent
                        {
                            Frequency = freq,
                            Period = 1,
                            PeriodUnit = Timing.UnitsOfTime.D
                        }
                    };

                    medReq.DosageInstruction = new List<Dosage> { dosageInst };
                }

                var reason = GetString(p, "reason");
                if (!string.IsNullOrEmpty(reason))
                {
                    medReq.ReasonCode = new List<CodeableConcept>
                    {
                        new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "55607006", reason)
                            }
                        }
                    };

                    var reasonCondition = new Condition
                    {
                        Id = $"Condition-PrescriptionReason-{medIndex}",
                        Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/Condition"),
                        ClinicalStatus = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding("http://terminology.hl7.org/CodeSystem/condition-clinical", "active", "Active")
                            }
                        },
                        Code = new CodeableConcept
                        {
                            Text = reason,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "404684003", reason)
                            }
                        },
                        Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName }
                    };

                    medReq.ReasonReference.Add(new ResourceReference($"Condition/{reasonCondition.Id}") { Display = reason });
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"Condition/{reasonCondition.Id}", Resource = reasonCondition });
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

        // 6. Add Observation for Notes if present
        var notes = "";
        if (root.TryGetProperty("clinicalNotes", out var notesProp) && notesProp.ValueKind == JsonValueKind.String)
        {
            notes = notesProp.GetString() ?? "";
        }
        else if (root.TryGetProperty("clinicalObservation", out var obsProp) && obsProp.ValueKind == JsonValueKind.String)
        {
            notes = obsProp.GetString() ?? "";
        }

        if (!string.IsNullOrEmpty(notes))
        {
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
        }

        // 6a. Add Chief Complaints as Condition resources
        var complaintsElement = GetProperty(root, "complaints");
        if (complaintsElement.ValueKind == JsonValueKind.Array && complaintsElement.GetArrayLength() > 0)
        {
            var complaintsSection = new Composition.SectionComponent
            {
                Title = "Chief complaints",
                Code = new CodeableConcept
                {
                    Text = "Chief complaints",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "422843007", "Chief complaint section")
                    }
                }
            };

            int complaintIndex = 1;
            foreach (var c in complaintsElement.EnumerateArray())
            {
                var text = c.GetString();
                if (!string.IsNullOrEmpty(text))
                {
                    var condition = new Condition
                    {
                        Id = $"Condition-Complaint-{complaintIndex}",
                        Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/Condition"),
                        ClinicalStatus = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding("http://terminology.hl7.org/CodeSystem/condition-clinical", "active", "Active")
                            }
                        },
                        Code = new CodeableConcept
                        {
                            Text = text,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "404684003", text)
                            }
                        },
                        Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName }
                    };

                    complaintsSection.Entry.Add(new ResourceReference($"Condition/{condition.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"Condition/{condition.Id}", Resource = condition });
                    complaintIndex++;
                }
            }

            if (complaintsSection.Entry.Count > 0)
            {
                composition.Section.Add(complaintsSection);
            }
        }

        // 6b. Add Allergies as AllergyIntolerance resources
        var allergiesElement = GetProperty(root, "allergies");
        if (allergiesElement.ValueKind == JsonValueKind.Array && allergiesElement.GetArrayLength() > 0)
        {
            var allergiesSection = new Composition.SectionComponent
            {
                Title = "Allergies",
                Code = new CodeableConcept
                {
                    Text = "Allergies",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "722446000", "Allergy record")
                    }
                }
            };

            int allergyIndex = 1;
            foreach (var a in allergiesElement.EnumerateArray())
            {
                var allergyName = GetString(a, "allergyName");
                var status = GetString(a, "status") ?? "active";

                if (!string.IsNullOrEmpty(allergyName))
                {
                    var allergy = new AllergyIntolerance
                    {
                        Id = $"AllergyIntolerance-{allergyIndex}",
                        Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/AllergyIntolerance"),
                        ClinicalStatus = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding("http://terminology.hl7.org/CodeSystem/allergyintolerance-clinical", status, status)
                            }
                        },
                        VerificationStatus = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding("http://terminology.hl7.org/CodeSystem/allergyintolerance-verification", "confirmed", "Confirmed")
                            }
                        },
                        Code = new CodeableConcept
                        {
                            Text = allergyName,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "716186003", allergyName)
                            }
                        },
                        Patient = new ResourceReference($"Patient/{patient.Id}") { Display = patientName }
                    };

                    allergiesSection.Entry.Add(new ResourceReference($"AllergyIntolerance/{allergy.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"AllergyIntolerance/{allergy.Id}", Resource = allergy });
                    allergyIndex++;
                }
            }

            if (allergiesSection.Entry.Count > 0)
            {
                composition.Section.Add(allergiesSection);
            }
        }

        // 6c. Add Medical History as Condition resources
        var historyElement = GetProperty(root, "medicalHistory");
        if (historyElement.ValueKind == JsonValueKind.Array && historyElement.GetArrayLength() > 0)
        {
            var historySection = new Composition.SectionComponent
            {
                Title = "Medical History",
                Code = new CodeableConcept
                {
                    Text = "Medical History",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "371529009", "History and physical report")
                    }
                }
            };

            int historyIndex = 1;
            foreach (var h in historyElement.EnumerateArray())
            {
                var text = h.GetString();
                if (!string.IsNullOrEmpty(text))
                {
                    var condition = new Condition
                    {
                        Id = $"Condition-History-{historyIndex}",
                        Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/Condition"),
                        ClinicalStatus = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding("http://terminology.hl7.org/CodeSystem/condition-clinical", "active", "Active")
                            }
                        },
                        Code = new CodeableConcept
                        {
                            Text = text,
                            Coding = new List<Coding>
                            {
                                new Coding(SNOMED_URL, "392521001", text)
                            }
                        },
                        Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName }
                    };

                    historySection.Entry.Add(new ResourceReference($"Condition/{condition.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"Condition/{condition.Id}", Resource = condition });
                    historyIndex++;
                }
            }

            if (historySection.Entry.Count > 0)
            {
                composition.Section.Add(historySection);
            }
        }

        // 6d. Add Diagnosis as Condition
        var diagnosisStr = GetString(root, "diagnosis");
        if (!string.IsNullOrEmpty(diagnosisStr))
        {
            var diagCondition = new Condition
            {
                Id = "Condition-Diagnosis",
                Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/Condition"),
                ClinicalStatus = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding("http://terminology.hl7.org/CodeSystem/condition-clinical", "active", "Active")
                    }
                },
                Code = new CodeableConcept
                {
                    Text = diagnosisStr,
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "439401001", diagnosisStr)
                    }
                },
                Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName }
            };

            encounter.Diagnosis.Add(new Encounter.DiagnosisComponent
            {
                Condition = new ResourceReference($"Condition/{diagCondition.Id}"),
                Use = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "39154008", "Clinical diagnosis")
                    }
                }
            });

            var diagnosisSection = new Composition.SectionComponent
            {
                Title = "Diagnosis",
                Code = new CodeableConcept
                {
                    Text = "Diagnosis",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "439401001", "Diagnosis")
                    }
                }
            };

            diagnosisSection.Entry.Add(new ResourceReference($"Condition/{diagCondition.Id}"));
            composition.Section.Add(diagnosisSection);
            entries.Add(new Bundle.EntryComponent { FullUrl = $"Condition/{diagCondition.Id}", Resource = diagCondition });
        }

        // 6e. Add Physical Findings (Vitals and Body Measurements)
        var vitalsElement = GetProperty(root, "vitals");
        var hasVitals = vitalsElement.ValueKind == JsonValueKind.Array && vitalsElement.GetArrayLength() > 0;
        var bodyMeasurementsElement = GetProperty(root, "bodyMeasurements");
        var hasBody = bodyMeasurementsElement.ValueKind == JsonValueKind.Object;

        if (hasVitals || hasBody)
        {
            var physicalSection = new Composition.SectionComponent
            {
                Title = "Physical Findings",
                Code = new CodeableConcept
                {
                    Text = "Physical Findings",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "2931005", "Physical Findings")
                    }
                }
            };

            int obsIdx = 2;

            if (hasVitals)
            {
                foreach (var v in vitalsElement.EnumerateArray())
                {
                    var vitalName = GetString(v, "vitalName");
                    var val = GetString(v, "value");
                    var unit = GetString(v, "unit");

                    if (!string.IsNullOrEmpty(vitalName) && !string.IsNullOrEmpty(val))
                    {
                        var vitalObs = new Observation
                        {
                            Id = $"Observation-Vital-{obsIdx}",
                            Meta = CreateMeta(PROFILE_OBSERVATION),
                            Status = ObservationStatus.Final,
                            Code = new CodeableConcept
                            {
                                Text = vitalName,
                                Coding = new List<Coding>
                                {
                                    new Coding(SNOMED_URL, "118247008", vitalName)
                                }
                            },
                            Subject = new ResourceReference($"Patient/{patient.Id}"),
                            Value = new FhirString(string.IsNullOrEmpty(unit) ? val : $"{val} {unit}")
                        };

                        physicalSection.Entry.Add(new ResourceReference($"Observation/{vitalObs.Id}"));
                        entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{vitalObs.Id}", Resource = vitalObs });
                        obsIdx++;
                    }
                }
            }

            if (hasBody)
            {
                var height = GetString(bodyMeasurementsElement, "heightCm");
                var weight = GetString(bodyMeasurementsElement, "weightKg");

                if (!string.IsNullOrEmpty(height))
                {
                    var heightObs = new Observation
                    {
                        Id = $"Observation-Height-{obsIdx}",
                        Meta = CreateMeta(PROFILE_OBSERVATION),
                        Status = ObservationStatus.Final,
                        Code = new CodeableConcept
                        {
                            Text = "Height",
                            Coding = new List<Coding> { new Coding(SNOMED_URL, "50373000", "Height") }
                        },
                        Subject = new ResourceReference($"Patient/{patient.Id}"),
                        Value = new FhirString($"{height} cm")
                    };
                    physicalSection.Entry.Add(new ResourceReference($"Observation/{heightObs.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{heightObs.Id}", Resource = heightObs });
                    obsIdx++;
                }

                if (!string.IsNullOrEmpty(weight))
                {
                    var weightObs = new Observation
                    {
                        Id = $"Observation-Weight-{obsIdx}",
                        Meta = CreateMeta(PROFILE_OBSERVATION),
                        Status = ObservationStatus.Final,
                        Code = new CodeableConcept
                        {
                            Text = "Weight",
                            Coding = new List<Coding> { new Coding(SNOMED_URL, "27113001", "Weight") }
                        },
                        Subject = new ResourceReference($"Patient/{patient.Id}"),
                        Value = new FhirString($"{weight} kg")
                    };
                    physicalSection.Entry.Add(new ResourceReference($"Observation/{weightObs.Id}"));
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{weightObs.Id}", Resource = weightObs });
                    obsIdx++;
                }
            }

            if (physicalSection.Entry.Count > 0)
            {
                composition.Section.Add(physicalSection);
            }
        }

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

        // 5b. Create Observations and DiagnosticReport for labResults if present
        var labResultsElement = GetProperty(root, "labResults");
        if (labResultsElement.ValueKind == JsonValueKind.Array && labResultsElement.GetArrayLength() > 0)
        {
            var diagnosticReport = new DiagnosticReport
            {
                Id = "DiagnosticReport-1",
                Meta = CreateMeta("https://nrces.in/ndhm/fhir/r4/StructureDefinition/DiagnosticReport"),
                Status = DiagnosticReport.DiagnosticReportStatus.Final,
                Code = new CodeableConcept
                {
                    Text = "Diagnostic Report",
                    Coding = new List<Coding>
                    {
                        new Coding(SNOMED_URL, "721981007", "Diagnostic Report")
                    }
                },
                Subject = new ResourceReference($"Patient/{patient.Id}") { Display = patientName },
                IssuedElement = new Instant(DateTimeOffset.UtcNow)
            };
            
            if (practitionersElement.ValueKind == JsonValueKind.Array && practitionersElement.GetArrayLength() > 0)
            {
                diagnosticReport.Performer.Add(new ResourceReference($"Practitioner/{practitioner.Id}") { Display = practitionerName });
            }

            int obsIndex = 1;
            foreach (var r in labResultsElement.EnumerateArray())
            {
                var tName = GetString(r, "testName") ?? "Unknown Test";
                var tVal = GetString(r, "value") ?? "";
                var tUnit = GetString(r, "unit") ?? "";

                var observation = new Observation
                {
                    Id = $"Observation-{obsIndex}",
                    Meta = CreateMeta(PROFILE_OBSERVATION),
                    Status = ObservationStatus.Final,
                    Code = new CodeableConcept
                    {
                        Text = tName,
                        Coding = new List<Coding>
                        {
                            new Coding(SNOMED_URL, "261665006", tName)
                        }
                    },
                    Subject = new ResourceReference($"Patient/{patient.Id}"),
                    Value = new FhirString(string.IsNullOrEmpty(tUnit) ? tVal : $"{tVal} {tUnit}")
                };

                entries.Add(new Bundle.EntryComponent { FullUrl = $"Observation/{observation.Id}", Resource = observation });
                diagnosticReport.Result.Add(new ResourceReference($"Observation/{observation.Id}"));
                obsIndex++;
            }

            entries.Add(new Bundle.EntryComponent { FullUrl = $"DiagnosticReport/{diagnosticReport.Id}", Resource = diagnosticReport });
            docSection.Entry.Add(new ResourceReference($"DiagnosticReport/{diagnosticReport.Id}"));
        }

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
                var manufacturer = GetString(immEl, "manufacturer");

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

                if (!string.IsNullOrEmpty(manufacturer))
                {
                    var manufacturerOrg = new Organization
                    {
                        Id = $"Organization-Manufacturer-{immIndex}",
                        Meta = CreateMeta(PROFILE_ORGANIZATION),
                        Name = manufacturer
                    };
                    manufacturerOrg.Identifier.Add(new Identifier
                    {
                        System = "https://facility.abdm.gov.in",
                        Value = $"MFG-{immIndex}",
                        Type = new CodeableConcept
                        {
                            Text = "Manufacturer number",
                            Coding = new List<Coding>
                            {
                                new Coding(IDENTIFIER_TYPE_SYSTEM, "PRN", "Manufacturer number")
                            }
                        }
                    });
                    entries.Add(new Bundle.EntryComponent { FullUrl = $"Organization/{manufacturerOrg.Id}", Resource = manufacturerOrg });
                    immunization.Manufacturer = new ResourceReference($"Organization/{manufacturerOrg.Id}") { Display = manufacturer };
                }

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

    private static string ResolveSnomedMedicineCode(string medName, string? explicitSnomedCode)
    {
        if (!string.IsNullOrWhiteSpace(explicitSnomedCode))
        {
            return explicitSnomedCode.Trim();
        }

        if (string.IsNullOrWhiteSpace(medName)) return "387517004";

        var clean = medName.Trim().ToLowerInvariant();

        // Antipyretics & Analgesics
        if (clean.Contains("dolo") || clean.Contains("paracetamol 650") || clean.Contains("pcm 650"))
            return "322236009"; // Paracetamol 650 mg
        if (clean.Contains("paracetamol") || clean.Contains("pcm") || clean.Contains("crocin") || clean.Contains("calpol"))
            return "387517004"; // Paracetamol
        if (clean.Contains("ibuprofen") || clean.Contains("brufen") || clean.Contains("combiflam"))
            return "387207008"; // Ibuprofen
        if (clean.Contains("diclofenac") || clean.Contains("voveran"))
            return "372572004"; // Diclofenac
        if (clean.Contains("aceclofenac") || clean.Contains("zerodol"))
            return "387522004"; // Aceclofenac
        if (clean.Contains("mefenamic") || clean.Contains("meftal"))
            return "372861001"; // Mefenamic Acid
        if (clean.Contains("tramadol") || clean.Contains("ultram"))
            return "387140003"; // Tramadol
        if (clean.Contains("nimesulide") || clean.Contains("nise"))
            return "372552003"; // Nimesulide

        // Antacids & GI
        if (clean.Contains("pantoprazole") || clean.Contains("pan 40") || clean.Contains("pan-40") || clean.Contains("pan-d"))
            return "372605007"; // Pantoprazole
        if (clean.Contains("omeprazole") || clean.Contains("omez"))
            return "372722002"; // Omeprazole
        if (clean.Contains("rabeprazole") || clean.Contains("razo") || clean.Contains("rablet"))
            return "372619001"; // Rabeprazole
        if (clean.Contains("ranitidine") || clean.Contains("aciloc") || clean.Contains("rantac"))
            return "372765001"; // Ranitidine
        if (clean.Contains("ondansetron") || clean.Contains("emeset") || clean.Contains("vomikind"))
            return "372561000"; // Ondansetron
        if (clean.Contains("domperidone") || clean.Contains("vomiplus"))
            return "372535003"; // Domperidone
        if (clean.Contains("sucralfate") || clean.Contains("sucrafil"))
            return "372810002"; // Sucralfate

        // Antibiotics
        if (clean.Contains("amoxicillin") || clean.Contains("moxikind") || clean.Contains("augmentin") || clean.Contains("novamox"))
            return "372687004"; // Amoxicillin
        if (clean.Contains("azithromycin") || clean.Contains("azee") || clean.Contains("zathrin"))
            return "372522002"; // Azithromycin
        if (clean.Contains("ciprofloxacin") || clean.Contains("cifran"))
            return "372828008"; // Ciprofloxacin
        if (clean.Contains("levofloxacin") || clean.Contains("levoquin"))
            return "372545009"; // Levofloxacin
        if (clean.Contains("cefixime") || clean.Contains("taxim") || clean.Contains("zifi"))
            return "372527008"; // Cefixime
        if (clean.Contains("cefpodoxime") || clean.Contains("gudcef"))
            return "372530006"; // Cefpodoxime
        if (clean.Contains("ceftriaxone") || clean.Contains("monocef"))
            return "372528003"; // Ceftriaxone
        if (clean.Contains("doxycycline") || clean.Contains("dox"))
            return "372827003"; // Doxycycline
        if (clean.Contains("metronidazole") || clean.Contains("flagyl"))
            return "372594002"; // Metronidazole

        // Antihistamines & Respiratory
        if (clean.Contains("cetirizine") || clean.Contains("cetzine") || clean.Contains("okacet"))
            return "372583007"; // Cetirizine
        if (clean.Contains("levocetirizine") || clean.Contains("lecope") || clean.Contains("1-al") || clean.Contains("montek"))
            return "372583007"; // Levocetirizine
        if (clean.Contains("fexofenadine") || clean.Contains("allegra"))
            return "372541005"; // Fexofenadine
        if (clean.Contains("salbutamol") || clean.Contains("asthalin") || clean.Contains("ascoril"))
            return "372599007"; // Salbutamol
        if (clean.Contains("budesonide") || clean.Contains("budecort"))
            return "372825006"; // Budesonide

        // Diabetes
        if (clean.Contains("metformin") || clean.Contains("glycomet") || clean.Contains("obimet"))
            return "372567009"; // Metformin
        if (clean.Contains("glimepiride") || clean.Contains("amaryl"))
            return "372548006"; // Glimepiride
        if (clean.Contains("teneligliptin") || clean.Contains("tenglyn"))
            return "712398001"; // Teneligliptin
        if (clean.Contains("sitagliptin") || clean.Contains("januvia"))
            return "702543004"; // Sitagliptin
        if (clean.Contains("dapagliflozin") || clean.Contains("forxiga"))
            return "703663004"; // Dapagliflozin
        if (clean.Contains("insulin") || clean.Contains("mixtard"))
            return "372560004"; // Insulin

        // Cardiovascular
        if (clean.Contains("amlodipine") || clean.Contains("stamlo") || clean.Contains("amlong"))
            return "372833007"; // Amlodipine
        if (clean.Contains("telmisartan") || clean.Contains("telma") || clean.Contains("tazloc"))
            return "372862008"; // Telmisartan
        if (clean.Contains("losartan") || clean.Contains("repace") || clean.Contains("losar"))
            return "372860000"; // Losartan
        if (clean.Contains("atenolol") || clean.Contains("aten"))
            return "372836004"; // Atenolol
        if (clean.Contains("atorvastatin") || clean.Contains("atorva"))
            return "372854002"; // Atorvastatin
        if (clean.Contains("rosuvastatin") || clean.Contains("rosuvas"))
            return "372856000"; // Rosuvastatin
        if (clean.Contains("aspirin") || clean.Contains("ecosprin"))
            return "387170009"; // Aspirin
        if (clean.Contains("clopidogrel") || clean.Contains("clopilet"))
            return "372850006"; // Clopidogrel

        // Supplements
        if (clean.Contains("calcium") || clean.Contains("shelcal") || clean.Contains("vitamin") || clean.Contains("becosules") || clean.Contains("neurobion"))
            return "421689004"; // Vitamin / Calcium Supplement

        return "387517004"; // Standard Active Pharmaceutical Product
    }
}
