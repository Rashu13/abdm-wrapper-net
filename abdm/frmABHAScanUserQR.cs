using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABHAScanUserQR : BaseForm
    {
        private int scanLineY = 0;
        private bool scanLineDirectionDown = true;

        public frmABHAScanUserQR()
        {
            InitializeComponent();
            btnClose.Click += (s, e) => this.Close();
            pnlCameraFrame.Paint += PnlCameraFrame_Paint;
            timerScanLine.Tick += TimerScanLine_Tick;
            btnScan.Click += BtnScan_Click;
            timerScanLine.Start();
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
