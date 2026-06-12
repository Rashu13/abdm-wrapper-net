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
        public AbhaProfile LoggedInProfile { get; private set; }

        public frmABHALogin()
        {
            InitializeComponent();
            _client = new AbdmApiClient();
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

                var request = new AbdmGenerateOtpRequest { LoginId = loginId };
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
                    TransactionId = _txnId
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

                        LoggedInProfile.Token = resp.Data.Token;
                        LoggedInProfile.RefreshToken = resp.Data.RefreshToken;
                        
                        // Save to Session Store
                        var session = SessionStore.Create(null, resp.Data.Token, LoggedInProfile);
                        SessionStore.Save(session);
                        
                        this.DialogResult = DialogResult.OK;
                        this.Close();
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
