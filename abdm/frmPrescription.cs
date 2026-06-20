using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmPrescription : Form
    {
        private readonly AbdmApiClient _client;
        private byte[] _pdfBytes = null;
        private string _pdfFileName = string.Empty;

        public frmPrescription()
        {
            InitializeComponent();
            _client = GetAbdmClient();
            InitializeDefaultValues();
        }

        private AbdmApiClient GetAbdmClient()
        {
            var settings = new AbdmSettings
            {
                BaseUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:BaseUrl"] ?? "https://sbx.wati.digital",
                AbhaServiceUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:AbhaServiceUrl"] ?? "https://abhasbx.abdm.gov.in/abha/api/v3",
                ClientId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientId"],
                ClientSecret = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientSecret"],
                HipId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipId"] ?? "IN0610090658",
                HipName = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipName"] ?? "MIDHA HOSPITAL",
                CmId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:CmId"] ?? "sbx",
                Environment = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:Environment"] ?? "Sandbox"
            };
            return new AbdmApiClient(settings);
        }

        private void InitializeDefaultValues()
        {
            txtPatientRef.Text = "PT-" + DateTime.Now.ToString("yyMMddHHmmss");
            txtCareContextRef.Text = "CC-" + DateTime.Now.ToString("yyMMddHHmmss");
            txtCareContextDisplay.Text = "Prescription on " + DateTime.Now.ToString("dd-MMM-yyyy");
            cmbGender.SelectedIndex = 0;
            cmbRecordType.SelectedIndex = 0; // PrescriptionRecord
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddMedicine_Click(object sender, EventArgs e)
        {
            string medName = txtMedicineName.Text.Trim();
            string dosage = txtDosage.Text.Trim();

            if (string.IsNullOrEmpty(medName))
            {
                MessageBox.Show("Please enter a medicine name.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = new ListViewItem(medName);
            item.SubItems.Add(dosage);
            lvMedicines.Items.Add(item);

            txtMedicineName.Clear();
            txtDosage.Clear();
            txtMedicineName.Focus();
        }

        private void btnRemoveMedicine_Click(object sender, EventArgs e)
        {
            if (lvMedicines.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lvMedicines.SelectedItems)
                {
                    lvMedicines.Items.Remove(item);
                }
            }
            else
            {
                MessageBox.Show("Please select a medicine from the list to remove.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUploadPdf_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PDF Files (*.pdf)|*.pdf";
                ofd.Title = "Select Prescription PDF";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _pdfBytes = File.ReadAllBytes(ofd.FileName);
                        _pdfFileName = Path.GetFileName(ofd.FileName);
                        lblPdfStatus.Text = $"Attached: {_pdfFileName} ({(_pdfBytes.Length / 1024.0):F1} KB)";
                        lblPdfStatus.ForeColor = Color.DarkGreen;
                        
                        // Switch record type to HealthDocumentRecord automatically as PDF is attached
                        cmbRecordType.SelectedIndex = 2; // HealthDocumentRecord
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void btnSaveAndPush_Click(object sender, EventArgs e)
        {
            await SaveAndPushActionAsync();
        }

        private async Task<bool> SaveAndPushActionAsync()
        {
            if (string.IsNullOrWhiteSpace(txtAbhaAddress.Text) || string.IsNullOrWhiteSpace(txtPatientName.Text))
            {
                MessageBox.Show("Please enter ABHA Address and Patient Name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Auto-generate PDF prescription report if not manually attached
            if (_pdfBytes == null)
            {
                AppendLog("No manual PDF uploaded. Auto-generating prescription PDF report...");
                var medListForPdf = new List<string[]>();
                foreach (ListViewItem item in lvMedicines.Items)
                {
                    medListForPdf.Add(new string[] { item.Text, item.SubItems[1].Text });
                }
                
                _pdfBytes = GeneratePrescriptionPdf(
                    txtPatientName.Text.Trim(),
                    txtAbhaAddress.Text.Trim(),
                    cmbGender.Text.Trim(),
                    txtDob.Text.Trim(),
                    txtCareContextRef.Text.Trim(),
                    medListForPdf
                );
                _pdfFileName = $"Prescription_{txtCareContextRef.Text.Trim()}.pdf";
                lblPdfStatus.Text = $"Auto-Generated: {_pdfFileName} ({(_pdfBytes.Length / 1024.0):F1} KB)";
                lblPdfStatus.ForeColor = Color.DarkGreen;
                
                // Automatically switch record type to HealthDocumentRecord to include the PDF attachment
                cmbRecordType.SelectedIndex = 2; // HealthDocumentRecord
            }

            try
            {
                AppendLog("1. Registering Patient and Care Context in Wrapper DB...");

                string hiType = "Prescription";
                string recordType = cmbRecordType.Text;
                if (recordType == "OPConsultationRecord")
                {
                    hiType = "OPConsultation";
                }
                else if (recordType == "HealthDocumentRecord")
                {
                    hiType = "HealthDocumentRecord";
                }

                var careContexts = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["referenceNumber"] = txtCareContextRef.Text.Trim(),
                        ["display"] = txtCareContextDisplay.Text.Trim(),
                        ["hiType"] = hiType
                    }
                };

                // Step A: Register Patient inside wrapper DB
                var resp = await _client.AddPatientsToWrapperAsync(
                    txtAbhaAddress.Text.Trim(),
                    txtPatientName.Text.Trim(),
                    cmbGender.Text.Trim(),
                    txtDob.Text.Trim(),
                    txtPatientRef.Text.Trim(),
                    txtPatientMobile.Text.Trim(),
                    careContexts
                );

                if (!resp.Success)
                {
                    AppendLog($"[REGISTER ERROR] {resp.Message}");
                    MessageBox.Show("Patient registration failed: " + resp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                AppendLog("[REGISTER SUCCESS] Patient registered successfully in Wrapper DB.");

                // Step B: Build FHIR-mappable payload
                var medicinesList = new List<object>();
                foreach (ListViewItem item in lvMedicines.Items)
                {
                    medicinesList.Add(new
                    {
                        medicine = item.Text,
                        dosage = item.SubItems[1].Text
                    });
                }

                var documentsList = new List<object>();
                if (_pdfBytes != null)
                {
                    documentsList.Add(new
                    {
                        contentType = "application/pdf",
                        type = "Prescription",
                        data = Convert.ToBase64String(_pdfBytes)
                    });
                }

                var parchiJson = new
                {
                    bundleType = recordType,
                    careContextReference = txtCareContextRef.Text.Trim(),
                    authoredOn = DateTime.UtcNow.ToString("o"),
                    patient = new
                    {
                        name = txtPatientName.Text.Trim(),
                        patientReference = txtPatientRef.Text.Trim(),
                        gender = cmbGender.Text.Trim().ToLower(),
                        birthDate = txtDob.Text.Trim()
                    },
                    practitioners = new[] { new { name = "Dr. Midha", practitionerId = "DOC-01" } },
                    organisation = new { facilityName = "MIDHA HOSPITAL", facilityId = "IN0610090658" },
                    clinicalNotes = "Prescription Details",
                    prescriptions = medicinesList.ToArray(),
                    documents = documentsList.ToArray()
                };

                var recordData = new
                {
                    AbhaAddress = txtAbhaAddress.Text.Trim(),
                    CareContextReference = txtCareContextRef.Text.Trim(),
                    RecordType = recordType,
                    FhirJsonPayload = System.Text.Json.JsonSerializer.Serialize(parchiJson)
                };

                AppendLog("2. Pushing Health Data Record to Wrapper...");

                using (var client = new HttpClient())
                {
                    string wrapperBaseUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:BaseUrl"] ?? "https://sbx.wati.digital";
                    string apiUrl = $"{wrapperBaseUrl.TrimEnd('/')}/v3/patient/health-data"; 
                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(recordData), Encoding.UTF8, "application/json");
                    
                    var healthResp = await client.PostAsync(apiUrl, content);
                    if (healthResp.IsSuccessStatusCode)
                    {
                        AppendLog($"[DATA PUSH SUCCESS] Prescription details & attachments saved to Wrapper DB.");
                        return true;
                    }
                    else
                    {
                        var errorResponse = await healthResp.Content.ReadAsStringAsync();
                        AppendLog($"[DATA PUSH ERROR] Status: {healthResp.StatusCode}, Response: {errorResponse}");
                        MessageBox.Show("Failed to push Health Data: " + errorResponse, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[EXCEPTION] {ex.Message}");
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void btnInitiateLink_Click(object sender, EventArgs e)
        {
            await InitiateLinkingActionAsync();
        }

        private async Task<bool> InitiateLinkingActionAsync()
        {
            if (string.IsNullOrWhiteSpace(txtAbhaAddress.Text))
            {
                MessageBox.Show("Please enter ABHA Address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                AppendLog("3. Initiating Care Context Linking on ABDM Gateway...");

                string hiType = "Prescription";
                string recordType = cmbRecordType.Text;
                if (recordType == "OPConsultationRecord")
                {
                    hiType = "OPConsultation";
                }
                else if (recordType == "HealthDocumentRecord")
                {
                    hiType = "HealthDocumentRecord";
                }

                var careContexts = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["referenceNumber"] = txtCareContextRef.Text.Trim(),
                        ["display"] = txtCareContextDisplay.Text.Trim(),
                        ["hiType"] = hiType
                    }
                };

                var resp = await _client.LinkCareContextsAsync(
                    txtAbhaAddress.Text.Trim(),
                    _client.Settings.HipId ?? "IN0610090658",
                    careContexts
                );

                if (resp.Success)
                {
                    AppendLog($"[LINK INITIATED] Success. Data: {resp.Data}");
                    return true;
                }
                else
                {
                    AppendLog($"[LINK ERROR] {resp.Message}");
                    MessageBox.Show("Linking failed: " + resp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[EXCEPTION] {ex.Message}");
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void btnAutoFlow_Click(object sender, EventArgs e)
        {
            btnAutoFlow.Enabled = false;
            btnAutoFlow.Text = "Processing...";
            try
            {
                // Step 1: Save & Push
                bool pushSuccess = await SaveAndPushActionAsync();
                if (pushSuccess)
                {
                    // Step 2: Initiate Linking
                    bool linkSuccess = await InitiateLinkingActionAsync();
                    if (linkSuccess)
                    {
                        MessageBox.Show("All actions completed successfully!\n1. Patient registered in DB\n2. Health record saved\n3. Care context link request sent to ABDM.", "Flow Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        InitializeDefaultValues(); // Automatically regenerate reference IDs for next prescription
                    }
                }
            }
            finally
            {
                btnAutoFlow.Enabled = true;
                btnAutoFlow.Text = "Auto Link & Push (All in One)";
            }
        }

        private void AppendLog(string message)
        {
            txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        public static byte[] GeneratePrescriptionPdf(string patientName, string abhaAddress, string gender, string dob, string careContextRef, List<string[]> medicines)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms, Encoding.ASCII))
                {
                    writer.Write("%PDF-1.4\n");
                    var objectOffsets = new List<long>();
                    
                    // Object 1: Catalog
                    objectOffsets.Add(ms.Position);
                    writer.Write("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
                    
                    // Object 2: Pages
                    objectOffsets.Add(ms.Position);
                    writer.Write("2 0 obj\n<< /Type /Pages /Kids [ 3 0 R ] /Count 1 >>\nendobj\n");
                    
                    var sb = new StringBuilder();
                    sb.Append("BT\n/F1 14 Tf\n50 750 Td\n(MIDHA HOSPITAL - PRESCRIPTION REPORT) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 710 Td\n(Patient Name: {patientName}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 690 Td\n(ABHA Address: {abhaAddress}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 670 Td\n(Gender: {gender}  |  DOB: {dob}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 650 Td\n(Care Context Ref: {careContextRef}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 630 Td\n(Date: {DateTime.Now:dd-MMM-yyyy HH:mm}) Tj\nET\n");
                    
                    sb.Append("BT\n/F1 12 Tf\n50 590 Td\n(Prescribed Medicines:) Tj\nET\n");
                    
                    int y = 570;
                    foreach (var med in medicines)
                    {
                        sb.Append($"BT\n/F1 10 Tf\n50 {y} Td\n(- {med[0]}  -  Dosage: {med[1]}) Tj\nET\n");
                        y -= 20;
                    }
                    
                    sb.Append($"BT\n/F1 10 Tf\n50 {y - 40} Td\n(Signed: Dr. Midha) Tj\nET\n");
                    
                    string contentStream = sb.ToString();
                    byte[] contentBytes = Encoding.ASCII.GetBytes(contentStream);
                    
                    // Object 3: Page
                    objectOffsets.Add(ms.Position);
                    writer.Write("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [ 0 0 595 842 ] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>\nendobj\n");
                    
                    // Object 4: Content
                    objectOffsets.Add(ms.Position);
                    writer.Write($"4 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n");
                    writer.Flush();
                    ms.Write(contentBytes, 0, contentBytes.Length);
                    writer.Write("\nendstream\nendobj\n");
                    
                    // Object 5: Font
                    objectOffsets.Add(ms.Position);
                    writer.Write("5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n");
                    
                    // Xref
                    long xrefOffset = ms.Position;
                    writer.Write($"xref\n0 {objectOffsets.Count + 1}\n0000000000 65535 f \n");
                    foreach (var offset in objectOffsets)
                    {
                        writer.Write($"{offset:D10} 00000 n \n");
                    }
                    
                    // Trailer
                    writer.Write($"trailer\n<< /Size {objectOffsets.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n");
                    writer.Flush();
                }
                return ms.ToArray();
            }
        }
    }
}
