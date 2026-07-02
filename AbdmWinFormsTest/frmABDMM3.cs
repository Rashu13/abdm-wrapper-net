using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABDMM3 : Form
    {
        private readonly AbdmApiClient _client;
        
        // Window dragging fields
        private bool _dragging = false;
        private Point _dragCursorPosition;
        private Point _dragFormPosition;

        public frmABDMM3()
        {
            InitializeComponent();
            _client = GetAbdmClient();

            // Set up form dragging
            this.pnlHeader.MouseDown += Header_MouseDown;
            this.pnlHeader.MouseMove += Header_MouseMove;
            this.pnlHeader.MouseUp += Header_MouseUp;

            // Set default date range values
            txtDateFrom.Text = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            txtDateTo.Text = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
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
        // === Form Dragging Logic ===
        // ==========================================
        private void Header_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragCursorPosition = Cursor.Position;
                _dragFormPosition = this.Location;
            }
        }

        private void Header_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point currentCursor = Cursor.Position;
                int diffX = currentCursor.X - _dragCursorPosition.X;
                int diffY = currentCursor.Y - _dragCursorPosition.Y;
                this.Location = new Point(_dragFormPosition.X + diffX, _dragFormPosition.Y + diffY);
            }
        }

        private void Header_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        // ==========================================
        // === M3 Event Handlers ===
        // ==========================================

        private async void btnInitiateSubscription_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAbhaAddress.Text))
            {
                MessageBox.Show("Please enter a valid Patient ABHA ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnInitiateSubscription.Enabled = false;
                btnInitiateSubscription.Text = "Initiating...";
                LogText($"[INIT] Starting Subscription Request for: {txtAbhaAddress.Text}...");

                List<string> categories = new List<string>();
                if (chkCategoryLink.Checked) categories.Add("LINK");
                if (chkCategoryData.Checked) categories.Add("DATA");

                if (categories.Count == 0)
                {
                    MessageBox.Show("Please select at least one category (LINK or DATA).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnInitiateSubscription.Enabled = true;
                    btnInitiateSubscription.Text = "1. Initiate Subscription Request";
                    return;
                }

                var resp = await _client.InitiateSubscriptionRequestAsync(
                    txtAbhaAddress.Text.Trim(),
                    txtPurposeCode.Text.Trim(),
                    categories,
                    txtDateFrom.Text.Trim(),
                    txtDateTo.Text.Trim()
                );

                if (resp.Success && resp.Data != null)
                {
                    LogText($"[SUCCESS] Subscription initiated response:\n{resp.Data}");
                    
                    // Parse clientRequestId from response if possible
                    // Response sample: {"clientRequestId":"...", "message":"..."}
                    string clientReqId = ExtractJsonValue(resp.Data, "clientRequestId");
                    if (!string.IsNullOrEmpty(clientReqId))
                    {
                        txtSubscriptionReqId.Text = clientReqId;
                        LogText($"[INFO] Extracted Request ID: {clientReqId}");
                    }
                }
                else
                {
                    LogText($"[ERROR] Failed to initiate subscription: {resp.Message}");
                    MessageBox.Show(resp.Message ?? "Failed to initiate subscription.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogText($"[EXCEPTION] {ex.Message}");
                MessageBox.Show($"Initiation failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnInitiateSubscription.Enabled = true;
                btnInitiateSubscription.Text = "1. Initiate Subscription Request";
            }
        }

        private async void btnCheckStatus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubscriptionReqId.Text))
            {
                MessageBox.Show("Please enter or generate a Subscription Request ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnCheckStatus.Enabled = false;
                btnCheckStatus.Text = "Checking...";
                LogText($"[STATUS] Checking subscription request status for: {txtSubscriptionReqId.Text}...");

                var resp = await _client.GetSubscriptionStatusAsync(txtSubscriptionReqId.Text.Trim());
                if (resp.Success && resp.Data != null)
                {
                    LogText($"[STATUS RESPONSE]\n{resp.Data}");
                }
                else
                {
                    LogText($"[ERROR] Failed to fetch subscription status: {resp.Message}");
                    MessageBox.Show(resp.Message ?? "Failed to fetch status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogText($"[EXCEPTION] {ex.Message}");
                MessageBox.Show($"Status check failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckStatus.Enabled = true;
                btnCheckStatus.Text = "2. Check Subscription Status";
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
                                    // Check if it's a standard FHIR Bundle
                                    if (bundle.ContainsKey("resourceType") && bundle["resourceType"]?.ToString() == "Bundle" && bundle.ContainsKey("entry") && bundle["entry"] is System.Collections.Generic.List<object> entries)
                                    {
                                        txtLog.AppendText("FHIR Bundle Contents:\n");
                                        
                                        foreach (var eObj in entries)
                                        {
                                            if (eObj is System.Collections.Generic.Dictionary<string, object> entry && entry.ContainsKey("resource") && entry["resource"] is System.Collections.Generic.Dictionary<string, object> res)
                                            {
                                                string rType = res.ContainsKey("resourceType") ? res["resourceType"]?.ToString() : "";
                                                
                                                if (rType == "Composition")
                                                {
                                                    string title = "Clinical Document";
                                                    if (res.ContainsKey("type") && res["type"] is System.Collections.Generic.Dictionary<string, object> tDict && tDict.ContainsKey("text"))
                                                    {
                                                        title = tDict["text"]?.ToString();
                                                    }
                                                    txtLog.AppendText($"  - Document Type: {title}\n");
                                                }
                                                else if (rType == "Practitioner")
                                                {
                                                    if (res.ContainsKey("name") && res["name"] is System.Collections.Generic.List<object> nList && nList.Count > 0 && nList[0] is System.Collections.Generic.Dictionary<string, object> nDict && nDict.ContainsKey("text"))
                                                    {
                                                        txtLog.AppendText($"  - Doctor: {nDict["text"]}\n");
                                                    }
                                                }
                                                else if (rType == "MedicationRequest")
                                                {
                                                    string med = "Unknown Medication";
                                                    if (res.ContainsKey("medicationCodeableConcept") && res["medicationCodeableConcept"] is System.Collections.Generic.Dictionary<string, object> mDict && mDict.ContainsKey("text"))
                                                    {
                                                        med = mDict["text"]?.ToString();
                                                    }
                                                    txtLog.AppendText($"  - Prescribed: {med}\n");
                                                }
                                                else if (rType == "Condition")
                                                {
                                                    string cond = "Unknown Condition";
                                                    if (res.ContainsKey("code") && res["code"] is System.Collections.Generic.Dictionary<string, object> cDict && cDict.ContainsKey("text"))
                                                    {
                                                        cond = cDict["text"]?.ToString();
                                                    }
                                                    txtLog.AppendText($"  - Condition/Symptom: {cond}\n");
                                                }
                                                else if (rType == "DiagnosticReport")
                                                {
                                                    txtLog.AppendText($"  - Diagnostic Report Attached\n");
                                                }
                                                else if (rType == "DocumentReference")
                                                {
                                                    txtLog.AppendText($"  - Attachment: PDF/Image Document (Data hidden)\n");
                                                    try
                                                    {
                                                        if (res.ContainsKey("content") && res["content"] is System.Collections.Generic.List<object> cList && cList.Count > 0)
                                                        {
                                                            if (cList[0] is System.Collections.Generic.Dictionary<string, object> cDict && cDict.ContainsKey("attachment") && cDict["attachment"] is System.Collections.Generic.Dictionary<string, object> att)
                                                            {
                                                                if (att.ContainsKey("data") && att["data"] != null)
                                                                {
                                                                    string base64 = att["data"].ToString();
                                                                    string ext = ".pdf";
                                                                    if (att.ContainsKey("contentType") && att["contentType"] != null)
                                                                    {
                                                                        string cType = att["contentType"].ToString().ToLower();
                                                                        if (cType.Contains("jpeg") || cType.Contains("jpg")) ext = ".jpg";
                                                                        else if (cType.Contains("png")) ext = ".png";
                                                                    }
                                                                    
                                                                    byte[] fileBytes = Convert.FromBase64String(base64);
                                                                    string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ABDM_Doc_{Guid.NewGuid().ToString().Substring(0, 8)}{ext}");
                                                                    System.IO.File.WriteAllBytes(tempFile, fileBytes);
                                                                    
                                                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                                                    {
                                                                        FileName = tempFile,
                                                                        UseShellExecute = true
                                                                    });
                                                                    txtLog.AppendText($"    -> Opened attachment automatically: {System.IO.Path.GetFileName(tempFile)}\n");
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        txtLog.AppendText($"    -> Failed to open attachment: {ex.Message}\n");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Fallback to old format if it's not a strict FHIR bundle
                                        string bType = bundle.ContainsKey("bundleType") ? bundle["bundleType"]?.ToString() : "Unknown";
                                        txtLog.AppendText($"Record Type: {bType}\n");
                                        if (bundle.ContainsKey("clinicalNotes") && bundle["clinicalNotes"] != null)
                                            txtLog.AppendText($"ClinicalNotes: {bundle["clinicalNotes"]}\n");
                                        if (bundle.ContainsKey("prescriptions") && bundle["prescriptions"] is System.Collections.Generic.List<object> meds)
                                        {
                                            txtLog.AppendText("Prescribed Medications:\n");
                                            foreach (System.Collections.Generic.Dictionary<string, object> med in meds)
                                            {
                                                string mName = med.ContainsKey("medicine") ? med["medicine"]?.ToString() : "";
                                                string mDose = med.ContainsKey("dosage") ? med["dosage"]?.ToString() : "";
                                                txtLog.AppendText($"  - {mName} ({mDose})\n");
                                            }
                                        }
                                        if (bundle.ContainsKey("documents") && bundle["documents"] is System.Collections.Generic.List<object> docs)
                                        {
                                            txtLog.AppendText($"Attached Documents: {docs.Count} file(s). (Base64 PDF data omitted for readability)\n");
                                            foreach (var dObj in docs)
                                            {
                                                try
                                                {
                                                    if (dObj is System.Collections.Generic.Dictionary<string, object> dDict && dDict.ContainsKey("data") && dDict["data"] != null)
                                                    {
                                                        string base64 = dDict["data"].ToString();
                                                        byte[] pdfBytes = Convert.FromBase64String(base64);
                                                        string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ABDM_Doc_{Guid.NewGuid().ToString().Substring(0, 8)}.pdf");
                                                        System.IO.File.WriteAllBytes(tempFile, pdfBytes);
                                                        
                                                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                                        {
                                                            FileName = tempFile,
                                                            UseShellExecute = true
                                                        });
                                                    }
                                                }
                                                catch { /* ignore errors */ }
                                            }
                                        }
                                    }
                                }
                                txtLog.AppendText("\n");
                            }
                            
                            // Hide large base64 strings in the raw JSON output for readability
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

        
        private void LogText(string text)
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private string ExtractJsonValue(string json, string key)
        {
            try
            {
                // Simple parser since we don't have JsonDocument reference in net10 winforms directly without packages
                string token = $"\"{key}\"";
                int idx = json.IndexOf(token);
                if (idx == -1) return string.Empty;
                
                int valStart = json.IndexOf(":", idx);
                if (valStart == -1) return string.Empty;
                
                int quoteStart = json.IndexOf("\"", valStart);
                if (quoteStart == -1 || quoteStart > valStart + 10)
                {
                    // Maybe numeric or boolean value, not string
                    int commaIdx = json.IndexOf(",", valStart);
                    int braceIdx = json.IndexOf("}", valStart);
                    int endIdx = (commaIdx == -1) ? braceIdx : (braceIdx == -1 ? commaIdx : Math.Min(commaIdx, braceIdx));
                    if (endIdx == -1) return string.Empty;
                    return json.Substring(valStart + 1, endIdx - valStart - 1).Trim().Replace("\"", "");
                }
                
                int quoteEnd = json.IndexOf("\"", quoteStart + 1);
                if (quoteEnd == -1) return string.Empty;
                return json.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
            }
            catch
            {
                return string.Empty;
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