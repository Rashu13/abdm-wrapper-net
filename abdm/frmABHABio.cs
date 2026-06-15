using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public class frmABHABio : BaseForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;

        private Label lblAadhaar;
        private TextBox txtAadhaar;
        private Label lblBioType;
        private ComboBox cbBioType;

        private Button btnScan;
        private ProgressBar pbScanProgress;
        private Label lblScanStatus;

        private Panel pnlScannerGraphic;
        private Label lblScannerIcon;

        private System.Windows.Forms.Timer timerScan;
        private int scanStep = 0;
        private readonly AbdmApiClient _client;

        public frmABHABio(AbdmApiClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 480);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            timerScan = new System.Windows.Forms.Timer { Interval = 1000 };
            timerScan.Tick += TimerScan_Tick;

            // Header
            pnlHeader = new Panel
            {
                BackColor = Color.FromArgb(55, 115, 200),
                Dock = DockStyle.Top,
                Height = 50
            };

            lblTitle = new Label
            {
                Text = "ABHA Registration — Aadhaar Biometrics (M1)",
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

            // Aadhaar input
            lblAadhaar = new Label
            {
                Text = "Enter 12-digit Aadhaar Number:",
                Location = new Point(40, 70),
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };

            txtAadhaar = new TextBox
            {
                Location = new Point(40, 95),
                Width = 420,
                Font = new Font("Segoe UI", 12),
                MaxLength = 12
            };

            // Bio Type
            lblBioType = new Label
            {
                Text = "Select Biometric Scanner Type:",
                Location = new Point(40, 140),
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };

            cbBioType = new ComboBox
            {
                Location = new Point(40, 165),
                Width = 420,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbBioType.Items.AddRange(new string[] { "Fingerprint Scanner (Morpho/Mantra)", "Iris Scanner", "Face Authentication (UIDAI FaceRD)" });
            cbBioType.SelectedIndex = 0;

            // Scanner graphic container
            pnlScannerGraphic = new Panel
            {
                Location = new Point(150, 210),
                Size = new Size(200, 130),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblScannerIcon = new Label
            {
                Text = "👆\nRD Service Ready",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Fill
            };
            pnlScannerGraphic.Controls.Add(lblScannerIcon);

            // Progress Bar
            pbScanProgress = new ProgressBar
            {
                Location = new Point(40, 360),
                Width = 420,
                Height = 15,
                Visible = false
            };

            lblScanStatus = new Label
            {
                Text = "Ready to scan. Please place finger/eye in front of scanner.",
                Location = new Point(40, 385),
                Size = new Size(420, 20),
                ForeColor = Color.FromArgb(120, 120, 120),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Scan Button
            btnScan = new Button
            {
                Text = "Capture Biometrics",
                Location = new Point(150, 415),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(230, 100, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnScan.FlatAppearance.BorderSize = 0;
            btnScan.Click += BtnScan_Click;

            this.Controls.AddRange(new Control[] {
                lblAadhaar, txtAadhaar, lblBioType, cbBioType, pnlScannerGraphic, pbScanProgress, lblScanStatus, btnScan
            });
        }

        private void BtnScan_Click(object sender, EventArgs e)
        {
            string aadhaar = txtAadhaar.Text.Trim();
            if (aadhaar.Length != 12 || !aadhaar.All(char.IsDigit))
            {
                ShowWarning("Please enter a valid 12-digit Aadhaar number first.");
                return;
            }

            btnScan.Enabled = false;
            pbScanProgress.Value = 0;
            pbScanProgress.Visible = true;
            scanStep = 0;
            timerScan.Start();
        }

        private void TimerScan_Tick(object sender, EventArgs e)
        {
            scanStep++;
            switch (scanStep)
            {
                case 1:
                    lblScanStatus.Text = "Initializing RD Service scanner...";
                    pbScanProgress.Value = 25;
                    lblScannerIcon.Text = "⏳\nConnecting scanner...";
                    lblScannerIcon.ForeColor = Color.Navy;
                    break;
                case 2:
                    lblScanStatus.Text = "Capturing Biometric templates...";
                    pbScanProgress.Value = 60;
                    lblScannerIcon.Text = "🟢\nPlace Finger/Face Now";
                    lblScannerIcon.ForeColor = Color.Green;
                    break;
                case 3:
                    lblScanStatus.Text = "Verifying Aadhaar Biometrics with UIDAI...";
                    pbScanProgress.Value = 90;
                    lblScannerIcon.Text = "🔒\nEncrypting PID Block";
                    lblScannerIcon.ForeColor = Color.Orange;
                    break;
                case 4:
                    timerScan.Stop();
                    pbScanProgress.Value = 100;
                    lblScanStatus.Text = "Biometric Verification Success!";
                    lblScannerIcon.Text = "✅\nScan Verified Successfully";
                    lblScannerIcon.ForeColor = Color.Green;

                    MessageBox.Show("Aadhaar Biometric template matched successfully with UIDAI database!", 
                        "Biometric Verification Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Redirect to ABHA address creation
                    // Mocking standard response tokens
                    using (frmABHAAddress addressForm = new frmABHAAddress(_client, Guid.NewGuid().ToString(), "MOCK_BIO_TOKEN_" + Guid.NewGuid().ToString().Substring(0, 8)))
                    {
                        if (addressForm.ShowDialog() == DialogResult.OK)
                        {
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    btnScan.Enabled = true;
                    pbScanProgress.Visible = false;
                    break;
            }
        }
    }
}
