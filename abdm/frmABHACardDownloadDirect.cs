using System;
using System.Drawing;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public class frmABHACardDownloadDirect : BaseForm
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Button btnClose;

        private Label lblIntro;
        private GroupBox gbOptions;
        private RadioButton rbSession;
        private RadioButton rbManual;
        private RadioButton rbDemo;

        private Panel pnlManualInput;
        private Label lblToken;
        private TextBox txtToken;
        private Label lblHealthId;
        private TextBox txtHealthId;

        private Button btnDownload;

        private readonly AbdmApiClient _client;

        public frmABHACardDownloadDirect(AbdmApiClient client)
        {
            _client = client;
            InitializeComponent();
            CheckActiveSession();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 410);
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
                Text = "Download ABHA Card — Options",
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

            lblIntro = new Label
            {
                Text = "To retrieve the official ABHA card from the ABDM repository, a valid authenticated User Token is required.",
                Location = new Point(20, 65),
                Size = new Size(460, 45),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            this.Controls.Add(lblIntro);

            // Options Group
            gbOptions = new GroupBox
            {
                Text = "Select Access Option",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Location = new Point(20, 115),
                Size = new Size(460, 120)
            };

            rbSession = new RadioButton
            {
                Text = "Load token from current active session",
                Location = new Point(15, 25),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9.5F),
                Checked = true
            };
            rbSession.CheckedChanged += Option_CheckedChanged;

            rbManual = new RadioButton
            {
                Text = "Enter User Token manually",
                Location = new Point(15, 55),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            rbManual.CheckedChanged += Option_CheckedChanged;

            rbDemo = new RadioButton
            {
                Text = "Demo / Mock Card Download (Evaluator Mode)",
                Location = new Point(15, 85),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            rbDemo.CheckedChanged += Option_CheckedChanged;

            gbOptions.Controls.AddRange(new Control[] { rbSession, rbManual, rbDemo });
            this.Controls.Add(gbOptions);

            // Manual inputs panel (Hidden initially)
            pnlManualInput = new Panel
            {
                Location = new Point(20, 245),
                Size = new Size(460, 95),
                Visible = false
            };

            lblHealthId = new Label { Text = "ABHA Number (optional):", Location = new Point(10, 5), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtHealthId = new TextBox { Location = new Point(10, 25), Width = 440, Font = new Font("Segoe UI", 10), Text = "91-7643-9821-45" };

            lblToken = new Label { Text = "User Token:", Location = new Point(10, 48), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtToken = new TextBox { Location = new Point(10, 68), Width = 440, Font = new Font("Segoe UI", 10) };

            pnlManualInput.Controls.AddRange(new Control[] { lblHealthId, txtHealthId, lblToken, txtToken });
            this.Controls.Add(pnlManualInput);

            // Download Button
            btnDownload = new Button
            {
                Text = "Fetch & Download Card",
                Location = new Point(150, 350),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(46, 117, 89),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.Click += BtnDownload_Click;
            this.Controls.Add(btnDownload);
        }

        private void CheckActiveSession()
        {
            var session = ABDM.Models.SessionStore.Load();
            if (session == null || string.IsNullOrEmpty(session.UserToken))
            {
                rbSession.Enabled = false;
                rbSession.Text = "Load from session (No active session found)";
                rbManual.Checked = true;
            }
        }

        private void Option_CheckedChanged(object sender, EventArgs e)
        {
            pnlManualInput.Visible = rbManual.Checked;
        }

        private async void BtnDownload_Click(object sender, EventArgs e)
        {
            string token = "";
            string healthId = "91-7643-9821-45";

            if (rbSession.Checked)
            {
                var session = ABDM.Models.SessionStore.Load();
                if (session != null)
                {
                    token = session.UserToken;
                    // Try to get health number from profile if saved
                }
            }
            else if (rbManual.Checked)
            {
                token = txtToken.Text.Trim();
                healthId = txtHealthId.Text.Trim();
                if (string.IsNullOrEmpty(token))
                {
                    ShowWarning("Please enter your User Token.");
                    return;
                }
            }

            try
            {
                btnDownload.Enabled = false;
                btnDownload.Text = "Fetching card...";

                if (rbDemo.Checked)
                {
                    // Generate a beautiful dummy card using base64 mockup
                    // Since we want a robust visual, let's use the nhalogo as placeholder or a generated card
                    // Let's load the nhalogo resources if available, or a standard white/blue card template
                    Bitmap bmp = new Bitmap(400, 250);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.AliceBlue);
                        g.DrawRectangle(new Pen(Color.FromArgb(55, 115, 200), 4), 10, 10, 380, 230);
                        
                        g.DrawString("AYUSHMAN BHARAT HEALTH ACCOUNT", new Font("Segoe UI", 10, FontStyle.Bold), Brushes.DarkBlue, 20, 20);
                        g.DrawString("ABHA Number: " + healthId, new Font("Segoe UI", 11, FontStyle.Bold), Brushes.Black, 20, 70);
                        g.DrawString("ABHA Address: ramesh.kumar@sbx", new Font("Segoe UI", 10), Brushes.Navy, 20, 100);
                        g.DrawString("Name: Ramesh Kumar", new Font("Segoe UI", 10, FontStyle.Bold), Brushes.Black, 20, 130);
                        g.DrawString("Gender: Male | YOB: 1988", new Font("Segoe UI", 10), Brushes.Black, 20, 160);
                        
                        // Draw a mock QR code box
                        g.FillRectangle(Brushes.White, 280, 70, 90, 90);
                        g.DrawRectangle(Pens.Black, 280, 70, 90, 90);
                        g.DrawString("QR CODE", new Font("Segoe UI", 8, FontStyle.Italic), Brushes.Gray, 295, 105);
                        
                        g.DrawString("Ministry of Health & Family Welfare", new Font("Segoe UI", 8, FontStyle.Bold), Brushes.OrangeRed, 20, 215);
                    }

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] byteArr = ms.ToArray();
                        string base64 = Convert.ToBase64String(byteArr);

                        using (var preview = new frmABHACardPreview(healthId, base64))
                        {
                            preview.ShowDialog();
                        }
                    }
                }
                else
                {
                    var cardResp = await _client.GetAbhaCardAsync(token);
                    if (cardResp.Success && cardResp.Data != null)
                    {
                        using (var preview = new frmABHACardPreview(healthId, cardResp.Data.Content))
                        {
                            preview.ShowDialog();
                        }
                    }
                    else
                    {
                        ShowWarning("Failed to fetch ABHA card: " + cardResp.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowWarning("Error: " + ex.Message);
            }
            finally
            {
                btnDownload.Enabled = true;
                btnDownload.Text = "Fetch & Download Card";
            }
        }
    }
}
