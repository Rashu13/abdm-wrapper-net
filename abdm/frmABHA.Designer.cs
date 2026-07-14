namespace HMS.abdm
{
    partial class frmABHA
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmABHA));
            pnlHeader = new Panel();
            btnClose = new Button();
            lblTitle = new Label();
            lblHeading = new Label();
            lblAadhaar = new Label();
            txtAadhaar1 = new TextBox();
            txtAadhaar2 = new TextBox();
            txtAadhaar3 = new TextBox();
            btnSubmit = new Button();
            btnCancel = new Button();
            pnlBanner = new Panel();
            lblBanner = new Label();
            lblMobile = new Label();
            txtMobile = new TextBox();
            lblOTP = new Label();
            txtOTP = new TextBox();
            lblResendOTP = new Label();
            lblMobileNote = new Label();
            timerOtp = new System.Windows.Forms.Timer(components);
            pictureBox1 = new PictureBox();
            pnlHeader.SuspendLayout();
            pnlBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(55, 115, 200);
            pnlHeader.Controls.Add(btnClose);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(650, 45);
            pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.Red;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(605, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(45, 45);
            btnClose.TabIndex = 1;
            btnClose.Text = "X";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(12, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(302, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Ayushman Bharat Health Account";
            // 
            // lblHeading
            // 
            lblHeading.AutoSize = true;
            lblHeading.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblHeading.ForeColor = Color.FromArgb(230, 100, 50);
            lblHeading.Location = new Point(210, 70);
            lblHeading.Name = "lblHeading";
            lblHeading.Size = new Size(259, 32);
            lblHeading.TabIndex = 1;
            lblHeading.Text = "ABHA REGISTRATION";
            // 
            // lblAadhaar
            // 
            lblAadhaar.AutoSize = true;
            lblAadhaar.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblAadhaar.Location = new Point(40, 140);
            lblAadhaar.Name = "lblAadhaar";
            lblAadhaar.Size = new Size(154, 23);
            lblAadhaar.TabIndex = 2;
            lblAadhaar.Text = "Aadhaar Number *";
            // 
            // txtAadhaar1
            // 
            txtAadhaar1.Font = new Font("Segoe UI", 12F);
            txtAadhaar1.Location = new Point(210, 135);
            txtAadhaar1.MaxLength = 4;
            txtAadhaar1.Name = "txtAadhaar1";
            txtAadhaar1.PasswordChar = 'X';
            txtAadhaar1.Size = new Size(80, 34);
            txtAadhaar1.TabIndex = 3;
            txtAadhaar1.TextAlign = HorizontalAlignment.Center;
            txtAadhaar1.TextChanged += txtAadhaar1_TextChanged;
            // 
            // txtAadhaar2
            // 
            txtAadhaar2.Font = new Font("Segoe UI", 12F);
            txtAadhaar2.Location = new Point(305, 135);
            txtAadhaar2.MaxLength = 4;
            txtAadhaar2.Name = "txtAadhaar2";
            txtAadhaar2.PasswordChar = 'X';
            txtAadhaar2.Size = new Size(80, 34);
            txtAadhaar2.TabIndex = 4;
            txtAadhaar2.TextAlign = HorizontalAlignment.Center;
            txtAadhaar2.TextChanged += txtAadhaar2_TextChanged;
            // 
            // txtAadhaar3
            // 
            txtAadhaar3.Font = new Font("Segoe UI", 12F);
            txtAadhaar3.Location = new Point(400, 135);
            txtAadhaar3.MaxLength = 4;
            txtAadhaar3.Name = "txtAadhaar3";
            txtAadhaar3.PasswordChar = 'X';
            txtAadhaar3.Size = new Size(80, 34);
            txtAadhaar3.TabIndex = 5;
            txtAadhaar3.TextAlign = HorizontalAlignment.Center;
            // 
            // btnSubmit
            // 
            btnSubmit.BackColor = Color.FromArgb(230, 100, 50);
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnSubmit.ForeColor = Color.White;
            btnSubmit.Location = new Point(210, 280);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(120, 45);
            btnSubmit.TabIndex = 6;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = false;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(55, 115, 200);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(340, 280);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 45);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // pnlBanner
            // 
            pnlBanner.BackColor = Color.FromArgb(0, 0, 100);
            pnlBanner.Controls.Add(lblBanner);
            pnlBanner.Dock = DockStyle.Bottom;
            pnlBanner.Location = new Point(0, 350);
            pnlBanner.Name = "pnlBanner";
            pnlBanner.Size = new Size(650, 50);
            pnlBanner.TabIndex = 8;
            // 
            // lblBanner
            // 
            lblBanner.Cursor = Cursors.Hand;
            lblBanner.Dock = DockStyle.Fill;
            lblBanner.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBanner.ForeColor = Color.Yellow;
            lblBanner.Location = new Point(0, 0);
            lblBanner.Name = "lblBanner";
            lblBanner.Size = new Size(650, 50);
            lblBanner.TabIndex = 0;
            lblBanner.Text = "Click here to Download ABHA CARD if already Registered";
            lblBanner.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblMobile
            // 
            lblMobile.AutoSize = true;
            lblMobile.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblMobile.Location = new Point(40, 190);
            lblMobile.Name = "lblMobile";
            lblMobile.Size = new Size(135, 46);
            lblMobile.TabIndex = 9;
            lblMobile.Text = "Correspondence\r\nMobile No.";
            // 
            // txtMobile
            // 
            txtMobile.Font = new Font("Segoe UI", 12F);
            txtMobile.Location = new Point(210, 195);
            txtMobile.MaxLength = 10;
            txtMobile.Name = "txtMobile";
            txtMobile.Size = new Size(175, 34);
            txtMobile.TabIndex = 10;
            // 
            // lblOTP
            // 
            lblOTP.AutoSize = true;
            lblOTP.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblOTP.Location = new Point(400, 200);
            lblOTP.Name = "lblOTP";
            lblOTP.Size = new Size(41, 23);
            lblOTP.TabIndex = 11;
            lblOTP.Text = "OTP";
            // 
            // txtOTP
            // 
            txtOTP.Font = new Font("Segoe UI", 12F);
            txtOTP.Location = new Point(450, 195);
            txtOTP.MaxLength = 6;
            txtOTP.Name = "txtOTP";
            txtOTP.Size = new Size(80, 34);
            txtOTP.TabIndex = 12;
            // 
            // lblResendOTP
            // 
            lblResendOTP.AutoSize = true;
            lblResendOTP.Cursor = Cursors.Hand;
            lblResendOTP.Font = new Font("Segoe UI", 8F, FontStyle.Underline);
            lblResendOTP.ForeColor = Color.Gray;
            lblResendOTP.Location = new Point(540, 195);
            lblResendOTP.Name = "lblResendOTP";
            lblResendOTP.Size = new Size(111, 38);
            lblResendOTP.TabIndex = 13;
            lblResendOTP.Text = "ReSend OTP (31)\r\nUpto 2 times";
            lblResendOTP.Click += lblResendOTP_Click;
            // 
            // lblMobileNote
            // 
            lblMobileNote.AutoSize = true;
            lblMobileNote.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblMobileNote.ForeColor = Color.FromArgb(0, 150, 0);
            lblMobileNote.Location = new Point(40, 245);
            lblMobileNote.Name = "lblMobileNote";
            lblMobileNote.Size = new Size(593, 20);
            lblMobileNote.TabIndex = 14;
            lblMobileNote.Text = "Note: This mobile number will be used for all the communications related to ABHA.";
            // 
            // timerOtp
            // 
            timerOtp.Interval = 1000;
            timerOtp.Tick += timerOtp_Tick;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(4, 51);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(171, 63);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 15;
            pictureBox1.TabStop = false;
            // 
            // frmABHA
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            ClientSize = new Size(650, 400);
            Controls.Add(pictureBox1);
            Controls.Add(lblMobileNote);
            Controls.Add(lblResendOTP);
            Controls.Add(txtOTP);
            Controls.Add(lblOTP);
            Controls.Add(txtMobile);
            Controls.Add(lblMobile);
            Controls.Add(pnlBanner);
            Controls.Add(btnCancel);
            Controls.Add(btnSubmit);
            Controls.Add(txtAadhaar3);
            Controls.Add(txtAadhaar2);
            Controls.Add(txtAadhaar1);
            Controls.Add(lblAadhaar);
            Controls.Add(lblHeading);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmABHA";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Load += frmABHA_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlBanner.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblAadhaar;
        private System.Windows.Forms.TextBox txtAadhaar1;
        private System.Windows.Forms.TextBox txtAadhaar2;
        private System.Windows.Forms.TextBox txtAadhaar3;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlBanner;
        private System.Windows.Forms.Label lblBanner;
        private System.Windows.Forms.Label lblMobile;
        private System.Windows.Forms.TextBox txtMobile;
        private System.Windows.Forms.Label lblOTP;
        private System.Windows.Forms.TextBox txtOTP;
        private System.Windows.Forms.Label lblResendOTP;
        private System.Windows.Forms.Label lblMobileNote;
        private System.Windows.Forms.Timer timerOtp;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

#endregion