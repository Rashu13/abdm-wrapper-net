using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABDMM2 : Form
    {
        private readonly AbdmApiClient _client;

        public frmABDMM2()
        {
            InitializeComponent();
            _client = GetAbdmClient();

            // === Set dynamic dates for HIU Consent Request ===
            // DateFrom: 1 year back to include all historical records
            txtHiuDateFrom.Text = DateTime.UtcNow.AddYears(-1).ToString("yyyy-MM-ddT00:00:00.000Z");
            // DateTo: current time (present date) — ensures all records up to this second are included
            txtHiuDateTo.Text = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            // EraseAt: 6 months from now
            txtHiuEraseAt.Text = DateTime.UtcNow.AddMonths(6).ToString("yyyy-MM-ddT00:00:00.000Z");

            // Check all HI types by default
            for (int i = 0; i < clbHiTypes.Items.Count; i++)
            {
                clbHiTypes.SetItemChecked(i, true);
            }
        }

        private AbdmApiClient GetAbdmClient()
        {
            var settings = new AbdmSettings
            {
                BaseUrl = System.Configuration.ConfigurationManager.AppSettings["WrapperBaseUrl"] ?? "https://sbx.wati.digital",
                AbhaServiceUrl = System.Configuration.ConfigurationManager.AppSettings["AbfaServiceUrl"] ?? "https://abhasbx.abdm.gov.in/abha/api/v3",
                ClientId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientId"],
                ClientSecret = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientSecret"],
                HipId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipId"] ?? "IN0610090658",
                HipName = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipName"] ?? "MIDHA HOSPITAL",
                CmId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:CmId"] ?? "sbx",
                Environment = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:Environment"] ?? "Sandbox"
            };
            return new AbdmApiClient(settings);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCreateRecord_Click(object sender, EventArgs e)
        {
            var frm = new frmPrescription();
            frm.Show();
        }

        // ==========================================
        // === HIP (Linking) Event Handlers ===
        // ==========================================

        private async void btnRegisterPatient_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHipAbhaAddress.Text) || string.IsNullOrWhiteSpace(txtHipPatientName.Text))
            {
                MessageBox.Show("Please enter ABHA Address and Patient Name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnRegisterPatient.Enabled = false;
                btnRegisterPatient.Text = "Registering...";

                string hiType = "Prescription";
                string recordType = "PrescriptionRecord";
                if (txtHipCareContextDisplay.Text.IndexOf("Consult", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    txtHipCareContextDisplay.Text.IndexOf("OP", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    txtHipCareContextRef.Text.IndexOf("OP", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    hiType = "OPConsultation";
                    recordType = "OPConsultationRecord";
                }
                else if (txtHipCareContextDisplay.Text.IndexOf("Invoice", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         txtHipCareContextDisplay.Text.IndexOf("Bill", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         txtHipCareContextDisplay.Text.IndexOf("Receipt", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         txtHipCareContextRef.Text.StartsWith("INV", StringComparison.OrdinalIgnoreCase))
                {
                    hiType = "Invoice";
                    recordType = "InvoiceRecord";
                }

                var careContexts = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["referenceNumber"] = txtHipCareContextRef.Text.Trim(),
                        ["display"] = txtHipCareContextDisplay.Text.Trim(),
                        ["hiType"] = hiType
                    }
                };

                var resp = await _client.AddPatientsToWrapperAsync(
                    txtHipAbhaAddress.Text.Trim(),
                    txtHipPatientName.Text.Trim(),
                    cmbHipGender.Text.Trim(),
                    txtHipDob.Text.Trim(),
                    txtHipPatientRef.Text.Trim(),
                    txtHipPatientMobile.Text.Trim(),
                    careContexts
                );

                ShowResult(resp.Success, resp.Message, resp.Data);

                // ==========================================
                // Automatically push Health Data (Prescription / OPConsultation) 
                // using the original data from this form!
                // ==========================================
                if (resp.Success)
                {
                    try
                    {
                        object parchiJson;
                        if (hiType == "Invoice")
                        {
                            parchiJson = new
                            {
                                bundleType = recordType,
                                careContextReference = txtHipCareContextRef.Text.Trim(),
                                authoredOn = DateTime.UtcNow.ToString("o"),
                                patient = new
                                {
                                    name = txtHipPatientName.Text.Trim(),
                                    patientReference = txtHipPatientRef.Text.Trim(),
                                    gender = cmbHipGender.Text.Trim(),
                                    birthDate = txtHipDob.Text.Trim()
                                },
                                practitioners = new[] { new { name = "Doctor", practitionerId = "DOC-01" } },
                                organisation = new { facilityName = "MIDHA HOSPITAL", facilityId = "IN0610090658" },
                                invoiceType = "Consultation",
                                lineItems = new[]
                                {
                                    new { itemName = "Consultation Fee", price = "500", quantity = 1, unit = "visit" },
                                    new { itemName = "Pharmacy Charges", price = "350", quantity = 2, unit = "box" }
                                }
                            };
                        }
                        else
                        {
                            parchiJson = new
                            {
                                bundleType = recordType,
                                careContextReference = txtHipCareContextRef.Text.Trim(),
                                authoredOn = DateTime.UtcNow.ToString("o"),
                                patient = new
                                {
                                    name = txtHipPatientName.Text.Trim(),
                                    patientReference = txtHipPatientRef.Text.Trim(),
                                    gender = cmbHipGender.Text.Trim(),
                                    birthDate = txtHipDob.Text.Trim()
                                },
                                practitioners = new[] { new { name = "Doctor", practitionerId = "DOC-01" } },
                                organisation = new { facilityName = "MIDHA HOSPITAL", facilityId = "IN0610090658" },
                                clinicalNotes = hiType == "OPConsultation" ? "OPD Consultation - Patient complained of headache and body pain." : null,
                                prescriptions = new[]
                                {
                                    new { medicine = "Dummy Original Medicine 500mg", dosage = "1-0-1" }
                                }
                            };
                        }

                        var recordData = new
                        {
                            AbhaAddress = txtHipAbhaAddress.Text.Trim(),
                            CareContextReference = txtHipCareContextRef.Text.Trim(),
                            RecordType = recordType,
                            FhirJsonPayload = System.Text.Json.JsonSerializer.Serialize(parchiJson)
                        };

                        using (var client = new System.Net.Http.HttpClient())
                        {
                            string wrapperBaseUrl = System.Configuration.ConfigurationManager.AppSettings["WrapperBaseUrl"] ?? "https://sbx.wati.digital";
                            string apiUrl = $"{wrapperBaseUrl.TrimEnd('/')}/v3/patient/health-data"; 
                            var content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(recordData), System.Text.Encoding.UTF8, "application/json");
                            
                            var healthResp = await client.PostAsync(apiUrl, content);
                            if (healthResp.IsSuccessStatusCode)
                            {
                                txtLog.AppendText($"\n[DATA PUSH] SUCCESS: Patient Health Data ({hiType}) is pushed to Wrapper!\n");
                            }
                            else
                            {
                                txtLog.AppendText($"\n[DATA PUSH] ERROR: Failed to push Health Data to Wrapper. Status: {healthResp.StatusCode}\n");
                            }
                        }
                    }
                    catch (Exception pushEx)
                    {
                        txtLog.AppendText($"\n[DATA PUSH] EXCEPTION: {pushEx.Message}\n");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRegisterPatient.Enabled = true;
                btnRegisterPatient.Text = "1. Register Patient in DB";
            }
        }

        private async void btnInitiateLink_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHipAbhaAddress.Text))
            {
                MessageBox.Show("Please enter ABHA Address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnInitiateLink.Enabled = false;
                btnInitiateLink.Text = "Initiating...";

                string hiType = "Prescription";
                if (txtHipCareContextDisplay.Text.IndexOf("Consult", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    txtHipCareContextDisplay.Text.IndexOf("OP", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    txtHipCareContextRef.Text.IndexOf("OP", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    hiType = "OPConsultation";
                }

                var careContexts = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["referenceNumber"] = txtHipCareContextRef.Text.Trim(),
                        ["display"] = txtHipCareContextDisplay.Text.Trim(),
                        ["hiType"] = hiType
                    }
                };

                var resp = await _client.LinkCareContextsAsync(
                    txtHipAbhaAddress.Text.Trim(),
                    _client.Settings.HipId ?? "IN0610090658",
                    careContexts
                );

                ShowResult(resp.Success, resp.Message, resp.Data);

                // Auto-fill Link Request ID for checking status
                if (resp.Success && !string.IsNullOrEmpty(resp.Data))
                {
                    var dict = SimpleJson.Deserialize(resp.Data);
                    if (dict.ContainsKey("clientRequestId"))
                    {
                        txtHipLinkId.Text = dict["clientRequestId"]?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnInitiateLink.Enabled = true;
                btnInitiateLink.Text = "2. Link Care Contexts";
            }
        }

        private async void btnCheckLinkStatus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHipLinkId.Text))
            {
                MessageBox.Show("Please enter a Link Request ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnCheckLinkStatus.Enabled = false;
                btnCheckLinkStatus.Text = "Checking...";

                var resp = await _client.GetLinkStatusAsync(txtHipLinkId.Text.Trim());
                ShowResult(resp.Success, resp.Message, resp.Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckLinkStatus.Enabled = true;
                btnCheckLinkStatus.Text = "3. Check Link Status";
            }
        }

        private async void btnSendSms_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHipPatientMobile.Text))
            {
                MessageBox.Show("Please enter Patient Mobile number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnSendSms.Enabled = false;
                btnSendSms.Text = "Sending...";

                var resp = await _client.SendDeepLinkingSmsAsync(
                    txtHipPatientMobile.Text.Trim(),
                    _client.Settings.HipId ?? "IN0610090658",
                    _client.Settings.HipName ?? "MIDHA HOSPITAL"
                );
                ShowResult(resp.Success, resp.Message, resp.Data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSendSms.Enabled = true;
                btnSendSms.Text = "Send SMS Notification";
            }
        }

        // ==========================================
        // === HIU (Consent & Data Flow) Event Handlers ===
        // ==========================================

        private async void btnRequestConsent_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHiuPatientAbha.Text))
            {
                MessageBox.Show("Please enter Patient ABHA Address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var autoPilotRes = MessageBox.Show("Do you want to run the entire flow in Auto-Pilot mode?\n\n(It will wait for you to approve on the ABHA app, then automatically fetch and show the records!)", "1-Click Auto Fetch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            bool isAutoPilot = (autoPilotRes == DialogResult.Yes);

            try
            {
                btnRequestConsent.Enabled = false;
                btnRequestConsent.Text = "Requesting Consent...";

                var hiTypes = new List<string>();
                foreach (var item in clbHiTypes.CheckedItems)
                {
                    hiTypes.Add(item.ToString());
                }
                if (hiTypes.Count == 0)
                {
                    MessageBox.Show("Please select at least one HI Type.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnRequestConsent.Enabled = true;
                    btnRequestConsent.Text = "1. Initiate Consent Request";
                    return;
                }

                var resp = await _client.InitiateConsentRequestAsync(
                    txtHiuPatientAbha.Text.Trim(),
                    cmbPurpose.Text.Split('-')[0].Trim(),
                    hiTypes,
                    txtHiuDateFrom.Text.Trim(),
                    txtHiuDateTo.Text.Trim(),
                    txtHiuEraseAt.Text.Trim()
                );

                ShowResult(resp.Success, resp.Message, resp.Data);

                // Auto-fill Consent Request ID
                string reqId = "";
                if (resp.Success && !string.IsNullOrEmpty(resp.Data))
                {
                    var dict = SimpleJson.Deserialize(resp.Data);
                    if (dict.ContainsKey("clientRequestId"))
                    {
                        reqId = dict["clientRequestId"]?.ToString() ?? "";
                        txtHiuConsentReqId.Text = reqId;
                    }
                }

                if (!isAutoPilot) return;
                if (string.IsNullOrEmpty(reqId))
                {
                    txtLog.AppendText("\nAuto-Pilot aborted: Could not get Consent Request ID.\n");
                    return;
                }

                txtLog.AppendText("\n========================================\n");
                txtLog.AppendText("🚀 AUTO-PILOT ACTIVATED 🚀\n");
                txtLog.AppendText("========================================\n\n");

                // --- PHASE 1: Wait for Consent Approval ---
                string consentId = "";
                bool isGranted = false;
                int attempts = 0;
                
                txtLog.SelectionColor = Color.Blue;
                txtLog.AppendText("⏳ Waiting for patient to APPROVE consent on ABHA App...\n");
                txtLog.SelectionColor = Color.Black;

                while (attempts < 60) // 5 minutes max (60 * 5s)
                {
                    await Task.Delay(5000); // poll every 5 seconds
                    attempts++;
                    
                    var statusResp = await _client.GetConsentStatusAsync(reqId);
                    if (statusResp.Success && !string.IsNullOrEmpty(statusResp.Data))
                    {
                        var dict = SimpleJson.Deserialize(statusResp.Data);
                        string status = dict.ContainsKey("status") ? dict["status"]?.ToString() ?? "" : "";
                        txtLog.AppendText($"[Polling] Current Status: {status}\n");
                        
                        if (status.IndexOf("hiu notify", StringComparison.OrdinalIgnoreCase) >= 0 || status.IndexOf("GRANTED", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // Extract Consent ID
                            if (dict.ContainsKey("consentDetails") && dict["consentDetails"] is Dictionary<string, object> cDetails)
                            {
                                if (cDetails.ContainsKey("consent") && cDetails["consent"] is List<object> cList && cList.Count > 0)
                                {
                                    if (cList[0] is Dictionary<string, object> cObj && cObj.ContainsKey("consentArtefacts") && cObj["consentArtefacts"] is List<object> aList && aList.Count > 0)
                                    {
                                        if (aList[0] is Dictionary<string, object> aObj && aObj.ContainsKey("id"))
                                        {
                                            consentId = aObj["id"]?.ToString() ?? "";
                                            txtHiuConsentId.Text = consentId;
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(consentId) && consentId != "CONSENT_ON_NOTIFY_RESPONSE")
                            {
                                isGranted = true;
                                break;
                            }
                            else if (consentId == "CONSENT_ON_NOTIFY_RESPONSE" || status.IndexOf("DENIED", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                txtLog.SelectionColor = Color.Red;
                                txtLog.AppendText($"❌ Consent was DENIED by the patient. Auto-Pilot aborted.\n");
                                txtLog.SelectionColor = Color.Black;
                                return;
                            }
                        }
                    }
                }

                if (!isGranted || string.IsNullOrEmpty(consentId))
                {
                    txtLog.SelectionColor = Color.Red;
                    txtLog.AppendText("❌ Timeout waiting for consent approval. Auto-Pilot aborted.\n");
                    txtLog.SelectionColor = Color.Black;
                    return;
                }

                txtLog.SelectionColor = Color.DarkGreen;
                txtLog.AppendText($"✅ Consent GRANTED! (Consent ID: {consentId})\n\n");
                txtLog.SelectionColor = Color.Black;

                // --- PHASE 2: Auto Fetch Health Records ---
                txtLog.SelectionColor = Color.Blue;
                txtLog.AppendText("⚡ Automatically requesting Health Records...\n");
                txtLog.SelectionColor = Color.Black;
                
                var fetchResp = await _client.FetchHealthInformationAsync(
                    consentId,
                    txtHiuDateFrom.Text.Trim(),
                    txtHiuDateTo.Text.Trim()
                );
                
                string txnReqId = "";
                if (fetchResp.Success && !string.IsNullOrEmpty(fetchResp.Data))
                {
                    var dict = SimpleJson.Deserialize(fetchResp.Data);
                    if (dict.ContainsKey("clientRequestId"))
                    {
                        txnReqId = dict["clientRequestId"]?.ToString() ?? "";
                        txtHiuTxnId.Text = txnReqId;
                    }
                }

                if (string.IsNullOrEmpty(txnReqId))
                {
                    txtLog.SelectionColor = Color.Red;
                    txtLog.AppendText("❌ Failed to initiate fetch request. Auto-Pilot aborted.\n");
                    txtLog.SelectionColor = Color.Black;
                    return;
                }

                // --- PHASE 3: Wait for Data Push ---
                txtLog.SelectionColor = Color.Blue;
                txtLog.AppendText("⏳ Waiting for HIP to encrypt and push data (this may take a few seconds)...\n");
                txtLog.SelectionColor = Color.Black;
                
                attempts = 0;
                List<object> decryptedRecords = null;

                while (attempts < 20) // 100 seconds max
                {
                    await Task.Delay(5000);
                    attempts++;
                    
                    var dataResp = await _client.GetHealthInformationStatusAsync(txnReqId);
                    if (dataResp.Success && !string.IsNullOrEmpty(dataResp.Data))
                    {
                        var dict = SimpleJson.Deserialize(dataResp.Data);
                        if (dict.ContainsKey("decryptedHealthInformation") && dict["decryptedHealthInformation"] is List<object> records)
                        {
                            decryptedRecords = records;
                            break;
                        }
                        
                        string status = dict.ContainsKey("status") ? dict["status"]?.ToString() ?? "" : "";
                        if (status.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            txtLog.SelectionColor = Color.Red;
                            txtLog.AppendText($"❌ Error occurred during data transfer: {status}\n");
                            txtLog.SelectionColor = Color.Black;
                            return;
                        }
                    }
                }

                if (decryptedRecords == null || decryptedRecords.Count == 0)
                {
                    txtLog.SelectionColor = Color.Red;
                    txtLog.AppendText("❌ Timeout waiting for health data. Auto-Pilot aborted.\n");
                    txtLog.SelectionColor = Color.Black;
                    return;
                }

                txtLog.SelectionColor = Color.DarkGreen;
                txtLog.AppendText($"✅ Decrypted {decryptedRecords.Count} Health Record(s) successfully!\n");
                txtLog.AppendText($"🚀 Opening Health Record Viewer UI...\n");
                txtLog.SelectionColor = Color.Black;

                // Hide base64 data in the final json printout to keep the log clean
                string rawJsonToDisplay = System.Text.RegularExpressions.Regex.Replace(fetchResp.Data, "\"data\"\\s*:\\s*\"[A-Za-z0-9+/=]{100,}\"", "\"data\": \"[BASE64_DATA_HIDDEN]\"");
                
                // Show the UI
                var viewer = new frmHealthRecordViewer(decryptedRecords);
                viewer.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRequestConsent.Enabled = true;
                btnRequestConsent.Text = "1. Initiate Consent Request";
            }
        }

        private async void btnCheckConsentStatus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHiuConsentReqId.Text))
            {
                MessageBox.Show("Please enter Consent Request ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnCheckConsentStatus.Enabled = false;
                btnCheckConsentStatus.Text = "Checking...";

                var resp = await _client.GetConsentStatusAsync(txtHiuConsentReqId.Text.Trim());
                ShowResult(resp.Success, resp.Message, resp.Data);

                // If Consent was granted, parse out the consent artefact ID
                if (resp.Success && !string.IsNullOrEmpty(resp.Data))
                {
                    var dict = SimpleJson.Deserialize(resp.Data);
                    if (dict.ContainsKey("consentDetails") && dict["consentDetails"] is Dictionary<string, object> details)
                    {
                        if (details.ContainsKey("consent") && details["consent"] is List<object> consents && consents.Count > 0)
                        {
                            var consentObj = consents[0] as Dictionary<string, object>;
                            if (consentObj != null && consentObj.ContainsKey("consentArtefacts") && consentObj["consentArtefacts"] is List<object> artefacts && artefacts.Count > 0)
                            {
                                var art = artefacts[0] as Dictionary<string, object>;
                                if (art != null && art.ContainsKey("id"))
                                {
                                    txtHiuConsentId.Text = art["id"]?.ToString() ?? "";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckConsentStatus.Enabled = true;
                btnCheckConsentStatus.Text = "2. Check Consent Status";
            }
        }

        private async void btnFetchRecords_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHiuConsentId.Text))
            {
                MessageBox.Show("Please enter a Consent ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnFetchRecords.Enabled = false;
                btnFetchRecords.Text = "Fetching...";

                var resp = await _client.FetchHealthInformationAsync(
                    txtHiuConsentId.Text.Trim(),
                    txtHiuDateFrom.Text.Trim(),
                    txtHiuDateTo.Text.Trim()
                );

                ShowResult(resp.Success, resp.Message, resp.Data);

                // Auto-fill Health Info Txn/Req ID
                if (resp.Success && !string.IsNullOrEmpty(resp.Data))
                {
                    var dict = SimpleJson.Deserialize(resp.Data);
                    if (dict.ContainsKey("clientRequestId"))
                    {
                        txtHiuTxnId.Text = dict["clientRequestId"]?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFetchRecords.Enabled = true;
                btnFetchRecords.Text = "3. Fetch Health Records";
            }
        }

        private async void btnGetData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHiuTxnId.Text))
            {
                MessageBox.Show("Please enter Health Request/Txn ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnGetData.Enabled = false;
                btnGetData.Text = "Loading...";

                var resp = await _client.GetHealthInformationStatusAsync(txtHiuTxnId.Text.Trim());
                if (resp.Success && !string.IsNullOrEmpty(resp.Data))
                {
                    try
                    {
                        var dict = SimpleJson.Deserialize(resp.Data);
                        if (dict.ContainsKey("decryptedHealthInformation") && dict["decryptedHealthInformation"] is System.Collections.Generic.List<object> records)
                        {
                            txtLog.Clear();
                            txtLog.SelectionColor = Color.DarkGreen;
                            txtLog.AppendText("[SUCCESS] Decrypted Health Records Received!\n\n");
                            txtLog.SelectionColor = Color.Black;

                            foreach (System.Collections.Generic.Dictionary<string, object> rec in records)
                            {
                                string ccRef = rec.ContainsKey("careContextReference") ? rec["careContextReference"]?.ToString() : "Unknown";
                                txtLog.SelectionColor = Color.Blue;
                                txtLog.AppendText($"--- Care Context: {ccRef} ---\n");
                                txtLog.SelectionColor = Color.Black;

                                if (rec.ContainsKey("fhirBundle") && rec["fhirBundle"] is System.Collections.Generic.Dictionary<string, object> bundle)
                                {
                                    string bType = bundle.ContainsKey("bundleType") ? bundle["bundleType"]?.ToString() : "Unknown";
                                    txtLog.AppendText($"Record Type: {bType}\n");
                                }
                                txtLog.AppendText("\n");
                            }
                            
                            txtLog.AppendText("Opening Health Record Viewer UI...\n");
                            
                            // Open the new UI viewer
                            var viewer = new frmHealthRecordViewer(records);
                            viewer.ShowDialog(this);
                            
                            // Still show raw json (with base64 hidden) for debugging
                            string rawJsonToDisplay = System.Text.RegularExpressions.Regex.Replace(resp.Data, "\"data\"\\s*:\\s*\"[A-Za-z0-9+/=]{100,}\"", "\"data\": \"[BASE64_DATA_HIDDEN]\"");
                            txtLog.AppendText("Raw JSON below:\n" + FormatJson(rawJsonToDisplay));
                        }
                        else
                        {
                            ShowResult(resp.Success, resp.Message, resp.Data);
                        }
                    }
                    catch
                    {
                        ShowResult(resp.Success, resp.Message, resp.Data);
                    }
                }
                else
                {
                    ShowResult(resp.Success, resp.Message, resp.Data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGetData.Enabled = true;
                btnGetData.Text = "4. View Decrypted Data";
            }
        }

        // Helper to format and show API results
        private void ShowResult(bool success, string message, string json)
        {
            txtLog.Clear();
            txtLog.SelectionColor = success ? Color.DarkGreen : Color.DarkRed;
            txtLog.AppendText($"[RESULT] {(success ? "SUCCESS" : "FAILURE")}\n");
            txtLog.SelectionColor = Color.Black;
            if (!string.IsNullOrEmpty(message))
            {
                txtLog.AppendText($"Message: {message}\n\n");
            }
            if (!string.IsNullOrEmpty(json))
            {
                txtLog.AppendText("Response JSON:\n" + FormatJson(json));
            }
        }

        private string FormatJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return "";
            try
            {
                // Basic pretty-print implementation
                int indent = 0;
                var sb = new System.Text.StringBuilder();
                bool inQuotes = false;
                for (int i = 0; i < json.Length; i++)
                {
                    char c = json[i];
                    if (c == '"')
                    {
                        inQuotes = !inQuotes;
                        sb.Append(c);
                    }
                    else if (inQuotes)
                    {
                        sb.Append(c);
                    }
                    else if (c == '{' || c == '[')
                    {
                        sb.Append(c);
                        sb.Append(Environment.NewLine);
                        indent++;
                        sb.Append(new string(' ', indent * 2));
                    }
                    else if (c == '}' || c == ']')
                    {
                        sb.Append(Environment.NewLine);
                        indent--;
                        sb.Append(new string(' ', indent * 2));
                        sb.Append(c);
                    }
                    else if (c == ',')
                    {
                        sb.Append(c);
                        sb.Append(Environment.NewLine);
                        sb.Append(new string(' ', indent * 2));
                    }
                    else if (c == ':')
                    {
                        sb.Append(c);
                        sb.Append(' ');
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            catch
            {
                return json;
            }
        }
    }
}
