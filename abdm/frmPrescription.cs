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
        private byte[]? _pdfBytes = null;
        private string _pdfFileName = string.Empty;
        private ListView lvItems_Inv => ucInvoice.lvItems_Inv;
        private TextBox txtItemName_Inv => ucInvoice.txtItemName_Inv;
        private TextBox txtAmount_Inv => ucInvoice.txtAmount_Inv;
        private TextBox txtQuantity_Inv => ucInvoice.txtQuantity_Inv;
        private TextBox txtUnit_Inv => ucInvoice.txtUnit_Inv;
        private ListView lvMedicines_Imm => ucImmunization.lvMedicines_Imm;
        private TextBox txtVaccineName_Imm => ucImmunization.txtVaccineName_Imm;
        private TextBox txtLotNumber_Imm => ucImmunization.txtLotNumber_Imm;
        private TextBox txtDoseNumber_Imm => ucImmunization.txtDoseNumber_Imm;

        private bool _isSyncing = false;

        public frmPrescription()
        {
            InitializeComponent();
            ApplyModernTheme();
            _client = GetAbdmClient();
            cmbRecordType.SelectedIndexChanged += cmbRecordType_SelectedIndexChanged;
            InitializeDefaultValues();
        }

        private void ApplyModernTheme()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            pnlHeader.BackColor = Color.FromArgb(44, 62, 80);
            
            btnSaveAndPush.BackColor = Color.FromArgb(41, 128, 185);
            btnSaveAndPush.ForeColor = Color.White;
            btnSaveAndPush.FlatAppearance.BorderSize = 0;
            btnSaveAndPush.Cursor = Cursors.Hand;

            btnInitiateLink.BackColor = Color.FromArgb(39, 174, 96);
            btnInitiateLink.ForeColor = Color.White;
            btnInitiateLink.FlatAppearance.BorderSize = 0;
            btnInitiateLink.Cursor = Cursors.Hand;

            btnAutoFlow.BackColor = Color.FromArgb(142, 68, 173);
            btnAutoFlow.ForeColor = Color.White;
            btnAutoFlow.FlatAppearance.BorderSize = 0;
            btnAutoFlow.Cursor = Cursors.Hand;

            btnUploadPdf.BackColor = Color.FromArgb(230, 126, 34);
            btnUploadPdf.ForeColor = Color.White;
            btnUploadPdf.FlatAppearance.BorderSize = 0;
            btnUploadPdf.Cursor = Cursors.Hand;

            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.MouseOverBackColor = Color.Red;

            txtLogs.BackColor = Color.FromArgb(30, 30, 30);
            txtLogs.ForeColor = Color.LightGreen;
            txtLogs.Font = new Font("Consolas", 10F);
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

            // Reset medicines and PDF status/attachment variables
            if (lvMedicines_Presc != null) lvMedicines_Presc.Items.Clear();
            if (lvMedicines_OP != null) lvMedicines_OP.Items.Clear();
            if (lvMedicines_Diag != null) lvMedicines_Diag.Items.Clear();
            if (lvMedicines_Disch != null) lvMedicines_Disch.Items.Clear();
            if (lvMedicines_Imm != null) lvMedicines_Imm.Items.Clear();
            if (lvObservations_Well != null) lvObservations_Well.Items.Clear();
            if (lvItems_Inv != null) lvItems_Inv.Items.Clear();

            _pdfBytes = null;
            _pdfFileName = string.Empty;
            lblPdfStatus.Text = "No PDF attached";
            lblPdfStatus.ForeColor = Color.Red;

            // Reset textboxes
            if (txtMedicineName_Presc != null) txtMedicineName_Presc.Text = "";
            if (txtMedicineName_OP != null) txtMedicineName_OP.Text = "";
            if (txtTestName_Diag != null) txtTestName_Diag.Text = "";
            if (txtMedicineName_Disch != null) txtMedicineName_Disch.Text = "";
            if (txtVaccineName_Imm != null) txtVaccineName_Imm.Text = "";
            if (txtObservation_Well != null) txtObservation_Well.Text = "";
            if (txtItemName_Inv != null) txtItemName_Inv.Text = "";

            cmbRecordType.SelectedIndex = 0; // PrescriptionRecord
        }

        private ListView GetActiveListView()
        {
            string recordType = cmbRecordType.Text;
            if (recordType == "PrescriptionRecord") return lvMedicines_Presc;
            if (recordType == "OPConsultationRecord") return lvMedicines_OP;
            if (recordType == "DiagnosticReport") return lvMedicines_Diag;
            if (recordType == "DischargeSummary") return lvMedicines_Disch;
            if (recordType == "ImmunizationRecord") return lvMedicines_Imm;
            if (recordType == "WellnessRecord") return lvObservations_Well;
            if (recordType == "Invoice") return lvItems_Inv;
            return null;
        }

        private void tcRecordDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            try
            {
                if (tcRecordDetails.SelectedTab == tpPrescription)
                    cmbRecordType.Text = "PrescriptionRecord";
                else if (tcRecordDetails.SelectedTab == tpOPConsultation)
                    cmbRecordType.Text = "OPConsultationRecord";
                else if (tcRecordDetails.SelectedTab == tpDiagnosticReport)
                    cmbRecordType.Text = "DiagnosticReport";
                else if (tcRecordDetails.SelectedTab == tpDischargeSummary)
                    cmbRecordType.Text = "DischargeSummary";
                else if (tcRecordDetails.SelectedTab == tpHealthDocument)
                    cmbRecordType.Text = "HealthDocumentRecord";
                else if (tcRecordDetails.SelectedTab == tpImmunization)
                    cmbRecordType.Text = "ImmunizationRecord";
                else if (tcRecordDetails.SelectedTab == tpWellness)
                    cmbRecordType.Text = "WellnessRecord";
                else if (tcRecordDetails.SelectedTab == tpInvoice)
                    cmbRecordType.Text = "Invoice";

                UpdateUiForRecordType();
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private void btnAddMedicine_Well_Click(object sender, EventArgs e)
        {
            string name = txtObservation_Well.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var item = new ListViewItem(name);
            item.SubItems.Add(txtResult_Well.Text.Trim());
            item.SubItems.Add(txtUnit_Well.Text.Trim());
            lvObservations_Well.Items.Add(item);
            txtObservation_Well.Clear();
            txtObservation_Well.Focus();
        }

        private void btnRemoveMedicine_Well_Click(object sender, EventArgs e)
        {
            if (lvObservations_Well.SelectedItems.Count > 0)
                lvObservations_Well.Items.Remove(lvObservations_Well.SelectedItems[0]);
        }



        private void btnAddMedicine_Presc_Click(object sender, EventArgs e)
        {
            string name = txtMedicineName_Presc.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var item = new ListViewItem(name);
            item.SubItems.Add(txtDosage_Presc.Text.Trim());
            item.SubItems.Add(txtTiming_Presc.Text.Trim());
            item.SubItems.Add(txtRoute_Presc.Text.Trim());
            item.SubItems.Add(txtMethod_Presc.Text.Trim());
            item.SubItems.Add(txtInstructions_Presc.Text.Trim());
            item.SubItems.Add(txtReason_Presc.Text.Trim());
            lvMedicines_Presc.Items.Add(item);
            txtMedicineName_Presc.Clear();
            txtMedicineName_Presc.Focus();
        }

        private void btnRemoveMedicine_Presc_Click(object sender, EventArgs e)
        {
            if (lvMedicines_Presc.SelectedItems.Count > 0)
                lvMedicines_Presc.Items.Remove(lvMedicines_Presc.SelectedItems[0]);
        }

        private void btnAddMedicine_OP_Click(object sender, EventArgs e)
        {
            string name = txtMedicineName_OP.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var item = new ListViewItem(name);
            item.SubItems.Add(txtDosage_OP.Text.Trim());
            item.SubItems.Add(txtTiming_OP.Text.Trim());
            item.SubItems.Add(txtRoute_OP.Text.Trim());
            item.SubItems.Add(txtMethod_OP.Text.Trim());
            item.SubItems.Add(txtInstructions_OP.Text.Trim());
            item.SubItems.Add(txtReason_OP.Text.Trim());
            lvMedicines_OP.Items.Add(item);
            txtMedicineName_OP.Clear();
            txtMedicineName_OP.Focus();
        }

        private void btnRemoveMedicine_OP_Click(object sender, EventArgs e)
        {
            if (lvMedicines_OP.SelectedItems.Count > 0)
                lvMedicines_OP.Items.Remove(lvMedicines_OP.SelectedItems[0]);
        }

        private void btnAddMedicine_Diag_Click(object sender, EventArgs e)
        {
            string name = txtTestName_Diag.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var item = new ListViewItem(name);
            item.SubItems.Add(txtSpecimen_Diag.Text.Trim());
            item.SubItems.Add(txtResult_Diag.Text.Trim());
            item.SubItems.Add(txtUnit_Diag.Text.Trim());
            item.SubItems.Add(txtRefRange_Diag.Text.Trim());
            item.SubItems.Add(txtRemarks_Diag.Text.Trim());
            item.SubItems.Add(txtInterpretation_Diag.Text.Trim());
            lvMedicines_Diag.Items.Add(item);
            txtTestName_Diag.Clear();
            txtTestName_Diag.Focus();
        }

        private void btnRemoveMedicine_Diag_Click(object sender, EventArgs e)
        {
            if (lvMedicines_Diag.SelectedItems.Count > 0)
                lvMedicines_Diag.Items.Remove(lvMedicines_Diag.SelectedItems[0]);
        }

        private void btnAddMedicine_Disch_Click(object sender, EventArgs e)
        {
            string name = txtMedicineName_Disch.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var item = new ListViewItem(name);
            item.SubItems.Add(txtDosage_Disch.Text.Trim());
            item.SubItems.Add(txtAdmDate_Disch.Text.Trim());
            item.SubItems.Add(txtDischDate_Disch.Text.Trim());
            item.SubItems.Add(txtCourse_Disch.Text.Trim());
            item.SubItems.Add(txtAdvice_Disch.Text.Trim());
            item.SubItems.Add(txtCondition_Disch.Text.Trim());
            lvMedicines_Disch.Items.Add(item);
            txtMedicineName_Disch.Clear();
            txtMedicineName_Disch.Focus();
        }

        private void btnRemoveMedicine_Disch_Click(object sender, EventArgs e)
        {
            if (lvMedicines_Disch.SelectedItems.Count > 0)
                lvMedicines_Disch.Items.Remove(lvMedicines_Disch.SelectedItems[0]);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                        // cmbRecordType.SelectedIndex = 2; // HealthDocumentRecord
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

        private async void btnInitiateLink_Click(object sender, EventArgs e)
        {
            await InitiateLinkingActionAsync();
        }

        private List<string[]> GetRecordItems(string recordType)
        {
            var items = new List<string[]>();
            ListView lv = null;
            if (recordType == "PrescriptionRecord") lv = lvMedicines_Presc;
            else if (recordType == "OPConsultationRecord") lv = lvMedicines_OP;
            else if (recordType == "DiagnosticReport") lv = lvMedicines_Diag;
            else if (recordType == "DischargeSummary") lv = lvMedicines_Disch;
            else if (recordType == "ImmunizationRecord") lv = lvMedicines_Imm;
            else if (recordType == "WellnessRecord") lv = lvObservations_Well;
            else if (recordType == "Invoice") lv = lvItems_Inv;

            if (lv != null && lv.Items.Count > 0)
            {
                foreach (ListViewItem item in lv.Items)
                {
                    items.Add(new string[] {
                        item.Text,
                        item.SubItems.Count > 1 ? item.SubItems[1].Text : "",
                        item.SubItems.Count > 2 ? item.SubItems[2].Text : "",
                        item.SubItems.Count > 3 ? item.SubItems[3].Text : "",
                        item.SubItems.Count > 4 ? item.SubItems[4].Text : "",
                        item.SubItems.Count > 5 ? item.SubItems[5].Text : "",
                        item.SubItems.Count > 6 ? item.SubItems[6].Text : ""
                    });
                }
            }
            else
            {
                // Fallback to text box values or hardcoded defaults so we always have sample data
                if (recordType == "PrescriptionRecord")
                {
                    items.Add(new string[] {
                        string.IsNullOrWhiteSpace(txtMedicineName_Presc.Text) ? "Paracetamol" : txtMedicineName_Presc.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtDosage_Presc.Text) ? "1-0-1" : txtDosage_Presc.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtTiming_Presc.Text) ? "1-1-D" : txtTiming_Presc.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtRoute_Presc.Text) ? "Oral" : txtRoute_Presc.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtMethod_Presc.Text) ? "swallow" : txtMethod_Presc.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtInstructions_Presc.Text) ? "after food" : txtInstructions_Presc.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtReason_Presc.Text) ? "Fever" : txtReason_Presc.Text.Trim()
                    });
                }
                else if (recordType == "OPConsultationRecord")
                {
                    items.Add(new string[] {
                        string.IsNullOrWhiteSpace(txtMedicineName_OP.Text) ? "Amoxicillin" : txtMedicineName_OP.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtDosage_OP.Text) ? "1-0-1" : txtDosage_OP.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtTiming_OP.Text) ? "1-1-D" : txtTiming_OP.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtRoute_OP.Text) ? "Oral" : txtRoute_OP.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtMethod_OP.Text) ? "swallow" : txtMethod_OP.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtInstructions_OP.Text) ? "after food" : txtInstructions_OP.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtReason_OP.Text) ? "Fever" : txtReason_OP.Text.Trim()
                    });
                }
                else if (recordType == "DiagnosticReport")
                {
                    items.Add(new string[] {
                        string.IsNullOrWhiteSpace(txtTestName_Diag.Text) ? "Hemoglobin" : txtTestName_Diag.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtSpecimen_Diag.Text) ? "Blood" : txtSpecimen_Diag.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtResult_Diag.Text) ? "14.5" : txtResult_Diag.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtUnit_Diag.Text) ? "mg/dL" : txtUnit_Diag.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtRefRange_Diag.Text) ? "13-17" : txtRefRange_Diag.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtRemarks_Diag.Text) ? "Normal" : txtRemarks_Diag.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtInterpretation_Diag.Text) ? "Clinically stable" : txtInterpretation_Diag.Text.Trim()
                    });
                }
                else if (recordType == "DischargeSummary")
                {
                    items.Add(new string[] {
                        string.IsNullOrWhiteSpace(txtMedicineName_Disch.Text) ? "Pantoprazole" : txtMedicineName_Disch.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtDosage_Disch.Text) ? "1-0-1" : txtDosage_Disch.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtAdmDate_Disch.Text) ? DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd") : txtAdmDate_Disch.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtDischDate_Disch.Text) ? DateTime.Now.ToString("yyyy-MM-dd") : txtDischDate_Disch.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtCourse_Disch.Text) ? "IV Fluids & Antibiotics" : txtCourse_Disch.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtAdvice_Disch.Text) ? "Take rest" : txtAdvice_Disch.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtCondition_Disch.Text) ? "Stable" : txtCondition_Disch.Text.Trim()
                    });
                }
                else if (recordType == "ImmunizationRecord")
                {
                    items.Add(new string[] {
                        string.IsNullOrWhiteSpace(txtVaccineName_Imm.Text) ? "Covishield" : txtVaccineName_Imm.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtLotNumber_Imm.Text) ? "LOT-1234" : txtLotNumber_Imm.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtDoseNumber_Imm.Text) ? "1" : txtDoseNumber_Imm.Text.Trim()
                    });
                }
                else if (recordType == "WellnessRecord")
                {
                    items.Add(new string[] {
                        string.IsNullOrWhiteSpace(txtObservation_Well.Text) ? "Heart rate" : txtObservation_Well.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtResult_Well.Text) ? "72" : txtResult_Well.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtUnit_Well.Text) ? "beats/minute" : txtUnit_Well.Text.Trim()
                    });
                }
                else if (recordType == "Invoice")
                {
                    items.Add(new string[] {
                        string.IsNullOrWhiteSpace(txtItemName_Inv.Text) ? "Consultation & Clinical Services" : txtItemName_Inv.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtAmount_Inv.Text) ? "500" : txtAmount_Inv.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtQuantity_Inv.Text) ? "1" : txtQuantity_Inv.Text.Trim(),
                        string.IsNullOrWhiteSpace(txtUnit_Inv.Text) ? "unit" : txtUnit_Inv.Text.Trim()
                    });
                }
            }
            return items;
        }

        private async Task<bool> SaveAndPushActionAsync()
        {
            if (string.IsNullOrWhiteSpace(txtAbhaAddress.Text) || string.IsNullOrWhiteSpace(txtPatientName.Text))
            {
                MessageBox.Show("Please enter ABHA Address and Patient Name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                var recordTypes = new[] {
                    new { Type = "PrescriptionRecord", HiType = "Prescription", Suffix = "-Presc", Display = "Prescription" },
                    new { Type = "OPConsultationRecord", HiType = "OPConsultation", Suffix = "-OP", Display = "OP Consultation" },
                    new { Type = "DiagnosticReport", HiType = "DiagnosticReport", Suffix = "-Diag", Display = "Diagnostic Report" },
                    new { Type = "DischargeSummary", HiType = "DischargeSummary", Suffix = "-Disch", Display = "Discharge Summary" },
                    new { Type = "HealthDocumentRecord", HiType = "HealthDocumentRecord", Suffix = "-Doc", Display = "Health Document" },
                    new { Type = "ImmunizationRecord", HiType = "ImmunizationRecord", Suffix = "-Imm", Display = "Immunization Record" },
                    new { Type = "WellnessRecord", HiType = "WellnessRecord", Suffix = "-Well", Display = "Wellness Record" },
                    new { Type = "Invoice", HiType = "Invoice", Suffix = "-Inv", Display = "Invoice" }
                };

                AppendLog("1. Registering Patient and Care Context in Wrapper DB...");

                var careContexts = new List<Dictionary<string, object>>();
                string baseRef = txtCareContextRef.Text.Trim();
                string baseDisplay = txtCareContextDisplay.Text.Trim();

                if (chkAllTypes.Checked)
                {
                    foreach (var rt in recordTypes)
                    {
                        careContexts.Add(new Dictionary<string, object>
                        {
                            ["referenceNumber"] = $"{baseRef}{rt.Suffix}",
                            ["display"] = $"{baseDisplay} {rt.Display}",
                            ["hiType"] = rt.HiType
                        });
                    }
                }
                else
                {
                    string recordType = cmbRecordType.Text;
                    string hiType = "Prescription";
                    if (recordType == "OPConsultationRecord") hiType = "OPConsultation";
                    else if (recordType == "HealthDocumentRecord") hiType = "HealthDocumentRecord";
                    else if (recordType == "DiagnosticReport") hiType = "DiagnosticReport";
                    else if (recordType == "DischargeSummary") hiType = "DischargeSummary";
                    else if (recordType == "ImmunizationRecord") hiType = "ImmunizationRecord";
                    else if (recordType == "WellnessRecord") hiType = "WellnessRecord";
                    else if (recordType == "Invoice") hiType = "Invoice";

                    careContexts.Add(new Dictionary<string, object>
                    {
                        ["referenceNumber"] = baseRef,
                        ["display"] = baseDisplay,
                        ["hiType"] = hiType
                    });
                }

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

                using (var client = new HttpClient())
                {
                    string wrapperBaseUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:BaseUrl"] ?? "https://sbx.wati.digital";
                    string apiUrl = $"{wrapperBaseUrl.TrimEnd('/')}/v3/patient/health-data";

                    var typesToPush = chkAllTypes.Checked 
                        ? recordTypes.Select(x => new { x.Type, x.HiType, Suffix = x.Suffix, Display = x.Display }).ToArray()
                        : new[] { new { Type = cmbRecordType.Text, HiType = careContexts[0]["hiType"].ToString(), Suffix = "", Display = cmbRecordType.Text } };

                    foreach (var rt in typesToPush)
                    {
                        string refNo = chkAllTypes.Checked ? $"{baseRef}{rt.Suffix}" : baseRef;
                        var items = GetRecordItems(rt.Type);

                        // Auto-generate PDF clinical report if not manually attached
                        byte[] pdfBytes = null;
                        if (rt.Type == "HealthDocumentRecord" && _pdfBytes != null)
                        {
                            pdfBytes = _pdfBytes;
                        }
                        else
                        {
                            pdfBytes = GenerateClinicalDocumentPdf(
                                rt.Type,
                                txtPatientName.Text.Trim(),
                                txtAbhaAddress.Text.Trim(),
                                cmbGender.Text.Trim(),
                                txtDob.Text.Trim(),
                                refNo,
                                items,
                                "Dr. Sudeep Munjal"
                            );
                        }

                        var medicinesList = new List<object>();
                        var immunizationsList = new List<object>();
                        var vitalSignsList = new List<object>();
                        var lineItemsList = new List<object>();

                        if (rt.Type == "PrescriptionRecord" || rt.Type == "OPConsultationRecord" || rt.Type == "DischargeSummary")
                        {
                            foreach (var item in items)
                            {
                                medicinesList.Add(new
                                {
                                    medicine = item[0],
                                    dosage = item.Length > 1 ? item[1] : "",
                                    timing = item.Length > 2 ? item[2] : "",
                                    route = item.Length > 3 ? item[3] : "",
                                    method = item.Length > 4 ? item[4] : "",
                                    additionalInstructions = item.Length > 5 ? item[5] : "",
                                    reason = item.Length > 6 ? item[6] : ""
                                });
                            }
                        }
                        else if (rt.Type == "DiagnosticReport")
                        {
                            foreach (var item in items)
                            {
                                medicinesList.Add(new
                                {
                                    name = item[0],
                                    specimen = item.Length > 1 ? item[1] : "",
                                    result = item.Length > 2 ? item[2] : "",
                                    unit = item.Length > 3 ? item[3] : "",
                                    referenceRange = item.Length > 4 ? item[4] : "",
                                    remarks = item.Length > 5 ? item[5] : "",
                                    interpretation = item.Length > 6 ? item[6] : ""
                                });
                            }
                        }
                        else if (rt.Type == "ImmunizationRecord")
                        {
                            foreach (var item in items)
                            {
                                int doseVal = 1;
                                int.TryParse(item.Length > 2 ? item[2] : "1", out doseVal);
                                immunizationsList.Add(new
                                {
                                    vaccineName = item[0],
                                    lotNumber = item.Length > 1 ? item[1] : "LOT123",
                                    doseNumber = doseVal,
                                    date = DateTime.UtcNow.ToString("o")
                                });
                            }
                        }
                        else if (rt.Type == "WellnessRecord")
                        {
                            foreach (var item in items)
                            {
                                vitalSignsList.Add(new
                                {
                                    codeText = item[0],
                                    value = item.Length > 1 ? item[1] : "",
                                    unit = item.Length > 2 ? item[2] : ""
                                });
                            }
                        }
                        else if (rt.Type == "Invoice")
                        {
                            foreach (var item in items)
                            {
                                lineItemsList.Add(new
                                {
                                    itemName = item[0],
                                    price = item.Length > 1 ? item[1] : "",
                                    quantity = item.Length > 2 ? item[2] : "1",
                                    unit = item.Length > 3 ? item[3] : "unit"
                                });
                            }
                        }

                        var documentsList = new List<object>();
                        if (pdfBytes != null)
                        {
                            documentsList.Add(new
                            {
                                contentType = "application/pdf",
                                type = rt.Type,
                                data = Convert.ToBase64String(pdfBytes)
                            });
                        }

                        var parchiJson = new
                        {
                            bundleType = rt.Type,
                            careContextReference = refNo,
                            authoredOn = DateTime.UtcNow.ToString("o"),
                            patient = new
                            {
                                name = txtPatientName.Text.Trim(),
                                patientReference = txtPatientRef.Text.Trim(),
                                gender = cmbGender.Text.Trim().ToLower(),
                                birthDate = txtDob.Text.Trim()
                            },
                            practitioners = new[] { new { name = "Dr. Sudeep Munjal", practitionerId = "DOC-01" } },
                            organisation = new { facilityName = "MIDHA HOSPITAL", facilityId = "IN0610090658" },
                            clinicalNotes = $"{rt.Display} Details",
                            prescriptions = medicinesList.ToArray(),
                            immunizations = immunizationsList.ToArray(),
                            vitalSigns = vitalSignsList.ToArray(),
                            lineItems = lineItemsList.ToArray(),
                            documents = documentsList.ToArray()
                        };

                        var recordData = new
                        {
                            AbhaAddress = txtAbhaAddress.Text.Trim(),
                            CareContextReference = refNo,
                            RecordType = rt.Type,
                            FhirJsonPayload = System.Text.Json.JsonSerializer.Serialize(parchiJson)
                        };

                        AppendLog($"2. Pushing Health Data Record ({rt.Display}) to Wrapper...");
                        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(recordData), Encoding.UTF8, "application/json");

                        var healthResp = await client.PostAsync(apiUrl, content);
                        if (!healthResp.IsSuccessStatusCode)
                        {
                            var errorResponse = await healthResp.Content.ReadAsStringAsync();
                            AppendLog($"[DATA PUSH ERROR] {rt.Display} Status: {healthResp.StatusCode}, Response: {errorResponse}");
                            MessageBox.Show($"Failed to push Health Data ({rt.Display}): " + errorResponse, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        AppendLog($"[DATA PUSH SUCCESS] {rt.Display} details & attachments saved to Wrapper DB.");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                AppendLog($"[EXCEPTION] {ex.Message}");
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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

                var careContexts = new List<Dictionary<string, object>>();
                string baseRef = txtCareContextRef.Text.Trim();
                string baseDisplay = txtCareContextDisplay.Text.Trim();

                if (chkAllTypes.Checked)
                {
                    var recordTypes = new[] {
                        new { HiType = "Prescription", Suffix = "-Presc", Display = "Prescription" },
                        new { HiType = "OPConsultation", Suffix = "-OP", Display = "OP Consultation" },
                        new { HiType = "DiagnosticReport", Suffix = "-Diag", Display = "Diagnostic Report" },
                        new { HiType = "DischargeSummary", Suffix = "-Disch", Display = "Discharge Summary" },
                        new { HiType = "HealthDocumentRecord", Suffix = "-Doc", Display = "Health Document" },
                        new { HiType = "ImmunizationRecord", Suffix = "-Imm", Display = "Immunization Record" },
                        new { HiType = "WellnessRecord", Suffix = "-Well", Display = "Wellness Record" },
                        new { HiType = "Invoice", Suffix = "-Inv", Display = "Invoice" }
                    };

                    foreach (var rt in recordTypes)
                    {
                        careContexts.Add(new Dictionary<string, object>
                        {
                            ["referenceNumber"] = $"{baseRef}{rt.Suffix}",
                            ["display"] = $"{baseDisplay} {rt.Display}",
                            ["hiType"] = rt.HiType
                        });
                    }
                }
                else
                {
                    string recordType = cmbRecordType.Text;
                    string hiType = "Prescription";
                    if (recordType == "OPConsultationRecord") hiType = "OPConsultation";
                    else if (recordType == "HealthDocumentRecord") hiType = "HealthDocumentRecord";
                    else if (recordType == "DiagnosticReport") hiType = "DiagnosticReport";
                    else if (recordType == "DischargeSummary") hiType = "DischargeSummary";
                    else if (recordType == "ImmunizationRecord") hiType = "ImmunizationRecord";
                    else if (recordType == "WellnessRecord") hiType = "WellnessRecord";
                    else if (recordType == "Invoice") hiType = "Invoice";

                    careContexts.Add(new Dictionary<string, object>
                    {
                        ["referenceNumber"] = baseRef,
                        ["display"] = baseDisplay,
                        ["hiType"] = hiType
                    });
                }

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

        private void cmbRecordType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcRecordDetails == null) return;
            if (_isSyncing) return;
            _isSyncing = true;
            try
            {
                string recordType = cmbRecordType.Text;
                if (recordType == "PrescriptionRecord")
                    tcRecordDetails.SelectedTab = tpPrescription;
                else if (recordType == "OPConsultationRecord")
                    tcRecordDetails.SelectedTab = tpOPConsultation;
                else if (recordType == "DiagnosticReport")
                    tcRecordDetails.SelectedTab = tpDiagnosticReport;
                else if (recordType == "DischargeSummary")
                    tcRecordDetails.SelectedTab = tpDischargeSummary;
                else if (recordType == "HealthDocumentRecord")
                    tcRecordDetails.SelectedTab = tpHealthDocument;
                else if (recordType == "ImmunizationRecord")
                    tcRecordDetails.SelectedTab = tpImmunization;
                else if (recordType == "WellnessRecord")
                    tcRecordDetails.SelectedTab = tpWellness;
                else if (recordType == "Invoice")
                    tcRecordDetails.SelectedTab = tpInvoice;

                UpdateUiForRecordType();
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private void UpdateUiForRecordType()
        {
            string recordType = cmbRecordType.Text;

            // Reset visibility of gbPrescribe by default
            gbPrescribe.Visible = true;

            // Restore original layout of the PDF and Logs groups
            gbPdf.Location = new Point(460, 360);
            txtLogs.Location = new Point(460, 500);
            txtLogs.Height = 180;

            if (recordType == "HealthDocumentRecord")
            {
                gbPrescribe.Visible = false;
                gbPdf.Location = new Point(460, 65);
                txtLogs.Location = new Point(460, 180);
                txtLogs.Height = 500;
            }
        }

        public static byte[] GenerateClinicalDocumentPdf(string recordType, string patientName, string abhaAddress, string gender, string dob, string careContextRef, List<string[]> items, string doctorName)
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

                    // Title based on Record Type
                    string title = "MIDHA HOSPITAL - CLINICAL RECORD";
                    if (recordType == "PrescriptionRecord") title = "MIDHA HOSPITAL - PRESCRIPTION REPORT";
                    else if (recordType == "OPConsultationRecord") title = "MIDHA HOSPITAL - OP CONSULTATION REPORT";
                    else if (recordType == "DiagnosticReport") title = "MIDHA HOSPITAL - DIAGNOSTIC REPORT";
                    else if (recordType == "DischargeSummary") title = "MIDHA HOSPITAL - DISCHARGE SUMMARY";
                    else if (recordType == "ImmunizationRecord") title = "MIDHA HOSPITAL - IMMUNIZATION RECORD";
                    else if (recordType == "WellnessRecord") title = "MIDHA HOSPITAL - WELLNESS RECORD";
                    else if (recordType == "Invoice") title = "MIDHA HOSPITAL - INVOICE";

                    sb.Append($"BT\n/F1 14 Tf\n50 750 Td\n({title}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 710 Td\n(Patient Name: {patientName}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 690 Td\n(ABHA Address: {abhaAddress}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 670 Td\n(Gender: {gender}  |  DOB: {dob}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 650 Td\n(Care Context Ref: {careContextRef}) Tj\nET\n");
                    sb.Append($"BT\n/F1 10 Tf\n50 630 Td\n(Date: {DateTime.Now:dd-MMM-yyyy HH:mm}) Tj\nET\n");

                    int y = 590;
                    if (recordType == "DiagnosticReport")
                    {
                        sb.Append("BT\n/F1 12 Tf\n50 590 Td\n(Test Results & Observations:) Tj\nET\n");
                        y = 560;
                        foreach (var item in items)
                        {
                            string testName = item[0];
                            string specimen = item.Length > 1 ? item[1] : "";
                            string result = item.Length > 2 ? item[2] : "";
                            string unit = item.Length > 3 ? item[3] : "";
                            string range = item.Length > 4 ? item[4] : "";
                            string remarks = item.Length > 5 ? item[5] : "";

                            sb.Append($"BT\n/F1 9 Tf\n50 {y} Td\n(- {testName} ({specimen}): {result} {unit}  [Ref: {range}]  {remarks}) Tj\nET\n");
                            y -= 20;
                        }
                    }
                    else if (recordType == "DischargeSummary")
                    {
                        if (items.Count > 0)
                        {
                            var first = items[0];
                            string adm = first.Length > 2 ? first[2] : "";
                            string dis = first.Length > 3 ? first[3] : "";
                            string course = first.Length > 4 ? first[4] : "";
                            string advice = first.Length > 5 ? first[5] : "";
                            string condition = first.Length > 6 ? first[6] : "";

                            sb.Append($"BT\n/F1 10 Tf\n50 590 Td\n(Admission Date: {adm}   |   Discharge Date: {dis}) Tj\nET\n");
                            sb.Append($"BT\n/F1 10 Tf\n50 570 Td\n(Treatment Course: {course}) Tj\nET\n");
                            sb.Append($"BT\n/F1 10 Tf\n50 550 Td\n(Discharge Advice: {advice}) Tj\nET\n");
                            sb.Append($"BT\n/F1 10 Tf\n50 530 Td\n(Condition on Discharge: {condition}) Tj\nET\n");
                            y = 500;
                        }

                        sb.Append($"BT\n/F1 12 Tf\n50 {y} Td\n(Discharge Medications Prescribed:) Tj\nET\n");
                        y -= 25;
                        foreach (var item in items)
                        {
                            string medName = item[0];
                            string dosage = item.Length > 1 ? item[1] : "";
                            sb.Append($"BT\n/F1 10 Tf\n50 {y} Td\n(- {medName}  -  Dosage: {dosage}) Tj\nET\n");
                            y -= 20;
                        }
                    }
                    else if (recordType == "OPConsultationRecord")
                    {
                        sb.Append("BT\n/F1 12 Tf\n50 590 Td\n(Clinical Consultation & Prescriptions:) Tj\nET\n");
                        y = 560;
                        foreach (var item in items)
                        {
                            string medName = item[0];
                            string dosage = item.Length > 1 ? item[1] : "";
                            string timing = item.Length > 2 ? item[2] : "";
                            string complaints = item.Length > 6 ? item[6] : "";
                            sb.Append($"BT\n/F1 10 Tf\n50 {y} Td\n(- {medName} [Dosage: {dosage}, Timing: {timing}]  Complaints: {complaints}) Tj\nET\n");
                            y -= 20;
                        }
                    }
                    else if (recordType == "ImmunizationRecord")
                    {
                        sb.Append("BT\n/F1 12 Tf\n50 590 Td\n(Immunization History:) Tj\nET\n");
                        y = 560;
                        foreach (var item in items)
                        {
                            string vaccineName = item[0];
                            string lotNo = item.Length > 1 ? item[1] : "";
                            string doseNo = item.Length > 2 ? item[2] : "";
                            sb.Append($"BT\n/F1 10 Tf\n50 {y} Td\n(- {vaccineName}  [Lot: {lotNo}, Dose: {doseNo}]) Tj\nET\n");
                            y -= 20;
                        }
                    }
                    else if (recordType == "WellnessRecord")
                    {
                        sb.Append("BT\n/F1 12 Tf\n50 590 Td\n(Vitals & Wellness Readings:) Tj\nET\n");
                        y = 560;
                        foreach (var item in items)
                        {
                            string obsName = item[0];
                            string val = item.Length > 1 ? item[1] : "";
                            string unit = item.Length > 2 ? item[2] : "";
                            sb.Append($"BT\n/F1 10 Tf\n50 {y} Td\n(- {obsName}: {val} {unit}) Tj\nET\n");
                            y -= 20;
                        }
                    }
                    else if (recordType == "Invoice")
                    {
                        sb.Append("BT\n/F1 12 Tf\n50 590 Td\n(Invoice Line Items:) Tj\nET\n");
                        y = 560;
                        foreach (var item in items)
                        {
                            string itemName = item[0];
                            string price = item.Length > 1 ? item[1] : "";
                            sb.Append($"BT\n/F1 10 Tf\n50 {y} Td\n(- {itemName}: Rs. {price}) Tj\nET\n");
                            y -= 20;
                        }
                    }
                    else // PrescriptionRecord
                    {
                        sb.Append("BT\n/F1 12 Tf\n50 590 Td\n(Prescribed Medicines:) Tj\nET\n");
                        y = 560;
                        foreach (var item in items)
                        {
                            string medName = item[0];
                            string dosage = item.Length > 1 ? item[1] : "";
                            string timing = item.Length > 2 ? item[2] : "";
                            string route = item.Length > 3 ? item[3] : "";
                            sb.Append($"BT\n/F1 10 Tf\n50 {y} Td\n(- {medName}  -  Dosage: {dosage} | Timing: {timing} | Route: {route}) Tj\nET\n");
                            y -= 20;
                        }
                    }

                    sb.Append($"BT\n/F1 10 Tf\n50 {y - 40} Td\n(Signed: {doctorName}) Tj\nET\n");

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
