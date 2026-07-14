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
            grpActions = new GroupBox();
            btnCreateAbha = new Button();
            btnLogin = new Button();
            btnScanShare = new Button();
            btnReKyc = new Button();
            btnClearSession = new Button();
            btnM2Dashboard = new Button();
            btnM3Dashboard = new Button();
            btnCreateRecord = new Button();
            btnBridgeConfig = new Button();
            btnAbhaPortal = new Button();
            grpProfile = new GroupBox();
            picPhoto = new PictureBox();
            lblNameTitle = new Label();
            lblNameVal = new Label();
            lblAbhaAddrTitle = new Label();
            lblAbhaAddrVal = new Label();
            lblAbhaNumTitle = new Label();
            lblAbhaNumVal = new Label();
            lblGenderTitle = new Label();
            lblGenderVal = new Label();
            lblDobTitle = new Label();
            lblDobVal = new Label();
            lblMobileTitle = new Label();
            lblMobileVal = new Label();
            btnDownloadCard = new Button();
            grpCardPreview = new GroupBox();
            picCard = new PictureBox();
            btnClose = new Button();
            lblStatus = new Label();
            grpActions.SuspendLayout();
            grpProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPhoto).BeginInit();
            grpCardPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCard).BeginInit();
            SuspendLayout();
            // 
            // grpActions
            // 
            grpActions.BackColor = Color.White;
            grpActions.Controls.Add(btnCreateAbha);
            grpActions.Controls.Add(btnLogin);
            grpActions.Controls.Add(btnScanShare);
            grpActions.Controls.Add(btnReKyc);
            grpActions.Controls.Add(btnClearSession);
            grpActions.Controls.Add(btnM2Dashboard);
            grpActions.Controls.Add(btnM3Dashboard);
            grpActions.Controls.Add(btnCreateRecord);
            grpActions.Controls.Add(btnBridgeConfig);
            grpActions.Controls.Add(btnAbhaPortal);
            grpActions.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            grpActions.Location = new Point(15, 15);
            grpActions.Name = "grpActions";
            grpActions.Size = new Size(250, 640);
            grpActions.TabIndex = 0;
            grpActions.TabStop = false;
            grpActions.Text = "ABHA Actions (M1)";
            // 
            // btnCreateAbha
            // 
            btnCreateAbha.BackColor = Color.FromArgb(41, 128, 185);
            btnCreateAbha.FlatStyle = FlatStyle.Flat;
            btnCreateAbha.Font = new Font("Segoe UI", 9.75F);
            btnCreateAbha.ForeColor = Color.White;
            btnCreateAbha.Location = new Point(20, 35);
            btnCreateAbha.Name = "btnCreateAbha";
            btnCreateAbha.Size = new Size(210, 45);
            btnCreateAbha.TabIndex = 0;
            btnCreateAbha.Text = "Create New ABHA";
            btnCreateAbha.UseVisualStyleBackColor = false;
            btnCreateAbha.Click += btnCreateAbha_Click;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(39, 174, 96);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 9.75F);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(20, 95);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(210, 45);
            btnLogin.TabIndex = 1;
            btnLogin.Text = "Login / Verify ABHA";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // btnScanShare
            // 
            btnScanShare.BackColor = Color.FromArgb(142, 68, 173);
            btnScanShare.FlatStyle = FlatStyle.Flat;
            btnScanShare.Font = new Font("Segoe UI", 9.75F);
            btnScanShare.ForeColor = Color.White;
            btnScanShare.Location = new Point(20, 155);
            btnScanShare.Name = "btnScanShare";
            btnScanShare.Size = new Size(210, 45);
            btnScanShare.TabIndex = 2;
            btnScanShare.Text = "Scan & Share Requests";
            btnScanShare.UseVisualStyleBackColor = false;
            btnScanShare.Click += btnScanShare_Click;
            // 
            // btnReKyc
            // 
            btnReKyc.BackColor = Color.FromArgb(230, 126, 34);
            btnReKyc.FlatStyle = FlatStyle.Flat;
            btnReKyc.Font = new Font("Segoe UI", 9.75F);
            btnReKyc.ForeColor = Color.White;
            btnReKyc.Location = new Point(20, 215);
            btnReKyc.Name = "btnReKyc";
            btnReKyc.Size = new Size(210, 45);
            btnReKyc.TabIndex = 3;
            btnReKyc.Text = "Verify Re-KYC";
            btnReKyc.UseVisualStyleBackColor = false;
            btnReKyc.Click += btnReKyc_Click;
            // 
            // btnClearSession
            // 
            btnClearSession.BackColor = Color.FromArgb(192, 57, 43);
            btnClearSession.FlatStyle = FlatStyle.Flat;
            btnClearSession.Font = new Font("Segoe UI", 9.75F);
            btnClearSession.ForeColor = Color.White;
            btnClearSession.Location = new Point(20, 275);
            btnClearSession.Name = "btnClearSession";
            btnClearSession.Size = new Size(210, 45);
            btnClearSession.TabIndex = 4;
            btnClearSession.Text = "Clear Session / Logout";
            btnClearSession.UseVisualStyleBackColor = false;
            btnClearSession.Click += btnClearSession_Click;
            // 
            // btnM2Dashboard
            // 
            btnM2Dashboard.BackColor = Color.FromArgb(52, 73, 94);
            btnM2Dashboard.FlatStyle = FlatStyle.Flat;
            btnM2Dashboard.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnM2Dashboard.ForeColor = Color.White;
            btnM2Dashboard.Location = new Point(20, 335);
            btnM2Dashboard.Name = "btnM2Dashboard";
            btnM2Dashboard.Size = new Size(210, 45);
            btnM2Dashboard.TabIndex = 5;
            btnM2Dashboard.Text = "ABDM M2 Dashboard";
            btnM2Dashboard.UseVisualStyleBackColor = false;
            btnM2Dashboard.Click += btnM2Dashboard_Click;
            // 
            // btnM3Dashboard
            // 
            btnM3Dashboard.BackColor = Color.FromArgb(41, 128, 185);
            btnM3Dashboard.FlatStyle = FlatStyle.Flat;
            btnM3Dashboard.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnM3Dashboard.ForeColor = Color.White;
            btnM3Dashboard.Location = new Point(20, 395);
            btnM3Dashboard.Name = "btnM3Dashboard";
            btnM3Dashboard.Size = new Size(210, 45);
            btnM3Dashboard.TabIndex = 6;
            btnM3Dashboard.Text = "ABDM M3 Dashboard";
            btnM3Dashboard.UseVisualStyleBackColor = false;
            btnM3Dashboard.Click += btnM3Dashboard_Click;
            // 
            // btnCreateRecord
            // 
            btnCreateRecord.BackColor = Color.FromArgb(230, 126, 34);
            btnCreateRecord.FlatStyle = FlatStyle.Flat;
            btnCreateRecord.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnCreateRecord.ForeColor = Color.White;
            btnCreateRecord.Location = new Point(20, 455);
            btnCreateRecord.Name = "btnCreateRecord";
            btnCreateRecord.Size = new Size(210, 45);
            btnCreateRecord.TabIndex = 7;
            btnCreateRecord.Text = "+ Create Health Record";
            btnCreateRecord.UseVisualStyleBackColor = false;
            btnCreateRecord.Click += btnCreateRecord_Click;
            // 
            // btnBridgeConfig
            // 
            btnBridgeConfig.BackColor = Color.FromArgb(22, 160, 133);
            btnBridgeConfig.FlatStyle = FlatStyle.Flat;
            btnBridgeConfig.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnBridgeConfig.ForeColor = Color.White;
            btnBridgeConfig.Location = new Point(20, 515);
            btnBridgeConfig.Name = "btnBridgeConfig";
            btnBridgeConfig.Size = new Size(210, 45);
            btnBridgeConfig.TabIndex = 8;
            btnBridgeConfig.Text = "Register Webhook / Bridge";
            btnBridgeConfig.UseVisualStyleBackColor = false;
            btnBridgeConfig.Click += btnBridgeConfig_Click;
            // 
            // btnAbhaPortal
            // 
            btnAbhaPortal.BackColor = Color.FromArgb(142, 68, 173);
            btnAbhaPortal.FlatStyle = FlatStyle.Flat;
            btnAbhaPortal.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnAbhaPortal.ForeColor = Color.White;
            btnAbhaPortal.Location = new Point(20, 575);
            btnAbhaPortal.Name = "btnAbhaPortal";
            btnAbhaPortal.Size = new Size(210, 45);
            btnAbhaPortal.TabIndex = 9;
            btnAbhaPortal.Text = "ABHA Portal Dashboard";
            btnAbhaPortal.UseVisualStyleBackColor = false;
            btnAbhaPortal.Click += btnAbhaPortal_Click;
            // 
            // grpProfile
            // 
            grpProfile.BackColor = Color.White;
            grpProfile.Controls.Add(picPhoto);
            grpProfile.Controls.Add(lblNameTitle);
            grpProfile.Controls.Add(lblNameVal);
            grpProfile.Controls.Add(lblAbhaAddrTitle);
            grpProfile.Controls.Add(lblAbhaAddrVal);
            grpProfile.Controls.Add(lblAbhaNumTitle);
            grpProfile.Controls.Add(lblAbhaNumVal);
            grpProfile.Controls.Add(lblGenderTitle);
            grpProfile.Controls.Add(lblGenderVal);
            grpProfile.Controls.Add(lblDobTitle);
            grpProfile.Controls.Add(lblDobVal);
            grpProfile.Controls.Add(lblMobileTitle);
            grpProfile.Controls.Add(lblMobileVal);
            grpProfile.Controls.Add(btnDownloadCard);
            grpProfile.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            grpProfile.Location = new Point(280, 15);
            grpProfile.Name = "grpProfile";
            grpProfile.Size = new Size(380, 640);
            grpProfile.TabIndex = 1;
            grpProfile.TabStop = false;
            grpProfile.Text = "Active Patient Profile";
            // 
            // picPhoto
            // 
            picPhoto.BackColor = Color.FromArgb(236, 240, 241);
            picPhoto.BorderStyle = BorderStyle.FixedSingle;
            picPhoto.Location = new Point(20, 35);
            picPhoto.Name = "picPhoto";
            picPhoto.Size = new Size(110, 130);
            picPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            picPhoto.TabIndex = 0;
            picPhoto.TabStop = false;
            // 
            // lblNameTitle
            // 
            lblNameTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNameTitle.Location = new Point(145, 35);
            lblNameTitle.Name = "lblNameTitle";
            lblNameTitle.Size = new Size(50, 22);
            lblNameTitle.TabIndex = 1;
            lblNameTitle.Text = "Name:";
            // 
            // lblNameVal
            // 
            lblNameVal.Font = new Font("Segoe UI", 9F);
            lblNameVal.Location = new Point(200, 35);
            lblNameVal.Name = "lblNameVal";
            lblNameVal.Size = new Size(170, 22);
            lblNameVal.TabIndex = 2;
            // 
            // lblAbhaAddrTitle
            // 
            lblAbhaAddrTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAbhaAddrTitle.Location = new Point(145, 65);
            lblAbhaAddrTitle.Name = "lblAbhaAddrTitle";
            lblAbhaAddrTitle.Size = new Size(55, 22);
            lblAbhaAddrTitle.TabIndex = 3;
            lblAbhaAddrTitle.Text = "ABHA ID:";
            // 
            // lblAbhaAddrVal
            // 
            lblAbhaAddrVal.Font = new Font("Segoe UI", 9F);
            lblAbhaAddrVal.Location = new Point(200, 65);
            lblAbhaAddrVal.Name = "lblAbhaAddrVal";
            lblAbhaAddrVal.Size = new Size(170, 22);
            lblAbhaAddrVal.TabIndex = 4;
            // 
            // lblAbhaNumTitle
            // 
            lblAbhaNumTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAbhaNumTitle.Location = new Point(145, 95);
            lblAbhaNumTitle.Name = "lblAbhaNumTitle";
            lblAbhaNumTitle.Size = new Size(60, 22);
            lblAbhaNumTitle.TabIndex = 5;
            lblAbhaNumTitle.Text = "ABHA No:";
            // 
            // lblAbhaNumVal
            // 
            lblAbhaNumVal.Font = new Font("Segoe UI", 9F);
            lblAbhaNumVal.Location = new Point(205, 95);
            lblAbhaNumVal.Name = "lblAbhaNumVal";
            lblAbhaNumVal.Size = new Size(165, 22);
            lblAbhaNumVal.TabIndex = 6;
            // 
            // lblGenderTitle
            // 
            lblGenderTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblGenderTitle.Location = new Point(145, 125);
            lblGenderTitle.Name = "lblGenderTitle";
            lblGenderTitle.Size = new Size(55, 22);
            lblGenderTitle.TabIndex = 7;
            lblGenderTitle.Text = "Gender:";
            // 
            // lblGenderVal
            // 
            lblGenderVal.Font = new Font("Segoe UI", 9F);
            lblGenderVal.Location = new Point(200, 125);
            lblGenderVal.Name = "lblGenderVal";
            lblGenderVal.Size = new Size(170, 22);
            lblGenderVal.TabIndex = 8;
            // 
            // lblDobTitle
            // 
            lblDobTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDobTitle.Location = new Point(20, 175);
            lblDobTitle.Name = "lblDobTitle";
            lblDobTitle.Size = new Size(90, 22);
            lblDobTitle.TabIndex = 9;
            lblDobTitle.Text = "Date of Birth:";
            // 
            // lblDobVal
            // 
            lblDobVal.Font = new Font("Segoe UI", 9F);
            lblDobVal.Location = new Point(120, 175);
            lblDobVal.Name = "lblDobVal";
            lblDobVal.Size = new Size(240, 22);
            lblDobVal.TabIndex = 10;
            // 
            // lblMobileTitle
            // 
            lblMobileTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMobileTitle.Location = new Point(20, 205);
            lblMobileTitle.Name = "lblMobileTitle";
            lblMobileTitle.Size = new Size(95, 22);
            lblMobileTitle.TabIndex = 11;
            lblMobileTitle.Text = "Mobile Number:";
            // 
            // lblMobileVal
            // 
            lblMobileVal.Font = new Font("Segoe UI", 9F);
            lblMobileVal.Location = new Point(120, 205);
            lblMobileVal.Name = "lblMobileVal";
            lblMobileVal.Size = new Size(240, 22);
            lblMobileVal.TabIndex = 12;
            // 
            // btnDownloadCard
            // 
            btnDownloadCard.BackColor = Color.FromArgb(52, 152, 219);
            btnDownloadCard.FlatStyle = FlatStyle.Flat;
            btnDownloadCard.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnDownloadCard.ForeColor = Color.White;
            btnDownloadCard.Location = new Point(20, 380);
            btnDownloadCard.Name = "btnDownloadCard";
            btnDownloadCard.Size = new Size(340, 45);
            btnDownloadCard.TabIndex = 13;
            btnDownloadCard.Text = "Download & Preview ABHA Card";
            btnDownloadCard.UseVisualStyleBackColor = false;
            btnDownloadCard.Click += btnDownloadCard_Click;
            // 
            // grpCardPreview
            // 
            grpCardPreview.BackColor = Color.White;
            grpCardPreview.Controls.Add(picCard);
            grpCardPreview.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            grpCardPreview.Location = new Point(675, 15);
            grpCardPreview.Name = "grpCardPreview";
            grpCardPreview.Size = new Size(245, 640);
            grpCardPreview.TabIndex = 2;
            grpCardPreview.TabStop = false;
            grpCardPreview.Text = "ABHA Physical Card Preview";
            // 
            // picCard
            // 
            picCard.BackColor = Color.FromArgb(236, 240, 241);
            picCard.BorderStyle = BorderStyle.FixedSingle;
            picCard.Location = new Point(15, 35);
            picCard.Name = "picCard";
            picCard.Size = new Size(215, 390);
            picCard.SizeMode = PictureBoxSizeMode.Zoom;
            picCard.TabIndex = 0;
            picCard.TabStop = false;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(149, 165, 166);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(770, 670);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(150, 40);
            btnClose.TabIndex = 4;
            btnClose.Text = "Close Dashboard";
            btnClose.UseVisualStyleBackColor = false;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblStatus.ForeColor = Color.FromArgb(127, 140, 141);
            lblStatus.Location = new Point(15, 675);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(740, 30);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Loading Session Info...";
            // 
            // frmAbdmM1Dashboard
            // 
            BackColor = Color.FromArgb(245, 246, 250);
            ClientSize = new Size(950, 730);
            Controls.Add(btnClose);
            Controls.Add(lblStatus);
            Controls.Add(grpCardPreview);
            Controls.Add(grpProfile);
            Controls.Add(grpActions);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "frmAbdmM1Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ABDM Milestone 1 (M1) Integration Dashboard";
            Load += frmAbdmM1Dashboard_Load_1;
            grpActions.ResumeLayout(false);
            grpProfile.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picPhoto).EndInit();
            grpCardPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picCard).EndInit();
            ResumeLayout(false);

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
