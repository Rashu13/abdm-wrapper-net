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

                var careContexts = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["referenceNumber"] = txtHipCareContextRef.Text.Trim(),
                        ["display"] = txtHipCareContextDisplay.Text.Trim(),
                        ["hiType"] = "Prescription"
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
                // Automatically push Health Data (Prescription) 
                // using the original data from this form!
                // ==========================================
                if (resp.Success)
                {
                    try
                    {
                        var parchiJson = new
                        {
                            bundleType = "PrescriptionRecord",
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
                            prescriptions = new[]
                            {
                                new { medicine = "Dummy Original Medicine 500mg", dosage = "1-0-1" }
                            }
                        };

                        var recordData = new
                        {
                            AbhaAddress = txtHipAbhaAddress.Text.Trim(),
                            CareContextReference = txtHipCareContextRef.Text.Trim(),
                            RecordType = "PrescriptionRecord",
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
                                txtLog.AppendText("\n[DATA PUSH] SUCCESS: Patient Health Data (Prescription) is pushed to Wrapper!\n");
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

                var careContexts = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["referenceNumber"] = txtHipCareContextRef.Text.Trim(),
                        ["display"] = txtHipCareContextDisplay.Text.Trim(),
                        ["hiType"] = "Prescription"
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

            try
            {
                btnRequestConsent.Enabled = false;
                btnRequestConsent.Text = "Requesting...";

                var hiTypes = new List<string>
                {
                    "DiagnosticReport",
                    "DischargeSummary",
                    "HealthDocumentRecord",
                    "ImmunizationRecord",
                    "OPConsultation",
                    "Prescription",
                    "WellnessRecord"
                };

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
                if (resp.Success && !string.IsNullOrEmpty(resp.Data))
                {
                    var dict = SimpleJson.Deserialize(resp.Data);
                    if (dict.ContainsKey("clientRequestId"))
                    {
                        txtHiuConsentReqId.Text = dict["clientRequestId"]?.ToString() ?? "";
                    }
                }
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
                ShowResult(resp.Success, resp.Message, resp.Data);
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
