using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABHALogin : Form
    {
        private readonly AbdmApiClient _client;
        private string _txnId;
        private string _loginType;
        public AbhaProfile LoggedInProfile { get; private set; }

        public frmABHALogin()
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private async void btnSendOtp_Click(object sender, EventArgs e)
        {
            string loginId = txtLoginId.Text.Trim();
            if (string.IsNullOrEmpty(loginId))
            {
                MessageBox.Show("Please enter Mobile or ABHA Number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnSendOtp.Enabled = false;
                btnSendOtp.Text = "Sending...";

                string loginType = "MOBILE";
                string loginIdClean = loginId.Replace("-", "").Trim();
                if (loginIdClean.Length == 12 && loginIdClean.All(char.IsDigit))
                {
                    loginType = "AADHAAR";
                }
                _loginType = loginType;

                var request = new AbdmGenerateOtpRequest 
                { 
                    LoginId = loginId,
                    LoginType = loginType
                };
                var resp = await _client.LoginRequestOtpAsync(request);

                if (resp.Success && resp.Data != null)
                {
                    _txnId = resp.Data.TransactionId;
                    pnlOtp.Visible = true;
                    txtLoginId.Enabled = false;
                    btnSendOtp.Visible = false;
                    this.Height = 450; // Expand to show OTP panel
                    MessageBox.Show("OTP sent to your registered mobile.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtOtp.Focus();
                }
                else
                {
                    MessageBox.Show(resp.Message ?? "Failed to send OTP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSendOtp.Enabled = true;
                btnSendOtp.Text = "Send OTP";
            }
        }

        private async void btnVerify_Click(object sender, EventArgs e)
        {
            string otp = txtOtp.Text.Trim();
            if (string.IsNullOrEmpty(otp))
            {
                MessageBox.Show("Please enter the OTP.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnVerify.Enabled = false;
                btnVerify.Text = "Verifying...";

                var request = new AbdmVerifyOtpRequest
                {
                    Otp = otp,
                    TransactionId = _txnId,
                    LoginType = _loginType
                };
                var resp = await _client.LoginVerifyOtpAsync(request);

                if (resp.Success && resp.Data != null)
                {
                    // For M1 login, we might get multiple accounts
                    if (resp.Data.Accounts != null && resp.Data.Accounts.Count > 0)
                    {
                        // TODO: If multiple accounts, let user pick. For now, take first.
                        var basicProfile = resp.Data.Accounts[0];
                        
                        // Fetch Full Profile for auto-filling Registration Form
                        btnVerify.Text = "Fetching Profile...";
                        var profileResp = await _client.GetAbhaProfileAsync(resp.Data.Token);
                        
                        if (profileResp.Success && profileResp.Data != null)
                        {
                            LoggedInProfile = profileResp.Data;
                        }
                        else
                        {
                            // Fallback to basic info if full profile fetch fails
                            LoggedInProfile = basicProfile;
                        }

                        if (LoggedInProfile != null)
                        {
                            LoggedInProfile.Token = resp.Data.Token;
                            LoggedInProfile.RefreshToken = resp.Data.RefreshToken;
                            
                            // Save to Session Store
                            var session = ABDM.Models.SessionStore.Create(null, resp.Data.Token, LoggedInProfile);
                            ABDM.Models.SessionStore.Save(session);

                            MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to load your ABHA profile.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No ABHA accounts linked to this ID.", "No Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(resp.Message ?? "OTP Verification Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnVerify.Enabled = true;
                btnVerify.Text = "Verify & Login";
            }
        }
    }
}
