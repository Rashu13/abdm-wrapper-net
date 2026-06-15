using System;
using System.Drawing;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public class frmABHAAddressCreationDirect : BaseForm
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
        private Label lblTxn;
        private TextBox txtTxn;
        private Label lblToken;
        private TextBox txtToken;

        private Button btnProceed;

        private readonly AbdmApiClient _client;

        public frmABHAAddressCreationDirect(AbdmApiClient client)
        {
            _client = client;
            InitializeComponent();
            CheckActiveSession();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 430);
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
                Text = "Create ABHA Address — Options",
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
                Text = "To create a customized ABHA Address (@sbx), ABDM requires an authenticated Transaction ID and User Token.",
                Location = new Point(20, 65),
                Size = new Size(460, 45),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            this.Controls.Add(lblIntro);

            // Options GroupBox
            gbOptions = new GroupBox
            {
                Text = "Select Authentication Source",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Location = new Point(20, 115),
                Size = new Size(460, 120)
            };

            rbSession = new RadioButton
            {
                Text = "Load credentials from current active session",
                Location = new Point(15, 25),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9.5F),
                Checked = true
            };
            rbSession.CheckedChanged += Source_CheckedChanged;

            rbManual = new RadioButton
            {
                Text = "Enter Transaction ID and User Token manually",
                Location = new Point(15, 55),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            rbManual.CheckedChanged += Source_CheckedChanged;

            rbDemo = new RadioButton
            {
                Text = "Demo Mode (Mock authentication transaction)",
                Location = new Point(15, 85),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            rbDemo.CheckedChanged += Source_CheckedChanged;

            gbOptions.Controls.AddRange(new Control[] { rbSession, rbManual, rbDemo });
            this.Controls.Add(gbOptions);

            // Manual inputs panel (Hidden initially)
            pnlManualInput = new Panel
            {
                Location = new Point(20, 245),
                Size = new Size(460, 110),
                Visible = false
            };

            lblTxn = new Label { Text = "Transaction ID:", Location = new Point(10, 5), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtTxn = new TextBox { Location = new Point(10, 25), Width = 440, Font = new Font("Segoe UI", 10) };

            lblToken = new Label { Text = "User Token:", Location = new Point(10, 55), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtToken = new TextBox { Location = new Point(10, 75), Width = 440, Font = new Font("Segoe UI", 10) };

            pnlManualInput.Controls.AddRange(new Control[] { lblTxn, txtTxn, lblToken, txtToken });
            this.Controls.Add(pnlManualInput);

            // Proceed Button
            btnProceed = new Button
            {
                Text = "Proceed to Creation",
                Location = new Point(150, 365),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(55, 115, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnProceed.FlatAppearance.BorderSize = 0;
            btnProceed.Click += BtnProceed_Click;
            this.Controls.Add(btnProceed);
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

        private void Source_CheckedChanged(object sender, EventArgs e)
        {
            pnlManualInput.Visible = rbManual.Checked;
        }

        private void BtnProceed_Click(object sender, EventArgs e)
        {
            string txnId = "";
            string token = "";

            if (rbSession.Checked)
            {
                var session = ABDM.Models.SessionStore.Load();
                if (session != null)
                {
                    txnId = session.TxnId;
                    token = session.UserToken;
                }
            }
            else if (rbManual.Checked)
            {
                txnId = txtTxn.Text.Trim();
                token = txtToken.Text.Trim();

                if (string.IsNullOrEmpty(txnId) || string.IsNullOrEmpty(token))
                {
                    ShowWarning("Please enter both Transaction ID and User Token.");
                    return;
                }
            }
            else if (rbDemo.Checked)
            {
                txnId = Guid.NewGuid().ToString();
                token = "DEMO_TOKEN_" + Guid.NewGuid().ToString().Substring(0, 8);
            }

            // Open the standard ABHA Address Form
            using (var addressForm = new frmABHAAddress(_client, txnId, token))
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
