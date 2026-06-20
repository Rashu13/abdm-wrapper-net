namespace HMS.abdm
{
    partial class frmABHAMain
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnBridgeConfig = new System.Windows.Forms.Button();
            this.btnTestAbdm = new System.Windows.Forms.Button();
            
            // Creation Group
            this.gbCreation = new System.Windows.Forms.GroupBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnBioEnroll = new System.Windows.Forms.Button();
            this.btnDlEnroll = new System.Windows.Forms.Button();
            this.btnCreateAddress = new System.Windows.Forms.Button();
            this.btnDownloadCard = new System.Windows.Forms.Button();

            // Verification Group
            this.gbVerification = new System.Windows.Forms.GroupBox();
            this.btnScanShare = new System.Windows.Forms.Button();
            this.btnScanUserQR = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnViewRequests = new System.Windows.Forms.Button();
            this.btnAccountOps = new System.Windows.Forms.Button();
            this.btnM2Testing = new System.Windows.Forms.Button();

            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gbCreation.SuspendLayout();
            this.gbVerification.SuspendLayout();
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
            this.pnlHeader.Size = new System.Drawing.Size(920, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(870, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(50, 50);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(434, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Ayushman Bharat Health Account (ABDM) Portal";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HMS.Properties.Resources.nhalogo;
            this.pictureBox1.Location = new System.Drawing.Point(20, 65);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 60);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // btnBridgeConfig
            // 
            this.btnBridgeConfig.BackColor = System.Drawing.Color.White;
            this.btnBridgeConfig.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnBridgeConfig.FlatAppearance.BorderSize = 2;
            this.btnBridgeConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBridgeConfig.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBridgeConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnBridgeConfig.Location = new System.Drawing.Point(700, 65);
            this.btnBridgeConfig.Name = "btnBridgeConfig";
            this.btnBridgeConfig.Size = new System.Drawing.Size(200, 50);
            this.btnBridgeConfig.TabIndex = 18;
            this.btnBridgeConfig.Text = "Gateway Configuration";
            this.btnBridgeConfig.UseVisualStyleBackColor = false;
            this.btnBridgeConfig.Click += new System.EventHandler(this.btnBridgeConfig_Click);
            // 
            // btnTestAbdm
            // 
            this.btnTestAbdm.BackColor = System.Drawing.Color.White;
            this.btnTestAbdm.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnTestAbdm.FlatAppearance.BorderSize = 2;
            this.btnTestAbdm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestAbdm.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnTestAbdm.ForeColor = System.Drawing.Color.Green;
            this.btnTestAbdm.Location = new System.Drawing.Point(260, 65);
            this.btnTestAbdm.Name = "btnTestAbdm";
            this.btnTestAbdm.Size = new System.Drawing.Size(400, 50);
            this.btnTestAbdm.TabIndex = 20;
            this.btnTestAbdm.Text = "Write Prescription (Dwai / PDF)";
            this.btnTestAbdm.UseVisualStyleBackColor = false;
            this.btnTestAbdm.Click += new System.EventHandler(this.btnTestAbdm_Click);
            // 
            // gbCreation
            // 
            this.gbCreation.Controls.Add(this.btnCreate);
            this.gbCreation.Controls.Add(this.btnBioEnroll);
            this.gbCreation.Controls.Add(this.btnDlEnroll);
            this.gbCreation.Controls.Add(this.btnCreateAddress);
            this.gbCreation.Controls.Add(this.btnDownloadCard);
            this.gbCreation.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.gbCreation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.gbCreation.Location = new System.Drawing.Point(20, 140);
            this.gbCreation.Name = "gbCreation";
            this.gbCreation.Size = new System.Drawing.Size(260, 370);
            this.gbCreation.TabIndex = 18;
            this.gbCreation.TabStop = false;
            this.gbCreation.Text = "ABHA Creation & Download";
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.White;
            this.btnCreate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnCreate.FlatAppearance.BorderSize = 2;
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCreate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnCreate.Location = new System.Drawing.Point(15, 30);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(230, 50);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Aadhaar OTP (Mandatory)";
            this.btnCreate.UseVisualStyleBackColor = false;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnBioEnroll
            // 
            this.btnBioEnroll.BackColor = System.Drawing.Color.White;
            this.btnBioEnroll.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnBioEnroll.FlatAppearance.BorderSize = 2;
            this.btnBioEnroll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBioEnroll.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBioEnroll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnBioEnroll.Location = new System.Drawing.Point(15, 95);
            this.btnBioEnroll.Name = "btnBioEnroll";
            this.btnBioEnroll.Size = new System.Drawing.Size(230, 50);
            this.btnBioEnroll.TabIndex = 2;
            this.btnBioEnroll.Text = "Aadhaar Biometrics (Opt)";
            this.btnBioEnroll.UseVisualStyleBackColor = false;
            this.btnBioEnroll.Click += new System.EventHandler(this.btnBioEnroll_Click);
            // 
            // btnDlEnroll
            // 
            this.btnDlEnroll.BackColor = System.Drawing.Color.White;
            this.btnDlEnroll.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnDlEnroll.FlatAppearance.BorderSize = 2;
            this.btnDlEnroll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDlEnroll.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDlEnroll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnDlEnroll.Location = new System.Drawing.Point(15, 160);
            this.btnDlEnroll.Name = "btnDlEnroll";
            this.btnDlEnroll.Size = new System.Drawing.Size(230, 50);
            this.btnDlEnroll.TabIndex = 3;
            this.btnDlEnroll.Text = "Driving License (Opt)";
            this.btnDlEnroll.UseVisualStyleBackColor = false;
            this.btnDlEnroll.Click += new System.EventHandler(this.btnDlEnroll_Click);
            // 
            // btnCreateAddress
            // 
            this.btnCreateAddress.BackColor = System.Drawing.Color.White;
            this.btnCreateAddress.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnCreateAddress.FlatAppearance.BorderSize = 2;
            this.btnCreateAddress.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateAddress.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCreateAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnCreateAddress.Location = new System.Drawing.Point(15, 225);
            this.btnCreateAddress.Name = "btnCreateAddress";
            this.btnCreateAddress.Size = new System.Drawing.Size(230, 50);
            this.btnCreateAddress.TabIndex = 4;
            this.btnCreateAddress.Text = "Create ABHA Address";
            this.btnCreateAddress.UseVisualStyleBackColor = false;
            this.btnCreateAddress.Click += new System.EventHandler(this.btnCreateAddress_Click);
            // 
            // btnDownloadCard
            // 
            this.btnDownloadCard.BackColor = System.Drawing.Color.White;
            this.btnDownloadCard.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnDownloadCard.FlatAppearance.BorderSize = 2;
            this.btnDownloadCard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownloadCard.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDownloadCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnDownloadCard.Location = new System.Drawing.Point(15, 290);
            this.btnDownloadCard.Name = "btnDownloadCard";
            this.btnDownloadCard.Size = new System.Drawing.Size(230, 50);
            this.btnDownloadCard.TabIndex = 5;
            this.btnDownloadCard.Text = "Download ABHA Card";
            this.btnDownloadCard.UseVisualStyleBackColor = false;
            this.btnDownloadCard.Click += new System.EventHandler(this.btnDownloadCard_Click);
            // 
            // gbVerification
            // 
            this.gbVerification.Controls.Add(this.btnScanShare);
            this.gbVerification.Controls.Add(this.btnScanUserQR);
            this.gbVerification.Controls.Add(this.btnLogin);
            this.gbVerification.Controls.Add(this.btnViewRequests);
            this.gbVerification.Controls.Add(this.btnAccountOps);
            this.gbVerification.Controls.Add(this.btnM2Testing);
            this.gbVerification.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.gbVerification.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.gbVerification.Location = new System.Drawing.Point(300, 140);
            this.gbVerification.Name = "gbVerification";
            this.gbVerification.Size = new System.Drawing.Size(600, 370);
            this.gbVerification.TabIndex = 19;
            this.gbVerification.TabStop = false;
            this.gbVerification.Text = "ABHA Address Verification & Management";
            // 
            // btnScanShare
            // 
            this.btnScanShare.BackColor = System.Drawing.Color.White;
            this.btnScanShare.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnScanShare.FlatAppearance.BorderSize = 2;
            this.btnScanShare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScanShare.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnScanShare.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnScanShare.Location = new System.Drawing.Point(20, 35);
            this.btnScanShare.Name = "btnScanShare";
            this.btnScanShare.Size = new System.Drawing.Size(265, 95);
            this.btnScanShare.TabIndex = 6;
            this.btnScanShare.Text = "Facility QR Code\r\n(Scan & Share)";
            this.btnScanShare.UseVisualStyleBackColor = false;
            this.btnScanShare.Click += new System.EventHandler(this.btnScanShare_Click);
            // 
            // btnScanUserQR
            // 
            this.btnScanUserQR.BackColor = System.Drawing.Color.White;
            this.btnScanUserQR.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnScanUserQR.FlatAppearance.BorderSize = 2;
            this.btnScanUserQR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScanUserQR.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnScanUserQR.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnScanUserQR.Location = new System.Drawing.Point(305, 35);
            this.btnScanUserQR.Name = "btnScanUserQR";
            this.btnScanUserQR.Size = new System.Drawing.Size(275, 95);
            this.btnScanUserQR.TabIndex = 7;
            this.btnScanUserQR.Text = "Scan Patient Card QR\r\n(Optional)";
            this.btnScanUserQR.UseVisualStyleBackColor = false;
            this.btnScanUserQR.Click += new System.EventHandler(this.btnScanUserQR_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.White;
            this.btnLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnLogin.FlatAppearance.BorderSize = 2;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnLogin.Location = new System.Drawing.Point(20, 145);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(265, 95);
            this.btnLogin.TabIndex = 8;
            this.btnLogin.Text = "Verify by OTP\r\n(ABHA Login)";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnViewRequests
            // 
            this.btnViewRequests.BackColor = System.Drawing.Color.White;
            this.btnViewRequests.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnViewRequests.FlatAppearance.BorderSize = 2;
            this.btnViewRequests.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewRequests.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnViewRequests.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnViewRequests.Location = new System.Drawing.Point(305, 145);
            this.btnViewRequests.Name = "btnViewRequests";
            this.btnViewRequests.Size = new System.Drawing.Size(275, 95);
            this.btnViewRequests.TabIndex = 9;
            this.btnViewRequests.Text = "View Shared Profiles\r\n(Scan & Share List)";
            this.btnViewRequests.UseVisualStyleBackColor = false;
            this.btnViewRequests.Click += new System.EventHandler(this.btnViewRequests_Click);
            // 
            // btnAccountOps
            // 
            this.btnAccountOps.BackColor = System.Drawing.Color.White;
            this.btnAccountOps.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnAccountOps.FlatAppearance.BorderSize = 2;
            this.btnAccountOps.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAccountOps.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnAccountOps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnAccountOps.Location = new System.Drawing.Point(20, 255);
            this.btnAccountOps.Name = "btnAccountOps";
            this.btnAccountOps.Size = new System.Drawing.Size(265, 95);
            this.btnAccountOps.TabIndex = 10;
            this.btnAccountOps.Text = "ABHA Account Operations\r\n(Profile Update / Re-KYC)";
            this.btnAccountOps.UseVisualStyleBackColor = false;
            this.btnAccountOps.Click += new System.EventHandler(this.btnAccountOps_Click);
            // 
            // btnM2Testing
            // 
            this.btnM2Testing.BackColor = System.Drawing.Color.White;
            this.btnM2Testing.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnM2Testing.FlatAppearance.BorderSize = 2;
            this.btnM2Testing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnM2Testing.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnM2Testing.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnM2Testing.Location = new System.Drawing.Point(305, 255);
            this.btnM2Testing.Name = "btnM2Testing";
            this.btnM2Testing.Size = new System.Drawing.Size(275, 95);
            this.btnM2Testing.TabIndex = 11;
            this.btnM2Testing.Text = "Milestone 2 (M2)\r\nConsent & Health Data";
            this.btnM2Testing.UseVisualStyleBackColor = false;
            this.btnM2Testing.Click += new System.EventHandler(this.btnM2Testing_Click);
            // 
            // frmABHAMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(920, 530);
            this.Controls.Add(this.gbVerification);
            this.Controls.Add(this.gbCreation);
            this.Controls.Add(this.btnBridgeConfig);
            this.Controls.Add(this.btnTestAbdm);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmABHAMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ABHA Portal Dashboard";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gbCreation.ResumeLayout(false);
            this.gbVerification.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnBridgeConfig;
        private System.Windows.Forms.Button btnTestAbdm;
        
        private System.Windows.Forms.GroupBox gbCreation;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnBioEnroll;
        private System.Windows.Forms.Button btnDlEnroll;
        private System.Windows.Forms.Button btnCreateAddress;
        private System.Windows.Forms.Button btnDownloadCard;

        private System.Windows.Forms.GroupBox gbVerification;
        private System.Windows.Forms.Button btnScanShare;
        private System.Windows.Forms.Button btnScanUserQR;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnViewRequests;
        private System.Windows.Forms.Button btnAccountOps;
        private System.Windows.Forms.Button btnM2Testing;
    }
}
