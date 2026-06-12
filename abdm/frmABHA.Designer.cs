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
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblAadhaar = new System.Windows.Forms.Label();
            this.txtAadhaar1 = new System.Windows.Forms.TextBox();
            this.txtAadhaar2 = new System.Windows.Forms.TextBox();
            this.txtAadhaar3 = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlBanner = new System.Windows.Forms.Panel();
            this.lblBanner = new System.Windows.Forms.Label();
            this.lblMobile = new System.Windows.Forms.Label();
            this.txtMobile = new System.Windows.Forms.TextBox();
            this.lblOTP = new System.Windows.Forms.Label();
            this.txtOTP = new System.Windows.Forms.TextBox();
            this.lblResendOTP = new System.Windows.Forms.Label();
            this.lblMobileNote = new System.Windows.Forms.Label();
            this.timerOtp = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlHeader.SuspendLayout();
            this.pnlBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(650, 45);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(605, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 45);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(302, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Ayushman Bharat Health Account";
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.lblHeading.Location = new System.Drawing.Point(210, 70);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(259, 32);
            this.lblHeading.TabIndex = 1;
            this.lblHeading.Text = "ABHA REGISTRATION";
            // 
            // lblAadhaar
            // 
            this.lblAadhaar.AutoSize = true;
            this.lblAadhaar.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblAadhaar.Location = new System.Drawing.Point(40, 140);
            this.lblAadhaar.Name = "lblAadhaar";
            this.lblAadhaar.Size = new System.Drawing.Size(154, 23);
            this.lblAadhaar.TabIndex = 2;
            this.lblAadhaar.Text = "Aadhaar Number *";
            // 
            // txtAadhaar1
            // 
            this.txtAadhaar1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtAadhaar1.Location = new System.Drawing.Point(210, 135);
            this.txtAadhaar1.MaxLength = 4;
            this.txtAadhaar1.Name = "txtAadhaar1";
            this.txtAadhaar1.Size = new System.Drawing.Size(80, 34);
            this.txtAadhaar1.TabIndex = 3;
            this.txtAadhaar1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAadhaar1.TextChanged += new System.EventHandler(this.txtAadhaar1_TextChanged);
            // 
            // txtAadhaar2
            // 
            this.txtAadhaar2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtAadhaar2.Location = new System.Drawing.Point(305, 135);
            this.txtAadhaar2.MaxLength = 4;
            this.txtAadhaar2.Name = "txtAadhaar2";
            this.txtAadhaar2.Size = new System.Drawing.Size(80, 34);
            this.txtAadhaar2.TabIndex = 4;
            this.txtAadhaar2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAadhaar2.TextChanged += new System.EventHandler(this.txtAadhaar2_TextChanged);
            // 
            // txtAadhaar3
            // 
            this.txtAadhaar3.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtAadhaar3.Location = new System.Drawing.Point(400, 135);
            this.txtAadhaar3.MaxLength = 4;
            this.txtAadhaar3.Name = "txtAadhaar3";
            this.txtAadhaar3.Size = new System.Drawing.Size(80, 34);
            this.txtAadhaar3.TabIndex = 5;
            this.txtAadhaar3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnSubmit.FlatAppearance.BorderSize = 0;
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(210, 280);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(120, 45);
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(340, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 45);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlBanner
            // 
            this.pnlBanner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(100)))));
            this.pnlBanner.Controls.Add(this.lblBanner);
            this.pnlBanner.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBanner.Location = new System.Drawing.Point(0, 350);
            this.pnlBanner.Name = "pnlBanner";
            this.pnlBanner.Size = new System.Drawing.Size(650, 50);
            this.pnlBanner.TabIndex = 8;
            // 
            // lblBanner
            // 
            this.lblBanner.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBanner.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.Yellow;
            this.lblBanner.Location = new System.Drawing.Point(0, 0);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Size = new System.Drawing.Size(650, 50);
            this.lblBanner.TabIndex = 0;
            this.lblBanner.Text = "Click here to Download ABHA CARD if already Registered";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMobile
            // 
            this.lblMobile.AutoSize = true;
            this.lblMobile.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblMobile.Location = new System.Drawing.Point(40, 190);
            this.lblMobile.Name = "lblMobile";
            this.lblMobile.Size = new System.Drawing.Size(135, 46);
            this.lblMobile.TabIndex = 9;
            this.lblMobile.Text = "Correspondence\r\nMobile No.";
            // 
            // txtMobile
            // 
            this.txtMobile.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtMobile.Location = new System.Drawing.Point(210, 195);
            this.txtMobile.MaxLength = 10;
            this.txtMobile.Name = "txtMobile";
            this.txtMobile.Size = new System.Drawing.Size(175, 34);
            this.txtMobile.TabIndex = 10;
            // 
            // lblOTP
            // 
            this.lblOTP.AutoSize = true;
            this.lblOTP.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblOTP.Location = new System.Drawing.Point(400, 200);
            this.lblOTP.Name = "lblOTP";
            this.lblOTP.Size = new System.Drawing.Size(41, 23);
            this.lblOTP.TabIndex = 11;
            this.lblOTP.Text = "OTP";
            // 
            // txtOTP
            // 
            this.txtOTP.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtOTP.Location = new System.Drawing.Point(450, 195);
            this.txtOTP.MaxLength = 6;
            this.txtOTP.Name = "txtOTP";
            this.txtOTP.Size = new System.Drawing.Size(80, 34);
            this.txtOTP.TabIndex = 12;
            // 
            // lblResendOTP
            // 
            this.lblResendOTP.AutoSize = true;
            this.lblResendOTP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblResendOTP.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Underline);
            this.lblResendOTP.ForeColor = System.Drawing.Color.Gray;
            this.lblResendOTP.Location = new System.Drawing.Point(540, 195);
            this.lblResendOTP.Name = "lblResendOTP";
            this.lblResendOTP.Size = new System.Drawing.Size(111, 38);
            this.lblResendOTP.TabIndex = 13;
            this.lblResendOTP.Text = "ReSend OTP (31)\r\nUpto 2 times";
            this.lblResendOTP.Click += new System.EventHandler(this.lblResendOTP_Click);
            // 
            // lblMobileNote
            // 
            this.lblMobileNote.AutoSize = true;
            this.lblMobileNote.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblMobileNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(0)))));
            this.lblMobileNote.Location = new System.Drawing.Point(40, 245);
            this.lblMobileNote.Name = "lblMobileNote";
            this.lblMobileNote.Size = new System.Drawing.Size(593, 20);
            this.lblMobileNote.TabIndex = 14;
            this.lblMobileNote.Text = "Note: This mobile number will be used for all the communications related to ABHA." +
    "";
            // 
            // timerOtp
            // 
            this.timerOtp.Interval = 1000;
            this.timerOtp.Tick += new System.EventHandler(this.timerOtp_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HMS.Properties.Resources.nhalogo;
            this.pictureBox1.Location = new System.Drawing.Point(4, 51);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(171, 63);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // frmABHA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(650, 400);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblMobileNote);
            this.Controls.Add(this.lblResendOTP);
            this.Controls.Add(this.txtOTP);
            this.Controls.Add(this.lblOTP);
            this.Controls.Add(this.txtMobile);
            this.Controls.Add(this.lblMobile);
            this.Controls.Add(this.pnlBanner);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtAadhaar3);
            this.Controls.Add(this.txtAadhaar2);
            this.Controls.Add(this.txtAadhaar1);
            this.Controls.Add(this.lblAadhaar);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmABHA";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmABHA_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlBanner.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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