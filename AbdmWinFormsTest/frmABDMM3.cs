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
    }
}
