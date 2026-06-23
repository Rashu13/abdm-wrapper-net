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

        private TabControl tcRecordDetails;
        private TabPage tpPrescription;
        private TabPage tpOPConsultation;
        private TabPage tpDiagnosticReport;
        private TabPage tpDischargeSummary;
        private TabPage tpHealthDocument;

        // Prescription Controls
        private TextBox txtMedicineName_Presc;
        private TextBox txtDosage_Presc;
        private TextBox txtTiming_Presc;
        private TextBox txtRoute_Presc;
        private TextBox txtMethod_Presc;
        private TextBox txtInstructions_Presc;
        private TextBox txtReason_Presc;
        private ListView lvMedicines_Presc;

        // OP Consultation Controls
        private TextBox txtMedicineName_OP;
        private TextBox txtDosage_OP;
        private TextBox txtTiming_OP;
        private TextBox txtRoute_OP;
        private TextBox txtMethod_OP;
        private TextBox txtInstructions_OP;
        private TextBox txtReason_OP;
        private ListView lvMedicines_OP;

        // Diagnostic Report Controls
        private TextBox txtTestName_Diag;
        private TextBox txtSpecimen_Diag;
        private TextBox txtResult_Diag;
        private TextBox txtUnit_Diag;
        private TextBox txtRefRange_Diag;
        private TextBox txtRemarks_Diag;
        private TextBox txtInterpretation_Diag;
        private ListView lvMedicines_Diag;

        // Discharge Summary Controls
        private TextBox txtMedicineName_Disch;
        private TextBox txtDosage_Disch;
        private TextBox txtAdmDate_Disch;
        private TextBox txtDischDate_Disch;
        private TextBox txtCourse_Disch;
        private TextBox txtAdvice_Disch;
        private TextBox txtCondition_Disch;
        private ListView lvMedicines_Disch;

        private bool _isSyncing = false;

        public frmPrescription()
        {
            InitializeComponent();
            SetupExtraMedicationFields();
            _client = GetAbdmClient();
            cmbRecordType.SelectedIndexChanged += cmbRecordType_SelectedIndexChanged;
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

            // Reset medicines and PDF status/attachment variables
            if (lvMedicines_Presc != null) lvMedicines_Presc.Items.Clear();
            if (lvMedicines_OP != null) lvMedicines_OP.Items.Clear();
            if (lvMedicines_Diag != null) lvMedicines_Diag.Items.Clear();
            if (lvMedicines_Disch != null) lvMedicines_Disch.Items.Clear();

            _pdfBytes = null;
            _pdfFileName = string.Empty;
            lblPdfStatus.Text = "No PDF attached";
            lblPdfStatus.ForeColor = Color.Red;

            // Reset textboxes
            if (txtMedicineName_Presc != null) txtMedicineName_Presc.Text = "";
            if (txtMedicineName_OP != null) txtMedicineName_OP.Text = "";
            if (txtTestName_Diag != null) txtTestName_Diag.Text = "";
            if (txtMedicineName_Disch != null) txtMedicineName_Disch.Text = "";

            cmbRecordType.SelectedIndex = 0; // PrescriptionRecord
        }

        private TextBox CreateInputRow(TabPage tp, string labelText, int xLabel, int yLabel, int xText, int yText, int wText, string defaultText = "")
        {
            var lbl = new Label { Text = labelText, Location = new Point(xLabel, yLabel), Size = new Size(xText - xLabel - 5, 21), Font = gbPrescribe.Font };
            var txt = new TextBox { Location = new Point(xText, yText), Size = new Size(wText, 25), Text = defaultText, Font = gbPrescribe.Font };
            tp.Controls.Add(lbl);
            tp.Controls.Add(txt);
            return txt;
        }

        private void CreateButtons(TabPage tp, EventHandler onAdd, EventHandler onRemove)
        {
            var btnAdd = new Button
            {
                Text = "Add Item",
                Location = new Point(580, 5),
                Size = new Size(110, 28),
                BackColor = Color.FromArgb(55, 115, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = gbPrescribe.Font
            };
            btnAdd.Click += onAdd;

            var btnRemove = new Button
            {
                Text = "Remove Selected",
                Location = new Point(580, 35),
                Size = new Size(110, 28),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = gbPrescribe.Font
            };
            btnRemove.Click += onRemove;

            tp.Controls.Add(btnAdd);
            tp.Controls.Add(btnRemove);
        }

        private ListView CreateListView(TabPage tp, string[] columns, int[] widths)
        {
            var lv = new ListView
            {
                Location = new Point(10, 125),
                Size = new Size(680, 95),
                FullRowSelect = true,
                GridLines = true,
                View = View.Details,
                HideSelection = false,
                Font = gbPrescribe.Font
            };
            for (int i = 0; i < columns.Length; i++)
            {
                lv.Columns.Add(columns[i], widths[i]);
            }
            tp.Controls.Add(lv);
            return lv;
        }

        private ListView GetActiveListView()
        {
            string recordType = cmbRecordType.Text;
            if (recordType == "PrescriptionRecord") return lvMedicines_Presc;
            if (recordType == "OPConsultationRecord") return lvMedicines_OP;
            if (recordType == "DiagnosticReport") return lvMedicines_Diag;
            if (recordType == "DischargeSummary") return lvMedicines_Disch;
            return null;
        }

        private void SetupExtraMedicationFields()
        {
            // Remove the designer created medication controls from gbPrescribe
            gbPrescribe.Controls.Clear();

            // Create TabControl
            tcRecordDetails = new TabControl
            {
                Location = new Point(10, 20),
                Size = new Size(705, 260),
                Font = gbPrescribe.Font
            };
            tcRecordDetails.SelectedIndexChanged += tcRecordDetails_SelectedIndexChanged;
            gbPrescribe.Controls.Add(tcRecordDetails);

            // Tab 1: Prescription
            tpPrescription = new TabPage("Prescription");
            tcRecordDetails.TabPages.Add(tpPrescription);

            txtMedicineName_Presc = CreateInputRow(tpPrescription, "Medicine Name", 10, 8, 115, 5, 200);
            txtDosage_Presc = CreateInputRow(tpPrescription, "Dosage", 330, 8, 415, 5, 150, "1-0-1");
            txtTiming_Presc = CreateInputRow(tpPrescription, "Timing", 10, 38, 115, 35, 200, "1-1-D");
            txtRoute_Presc = CreateInputRow(tpPrescription, "Route", 330, 38, 415, 35, 150, "Oral");
            txtMethod_Presc = CreateInputRow(tpPrescription, "Method", 10, 68, 115, 65, 200, "swallow");
            txtInstructions_Presc = CreateInputRow(tpPrescription, "Instructions", 330, 68, 415, 65, 150, "after food");
            txtReason_Presc = CreateInputRow(tpPrescription, "Reason", 10, 98, 115, 95, 450, "Fever");

            CreateButtons(tpPrescription, btnAddMedicine_Presc_Click, btnRemoveMedicine_Presc_Click);
            lvMedicines_Presc = CreateListView(tpPrescription,
                new[] { "Medicine", "Dosage", "Timing", "Route", "Method", "Instructions", "Reason" },
                new[] { 170, 70, 80, 70, 70, 110, 110 });

            // Tab 2: OP Consultation
            tpOPConsultation = new TabPage("OP Consultation");
            tcRecordDetails.TabPages.Add(tpOPConsultation);

            txtMedicineName_OP = CreateInputRow(tpOPConsultation, "Prescribed Med", 10, 8, 115, 5, 200);
            txtDosage_OP = CreateInputRow(tpOPConsultation, "Dosage", 330, 8, 415, 5, 150, "1-0-1");
            txtTiming_OP = CreateInputRow(tpOPConsultation, "Timing", 10, 38, 115, 35, 200, "1-1-D");
            txtRoute_OP = CreateInputRow(tpOPConsultation, "Route", 330, 38, 415, 35, 150, "Oral");
            txtMethod_OP = CreateInputRow(tpOPConsultation, "Method", 10, 68, 115, 65, 200, "swallow");
            txtInstructions_OP = CreateInputRow(tpOPConsultation, "Instructions", 330, 68, 415, 65, 150, "after food");
            txtReason_OP = CreateInputRow(tpOPConsultation, "Chief Complaints", 10, 98, 115, 95, 450, "Fever");

            CreateButtons(tpOPConsultation, btnAddMedicine_OP_Click, btnRemoveMedicine_OP_Click);
            lvMedicines_OP = CreateListView(tpOPConsultation,
                new[] { "Medicine", "Dosage", "Timing", "Route", "Method", "Instructions", "Complaints" },
                new[] { 170, 70, 80, 70, 70, 110, 110 });

            // Tab 3: Diagnostic Report
            tpDiagnosticReport = new TabPage("Diagnostic Report");
            tcRecordDetails.TabPages.Add(tpDiagnosticReport);

            txtTestName_Diag = CreateInputRow(tpDiagnosticReport, "Test Name", 10, 8, 115, 5, 200);
            txtSpecimen_Diag = CreateInputRow(tpDiagnosticReport, "Specimen", 330, 8, 415, 5, 150, "Blood");
            txtResult_Diag = CreateInputRow(tpDiagnosticReport, "Result Value", 10, 38, 115, 35, 200);
            txtUnit_Diag = CreateInputRow(tpDiagnosticReport, "Unit", 330, 38, 415, 35, 150, "mg/dL");
            txtRefRange_Diag = CreateInputRow(tpDiagnosticReport, "Ref Range", 10, 68, 115, 65, 200, "70-100");
            txtRemarks_Diag = CreateInputRow(tpDiagnosticReport, "Remarks", 330, 68, 415, 65, 150, "Normal");
            txtInterpretation_Diag = CreateInputRow(tpDiagnosticReport, "Interpretation", 10, 98, 115, 95, 450, "Clinically stable");

            CreateButtons(tpDiagnosticReport, btnAddMedicine_Diag_Click, btnRemoveMedicine_Diag_Click);
            lvMedicines_Diag = CreateListView(tpDiagnosticReport,
                new[] { "Test Name", "Specimen", "Result", "Unit", "Ref Range", "Remarks", "Interpretation" },
                new[] { 170, 80, 80, 60, 90, 100, 100 });

            // Tab 4: Discharge Summary
            tpDischargeSummary = new TabPage("Discharge Summary");
            tcRecordDetails.TabPages.Add(tpDischargeSummary);

            txtMedicineName_Disch = CreateInputRow(tpDischargeSummary, "Discharge Med", 10, 8, 115, 5, 200);
            txtDosage_Disch = CreateInputRow(tpDischargeSummary, "Dosage", 330, 8, 415, 5, 150, "1-0-1");
            txtAdmDate_Disch = CreateInputRow(tpDischargeSummary, "Admission Date", 10, 38, 115, 35, 200, DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd"));
            txtDischDate_Disch = CreateInputRow(tpDischargeSummary, "Discharge Date", 330, 38, 415, 35, 150, DateTime.Now.ToString("yyyy-MM-dd"));
            txtCourse_Disch = CreateInputRow(tpDischargeSummary, "Treatment Course", 10, 68, 115, 65, 200, "IV Fluids & Antibiotics");
            txtAdvice_Disch = CreateInputRow(tpDischargeSummary, "Discharge Advice", 330, 68, 415, 65, 150, "Take rest");
            txtCondition_Disch = CreateInputRow(tpDischargeSummary, "Condition on Disch", 10, 98, 115, 95, 450, "Stable");

            CreateButtons(tpDischargeSummary, btnAddMedicine_Disch_Click, btnRemoveMedicine_Disch_Click);
            lvMedicines_Disch = CreateListView(tpDischargeSummary,
                new[] { "Discharge Med", "Dosage", "Adm Date", "Disch Date", "Course", "Advice", "Condition" },
                new[] { 170, 70, 80, 80, 95, 95, 90 });

            // Tab 5: Health Document (PDF Only)
            tpHealthDocument = new TabPage("Health Document");
            tcRecordDetails.TabPages.Add(tpHealthDocument);

            var lblInfo = new Label
            {
                Text = "Attach the clinical document (PDF) in the panel below.\nNo structured medication/test entry is needed for this type.",
                Location = new Point(20, 50),
                Size = new Size(500, 100),
                Font = gbPrescribe.Font,
                ForeColor = Color.DarkBlue
            };
            tpHealthDocument.Controls.Add(lblInfo);
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

                UpdateUiForRecordType();
            }
            finally
            {
                _isSyncing = false;
            }
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

        private async Task<bool> SaveAndPushActionAsync()
        {
            if (string.IsNullOrWhiteSpace(txtAbhaAddress.Text) || string.IsNullOrWhiteSpace(txtPatientName.Text))
            {
                MessageBox.Show("Please enter ABHA Address and Patient Name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string recordType = cmbRecordType.Text;

            // Auto-generate PDF clinical report if not manually attached
            if (_pdfBytes == null)
            {
                AppendLog($"No manual PDF uploaded. Auto-generating {recordType} PDF report...");
                var itemsForPdf = new List<string[]>();
                var activeLv = GetActiveListView();
                if (activeLv != null)
                {
                    foreach (ListViewItem item in activeLv.Items)
                    {
                        itemsForPdf.Add(new string[] {
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

                _pdfBytes = GenerateClinicalDocumentPdf(
                    recordType,
                    txtPatientName.Text.Trim(),
                    txtAbhaAddress.Text.Trim(),
                    cmbGender.Text.Trim(),
                    txtDob.Text.Trim(),
                    txtCareContextRef.Text.Trim(),
                    itemsForPdf,
                    "Dr. Sudeep Munjal"
                );
                _pdfFileName = $"{recordType}_{txtCareContextRef.Text.Trim()}.pdf";
                lblPdfStatus.Text = $"Auto-Generated: {_pdfFileName} ({(_pdfBytes.Length / 1024.0):F1} KB)";
                lblPdfStatus.ForeColor = Color.DarkGreen;
            }

            try
            {
                AppendLog("1. Registering Patient and Care Context in Wrapper DB...");

                string hiType = "Prescription";
                if (recordType == "OPConsultationRecord")
                {
                    hiType = "OPConsultation";
                }
                else if (recordType == "HealthDocumentRecord")
                {
                    hiType = "HealthDocumentRecord";
                }
                else if (recordType == "DiagnosticReport")
                {
                    hiType = "DiagnosticReport";
                }
                else if (recordType == "DischargeSummary")
                {
                    hiType = "DischargeSummary";
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
                var activeLv2 = GetActiveListView();
                if (activeLv2 != null)
                {
                    foreach (ListViewItem item in activeLv2.Items)
                    {
                        medicinesList.Add(new
                        {
                            medicine = item.Text,
                            dosage = item.SubItems.Count > 1 ? item.SubItems[1].Text : "",
                            timing = item.SubItems.Count > 2 ? item.SubItems[2].Text : "",
                            route = item.SubItems.Count > 3 ? item.SubItems[3].Text : "",
                            method = item.SubItems.Count > 4 ? item.SubItems[4].Text : "",
                            additionalInstructions = item.SubItems.Count > 5 ? item.SubItems[5].Text : "",
                            reason = item.SubItems.Count > 6 ? item.SubItems[6].Text : ""
                        });
                    }
                }

                var documentsList = new List<object>();
                if (_pdfBytes != null)
                {
                    documentsList.Add(new
                    {
                        contentType = "application/pdf",
                        type = recordType,
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
                    practitioners = new[] { new { name = "Dr. Sudeep Munjal", practitionerId = "DOC-01" } },
                    organisation = new { facilityName = "MIDHA HOSPITAL", facilityId = "IN0610090658" },
                    clinicalNotes = $"{recordType} Details",
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
                        AppendLog($"[DATA PUSH SUCCESS] {recordType} details & attachments saved to Wrapper DB.");
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
                else if (recordType == "DiagnosticReport")
                {
                    hiType = "DiagnosticReport";
                }
                else if (recordType == "DischargeSummary")
                {
                    hiType = "DischargeSummary";
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
