using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmAbdmM1Dashboard : Form
    {
        private readonly AbdmApiClient _client;
        private SavedSession _currentSession;

        public frmAbdmM1Dashboard()
        {
            InitializeComponent();
            _client = GetAbdmClient();
        }

        private AbdmApiClient GetAbdmClient()
        {
            var settings = new AbdmSettings
            {
                BaseUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:BaseUrl"] ?? "https://dev.abdm.gov.in/api/hiecm/gateway",
                AbhaServiceUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:AbhaServiceUrl"] ?? "https://abhasbx.abdm.gov.in/abha/api/v3",
                ClientId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientId"],
                ClientSecret = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientSecret"],
                HipId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipId"],
                HipName = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipName"],
                CmId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:CmId"] ?? "sbx",
                Environment = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:Environment"] ?? "Sandbox"
            };
            return new AbdmApiClient(settings);
        }

        private void frmAbdmM1Dashboard_Load(object sender, EventArgs e)
        {
            RefreshSessionDisplay();
        }

        private void RefreshSessionDisplay()
        {
            try
            {
                _currentSession = ABDM.Models.SessionStore.Load();
                if (_currentSession != null && !string.IsNullOrEmpty(_currentSession.UserToken))
                {
                    lblNameVal.Text = _currentSession.UserName;
                    lblAbhaAddrVal.Text = _currentSession.AbhaAddress;
                    lblAbhaNumVal.Text = _currentSession.HealthIdNumber;
                    lblGenderVal.Text = _currentSession.Gender == "M" ? "Male" : (_currentSession.Gender == "F" ? "Female" : _currentSession.Gender);
                    lblDobVal.Text = _currentSession.Dob;
                    lblMobileVal.Text = _currentSession.Mobile;
                    
                    lblStatus.Text = $"Active Session Loaded. Token Expires: {_currentSession.SavedAtUtc.AddHours(24).ToLocalTime().ToString("g")}";
                    btnReKyc.Enabled = true;
                    btnDownloadCard.Enabled = true;

                    // Load User Photo if available
                    if (!string.IsNullOrEmpty(_currentSession.Photo))
                    {
                        try
                        {
                            byte[] imgBytes = Convert.FromBase64String(_currentSession.Photo);
                            using (var ms = new MemoryStream(imgBytes))
                            {
                                picPhoto.Image = Image.FromStream(ms);
                            }
                        }
                        catch
                        {
                            picPhoto.Image = null;
                        }
                    }
                    else if (!string.IsNullOrEmpty(_currentSession.ProfilePhoto))
                    {
                        try
                        {
                            byte[] imgBytes = Convert.FromBase64String(_currentSession.ProfilePhoto);
                            using (var ms = new MemoryStream(imgBytes))
                            {
                                picPhoto.Image = Image.FromStream(ms);
                            }
                        }
                        catch
                        {
                            picPhoto.Image = null;
                        }
                    }
                    else
                    {
                        picPhoto.Image = null;
                    }
                }
                else
                {
                    lblNameVal.Text = "Not Logged In";
                    lblAbhaAddrVal.Text = "-";
                    lblAbhaNumVal.Text = "-";
                    lblGenderVal.Text = "-";
                    lblDobVal.Text = "-";
                    lblMobileVal.Text = "-";
                    picPhoto.Image = null;
                    picCard.Image = null;

                    lblStatus.Text = "No active user session. Please Create a new ABHA or Login using an existing one.";
                    btnReKyc.Enabled = false;
                    btnDownloadCard.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Session display refresh failed: " + ex.Message;
            }
        }

        private void btnCreateAbha_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHA())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("New ABHA Number Created Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshSessionDisplay();
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHALogin())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    RefreshSessionDisplay();
                }
            }
        }

        private void btnScanShare_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHAShareList())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    RefreshSessionDisplay();
                }
            }
        }

        private async void btnReKyc_Click(object sender, EventArgs e)
        {
            if (_currentSession == null || string.IsNullOrEmpty(_currentSession.UserToken))
            {
                MessageBox.Show("Please login first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnReKyc.Enabled = false;
                btnReKyc.Text = "Sending OTP...";
                lblStatus.Text = "Initiating Re-KYC request...";

                var resp = await _client.RequestReKycOtpAsync(_currentSession.UserToken, _currentSession.HealthIdNumber, _currentSession.AbhaAddress);
                if (resp.Success && resp.Data != null)
                {
                    string txnId = resp.Data.TransactionId;
                    
                    // Simple input dialog for Re-KYC OTP verification
                    string otpVal = PromptDialog("Re-KYC OTP sent to Aadhaar-linked mobile. Enter OTP:", "Re-KYC Verification");
                    if (!string.IsNullOrEmpty(otpVal))
                    {
                        lblStatus.Text = "Verifying Re-KYC OTP...";
                        var verifyResp = await _client.VerifyReKycOtpAsync(_currentSession.UserToken, txnId, otpVal);
                        
                        if (verifyResp.Success)
                        {
                            MessageBox.Show("Re-KYC demographic verification successful!", "Re-KYC Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Re-fetch Profile to update database & session
                            var profileResp = await _client.GetAbhaProfileAsync(_currentSession.UserToken);
                            if (profileResp.Success && profileResp.Data != null)
                            {
                                var newSession = ABDM.Models.SessionStore.Create(null, _currentSession.UserToken, profileResp.Data);
                                ABDM.Models.SessionStore.Save(newSession);
                                RefreshSessionDisplay();
                            }
                        }
                        else
                        {
                            MessageBox.Show(verifyResp.Message ?? "Failed to verify Re-KYC OTP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(resp.Message ?? "Failed to trigger Re-KYC OTP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Re-KYC Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnReKyc.Enabled = true;
                btnReKyc.Text = "Verify Re-KYC";
                lblStatus.Text = "Session ready.";
            }
        }

        private async void btnDownloadCard_Click(object sender, EventArgs e)
        {
            if (_currentSession == null || string.IsNullOrEmpty(_currentSession.UserToken)) return;

            try
            {
                btnDownloadCard.Enabled = false;
                btnDownloadCard.Text = "Downloading...";
                lblStatus.Text = "Downloading official ABHA Card image from gateway...";

                var resp = await _client.GetAbhaCardAsync(_currentSession.UserToken);
                if (resp.Success && resp.Data != null)
                {
                    byte[] cardBytes = Convert.FromBase64String(resp.Data.Content);
                    using (var ms = new MemoryStream(cardBytes))
                    {
                        picCard.Image = Image.FromStream(ms);
                    }
                    lblStatus.Text = "ABHA Card downloaded successfully.";
                }
                else
                {
                    MessageBox.Show(resp.Message ?? "Failed to download ABHA card.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Download failed.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Download Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Download error.";
            }
            finally
            {
                btnDownloadCard.Enabled = true;
                btnDownloadCard.Text = "Download & Preview ABHA Card";
            }
        }

        private void btnClearSession_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the current active session?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ABDM.Models.SessionStore.Clear();
                RefreshSessionDisplay();
            }
        }

        private void btnM2Dashboard_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABDMM2())
            {
                frm.ShowDialog();
            }
        }

        private void btnM3Dashboard_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABDMM3())
            {
                frm.ShowDialog();
            }
        }

        private void btnBridgeConfig_Click(object sender, EventArgs e)
        {
            using (var frm = new frmBridgeConfig())
            {
                frm.ShowDialog();
            }
        }

        private void btnCreateRecord_Click(object sender, EventArgs e)
        {
            var frm = new frmPrescription();
            frm.Show();
        }

        private void btnAbhaPortal_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHAMain())
            {
                frm.ShowDialog();
            }
        }

        private string PromptDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 350 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 350, Font = new Font("Segoe UI", 12F) };
            Button confirmation = new Button() { Text = "Submit", Left = 270, Width = 100, Top = 85, DialogResult = DialogResult.OK };
            
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text.Trim() : "";
        }
    }
}
