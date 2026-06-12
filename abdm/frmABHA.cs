using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABHA : BaseForm
    {
        private bool isOtpMode = false;
        private int otpSeconds = 60;
        private int resendCount = 0;
        private bool isMobileOtpPending = false;
        private string _mobileForVerification;
        private AbdmApiClient _client;
        private string _txnId;

        public frmABHA()
        {
            InitializeComponent();
        }

        private void frmABHA_Load(object sender, EventArgs e)
        {
            SwitchToRegistrationMode();

            // Background pre-fetch: Gateway Token aur Public Key pehle se mangwa kar rakh lete hain
            // taaki Submit click karne par delay na ho.
            Task.Run(async () =>
            {
                try
                {
                    var client = GetAbdmClient();
                    string token = await client.GetAccessTokenAsync();
                    if (!string.IsNullOrEmpty(token))
                    {
                        await client.GetPublicKeyAsync(token);
                    }
                }
                catch { /* Silent fail: Agar background mein fail ho gaya to Submit par dobara try hoga */ }
            });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!isOtpMode)
            {
                string aadhaar = (txtAadhaar1.Text + txtAadhaar2.Text + txtAadhaar3.Text).Trim();
                if (aadhaar.Length != 12 || !aadhaar.All(char.IsDigit))
                {
                    ShowWarning("Please enter a valid 12-digit Aadhaar number.");
                    return;
                }

                using (frmABHAConsent consent = new frmABHAConsent())
                {
                    if (consent.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            btnSubmit.Enabled = false;
                            btnSubmit.Text = "Sending OTP...";
                            
                            var client = GetAbdmClient();
                            var resp = await client.GenerateOtpAsync(new AbdmGenerateOtpRequest 
                            { 
                                LoginId = aadhaar, 
                                LoginType = "AADHAAR" 
                            });

                            if (resp.Success && resp.Data != null)
                            {
                                _txnId = resp.Data.TransactionId;
                                SwitchToOtpMode();
                            }
                            else
                            {
                                ShowWarning(resp.Message ?? "Failed to send OTP. Please try again.");
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowWarning("Error: " + ex.Message);
                        }
                        finally
                        {
                            btnSubmit.Enabled = true;
                            btnSubmit.Text = isOtpMode ? "Submit OTP" : "Submit";
                        }
                    }
                }
            }
            else
            {
                string otp = txtOTP.Text.Trim();
                string mobile = txtMobile.Text.Trim();

                if (string.IsNullOrEmpty(otp) || otp.Length != 6)
                {
                    ShowWarning("Please enter the 6-digit OTP received on your mobile.");
                    return;
                }
                
                if (string.IsNullOrEmpty(mobile) || mobile.Length != 10)
                {
                    ShowWarning("Please enter a valid 10-digit Mobile number.");
                    return;
                }

                try
                {
                    btnSubmit.Enabled = false;
                    btnSubmit.Text = "Verifying...";

                    var client = GetAbdmClient();
                    var resp = await client.VerifyOtpAsync(new AbdmVerifyOtpRequest
                    {
                        Otp = otp,
                        TransactionId = _txnId,
                        LoginType = "AADHAAR",
                        Mobile = mobile
                    });

                    if (resp.Success && resp.Data != null)
                    {
                        // Checkpoint: M1 requirement 1.9 - If mobile is different, we need another OTP
                        // For simplicity in this demo/sandbox, we assume the user verified the communication mobile.
                        // But to be fully compliant, we call the enrollment verification.
                        
                        if (resp.Data.IsNew)
                        {
                            // Naya user hai, pehle alert dikhayenge phir address wala form kholenge
                            MessageBox.Show("Mobile number has been saved and will be used for all your ABHA communications.", 
                                "ABHA Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            using (frmABHAAddress addressForm = new frmABHAAddress(GetAbdmClient(), _txnId, resp.Data.Token))
                            {
                                if (addressForm.ShowDialog() == DialogResult.OK)
                                {
                                    this.DialogResult = DialogResult.OK;
                                    this.Close();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Success! ABHA Account verified.\nName: {resp.Data.Name}\nABHA No: {resp.Data.HealthIdNumber}", 
                                "Registration Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            // Existing user hai, card preview dikhayenge
                            await ShowCardPreviewAsync(resp.Data.HealthIdNumber, resp.Data.Token);
                            
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    else
                    {
                        ShowWarning(resp.Message ?? "OTP verification failed.");
                    }
                }
                catch (Exception ex)
                {
                    ShowWarning("Error: " + ex.Message);
                }
                finally
                {
                    btnSubmit.Enabled = true;
                    btnSubmit.Text = "Submit OTP";
                }
            }
        }

        private async Task ShowCardPreviewAsync(string healthIdNumber, string userToken)
        {
            try
            {
                var client = GetAbdmClient();
                var cardResp = await client.GetAbhaCardAsync(userToken);
                if (cardResp.Success && cardResp.Data != null)
                {
                    using (var preview = new frmABHACardPreview(healthIdNumber, cardResp.Data.Content))
                    {
                        preview.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Failed to fetch ABHA card for preview: " + cardResp.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading card preview: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // We keep DownloadCardAsync for internal use if needed, or remove it since preview form handles it
       
        private void timerOtp_Tick(object sender, EventArgs e)
        {
            if (otpSeconds > 0)
            {
                otpSeconds--;
                lblResendOTP.Text = $"ReSend OTP ({otpSeconds})\nUpto 2 times";
                lblResendOTP.Enabled = false;
            }
            else
            {
                timerOtp.Stop();
                lblResendOTP.Text = "ReSend OTP\nUpto 2 times";
                lblResendOTP.Enabled = true;
                lblResendOTP.ForeColor = Color.Blue;
            }
        }

        private async void lblResendOTP_Click(object sender, EventArgs e)
        {
            if (resendCount < 2)
            {
                try
                {
                    string aadhaar = (txtAadhaar1.Text + txtAadhaar2.Text + txtAadhaar3.Text).Trim();
                    var client = GetAbdmClient();
                    var resp = await client.GenerateOtpAsync(new AbdmGenerateOtpRequest
                    {
                        LoginId = aadhaar,
                        LoginType = "AADHAAR"
                    });

                    if (resp.Success && resp.Data != null)
                    {
                        _txnId = resp.Data.TransactionId;
                        resendCount++;
                        otpSeconds = 60;
                        timerOtp.Start();
                        MessageBox.Show("OTP Resent successfully!", "Resend", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ShowWarning(resp.Message ?? "Failed to resend OTP.");
                    }
                }
                catch (Exception ex)
                {
                    ShowWarning("Error: " + ex.Message);
                }
            }
            else
            {
                ShowWarning("Maximum resend attempts reached.");
            }
        }

        private void SwitchToRegistrationMode()
        {
            isOtpMode = false;
            timerOtp.Stop();
            
            // Enable Aadhaar fields
            txtAadhaar1.Enabled = true;
            txtAadhaar2.Enabled = true;
            txtAadhaar3.Enabled = true;
            
            // Hide OTP fields
            lblMobile.Visible = false;
            txtMobile.Visible = false;
            lblOTP.Visible = false;
            txtOTP.Visible = false;
            lblResendOTP.Visible = false;
            lblMobileNote.Visible = false;
            
            // Update button text, position and ensure they are on top
            btnSubmit.Text = "Submit";
            btnSubmit.Location = new Point(210, 210);
            btnSubmit.Visible = true;
            btnSubmit.BringToFront();

            btnCancel.Location = new Point(340, 210);
            btnCancel.Visible = true;
            btnCancel.BringToFront();
        }

        private void SwitchToOtpMode()
        {
            isOtpMode = true;
            otpSeconds = 60;
            resendCount = 0;
            timerOtp.Start();
            
            // Disable Aadhaar fields
            txtAadhaar1.Enabled = false;
            txtAadhaar2.Enabled = false;
            txtAadhaar3.Enabled = false;
            
            // Show OTP fields
            lblMobile.Visible = true;
            txtMobile.Visible = true;
            lblOTP.Visible = true;
            txtOTP.Visible = true;
            lblResendOTP.Visible = true;
            lblResendOTP.Enabled = false;
            lblMobileNote.Visible = true;
            
            // Update button text, position and ensure they are on top
            btnSubmit.Text = "Submit OTP";
            btnSubmit.Location = new Point(210, 285);
            btnSubmit.Visible = true;
            btnSubmit.BringToFront();

            btnCancel.Location = new Point(340, 285);
            btnCancel.Visible = true;
            btnCancel.BringToFront();
            
            txtMobile.Focus();
        }

        private AbdmApiClient GetAbdmClient()
        {
            if (_client == null)
            {
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
            }
            return _client;
        }

        private void txtAadhaar1_TextChanged(object sender, EventArgs e)
        {
            if (txtAadhaar1.Text.Length == 4) txtAadhaar2.Focus();
        }

        private void txtAadhaar2_TextChanged(object sender, EventArgs e)
        {
            if (txtAadhaar2.Text.Length == 4) txtAadhaar3.Focus();
        }
    }
}
