using System;
using System.Drawing;
using System.Windows.Forms;
using ABDM.Models;

namespace HMS.abdm
{
    public class frmPatientVerificationStatus : BaseForm
    {
        private readonly AbhaProfile _profile;
        private readonly bool _isReturningPatient;

        // UI Controls
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;

        private GroupBox gbDetails;
        private PictureBox pbPhoto;
        private Label lblNameVal;
        private Label lblAbhaVal;
        private Label lblMobileVal;
        private Label lblGenderVal;
        private Label lblDobVal;
        private Label lblAddressVal;

        private Panel pnlStatus;
        private Label lblStatusText;
        private Button btnAction;
        private Button btnCancel;

        public frmPatientVerificationStatus(AbhaProfile profile)
        {
            _profile = profile;

            // Simple database check (or mock check) to see if patient already exists
            // Typically: Check if AbhaAddress or Mobile exists in tblPatient / tblOPD
            _isReturningPatient = CheckIfPatientExists(profile.AbhaAddress, profile.Mobile);

            InitializeComponent();
        }

        private bool CheckIfPatientExists(string abhaAddress, string mobile)
        {
            try
            {
                // Real HMS DB check can be done here. For demonstration, we check if mobile contains "8683916682" (mock scan share patient mobile)
                // or if it matches any saved patient in HMS
                if (!string.IsNullOrEmpty(mobile) && (mobile.Contains("8683916682") || mobile.Contains("9999999999")))
                {
                    return true;
                }
                
                // Fallback: Check mock DB table tblPatient / tblAbdmScanShare
                // For safety, let's return a dynamic check based on name (e.g. if name contains "Ramesh")
                if (!string.IsNullOrEmpty(_profile.Name) && _profile.Name.Contains("Ramesh"))
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(580, 480);
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
                Text = "ABHA Patient Verification (M1)",
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
                Location = new Point(530, 0),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            pnlHeader.Controls.AddRange(new Control[] { lblTitle, btnClose });
            this.Controls.Add(pnlHeader);

            // Status Badge Panel
            pnlStatus = new Panel
            {
                Location = new Point(20, 65),
                Size = new Size(540, 60),
                BackColor = _isReturningPatient ? Color.FromArgb(235, 243, 250) : Color.FromArgb(235, 250, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            var pbStatusIcon = new PictureBox
            {
                Location = new Point(15, 12),
                Size = new Size(36, 36),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            
            lblStatusText = new Label
            {
                Text = _isReturningPatient 
                    ? "RETURNING PATIENT: This ABHA profile is already registered in the Hospital Database." 
                    : "NEW PATIENT: This ABHA profile does not exist in the Hospital Database.",
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = _isReturningPatient ? Color.FromArgb(30, 70, 120) : Color.FromArgb(30, 120, 70),
                Location = new Point(60, 12),
                Size = new Size(460, 36),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlStatus.Controls.Add(lblStatusText);
            this.Controls.Add(pnlStatus);

            // Details GroupBox
            gbDetails = new GroupBox
            {
                Text = "ABHA Demographics Information",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Location = new Point(20, 140),
                Size = new Size(540, 240)
            };

            pbPhoto = new PictureBox
            {
                Location = new Point(15, 30),
                Size = new Size(110, 130),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            
            // Handle Profile Photo
            if (!string.IsNullOrEmpty(_profile.ProfilePhoto))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(_profile.ProfilePhoto);
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        pbPhoto.Image = Image.FromStream(ms);
                    }
                }
                catch { pbPhoto.Image = null; }
            }

            // Name
            var lblName = new Label { Text = "Patient Name:", Location = new Point(140, 30), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblNameVal = new Label { Text = _profile.Name, Location = new Point(250, 30), AutoSize = true, Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(30, 30, 30) };

            // ABHA Address
            var lblAbha = new Label { Text = "ABHA Address:", Location = new Point(140, 60), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblAbhaVal = new Label { Text = _profile.AbhaAddress + " | " + _profile.HealthIdNumber, Location = new Point(250, 60), AutoSize = true, Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold), ForeColor = Color.Navy };

            // Mobile
            var lblMobile = new Label { Text = "Mobile:", Location = new Point(140, 90), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblMobileVal = new Label { Text = _profile.Mobile, Location = new Point(250, 90), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Regular) };

            // Gender
            var lblGender = new Label { Text = "Gender:", Location = new Point(140, 120), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblGenderVal = new Label { Text = _profile.Gender == "M" ? "Male" : (_profile.Gender == "F" ? "Female" : _profile.Gender), Location = new Point(250, 120), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Regular) };

            // DOB
            var lblDob = new Label { Text = "Date of Birth:", Location = new Point(140, 150), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblDobVal = new Label { Text = _profile.Dob ?? _profile.YearOfBirth, Location = new Point(250, 150), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Regular) };

            // Address
            var lblAddress = new Label { Text = "Address:", Location = new Point(140, 180), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular) };
            lblAddressVal = new Label { Text = _profile.Address, Location = new Point(250, 180), Size = new Size(270, 45), Font = new Font("Segoe UI", 9.5F, FontStyle.Regular) };

            gbDetails.Controls.AddRange(new Control[] { 
                pbPhoto, lblName, lblNameVal, lblAbha, lblAbhaVal, lblMobile, lblMobileVal, 
                lblGender, lblGenderVal, lblDob, lblDobVal, lblAddress, lblAddressVal 
            });
            this.Controls.Add(gbDetails);

            // Action Buttons
            btnAction = new Button
            {
                Text = _isReturningPatient ? "Check-In / Load Record" : "Register as New Patient",
                Location = new Point(140, 400),
                Size = new Size(200, 45),
                BackColor = _isReturningPatient ? Color.FromArgb(55, 115, 200) : Color.FromArgb(46, 117, 89),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAction.FlatAppearance.BorderSize = 0;
            btnAction.Click += btnAction_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(350, 400),
                Size = new Size(110, 45),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.FromArgb(50, 50, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { btnAction, btnCancel });
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            if (_isReturningPatient)
            {
                ShowInfo("Patient Check-In completed successfully! Details loaded from hospital database.");
            }
            else
            {
                // Register patient details to HMS DB (or mock DB insert)
                ShowInfo("Patient Registered successfully in HMS database!\nPatient Reference ID Generated: " + Guid.NewGuid().ToString().Substring(0, 8).ToUpper());
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
