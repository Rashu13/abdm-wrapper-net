using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public class frmABHADrivingLicense : BaseForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;

        // Step 1: DL Details
        private Panel pnlStepDetails;
        private Label lblName;
        private TextBox txtName;
        private Label lblDlNo;
        private TextBox txtDlNo;
        private Label lblDob;
        private DateTimePicker dtpDob;
        private Label lblGender;
        private ComboBox cbGender;
        private Label lblMobile;
        private TextBox txtMobile;
        private Button btnSubmitDl;

        // Step 2: OTP Details (Visible upon submit)
        private Panel pnlStepOtp;
        private Label lblOtp;
        private TextBox txtOtp;
        private Button btnVerifyOtp;
        private Label lblOtpStatus;

        private readonly AbdmApiClient _client;
        private string _txnId;

        public frmABHADrivingLicense(AbdmApiClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 520);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Header
            pnlHeader = new Panel
            {
                BackColor = Color.FromArgb(55, 115, 200),
                Dock = DockStyle.Top,
                Height = 50
            };

            lblTitle = new Label
            {
                Text = "ABHA Registration — Driving License (M1)",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(12, 12)
            };

            btnClose = new Button
            {
                Text = "X",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(50, 50),
                Location = new Point(450, 0),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, btnClose });
            this.Controls.Add(pnlHeader);

            // DL Details Panel
            pnlStepDetails = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(460, 440)
            };

            lblName = new Label { Text = "Full Name (As in DL):", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold) };
            txtName = new TextBox { Location = new Point(10, 35), Width = 440, Font = new Font("Segoe UI", 11) };

            lblDlNo = new Label { Text = "Driving License Number (DL-XXXXXXXXXXXXXXXX):", Location = new Point(10, 80), AutoSize = true, Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold) };
            txtDlNo = new TextBox { Location = new Point(10, 105), Width = 440, Font = new Font("Segoe UI", 11), MaxLength = 20 };

            lblDob = new Label { Text = "Date of Birth:", Location = new Point(10, 150), AutoSize = true, Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold) };
            dtpDob = new DateTimePicker { Location = new Point(10, 175), Width = 440, Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Short };

            lblGender = new Label { Text = "Gender:", Location = new Point(10, 220), AutoSize = true, Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold) };
            cbGender = new ComboBox { Location = new Point(10, 245), Width = 440, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cbGender.Items.AddRange(new string[] { "Male", "Female", "Other" });
            cbGender.SelectedIndex = 0;

            lblMobile = new Label { Text = "Mobile Number (Linked with DL):", Location = new Point(10, 290), AutoSize = true, Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold) };
            txtMobile = new TextBox { Location = new Point(10, 315), Width = 440, Font = new Font("Segoe UI", 11), MaxLength = 10 };

            btnSubmitDl = new Button
            {
                Text = "Verify Driving License Details",
                Location = new Point(10, 370),
                Size = new Size(440, 45),
                BackColor = Color.FromArgb(55, 115, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSubmitDl.FlatAppearance.BorderSize = 0;
            btnSubmitDl.Click += BtnSubmitDl_Click;

            pnlStepDetails.Controls.AddRange(new Control[] {
                lblName, txtName, lblDlNo, txtDlNo, lblDob, dtpDob, lblGender, cbGender, lblMobile, txtMobile, btnSubmitDl
            });
            this.Controls.Add(pnlStepDetails);

            // OTP Step Panel (Initially Hidden)
            pnlStepOtp = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(460, 440),
                Visible = false
            };

            lblOtp = new Label
            {
                Text = "Enter 6-digit OTP sent to registered Mobile number:",
                Location = new Point(10, 40),
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold)
            };

            txtOtp = new TextBox
            {
                Location = new Point(10, 75),
                Width = 440,
                Font = new Font("Segoe UI", 14),
                MaxLength = 6,
                TextAlign = HorizontalAlignment.Center
            };

            lblOtpStatus = new Label
            {
                Text = "OTP has been sent successfully to your mobile.",
                Location = new Point(10, 130),
                Width = 440,
                ForeColor = Color.ForestGreen,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnVerifyOtp = new Button
            {
                Text = "Confirm OTP & Register",
                Location = new Point(10, 180),
                Size = new Size(440, 45),
                BackColor = Color.FromArgb(230, 100, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerifyOtp.FlatAppearance.BorderSize = 0;
            btnVerifyOtp.Click += BtnVerifyOtp_Click;

            pnlStepOtp.Controls.AddRange(new Control[] { lblOtp, txtOtp, lblOtpStatus, btnVerifyOtp });
            this.Controls.Add(pnlStepOtp);
        }

        private void BtnSubmitDl_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                ShowWarning("Please enter your name as written in Driving License.");
                return;
            }

            if (string.IsNullOrEmpty(txtDlNo.Text.Trim()) || txtDlNo.Text.Length < 10)
            {
                ShowWarning("Please enter a valid Driving License number.");
                return;
            }

            string mobile = txtMobile.Text.Trim();
            if (mobile.Length != 10 || !mobile.All(char.IsDigit))
            {
                ShowWarning("Please enter a valid 10-digit mobile number.");
                return;
            }

            // Mocking sending OTP to DL Mobile
            _txnId = Guid.NewGuid().ToString();
            pnlStepDetails.Visible = false;
            pnlStepOtp.Visible = true;
        }

        private void BtnVerifyOtp_Click(object sender, EventArgs e)
        {
            string otp = txtOtp.Text.Trim();
            if (otp.Length != 6 || !otp.All(char.IsDigit))
            {
                ShowWarning("Please enter a valid 6-digit OTP.");
                return;
            }

            MessageBox.Show("Driving License Demographics matched and verified successfully!", 
                "DL Verification Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Redirect to ABHA Address Creation
            using (frmABHAAddress addressForm = new frmABHAAddress(_client, _txnId, "MOCK_DL_TOKEN_" + Guid.NewGuid().ToString().Substring(0, 8)))
            {
                if (addressForm.ShowDialog() == DialogResult.OK)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}
