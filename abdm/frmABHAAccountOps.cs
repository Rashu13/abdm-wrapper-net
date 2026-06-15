using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public class frmABHAAccountOps : BaseForm
    {
        private readonly AbdmApiClient _client;
        private readonly SavedSession _session;
        private string _mobileTxnId;
        private string _reKycTxnId;

        // UI Controls
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;
        
        // Session Panel
        private GroupBox gbSession;
        private Label lblNameVal;
        private Label lblAbhaVal;
        private Label lblMobileVal;
        private Label lblEmailVal;

        // Mobile Update Panel
        private GroupBox gbMobile;
        private Label lblNewMobile;
        private TextBox txtNewMobile;
        private Button btnSendMobileOtp;
        private Panel pnlMobileOtp;
        private Label lblMobileOtp;
        private TextBox txtMobileOtp;
        private Button btnVerifyMobileOtp;

        // Email Verification Panel
        private GroupBox gbEmail;
        private Label lblNewEmail;
        private TextBox txtNewEmail;
        private Button btnSendEmailLink;

        // Re-KYC Panel
        private GroupBox gbReKyc;
        private Button btnSendReKycOtp;
        private Panel pnlReKycOtp;
        private Label lblReKycOtp;
        private TextBox txtReKycOtp;
        private Button btnVerifyReKycOtp;

        public frmABHAAccountOps(SavedSession session)
        {
            _session = session;
            
            var settings = new AbdmSettings
            {
                BaseUrl = ConfigurationManager.AppSettings["AbdmSettings:BaseUrl"],
                AbhaServiceUrl = ConfigurationManager.AppSettings["AbdmSettings:AbhaServiceUrl"],
                ClientId = ConfigurationManager.AppSettings["AbdmSettings:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["AbdmSettings:ClientSecret"],
                HipId = ConfigurationManager.AppSettings["AbdmSettings:HipId"],
                HipName = ConfigurationManager.AppSettings["AbdmSettings:HipName"],
                CmId = ConfigurationManager.AppSettings["AbdmSettings:CmId"] ?? "sbx",
                Environment = ConfigurationManager.AppSettings["AbdmSettings:Environment"] ?? "Sandbox"
            };
            _client = new AbdmApiClient(settings);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 520);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Header Panel
            pnlHeader = new Panel
            {
                BackColor = Color.FromArgb(55, 115, 200),
                Dock = DockStyle.Top,
                Height = 50
            };

            lblTitle = new Label
            {
                Text = "ABHA Account Operations",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 12)
            };

            btnClose = new Button
            {
                Text = "X",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(50, 50),
                Location = new Point(750, 0),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnClose);
            this.Controls.Add(pnlHeader);

            // 1. Session Information GroupBox
            gbSession = new GroupBox
            {
                Text = "Active ABHA Profile Information",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Location = new Point(20, 65),
                Size = new Size(760, 95)
            };

            Label lblName = new Label { Text = "Name:", Location = new Point(15, 28), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblNameVal = new Label { Text = _session.UserName, Location = new Point(120, 28), AutoSize = true, ForeColor = Color.Navy };

            Label lblAbha = new Label { Text = "ABHA Address:", Location = new Point(15, 58), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblAbhaVal = new Label { Text = _session.AbhaAddress + " | " + _session.HealthIdNumber, Location = new Point(120, 58), AutoSize = true, ForeColor = Color.Navy };

            Label lblMobile = new Label { Text = "Mobile:", Location = new Point(420, 28), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblMobileVal = new Label { Text = _session.Mobile, Location = new Point(500, 28), AutoSize = true, ForeColor = Color.Navy };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(420, 58), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblEmailVal = new Label { Text = string.IsNullOrEmpty(_session.Email) ? "Not Linked" : _session.Email, Location = new Point(500, 58), AutoSize = true, ForeColor = Color.Navy };

            gbSession.Controls.AddRange(new Control[] { lblName, lblNameVal, lblAbha, lblAbhaVal, lblMobile, lblMobileVal, lblEmail, lblEmailVal });
            this.Controls.Add(gbSession);

            // 2. Update Mobile GroupBox
            gbMobile = new GroupBox
            {
                Text = "Update Registered Mobile Number",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Location = new Point(20, 175),
                Size = new Size(370, 190)
            };

            lblNewMobile = new Label { Text = "New Mobile:", Location = new Point(15, 30), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            txtNewMobile = new TextBox { Location = new Point(130, 28), Width = 130, MaxLength = 10, Font = new Font("Segoe UI", 9) };
            
            btnSendMobileOtp = new Button
            {
                Text = "Send OTP",
                Location = new Point(270, 27),
                Size = new Size(85, 26),
                BackColor = Color.FromArgb(55, 115, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSendMobileOtp.FlatAppearance.BorderSize = 0;
            btnSendMobileOtp.Click += btnSendMobileOtp_Click;

            pnlMobileOtp = new Panel { Location = new Point(15, 70), Size = new Size(340, 110), Visible = false };
            lblMobileOtp = new Label { Text = "Enter OTP:", Location = new Point(0, 10), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            txtMobileOtp = new TextBox { Location = new Point(115, 8), Width = 100, MaxLength = 6, Font = new Font("Segoe UI", 9) };
            
            btnVerifyMobileOtp = new Button
            {
                Text = "Verify & Update",
                Location = new Point(115, 45),
                Size = new Size(130, 32),
                BackColor = Color.FromArgb(46, 117, 89),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerifyMobileOtp.FlatAppearance.BorderSize = 0;
            btnVerifyMobileOtp.Click += btnVerifyMobileOtp_Click;

            pnlMobileOtp.Controls.AddRange(new Control[] { lblMobileOtp, txtMobileOtp, btnVerifyMobileOtp });
            gbMobile.Controls.AddRange(new Control[] { lblNewMobile, txtNewMobile, btnSendMobileOtp, pnlMobileOtp });
            this.Controls.Add(gbMobile);

            // 3. Email Verification GroupBox
            gbEmail = new GroupBox
            {
                Text = "Link / Verify Email Address",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Location = new Point(410, 175),
                Size = new Size(370, 110)
            };

            lblNewEmail = new Label { Text = "Email Address:", Location = new Point(15, 32), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            txtNewEmail = new TextBox { Location = new Point(130, 30), Width = 220, Font = new Font("Segoe UI", 9) };
            
            btnSendEmailLink = new Button
            {
                Text = "Send Verification Link",
                Location = new Point(130, 65),
                Size = new Size(180, 32),
                BackColor = Color.FromArgb(55, 115, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSendEmailLink.FlatAppearance.BorderSize = 0;
            btnSendEmailLink.Click += btnSendEmailLink_Click;

            gbEmail.Controls.AddRange(new Control[] { lblNewEmail, txtNewEmail, btnSendEmailLink });
            this.Controls.Add(gbEmail);

            // 4. Re-KYC GroupBox
            gbReKyc = new GroupBox
            {
                Text = "Re-KYC Profile Verification",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Location = new Point(410, 295),
                Size = new Size(370, 200)
            };

            btnSendReKycOtp = new Button
            {
                Text = "Initiate Re-KYC (Aadhaar OTP)",
                Location = new Point(15, 30),
                Size = new Size(230, 32),
                BackColor = Color.FromArgb(55, 115, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSendReKycOtp.FlatAppearance.BorderSize = 0;
            btnSendReKycOtp.Click += btnSendReKycOtp_Click;

            pnlReKycOtp = new Panel { Location = new Point(15, 75), Size = new Size(340, 115), Visible = false };
            lblReKycOtp = new Label { Text = "Enter OTP:", Location = new Point(0, 10), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            txtReKycOtp = new TextBox { Location = new Point(115, 8), Width = 100, MaxLength = 6, Font = new Font("Segoe UI", 9) };
            
            btnVerifyReKycOtp = new Button
            {
                Text = "Confirm Re-KYC Verification",
                Location = new Point(115, 45),
                Size = new Size(200, 32),
                BackColor = Color.FromArgb(46, 117, 89),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerifyReKycOtp.FlatAppearance.BorderSize = 0;
            btnVerifyReKycOtp.Click += btnVerifyReKycOtp_Click;

            pnlReKycOtp.Controls.AddRange(new Control[] { lblReKycOtp, txtReKycOtp, btnVerifyReKycOtp });
            gbReKyc.Controls.AddRange(new Control[] { btnSendReKycOtp, pnlReKycOtp });
            this.Controls.Add(gbReKyc);
        }

        // --- Event Handlers ---

        private async void btnSendMobileOtp_Click(object sender, EventArgs e)
        {
            string mobile = txtNewMobile.Text.Trim();
            if (mobile.Length != 10 || !mobile.All(char.IsDigit))
            {
                ShowWarning("Please enter a valid 10-digit mobile number.");
                return;
            }

            try
            {
                btnSendMobileOtp.Enabled = false;
                btnSendMobileOtp.Text = "Sending...";

                var request = new MobileUpdateOtpRequest
                {
                    TransactionId = _session.TxnId,
                    Mobile = mobile,
                    UserToken = _session.UserToken
                };

                var resp = await _client.MobileUpdateSendOtpAsync(request);
                if (resp.Success && resp.Data != null)
                {
                    _mobileTxnId = resp.Data.TransactionId;
                    pnlMobileOtp.Visible = true;
                    txtNewMobile.Enabled = false;
                    btnSendMobileOtp.Visible = false;
                    ShowInfo("OTP sent successfully to the new mobile number.");
                }
                else
                {
                    ShowWarning(resp.Message ?? "Failed to send mobile OTP.");
                }
            }
            catch (Exception ex)
            {
                ShowWarning("Error sending OTP: " + ex.Message);
            }
            finally
            {
                btnSendMobileOtp.Enabled = true;
                btnSendMobileOtp.Text = "Send OTP";
            }
        }

        private async void btnVerifyMobileOtp_Click(object sender, EventArgs e)
        {
            string otp = txtMobileOtp.Text.Trim();
            if (otp.Length != 6 || !otp.All(char.IsDigit))
            {
                ShowWarning("Please enter a valid 6-digit OTP.");
                return;
            }

            try
            {
                btnVerifyMobileOtp.Enabled = false;
                btnVerifyMobileOtp.Text = "Updating...";

                var request = new MobileUpdateVerifyRequest
                {
                    Otp = otp,
                    TransactionId = _mobileTxnId,
                    UserToken = _session.UserToken
                };

                var resp = await _client.MobileUpdateVerifyOtpAsync(request);
                if (resp.Success)
                {
                    _session.Mobile = txtNewMobile.Text.Trim();
                    ABDM.Models.SessionStore.Save(_session);
                    lblMobileVal.Text = _session.Mobile;

                    ShowInfo("Mobile number updated successfully in ABHA profile.");
                    pnlMobileOtp.Visible = false;
                    txtNewMobile.Enabled = true;
                    btnSendMobileOtp.Visible = true;
                    txtNewMobile.Text = "";
                }
                else
                {
                    ShowWarning(resp.Message ?? "Failed to verify OTP.");
                }
            }
            catch (Exception ex)
            {
                ShowWarning("Error verifying OTP: " + ex.Message);
            }
            finally
            {
                btnVerifyMobileOtp.Enabled = true;
                btnVerifyMobileOtp.Text = "Verify & Update";
            }
        }

        private async void btnSendEmailLink_Click(object sender, EventArgs e)
        {
            string email = txtNewEmail.Text.Trim();
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                ShowWarning("Please enter a valid email address.");
                return;
            }

            try
            {
                btnSendEmailLink.Enabled = false;
                btnSendEmailLink.Text = "Sending Link...";

                var request = new EmailVerificationLinkRequest
                {
                    Email = email,
                    UserToken = _session.UserToken
                };

                var resp = await _client.RequestEmailVerificationLinkAsync(request);
                if (resp.Success)
                {
                    _session.Email = email;
                    ABDM.Models.SessionStore.Save(_session);
                    lblEmailVal.Text = email;

                    ShowInfo("Verification link has been sent to your email. Please click it to complete linking.");
                    txtNewEmail.Text = "";
                }
                else
                {
                    ShowWarning(resp.Message ?? "Failed to send email verification link.");
                }
            }
            catch (Exception ex)
            {
                ShowWarning("Error sending link: " + ex.Message);
            }
            finally
            {
                btnSendEmailLink.Enabled = true;
                btnSendEmailLink.Text = "Send Verification Link";
            }
        }

        private async void btnSendReKycOtp_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendReKycOtp.Enabled = false;
                btnSendReKycOtp.Text = "Initiating...";

                var resp = await _client.RequestReKycOtpAsync(_session.UserToken, _session.HealthIdNumber, _session.AbhaAddress);
                if (resp.Success && resp.Data != null)
                {
                    _reKycTxnId = resp.Data.TransactionId;
                    pnlReKycOtp.Visible = true;
                    btnSendReKycOtp.Enabled = false;
                    ShowInfo("Re-KYC OTP has been sent to your Aadhaar-registered mobile.");
                }
                else
                {
                    ShowWarning(resp.Message ?? "Failed to initiate Re-KYC.");
                }
            }
            catch (Exception ex)
            {
                ShowWarning("Error initiating Re-KYC: " + ex.Message);
            }
            finally
            {
                if (pnlReKycOtp.Visible == false)
                {
                    btnSendReKycOtp.Enabled = true;
                    btnSendReKycOtp.Text = "Initiate Re-KYC (Aadhaar OTP)";
                }
            }
        }

        private async void btnVerifyReKycOtp_Click(object sender, EventArgs e)
        {
            string otp = txtReKycOtp.Text.Trim();
            if (otp.Length != 6 || !otp.All(char.IsDigit))
            {
                ShowWarning("Please enter a valid 6-digit OTP.");
                return;
            }

            try
            {
                btnVerifyReKycOtp.Enabled = false;
                btnVerifyReKycOtp.Text = "Verifying Re-KYC...";

                var resp = await _client.VerifyReKycOtpAsync(_session.UserToken, _reKycTxnId, otp);
                if (resp.Success)
                {
                    ShowInfo("Re-KYC profile verification completed successfully!");
                    pnlReKycOtp.Visible = false;
                    btnSendReKycOtp.Enabled = true;
                    btnSendReKycOtp.Text = "Initiate Re-KYC (Aadhaar OTP)";
                    txtReKycOtp.Text = "";
                }
                else
                {
                    ShowWarning(resp.Message ?? "Re-KYC verification failed.");
                }
            }
            catch (Exception ex)
            {
                ShowWarning("Error verifying Re-KYC OTP: " + ex.Message);
            }
            finally
            {
                btnVerifyReKycOtp.Enabled = true;
                btnVerifyReKycOtp.Text = "Confirm Re-KYC Verification";
            }
        }
    }
}
