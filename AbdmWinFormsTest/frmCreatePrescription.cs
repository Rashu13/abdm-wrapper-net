using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.Json;

namespace HMS.abdm
{
    public partial class frmCreatePrescription : Form
    {
        public frmCreatePrescription()
        {
            InitializeComponent();
        }

        private void BtnGenerateFHIR_Click(object sender, EventArgs e)
        {
            try
            {
                var bundle = GeneratePrescriptionBundle();
                string jsonString = JsonSerializer.Serialize(bundle, new JsonSerializerOptions { WriteIndented = true });
                txtOutput.Text = jsonString;
                MessageBox.Show("FHIR Bundle Generated Successfully! Yeh JSON ab ABDM me Data Push API ke through bheja ja sakta hai.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating FHIR: " + ex.Message);
            }
        }

        private Dictionary<string, object> GeneratePrescriptionBundle()
        {
            string patientName = txtPatientName.Text;
            string abha = txtPatientABHA.Text;
            string diagnosis = txtDiagnosis.Text;
            
            var bundle = new Dictionary<string, object>
            {
                { "resourceType", "Bundle" },
                { "id", Guid.NewGuid().ToString() },
                { "type", "document" },
                { "timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                { "identifier", new Dictionary<string, object>
                    {
                        { "system", "https://ndhm.in/hip/fhir/OPD/prescription" },
                        { "value", "PR-" + DateTime.Now.Ticks.ToString() }
                    }
                }
            };

            var entries = new List<Dictionary<string, object>>();

            string compId = Guid.NewGuid().ToString();
            string patientId = Guid.NewGuid().ToString();
            string doctorId = Guid.NewGuid().ToString();
            
            // 1. Composition (The main wrapper)
            entries.Add(new Dictionary<string, object>
            {
                { "fullUrl", "urn:uuid:" + compId },
                { "resource", new Dictionary<string, object>
                    {
                        { "resourceType", "Composition" },
                        { "id", compId },
                        { "status", "final" },
                        { "type", new Dictionary<string, object> { { "text", "Prescription record" } } },
                        { "subject", new Dictionary<string, object> { { "reference", "urn:uuid:" + patientId } } },
                        { "author", new List<object> { new Dictionary<string, object> { { "reference", "urn:uuid:" + doctorId } } } },
                        { "title", "OPD Prescription" },
                        { "date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                    }
                }
            });

            // 2. Patient
            entries.Add(new Dictionary<string, object>
            {
                { "fullUrl", "urn:uuid:" + patientId },
                { "resource", new Dictionary<string, object>
                    {
                        { "resourceType", "Patient" },
                        { "id", patientId },
                        { "name", new List<object> { new Dictionary<string, object> { { "text", patientName } } } },
                        { "identifier", new List<object> 
                            { 
                                new Dictionary<string, object> 
                                { 
                                    { "system", "https://healthid.ndhm.gov.in" },
                                    { "value", abha }
                                } 
                            } 
                        }
                    }
                }
            });

            // 3. Condition
            entries.Add(new Dictionary<string, object>
            {
                { "fullUrl", "urn:uuid:" + Guid.NewGuid().ToString() },
                { "resource", new Dictionary<string, object>
                    {
                        { "resourceType", "Condition" },
                        { "code", new Dictionary<string, object> { { "text", diagnosis } } },
                        { "subject", new Dictionary<string, object> { { "reference", "urn:uuid:" + patientId } } }
                    }
                }
            });

            // 4. Medications
            foreach (DataGridViewRow row in gridMedicines.Rows)
            {
                if (row.IsNewRow) continue;
                
                string medName = row.Cells["Medicine"].Value?.ToString() ?? "";
                string dosage = row.Cells["Dosage"].Value?.ToString() ?? "";
                
                if (!string.IsNullOrEmpty(medName))
                {
                    entries.Add(new Dictionary<string, object>
                    {
                        { "fullUrl", "urn:uuid:" + Guid.NewGuid().ToString() },
                        { "resource", new Dictionary<string, object>
                            {
                                { "resourceType", "MedicationRequest" },
                                { "status", "active" },
                                { "intent", "order" },
                                { "subject", new Dictionary<string, object> { { "reference", "urn:uuid:" + patientId } } },
                                { "medicationCodeableConcept", new Dictionary<string, object> { { "text", medName } } },
                                { "dosageInstruction", new List<object> { new Dictionary<string, object> { { "text", dosage } } } }
                            }
                        }
                    });
                }
            }

            bundle["entry"] = entries;
            return bundle;
        }
    }
}
