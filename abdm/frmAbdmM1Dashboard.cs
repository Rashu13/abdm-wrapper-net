using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public class frmAbdmM1Dashboard : Form
    {
        private readonly AbdmApiClient _client;
        private SavedSession _currentSession;

        // UI Components
        private GroupBox grpActions;
        private GroupBox grpProfile;
        private GroupBox grpCardPreview;
        
        private Button btnCreateAbha;
        private Button btnLogin;
        private Button btnReKyc;
        private Button btnScanShare;
        private Button btnClearSession;
        private Button btnM2Dashboard;
        private Button btnDownloadCard;
        private Button btnClose;

        private Label lblNameTitle;
        private Label lblNameVal;
        private Label lblAbhaAddrTitle;
        private Label lblAbhaAddrVal;
        private Label lblAbhaNumTitle;
        private Label lblAbhaNumVal;
        private Label lblGenderTitle;
        private Label lblGenderVal;
        private Label lblDobTitle;
        private Label lblDobVal;
        private Label lblMobileTitle;
        private Label lblMobileVal;
        private Label lblStatus;

        private PictureBox picPhoto;
        private PictureBox picCard;

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

        private void InitializeComponent()
        {
            this.grpActions = new GroupBox();
            this.grpProfile = new GroupBox();
            this.grpCardPreview = new GroupBox();
            
            this.btnCreateAbha = new Button();
            this.btnLogin = new Button();
            this.btnReKyc = new Button();
            this.btnScanShare = new Button();
            this.btnClearSession = new Button();
            this.btnDownloadCard = new Button();
            this.btnClose = new Button();

            this.lblNameTitle = new Label();
            this.lblNameVal = new Label();
            this.lblAbhaAddrTitle = new Label();
            this.lblAbhaAddrVal = new Label();
            this.lblAbhaNumTitle = new Label();
            this.lblAbhaNumVal = new Label();
            this.lblGenderTitle = new Label();
            this.lblGenderVal = new Label();
            this.lblDobTitle = new Label();
            this.lblDobVal = new Label();
            this.lblMobileTitle = new Label();
            this.lblMobileVal = new Label();
            this.lblStatus = new Label();

            this.picPhoto = new PictureBox();
            this.picCard = new PictureBox();

            // Form Layout
            this.Text = "ABDM Milestone 1 (M1) Integration Dashboard";
            this.Size = new Size(950, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 246, 250);

            // GroupBox Actions
            this.grpActions.Text = "ABHA Actions (M1)";
            this.grpActions.Location = new Point(15, 15);
            this.grpActions.Size = new Size(250, 450);
            this.grpActions.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.grpActions.BackColor = Color.White;

            int btnY = 35;
            int btnHeight = 45;
            int spacing = 15;

            // btnCreateAbha
            this.btnCreateAbha.Text = "Create New ABHA";
            this.btnCreateAbha.Location = new Point(20, btnY);
            this.btnCreateAbha.Size = new Size(210, btnHeight);
            this.btnCreateAbha.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            this.btnCreateAbha.BackColor = Color.FromArgb(41, 128, 185);
            this.btnCreateAbha.ForeColor = Color.White;
            this.btnCreateAbha.FlatStyle = FlatStyle.Flat;
            this.btnCreateAbha.Click += btnCreateAbha_Click;
            this.grpActions.Controls.Add(this.btnCreateAbha);

            // btnLogin
            btnY += btnHeight + spacing;
            this.btnLogin.Text = "Login / Verify ABHA";
            this.btnLogin.Location = new Point(20, btnY);
            this.btnLogin.Size = new Size(210, btnHeight);
            this.btnLogin.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            this.btnLogin.BackColor = Color.FromArgb(39, 174, 96);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.Click += btnLogin_Click;
            this.grpActions.Controls.Add(this.btnLogin);

            // btnScanShare
            btnY += btnHeight + spacing;
            this.btnScanShare.Text = "Scan & Share Requests";
            this.btnScanShare.Location = new Point(20, btnY);
            this.btnScanShare.Size = new Size(210, btnHeight);
            this.btnScanShare.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            this.btnScanShare.BackColor = Color.FromArgb(142, 68, 173);
            this.btnScanShare.ForeColor = Color.White;
            this.btnScanShare.FlatStyle = FlatStyle.Flat;
            this.btnScanShare.Click += btnScanShare_Click;
            this.grpActions.Controls.Add(this.btnScanShare);

            // btnReKyc
            btnY += btnHeight + spacing;
            this.btnReKyc.Text = "Verify Re-KYC";
            this.btnReKyc.Location = new Point(20, btnY);
            this.btnReKyc.Size = new Size(210, btnHeight);
            this.btnReKyc.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            this.btnReKyc.BackColor = Color.FromArgb(230, 126, 34);
            this.btnReKyc.ForeColor = Color.White;
            this.btnReKyc.FlatStyle = FlatStyle.Flat;
            this.btnReKyc.Click += btnReKyc_Click;
            this.grpActions.Controls.Add(this.btnReKyc);

            // btnClearSession
            btnY += btnHeight + spacing;
            this.btnClearSession.Text = "Clear Session / Logout";
            this.btnClearSession.Location = new Point(20, btnY);
            this.btnClearSession.Size = new Size(210, btnHeight);
            this.btnClearSession.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            this.btnClearSession.BackColor = Color.FromArgb(192, 57, 43);
            this.btnClearSession.ForeColor = Color.White;
            this.btnClearSession.FlatStyle = FlatStyle.Flat;
            this.btnClearSession.Click += btnClearSession_Click;
            this.grpActions.Controls.Add(this.btnClearSession);

            // btnM2Dashboard
            btnY += btnHeight + spacing;
            this.btnM2Dashboard = new Button();
            this.btnM2Dashboard.Text = "ABDM M2 Dashboard";
            this.btnM2Dashboard.Location = new Point(20, btnY);
            this.btnM2Dashboard.Size = new Size(210, btnHeight);
            this.btnM2Dashboard.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.btnM2Dashboard.BackColor = Color.FromArgb(52, 73, 94);
            this.btnM2Dashboard.ForeColor = Color.White;
            this.btnM2Dashboard.FlatStyle = FlatStyle.Flat;
            this.btnM2Dashboard.Click += btnM2Dashboard_Click;
            this.grpActions.Controls.Add(this.btnM2Dashboard);

            // GroupBox Profile
            this.grpProfile.Text = "Active Patient Profile";
            this.grpProfile.Location = new Point(280, 15);
            this.grpProfile.Size = new Size(380, 450);
            this.grpProfile.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.grpProfile.BackColor = Color.White;

            // picPhoto
            this.picPhoto.Location = new Point(20, 35);
            this.picPhoto.Size = new Size(110, 130);
            this.picPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            this.picPhoto.BorderStyle = BorderStyle.FixedSingle;
            this.picPhoto.BackColor = Color.FromArgb(236, 240, 241);
            this.grpProfile.Controls.Add(this.picPhoto);

            int lblX = 145;
            int lblY = 35;
            int lblHeight = 22;

            // Name
            this.lblNameTitle.Text = "Name:";
            this.lblNameTitle.Location = new Point(lblX, lblY);
            this.lblNameTitle.Size = new Size(50, lblHeight);
            this.lblNameTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblNameVal.Location = new Point(lblX + 55, lblY);
            this.lblNameVal.Size = new Size(170, lblHeight);
            this.lblNameVal.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.grpProfile.Controls.Add(this.lblNameTitle);
            this.grpProfile.Controls.Add(this.lblNameVal);

            // ABHA Address
            lblY += 30;
            this.lblAbhaAddrTitle.Text = "ABHA ID:";
            this.lblAbhaAddrTitle.Location = new Point(lblX, lblY);
            this.lblAbhaAddrTitle.Size = new Size(55, lblHeight);
            this.lblAbhaAddrTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblAbhaAddrVal.Location = new Point(lblX + 55, lblY);
            this.lblAbhaAddrVal.Size = new Size(170, lblHeight);
            this.lblAbhaAddrVal.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.grpProfile.Controls.Add(this.lblAbhaAddrTitle);
            this.grpProfile.Controls.Add(this.lblAbhaAddrVal);

            // ABHA Number
            lblY += 30;
            this.lblAbhaNumTitle.Text = "ABHA No:";
            this.lblAbhaNumTitle.Location = new Point(lblX, lblY);
            this.lblAbhaNumTitle.Size = new Size(60, lblHeight);
            this.lblAbhaNumTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblAbhaNumVal.Location = new Point(lblX + 60, lblY);
            this.lblAbhaNumVal.Size = new Size(165, lblHeight);
            this.lblAbhaNumVal.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.grpProfile.Controls.Add(this.lblAbhaNumTitle);
            this.grpProfile.Controls.Add(this.lblAbhaNumVal);

            // Gender
            lblY += 30;
            this.lblGenderTitle.Text = "Gender:";
            this.lblGenderTitle.Location = new Point(lblX, lblY);
            this.lblGenderTitle.Size = new Size(55, lblHeight);
            this.lblGenderTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblGenderVal.Location = new Point(lblX + 55, lblY);
            this.lblGenderVal.Size = new Size(170, lblHeight);
            this.lblGenderVal.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.grpProfile.Controls.Add(this.lblGenderTitle);
            this.grpProfile.Controls.Add(this.lblGenderVal);

            // DOB
            lblY = 175;
            this.lblDobTitle.Text = "Date of Birth:";
            this.lblDobTitle.Location = new Point(20, lblY);
            this.lblDobTitle.Size = new Size(90, lblHeight);
            this.lblDobTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblDobVal.Location = new Point(120, lblY);
            this.lblDobVal.Size = new Size(240, lblHeight);
            this.lblDobVal.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.grpProfile.Controls.Add(this.lblDobTitle);
            this.grpProfile.Controls.Add(this.lblDobVal);

            // Mobile
            lblY += 30;
            this.lblMobileTitle.Text = "Mobile Number:";
            this.lblMobileTitle.Location = new Point(20, lblY);
            this.lblMobileTitle.Size = new Size(95, lblHeight);
            this.lblMobileTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblMobileVal.Location = new Point(120, lblY);
            this.lblMobileVal.Size = new Size(240, lblHeight);
            this.lblMobileVal.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.grpProfile.Controls.Add(this.lblMobileTitle);
            this.grpProfile.Controls.Add(this.lblMobileVal);

            // Card Downloader Button inside profile
            this.btnDownloadCard.Text = "Download & Preview ABHA Card";
            this.btnDownloadCard.Location = new Point(20, 380);
            this.btnDownloadCard.Size = new Size(340, 45);
            this.btnDownloadCard.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.btnDownloadCard.BackColor = Color.FromArgb(52, 152, 219);
            this.btnDownloadCard.ForeColor = Color.White;
            this.btnDownloadCard.FlatStyle = FlatStyle.Flat;
            this.btnDownloadCard.Click += btnDownloadCard_Click;
            this.grpProfile.Controls.Add(this.btnDownloadCard);

            // GroupBox Card Preview
            this.grpCardPreview.Text = "ABHA Physical Card Preview";
            this.grpCardPreview.Location = new Point(675, 15);
            this.grpCardPreview.Size = new Size(245, 450);
            this.grpCardPreview.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.grpCardPreview.BackColor = Color.White;

            // picCard
            this.picCard.Location = new Point(15, 35);
            this.picCard.Size = new Size(215, 390);
            this.picCard.SizeMode = PictureBoxSizeMode.Zoom;
            this.picCard.BorderStyle = BorderStyle.FixedSingle;
            this.picCard.BackColor = Color.FromArgb(236, 240, 241);
            this.grpCardPreview.Controls.Add(this.picCard);

            // Status bar label
            this.lblStatus.Location = new Point(15, 485);
            this.lblStatus.Size = new Size(740, 30);
            this.lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            this.lblStatus.ForeColor = Color.FromArgb(127, 140, 141);
            this.lblStatus.Text = "Loading Session Info...";

            // btnClose
            this.btnClose.Text = "Close Dashboard";
            this.btnClose.Location = new Point(770, 480);
            this.btnClose.Size = new Size(150, 40);
            this.btnClose.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this.btnClose.BackColor = Color.FromArgb(149, 165, 166);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.Click += (s, e) => this.Close();

            // Add all controls to form
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpProfile);
            this.Controls.Add(this.grpCardPreview);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnClose);

            this.Load += frmAbdmM1Dashboard_Load;
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
