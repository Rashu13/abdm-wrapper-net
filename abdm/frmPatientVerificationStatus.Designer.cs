using System.Windows.Forms;

namespace HMS.abdm
{
    partial class frmPatientVerificationStatus
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.pbStatusIcon = new System.Windows.Forms.PictureBox();
            this.lblStatusText = new System.Windows.Forms.Label();
            this.gbDetails = new System.Windows.Forms.GroupBox();
            this.pbPhoto = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblNameVal = new System.Windows.Forms.Label();
            this.lblAbha = new System.Windows.Forms.Label();
            this.lblAbhaVal = new System.Windows.Forms.Label();
            this.lblMobile = new System.Windows.Forms.Label();
            this.lblMobileVal = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.lblGenderVal = new System.Windows.Forms.Label();
            this.lblDob = new System.Windows.Forms.Label();
            this.lblDobVal = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblAddressVal = new System.Windows.Forms.Label();
            this.btnAction = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatusIcon)).BeginInit();
            this.gbDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(55, 115, 200);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(580, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(242, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ABHA Patient Verification (M1)";
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(530, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(50, 50);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // pnlStatus
            // 
            this.pnlStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStatus.Controls.Add(this.pbStatusIcon);
            this.pnlStatus.Controls.Add(this.lblStatusText);
            this.pnlStatus.Location = new System.Drawing.Point(20, 65);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(540, 60);
            this.pnlStatus.TabIndex = 1;
            // 
            // pbStatusIcon
            // 
            this.pbStatusIcon.Location = new System.Drawing.Point(15, 12);
            this.pbStatusIcon.Name = "pbStatusIcon";
            this.pbStatusIcon.Size = new System.Drawing.Size(36, 36);
            this.pbStatusIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbStatusIcon.TabIndex = 0;
            this.pbStatusIcon.TabStop = false;
            // 
            // lblStatusText
            // 
            this.lblStatusText.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatusText.Location = new System.Drawing.Point(60, 12);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(460, 36);
            this.lblStatusText.TabIndex = 1;
            this.lblStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbDetails
            // 
            this.gbDetails.Controls.Add(this.pbPhoto);
            this.gbDetails.Controls.Add(this.lblName);
            this.gbDetails.Controls.Add(this.lblNameVal);
            this.gbDetails.Controls.Add(this.lblAbha);
            this.gbDetails.Controls.Add(this.lblAbhaVal);
            this.gbDetails.Controls.Add(this.lblMobile);
            this.gbDetails.Controls.Add(this.lblMobileVal);
            this.gbDetails.Controls.Add(this.lblGender);
            this.gbDetails.Controls.Add(this.lblGenderVal);
            this.gbDetails.Controls.Add(this.lblDob);
            this.gbDetails.Controls.Add(this.lblDobVal);
            this.gbDetails.Controls.Add(this.lblAddress);
            this.gbDetails.Controls.Add(this.lblAddressVal);
            this.gbDetails.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.gbDetails.Location = new System.Drawing.Point(20, 140);
            this.gbDetails.Name = "gbDetails";
            this.gbDetails.Size = new System.Drawing.Size(540, 240);
            this.gbDetails.TabIndex = 2;
            this.gbDetails.TabStop = false;
            this.gbDetails.Text = "ABHA Demographics Information";
            // 
            // pbPhoto
            // 
            this.pbPhoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPhoto.Location = new System.Drawing.Point(15, 30);
            this.pbPhoto.Name = "pbPhoto";
            this.pbPhoto.Size = new System.Drawing.Size(110, 130);
            this.pbPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPhoto.TabIndex = 0;
            this.pbPhoto.TabStop = false;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblName.Location = new System.Drawing.Point(140, 30);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(81, 15);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Patient Name:";
            // 
            // lblNameVal
            // 
            this.lblNameVal.AutoSize = true;
            this.lblNameVal.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblNameVal.ForeColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.lblNameVal.Location = new System.Drawing.Point(250, 30);
            this.lblNameVal.Name = "lblNameVal";
            this.lblNameVal.Size = new System.Drawing.Size(0, 17);
            this.lblNameVal.TabIndex = 2;
            // 
            // lblAbha
            // 
            this.lblAbha.AutoSize = true;
            this.lblAbha.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAbha.Location = new System.Drawing.Point(140, 60);
            this.lblAbha.Name = "lblAbha";
            this.lblAbha.Size = new System.Drawing.Size(86, 15);
            this.lblAbha.TabIndex = 3;
            this.lblAbha.Text = "ABHA Address:";
            // 
            // lblAbhaVal
            // 
            this.lblAbhaVal.AutoSize = true;
            this.lblAbhaVal.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblAbhaVal.ForeColor = System.Drawing.Color.Navy;
            this.lblAbhaVal.Location = new System.Drawing.Point(250, 60);
            this.lblAbhaVal.Name = "lblAbhaVal";
            this.lblAbhaVal.Size = new System.Drawing.Size(0, 17);
            this.lblAbhaVal.TabIndex = 4;
            // 
            // lblMobile
            // 
            this.lblMobile.AutoSize = true;
            this.lblMobile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMobile.Location = new System.Drawing.Point(140, 90);
            this.lblMobile.Name = "lblMobile";
            this.lblMobile.Size = new System.Drawing.Size(47, 15);
            this.lblMobile.TabIndex = 5;
            this.lblMobile.Text = "Mobile:";
            // 
            // lblMobileVal
            // 
            this.lblMobileVal.AutoSize = true;
            this.lblMobileVal.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblMobileVal.Location = new System.Drawing.Point(250, 90);
            this.lblMobileVal.Name = "lblMobileVal";
            this.lblMobileVal.Size = new System.Drawing.Size(0, 17);
            this.lblMobileVal.TabIndex = 6;
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblGender.Location = new System.Drawing.Point(140, 120);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(48, 15);
            this.lblGender.TabIndex = 7;
            this.lblGender.Text = "Gender:";
            // 
            // lblGenderVal
            // 
            this.lblGenderVal.AutoSize = true;
            this.lblGenderVal.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblGenderVal.Location = new System.Drawing.Point(250, 120);
            this.lblGenderVal.Name = "lblGenderVal";
            this.lblGenderVal.Size = new System.Drawing.Size(0, 17);
            this.lblGenderVal.TabIndex = 8;
            // 
            // lblDob
            // 
            this.lblDob.AutoSize = true;
            this.lblDob.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDob.Location = new System.Drawing.Point(140, 150);
            this.lblDob.Name = "lblDob";
            this.lblDob.Size = new System.Drawing.Size(76, 15);
            this.lblDob.TabIndex = 9;
            this.lblDob.Text = "Date of Birth:";
            // 
            // lblDobVal
            // 
            this.lblDobVal.AutoSize = true;
            this.lblDobVal.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblDobVal.Location = new System.Drawing.Point(250, 150);
            this.lblDobVal.Name = "lblDobVal";
            this.lblDobVal.Size = new System.Drawing.Size(0, 17);
            this.lblDobVal.TabIndex = 10;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAddress.Location = new System.Drawing.Point(140, 180);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(52, 15);
            this.lblAddress.TabIndex = 11;
            this.lblAddress.Text = "Address:";
            // 
            // lblAddressVal
            // 
            this.lblAddressVal.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblAddressVal.Location = new System.Drawing.Point(250, 180);
            this.lblAddressVal.Name = "lblAddressVal";
            this.lblAddressVal.Size = new System.Drawing.Size(270, 45);
            this.lblAddressVal.TabIndex = 12;
            // 
            // btnAction
            // 
            this.btnAction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAction.FlatAppearance.BorderSize = 0;
            this.btnAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAction.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnAction.ForeColor = System.Drawing.Color.White;
            this.btnAction.Location = new System.Drawing.Point(140, 400);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(200, 45);
            this.btnAction.TabIndex = 3;
            this.btnAction.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.btnCancel.Location = new System.Drawing.Point(350, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 45);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmPatientVerificationStatus
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(580, 480);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.gbDetails);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPatientVerificationStatus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ABHA Patient Verification (M1)";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbStatusIcon)).EndInit();
            this.gbDetails.ResumeLayout(false);
            this.gbDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPhoto)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.PictureBox pbStatusIcon;
        private System.Windows.Forms.Label lblStatusText;
        private System.Windows.Forms.GroupBox gbDetails;
        private System.Windows.Forms.PictureBox pbPhoto;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblNameVal;
        private System.Windows.Forms.Label lblAbha;
        private System.Windows.Forms.Label lblAbhaVal;
        private System.Windows.Forms.Label lblMobile;
        private System.Windows.Forms.Label lblMobileVal;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.Label lblGenderVal;
        private System.Windows.Forms.Label lblDob;
        private System.Windows.Forms.Label lblDobVal;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblAddressVal;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.Button btnCancel;
    }
}
