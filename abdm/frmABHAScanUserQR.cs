using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABDM.Models;

namespace HMS.abdm
{
    public class frmABHAScanUserQR : BaseForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;

        private Panel pnlCameraFrame;
        private Label lblCameraOverlay;
        private System.Windows.Forms.Timer timerScanLine;
        private int scanLineY = 0;
        private bool scanLineDirectionDown = true;

        private Button btnScan;
        private Label lblInstruction;

        public frmABHAScanUserQR()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 480);
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
                Text = "Scan Patient ABHA Card QR (M1)",
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

            // Instructions
            lblInstruction = new Label
            {
                Text = "Position the patient's ABHA QR code in front of the scanner webcam.",
                Location = new Point(20, 65),
                Size = new Size(460, 45),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 70, 70),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblInstruction);

            // Camera Frame/Scanner Simulation panel
            pnlCameraFrame = new Panel
            {
                Location = new Point(80, 120),
                Size = new Size(340, 240),
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlCameraFrame.Paint += PnlCameraFrame_Paint;

            lblCameraOverlay = new Label
            {
                Text = "📷 Camera View Active\n[Waiting for ABHA QR Code]",
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            pnlCameraFrame.Controls.Add(lblCameraOverlay);
            this.Controls.Add(pnlCameraFrame);

            // Scan line animator timer
            timerScanLine = new System.Windows.Forms.Timer { Interval = 30 };
            timerScanLine.Tick += TimerScanLine_Tick;
            timerScanLine.Start();

            // Action Button
            btnScan = new Button
            {
                Text = "Simulate Scan Card QR",
                Location = new Point(150, 395),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(46, 117, 89),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnScan.FlatAppearance.BorderSize = 0;
            btnScan.Click += BtnScan_Click;
            this.Controls.Add(btnScan);
        }

        private void TimerScanLine_Tick(object sender, EventArgs e)
        {
            if (scanLineDirectionDown)
            {
                scanLineY += 5;
                if (scanLineY >= pnlCameraFrame.Height) scanLineDirectionDown = false;
            }
            else
            {
                scanLineY -= 5;
                if (scanLineY <= 0) scanLineDirectionDown = true;
            }
            pnlCameraFrame.Invalidate();
        }

        private void PnlCameraFrame_Paint(object sender, PaintEventArgs e)
        {
            // Draw a green scanning laser line
            using (Pen greenPen = new Pen(Color.FromArgb(0, 255, 0), 2))
            {
                e.Graphics.DrawLine(greenPen, 0, scanLineY, pnlCameraFrame.Width, scanLineY);
            }

            // Draw crosshair corners
            using (Pen cornerPen = new Pen(Color.White, 3))
            {
                // Top Left
                e.Graphics.DrawLine(cornerPen, 20, 20, 40, 20);
                e.Graphics.DrawLine(cornerPen, 20, 20, 20, 40);

                // Top Right
                e.Graphics.DrawLine(cornerPen, pnlCameraFrame.Width - 20, 20, pnlCameraFrame.Width - 40, 20);
                e.Graphics.DrawLine(cornerPen, pnlCameraFrame.Width - 20, 20, pnlCameraFrame.Width - 20, 40);

                // Bottom Left
                e.Graphics.DrawLine(cornerPen, 20, pnlCameraFrame.Height - 20, 40, pnlCameraFrame.Height - 20);
                e.Graphics.DrawLine(cornerPen, 20, pnlCameraFrame.Height - 20, 20, pnlCameraFrame.Height - 40);

                // Bottom Right
                e.Graphics.DrawLine(cornerPen, pnlCameraFrame.Width - 20, pnlCameraFrame.Height - 20, pnlCameraFrame.Width - 40, pnlCameraFrame.Height - 20);
                e.Graphics.DrawLine(cornerPen, pnlCameraFrame.Width - 20, pnlCameraFrame.Height - 20, pnlCameraFrame.Width - 20, pnlCameraFrame.Height - 40);
            }
        }

        private async void BtnScan_Click(object sender, EventArgs e)
        {
            btnScan.Enabled = false;
            timerScanLine.Stop();
            lblCameraOverlay.Text = "🔍 Decoding QR Code...";
            lblCameraOverlay.ForeColor = Color.Yellow;

            await Task.Delay(1500); // Simulate decoding delay

            // Create a mock profile decoded from the user card's QR code
            var mockProfile = new AbhaProfile
            {
                HealthIdNumber = "91-7643-9821-45",
                AbhaAddress = "ramesh.kumar@sbx",
                Name = "Ramesh Kumar",
                Gender = "M",
                YearOfBirth = "1988",
                Dob = "15-08-1988",
                Mobile = "8683916682",
                Address = "H.No 45, Sector 15, Rohini, New Delhi, Delhi - 110085",
                City = "Rohini",
                State = "Delhi"
            };

            lblCameraOverlay.Text = "✅ Decoded Successfully!";
            lblCameraOverlay.ForeColor = Color.LimeGreen;

            // Direct to Patient Verification Screen (New vs Returning Check)
            using (var verificationForm = new frmPatientVerificationStatus(mockProfile))
            {
                verificationForm.ShowDialog();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
