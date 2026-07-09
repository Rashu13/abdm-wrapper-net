using System.Windows.Forms;

namespace HMS.abdm
{
    partial class frmAbdmM1Dashboard
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
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.grpProfile = new System.Windows.Forms.GroupBox();
            this.grpCardPreview = new System.Windows.Forms.GroupBox();
            this.btnCreateAbha = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnReKyc = new System.Windows.Forms.Button();
            this.btnScanShare = new System.Windows.Forms.Button();
            this.btnClearSession = new System.Windows.Forms.Button();
            this.btnM2Dashboard = new System.Windows.Forms.Button();
            this.btnM3Dashboard = new System.Windows.Forms.Button();
            this.btnCreateRecord = new System.Windows.Forms.Button();
            this.btnBridgeConfig = new System.Windows.Forms.Button();
            this.btnAbhaPortal = new System.Windows.Forms.Button();
            this.btnDownloadCard = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblNameTitle = new System.Windows.Forms.Label();
            this.lblNameVal = new System.Windows.Forms.Label();
            this.lblAbhaAddrTitle = new System.Windows.Forms.Label();
            this.lblAbhaAddrVal = new System.Windows.Forms.Label();
            this.lblAbhaNumTitle = new System.Windows.Forms.Label();
            this.lblAbhaNumVal = new System.Windows.Forms.Label();
            this.lblGenderTitle = new System.Windows.Forms.Label();
            this.lblGenderVal = new System.Windows.Forms.Label();
            this.lblDobTitle = new System.Windows.Forms.Label();
            this.lblDobVal = new System.Windows.Forms.Label();
            this.lblMobileTitle = new System.Windows.Forms.Label();
            this.lblMobileVal = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.picPhoto = new System.Windows.Forms.PictureBox();
            this.picCard = new System.Windows.Forms.PictureBox();
            this.grpActions.SuspendLayout();
            this.grpProfile.SuspendLayout();
            this.grpCardPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCard)).BeginInit();
            this.SuspendLayout();
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnCreateAbha);
            this.grpActions.Controls.Add(this.btnLogin);
            this.grpActions.Controls.Add(this.btnScanShare);
            this.grpActions.Controls.Add(this.btnReKyc);
            this.grpActions.Controls.Add(this.btnClearSession);
            this.grpActions.Controls.Add(this.btnM2Dashboard);
            this.grpActions.Controls.Add(this.btnM3Dashboard);
            this.grpActions.Controls.Add(this.btnCreateRecord);
            this.grpActions.Controls.Add(this.btnBridgeConfig);
            this.grpActions.Controls.Add(this.btnAbhaPortal);
            this.grpActions.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.grpActions.Location = new System.Drawing.Point(15, 15);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(250, 640);
            this.grpActions.TabIndex = 0;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "ABHA Actions (M1)";
            this.grpActions.BackColor = System.Drawing.Color.White;
            // 
            // btnCreateAbha
            // 
            this.btnCreateAbha.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnCreateAbha.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateAbha.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular);
            this.btnCreateAbha.ForeColor = System.Drawing.Color.White;
            this.btnCreateAbha.Location = new System.Drawing.Point(20, 35);
            this.btnCreateAbha.Name = "btnCreateAbha";
            this.btnCreateAbha.Size = new System.Drawing.Size(210, 45);
            this.btnCreateAbha.TabIndex = 0;
            this.btnCreateAbha.Text = "Create New ABHA";
            this.btnCreateAbha.UseVisualStyleBackColor = false;
            this.btnCreateAbha.Click += new System.EventHandler(this.btnCreateAbha_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(20, 95);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(210, 45);
            this.btnLogin.TabIndex = 1;
            this.btnLogin.Text = "Login / Verify ABHA";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnScanShare
            // 
            this.btnScanShare.BackColor = System.Drawing.Color.FromArgb(142, 68, 173);
            this.btnScanShare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScanShare.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular);
            this.btnScanShare.ForeColor = System.Drawing.Color.White;
            this.btnScanShare.Location = new System.Drawing.Point(20, 155);
            this.btnScanShare.Name = "btnScanShare";
            this.btnScanShare.Size = new System.Drawing.Size(210, 45);
            this.btnScanShare.TabIndex = 2;
            this.btnScanShare.Text = "Scan & Share Requests";
            this.btnScanShare.UseVisualStyleBackColor = false;
            this.btnScanShare.Click += new System.EventHandler(this.btnScanShare_Click);
            // 
            // btnReKyc
            // 
            this.btnReKyc.BackColor = System.Drawing.Color.FromArgb(230, 126, 34);
            this.btnReKyc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReKyc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular);
            this.btnReKyc.ForeColor = System.Drawing.Color.White;
            this.btnReKyc.Location = new System.Drawing.Point(20, 215);
            this.btnReKyc.Name = "btnReKyc";
            this.btnReKyc.Size = new System.Drawing.Size(210, 45);
            this.btnReKyc.TabIndex = 3;
            this.btnReKyc.Text = "Verify Re-KYC";
            this.btnReKyc.UseVisualStyleBackColor = false;
            this.btnReKyc.Click += new System.EventHandler(this.btnReKyc_Click);
            // 
            // btnClearSession
            // 
            this.btnClearSession.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnClearSession.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSession.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular);
            this.btnClearSession.ForeColor = System.Drawing.Color.White;
            this.btnClearSession.Location = new System.Drawing.Point(20, 275);
            this.btnClearSession.Name = "btnClearSession";
            this.btnClearSession.Size = new System.Drawing.Size(210, 45);
            this.btnClearSession.TabIndex = 4;
            this.btnClearSession.Text = "Clear Session / Logout";
            this.btnClearSession.UseVisualStyleBackColor = false;
            this.btnClearSession.Click += new System.EventHandler(this.btnClearSession_Click);
            // 
            // btnM2Dashboard
            // 
            this.btnM2Dashboard.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.btnM2Dashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnM2Dashboard.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnM2Dashboard.ForeColor = System.Drawing.Color.White;
            this.btnM2Dashboard.Location = new System.Drawing.Point(20, 335);
            this.btnM2Dashboard.Name = "btnM2Dashboard";
            this.btnM2Dashboard.Size = new System.Drawing.Size(210, 45);
            this.btnM2Dashboard.TabIndex = 5;
            this.btnM2Dashboard.Text = "ABDM M2 Dashboard";
            this.btnM2Dashboard.UseVisualStyleBackColor = false;
            this.btnM2Dashboard.Click += new System.EventHandler(this.btnM2Dashboard_Click);
            // 
            // btnM3Dashboard
            // 
            this.btnM3Dashboard.BackColor = System.Drawing.Color.FromArgb(41, 128, 185);
            this.btnM3Dashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnM3Dashboard.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnM3Dashboard.ForeColor = System.Drawing.Color.White;
            this.btnM3Dashboard.Location = new System.Drawing.Point(20, 395);
            this.btnM3Dashboard.Name = "btnM3Dashboard";
            this.btnM3Dashboard.Size = new System.Drawing.Size(210, 45);
            this.btnM3Dashboard.TabIndex = 6;
            this.btnM3Dashboard.Text = "ABDM M3 Dashboard";
            this.btnM3Dashboard.UseVisualStyleBackColor = false;
            this.btnM3Dashboard.Click += new System.EventHandler(this.btnM3Dashboard_Click);
            // 
            // btnCreateRecord
            // 
            this.btnCreateRecord.BackColor = System.Drawing.Color.FromArgb(230, 126, 34);
            this.btnCreateRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateRecord.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnCreateRecord.ForeColor = System.Drawing.Color.White;
            this.btnCreateRecord.Location = new System.Drawing.Point(20, 455);
            this.btnCreateRecord.Name = "btnCreateRecord";
            this.btnCreateRecord.Size = new System.Drawing.Size(210, 45);
            this.btnCreateRecord.TabIndex = 7;
            this.btnCreateRecord.Text = "+ Create Health Record";
            this.btnCreateRecord.UseVisualStyleBackColor = false;
            this.btnCreateRecord.Click += new System.EventHandler(this.btnCreateRecord_Click);
            // 
            // btnBridgeConfig
            // 
            this.btnBridgeConfig.BackColor = System.Drawing.Color.FromArgb(22, 160, 133);
            this.btnBridgeConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBridgeConfig.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnBridgeConfig.ForeColor = System.Drawing.Color.White;
            this.btnBridgeConfig.Location = new System.Drawing.Point(20, 515);
            this.btnBridgeConfig.Name = "btnBridgeConfig";
            this.btnBridgeConfig.Size = new System.Drawing.Size(210, 45);
            this.btnBridgeConfig.TabIndex = 8;
            this.btnBridgeConfig.Text = "Register Webhook / Bridge";
            this.btnBridgeConfig.UseVisualStyleBackColor = false;
            this.btnBridgeConfig.Click += new System.EventHandler(this.btnBridgeConfig_Click);
            // 
            // btnAbhaPortal
            // 
            this.btnAbhaPortal.BackColor = System.Drawing.Color.FromArgb(142, 68, 173);
            this.btnAbhaPortal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbhaPortal.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnAbhaPortal.ForeColor = System.Drawing.Color.White;
            this.btnAbhaPortal.Location = new System.Drawing.Point(20, 575);
            this.btnAbhaPortal.Name = "btnAbhaPortal";
            this.btnAbhaPortal.Size = new System.Drawing.Size(210, 45);
            this.btnAbhaPortal.TabIndex = 9;
            this.btnAbhaPortal.Text = "ABHA Portal Dashboard";
            this.btnAbhaPortal.UseVisualStyleBackColor = false;
            this.btnAbhaPortal.Click += new System.EventHandler(this.btnAbhaPortal_Click);
            // 
            // grpProfile
            // 
            this.grpProfile.Controls.Add(this.picPhoto);
            this.grpProfile.Controls.Add(this.lblNameTitle);
            this.grpProfile.Controls.Add(this.lblNameVal);
            this.grpProfile.Controls.Add(this.lblAbhaAddrTitle);
            this.grpProfile.Controls.Add(this.lblAbhaAddrVal);
            this.grpProfile.Controls.Add(this.lblAbhaNumTitle);
            this.grpProfile.Controls.Add(this.lblAbhaNumVal);
            this.grpProfile.Controls.Add(this.lblGenderTitle);
            this.grpProfile.Controls.Add(this.lblGenderVal);
            this.grpProfile.Controls.Add(this.lblDobTitle);
            this.grpProfile.Controls.Add(this.lblDobVal);
            this.grpProfile.Controls.Add(this.lblMobileTitle);
            this.grpProfile.Controls.Add(this.lblMobileVal);
            this.grpProfile.Controls.Add(this.btnDownloadCard);
            this.grpProfile.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.grpProfile.Location = new System.Drawing.Point(280, 15);
            this.grpProfile.Name = "grpProfile";
            this.grpProfile.Size = new System.Drawing.Size(380, 640);
            this.grpProfile.TabIndex = 1;
            this.grpProfile.TabStop = false;
            this.grpProfile.Text = "Active Patient Profile";
            this.grpProfile.BackColor = System.Drawing.Color.White;
            // 
            // picPhoto
            // 
            this.picPhoto.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.picPhoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPhoto.Location = new System.Drawing.Point(20, 35);
            this.picPhoto.Name = "picPhoto";
            this.picPhoto.Size = new System.Drawing.Size(110, 130);
            this.picPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPhoto.TabIndex = 0;
            this.picPhoto.TabStop = false;
            // 
            // lblNameTitle
            // 
            this.lblNameTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNameTitle.Location = new System.Drawing.Point(145, 35);
            this.lblNameTitle.Name = "lblNameTitle";
            this.lblNameTitle.Size = new System.Drawing.Size(50, 22);
            this.lblNameTitle.TabIndex = 1;
            this.lblNameTitle.Text = "Name:";
            // 
            // lblNameVal
            // 
            this.lblNameVal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblNameVal.Location = new System.Drawing.Point(200, 35);
            this.lblNameVal.Name = "lblNameVal";
            this.lblNameVal.Size = new System.Drawing.Size(170, 22);
            this.lblNameVal.TabIndex = 2;
            // 
            // lblAbhaAddrTitle
            // 
            this.lblAbhaAddrTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAbhaAddrTitle.Location = new System.Drawing.Point(145, 65);
            this.lblAbhaAddrTitle.Name = "lblAbhaAddrTitle";
            this.lblAbhaAddrTitle.Size = new System.Drawing.Size(55, 22);
            this.lblAbhaAddrTitle.TabIndex = 3;
            this.lblAbhaAddrTitle.Text = "ABHA ID:";
            // 
            // lblAbhaAddrVal
            // 
            this.lblAbhaAddrVal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblAbhaAddrVal.Location = new System.Drawing.Point(200, 65);
            this.lblAbhaAddrVal.Name = "lblAbhaAddrVal";
            this.lblAbhaAddrVal.Size = new System.Drawing.Size(170, 22);
            this.lblAbhaAddrVal.TabIndex = 4;
            // 
            // lblAbhaNumTitle
            // 
            this.lblAbhaNumTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAbhaNumTitle.Location = new System.Drawing.Point(145, 95);
            this.lblAbhaNumTitle.Name = "lblAbhaNumTitle";
            this.lblAbhaNumTitle.Size = new System.Drawing.Size(60, 22);
            this.lblAbhaNumTitle.TabIndex = 5;
            this.lblAbhaNumTitle.Text = "ABHA No:";
            // 
            // lblAbhaNumVal
            // 
            this.lblAbhaNumVal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblAbhaNumVal.Location = new System.Drawing.Point(205, 95);
            this.lblAbhaNumVal.Name = "lblAbhaNumVal";
            this.lblAbhaNumVal.Size = new System.Drawing.Size(165, 22);
            this.lblAbhaNumVal.TabIndex = 6;
            // 
            // lblGenderTitle
            // 
            this.lblGenderTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblGenderTitle.Location = new System.Drawing.Point(145, 125);
            this.lblGenderTitle.Name = "lblGenderTitle";
            this.lblGenderTitle.Size = new System.Drawing.Size(55, 22);
            this.lblGenderTitle.TabIndex = 7;
            this.lblGenderTitle.Text = "Gender:";
            // 
            // lblGenderVal
            // 
            this.lblGenderVal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblGenderVal.Location = new System.Drawing.Point(200, 125);
            this.lblGenderVal.Name = "lblGenderVal";
            this.lblGenderVal.Size = new System.Drawing.Size(170, 22);
            this.lblGenderVal.TabIndex = 8;
            // 
            // lblDobTitle
            // 
            this.lblDobTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDobTitle.Location = new System.Drawing.Point(20, 175);
            this.lblDobTitle.Name = "lblDobTitle";
            this.lblDobTitle.Size = new System.Drawing.Size(90, 22);
            this.lblDobTitle.TabIndex = 9;
            this.lblDobTitle.Text = "Date of Birth:";
            // 
            // lblDobVal
            // 
            this.lblDobVal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblDobVal.Location = new System.Drawing.Point(120, 175);
            this.lblDobVal.Name = "lblDobVal";
            this.lblDobVal.Size = new System.Drawing.Size(240, 22);
            this.lblDobVal.TabIndex = 10;
            // 
            // lblMobileTitle
            // 
            this.lblMobileTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMobileTitle.Location = new System.Drawing.Point(20, 205);
            this.lblMobileTitle.Name = "lblMobileTitle";
            this.lblMobileTitle.Size = new System.Drawing.Size(95, 22);
            this.lblMobileTitle.TabIndex = 11;
            this.lblMobileTitle.Text = "Mobile Number:";
            // 
            // lblMobileVal
            // 
            this.lblMobileVal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblMobileVal.Location = new System.Drawing.Point(120, 205);
            this.lblMobileVal.Name = "lblMobileVal";
            this.lblMobileVal.Size = new System.Drawing.Size(240, 22);
            this.lblMobileVal.TabIndex = 12;
            // 
            // btnDownloadCard
            // 
            this.btnDownloadCard.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnDownloadCard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownloadCard.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnDownloadCard.ForeColor = System.Drawing.Color.White;
            this.btnDownloadCard.Location = new System.Drawing.Point(20, 380);
            this.btnDownloadCard.Name = "btnDownloadCard";
            this.btnDownloadCard.Size = new System.Drawing.Size(340, 45);
            this.btnDownloadCard.TabIndex = 13;
            this.btnDownloadCard.Text = "Download & Preview ABHA Card";
            this.btnDownloadCard.UseVisualStyleBackColor = false;
            this.btnDownloadCard.Click += new System.EventHandler(this.btnDownloadCard_Click);
            // 
            // grpCardPreview
            // 
            this.grpCardPreview.Controls.Add(this.picCard);
            this.grpCardPreview.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.grpCardPreview.Location = new System.Drawing.Point(675, 15);
            this.grpCardPreview.Name = "grpCardPreview";
            this.grpCardPreview.Size = new System.Drawing.Size(245, 640);
            this.grpCardPreview.TabIndex = 2;
            this.grpCardPreview.TabStop = false;
            this.grpCardPreview.Text = "ABHA Physical Card Preview";
            this.grpCardPreview.BackColor = System.Drawing.Color.White;
            // 
            // picCard
            // 
            this.picCard.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.picCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picCard.Location = new System.Drawing.Point(15, 35);
            this.picCard.Name = "picCard";
            this.picCard.Size = new System.Drawing.Size(215, 390);
            this.picCard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCard.TabIndex = 0;
            this.picCard.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(127, 140, 141);
            this.lblStatus.Location = new System.Drawing.Point(15, 675);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(740, 30);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Loading Session Info...";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(770, 670);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(150, 40);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close Dashboard";
            this.btnClose.UseVisualStyleBackColor = false;
            // 
            // frmAbdmM1Dashboard
            // 
            this.BackColor = System.Drawing.Color.FromArgb(245, 246, 250);
            this.ClientSize = new System.Drawing.Size(950, 730);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.grpCardPreview);
            this.Controls.Add(this.grpProfile);
            this.Controls.Add(this.grpActions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmAbdmM1Dashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ABDM Milestone 1 (M1) Integration Dashboard";
            this.grpActions.ResumeLayout(false);
            this.grpProfile.ResumeLayout(false);
            this.grpCardPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCard)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.GroupBox grpProfile;
        private System.Windows.Forms.GroupBox grpCardPreview;
        private System.Windows.Forms.Button btnCreateAbha;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnReKyc;
        private System.Windows.Forms.Button btnScanShare;
        private System.Windows.Forms.Button btnClearSession;
        private System.Windows.Forms.Button btnM2Dashboard;
        private System.Windows.Forms.Button btnM3Dashboard;
        private System.Windows.Forms.Button btnCreateRecord;
        private System.Windows.Forms.Button btnBridgeConfig;
        private System.Windows.Forms.Button btnAbhaPortal;
        private System.Windows.Forms.Button btnDownloadCard;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblNameTitle;
        private System.Windows.Forms.Label lblNameVal;
        private System.Windows.Forms.Label lblAbhaAddrTitle;
        private System.Windows.Forms.Label lblAbhaAddrVal;
        private System.Windows.Forms.Label lblAbhaNumTitle;
        private System.Windows.Forms.Label lblAbhaNumVal;
        private System.Windows.Forms.Label lblGenderTitle;
        private System.Windows.Forms.Label lblGenderVal;
        private System.Windows.Forms.Label lblDobTitle;
        private System.Windows.Forms.Label lblDobVal;
        private System.Windows.Forms.Label lblMobileTitle;
        private System.Windows.Forms.Label lblMobileVal;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox picPhoto;
        private System.Windows.Forms.PictureBox picCard;
    }
}
