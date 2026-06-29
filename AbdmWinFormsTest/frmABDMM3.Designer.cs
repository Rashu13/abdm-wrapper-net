namespace HMS.abdm
{
    partial class frmABDMM3
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.gbSubscriptionReq = new System.Windows.Forms.GroupBox();
            this.lblAbhaAddress = new System.Windows.Forms.Label();
            this.txtAbhaAddress = new System.Windows.Forms.TextBox();
            this.lblPurposeCode = new System.Windows.Forms.Label();
            this.txtPurposeCode = new System.Windows.Forms.TextBox();
            this.lblCategories = new System.Windows.Forms.Label();
            this.chkCategoryLink = new System.Windows.Forms.CheckBox();
            this.chkCategoryData = new System.Windows.Forms.CheckBox();
            this.lblDateFrom = new System.Windows.Forms.Label();
            this.txtDateFrom = new System.Windows.Forms.TextBox();
            this.lblDateTo = new System.Windows.Forms.Label();
            this.txtDateTo = new System.Windows.Forms.TextBox();
            this.btnInitiateSubscription = new System.Windows.Forms.Button();
            this.gbSubscriptionStatus = new System.Windows.Forms.GroupBox();
            this.lblSubscriptionReqId = new System.Windows.Forms.Label();
            this.txtSubscriptionReqId = new System.Windows.Forms.TextBox();
            this.btnCheckStatus = new System.Windows.Forms.Button();
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();

            this.pnlHeader.SuspendLayout();
            this.gbSubscriptionReq.SuspendLayout();
            this.gbSubscriptionStatus.SuspendLayout();
            this.gbLog.SuspendLayout();
            this.SuspendLayout();

            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(54)))));
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(980, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(930, 0);
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
            this.lblTitle.Size = new System.Drawing.Size(490, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Milestone 3 (M3) Verification: Health Information Subscription";
            // 
            // gbSubscriptionReq
            // 
            this.gbSubscriptionReq.BackColor = System.Drawing.Color.White;
            this.gbSubscriptionReq.Controls.Add(this.btnInitiateSubscription);
            this.gbSubscriptionReq.Controls.Add(this.txtDateTo);
            this.gbSubscriptionReq.Controls.Add(this.lblDateTo);
            this.gbSubscriptionReq.Controls.Add(this.txtDateFrom);
            this.gbSubscriptionReq.Controls.Add(this.lblDateFrom);
            this.gbSubscriptionReq.Controls.Add(this.chkCategoryData);
            this.gbSubscriptionReq.Controls.Add(this.chkCategoryLink);
            this.gbSubscriptionReq.Controls.Add(this.lblCategories);
            this.gbSubscriptionReq.Controls.Add(this.txtPurposeCode);
            this.gbSubscriptionReq.Controls.Add(this.lblPurposeCode);
            this.gbSubscriptionReq.Controls.Add(this.txtAbhaAddress);
            this.gbSubscriptionReq.Controls.Add(this.lblAbhaAddress);
            this.gbSubscriptionReq.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbSubscriptionReq.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(54)))));
            this.gbSubscriptionReq.Location = new System.Drawing.Point(15, 65);
            this.gbSubscriptionReq.Name = "gbSubscriptionReq";
            this.gbSubscriptionReq.Size = new System.Drawing.Size(460, 460);
            this.gbSubscriptionReq.TabIndex = 1;
            this.gbSubscriptionReq.TabStop = false;
            this.gbSubscriptionReq.Text = "Subscription Request Setup";
            // 
            // lblAbhaAddress
            // 
            this.lblAbhaAddress.AutoSize = true;
            this.lblAbhaAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAbhaAddress.ForeColor = System.Drawing.Color.Black;
            this.lblAbhaAddress.Location = new System.Drawing.Point(15, 45);
            this.lblAbhaAddress.Name = "lblAbhaAddress";
            this.lblAbhaAddress.Size = new System.Drawing.Size(107, 20);
            this.lblAbhaAddress.TabIndex = 0;
            this.lblAbhaAddress.Text = "Patient ABHA ID:";
            // 
            // txtAbhaAddress
            // 
            this.txtAbhaAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAbhaAddress.Location = new System.Drawing.Point(135, 42);
            this.txtAbhaAddress.Name = "txtAbhaAddress";
            this.txtAbhaAddress.Size = new System.Drawing.Size(305, 27);
            this.txtAbhaAddress.TabIndex = 1;
            this.txtAbhaAddress.Text = "health.test@sbx";
            // 
            // lblPurposeCode
            // 
            this.lblPurposeCode.AutoSize = true;
            this.lblPurposeCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPurposeCode.ForeColor = System.Drawing.Color.Black;
            this.lblPurposeCode.Location = new System.Drawing.Point(15, 85);
            this.lblPurposeCode.Name = "lblPurposeCode";
            this.lblPurposeCode.Size = new System.Drawing.Size(103, 20);
            this.lblPurposeCode.TabIndex = 2;
            this.lblPurposeCode.Text = "Purpose Code:";
            // 
            // txtPurposeCode
            // 
            this.txtPurposeCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPurposeCode.Location = new System.Drawing.Point(135, 82);
            this.txtPurposeCode.Name = "txtPurposeCode";
            this.txtPurposeCode.Size = new System.Drawing.Size(305, 27);
            this.txtPurposeCode.TabIndex = 3;
            this.txtPurposeCode.Text = "CACT";
            // 
            // lblCategories
            // 
            this.lblCategories.AutoSize = true;
            this.lblCategories.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCategories.ForeColor = System.Drawing.Color.Black;
            this.lblCategories.Location = new System.Drawing.Point(15, 130);
            this.lblCategories.Name = "lblCategories";
            this.lblCategories.Size = new System.Drawing.Size(83, 20);
            this.lblCategories.TabIndex = 4;
            this.lblCategories.Text = "Categories:";
            // 
            // chkCategoryLink
            // 
            this.chkCategoryLink.AutoSize = true;
            this.chkCategoryLink.Checked = true;
            this.chkCategoryLink.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategoryLink.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkCategoryLink.ForeColor = System.Drawing.Color.Black;
            this.chkCategoryLink.Location = new System.Drawing.Point(135, 129);
            this.chkCategoryLink.Name = "chkCategoryLink";
            this.chkCategoryLink.Size = new System.Drawing.Size(63, 24);
            this.chkCategoryLink.TabIndex = 5;
            this.chkCategoryLink.Text = "LINK";
            this.chkCategoryLink.UseVisualStyleBackColor = true;
            // 
            // chkCategoryData
            // 
            this.chkCategoryData.AutoSize = true;
            this.chkCategoryData.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkCategoryData.ForeColor = System.Drawing.Color.Black;
            this.chkCategoryData.Location = new System.Drawing.Point(215, 129);
            this.chkCategoryData.Name = "chkCategoryData";
            this.chkCategoryData.Size = new System.Drawing.Size(68, 24);
            this.chkCategoryData.TabIndex = 6;
            this.chkCategoryData.Text = "DATA";
            this.chkCategoryData.UseVisualStyleBackColor = true;
            // 
            // lblDateFrom
            // 
            this.lblDateFrom.AutoSize = true;
            this.lblDateFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDateFrom.ForeColor = System.Drawing.Color.Black;
            this.lblDateFrom.Location = new System.Drawing.Point(15, 175);
            this.lblDateFrom.Name = "lblDateFrom";
            this.lblDateFrom.Size = new System.Drawing.Size(82, 20);
            this.lblDateFrom.TabIndex = 7;
            this.lblDateFrom.Text = "Period From:";
            // 
            // txtDateFrom
            // 
            this.txtDateFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDateFrom.Location = new System.Drawing.Point(135, 172);
            this.txtDateFrom.Name = "txtDateFrom";
            this.txtDateFrom.Size = new System.Drawing.Size(305, 27);
            this.txtDateFrom.TabIndex = 8;
            // 
            // lblDateTo
            // 
            this.lblDateTo.AutoSize = true;
            this.lblDateTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDateTo.ForeColor = System.Drawing.Color.Black;
            this.lblDateTo.Location = new System.Drawing.Point(15, 215);
            this.lblDateTo.Name = "lblDateTo";
            this.lblDateTo.Size = new System.Drawing.Size(66, 20);
            this.lblDateTo.TabIndex = 9;
            this.lblDateTo.Text = "Period To:";
            // 
            // txtDateTo
            // 
            this.txtDateTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDateTo.Location = new System.Drawing.Point(135, 212);
            this.txtDateTo.Name = "txtDateTo";
            this.txtDateTo.Size = new System.Drawing.Size(305, 27);
            this.txtDateTo.TabIndex = 10;
            // 
            // btnInitiateSubscription
            // 
            this.btnInitiateSubscription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnInitiateSubscription.FlatAppearance.BorderSize = 0;
            this.btnInitiateSubscription.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInitiateSubscription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnInitiateSubscription.ForeColor = System.Drawing.Color.White;
            this.btnInitiateSubscription.Location = new System.Drawing.Point(15, 260);
            this.btnInitiateSubscription.Name = "btnInitiateSubscription";
            this.btnInitiateSubscription.Size = new System.Drawing.Size(425, 35);
            this.btnInitiateSubscription.TabIndex = 11;
            this.btnInitiateSubscription.Text = "1. Initiate Subscription Request";
            this.btnInitiateSubscription.UseVisualStyleBackColor = false;
            this.btnInitiateSubscription.Click += new System.EventHandler(this.btnInitiateSubscription_Click);
            // 
            // gbSubscriptionStatus
            // 
            this.gbSubscriptionStatus.BackColor = System.Drawing.Color.White;
            this.gbSubscriptionStatus.Controls.Add(this.btnCheckStatus);
            this.gbSubscriptionStatus.Controls.Add(this.txtSubscriptionReqId);
            this.gbSubscriptionStatus.Controls.Add(this.lblSubscriptionReqId);
            this.gbSubscriptionStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbSubscriptionStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.gbSubscriptionStatus.Location = new System.Drawing.Point(495, 65);
            this.gbSubscriptionStatus.Name = "gbSubscriptionStatus";
            this.gbSubscriptionStatus.Size = new System.Drawing.Size(470, 460);
            this.gbSubscriptionStatus.TabIndex = 2;
            this.gbSubscriptionStatus.TabStop = false;
            this.gbSubscriptionStatus.Text = "Subscription Status Tracking";
            // 
            // lblSubscriptionReqId
            // 
            this.lblSubscriptionReqId.AutoSize = true;
            this.lblSubscriptionReqId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubscriptionReqId.ForeColor = System.Drawing.Color.Black;
            this.lblSubscriptionReqId.Location = new System.Drawing.Point(20, 45);
            this.lblSubscriptionReqId.Name = "lblSubscriptionReqId";
            this.lblSubscriptionReqId.Size = new System.Drawing.Size(117, 20);
            this.lblSubscriptionReqId.TabIndex = 0;
            this.lblSubscriptionReqId.Text = "Subscription Req ID:";
            // 
            // txtSubscriptionReqId
            // 
            this.txtSubscriptionReqId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSubscriptionReqId.Location = new System.Drawing.Point(145, 42);
            this.txtSubscriptionReqId.Name = "txtSubscriptionReqId";
            this.txtSubscriptionReqId.Size = new System.Drawing.Size(310, 27);
            this.txtSubscriptionReqId.TabIndex = 1;
            // 
            // btnCheckStatus
            // 
            this.btnCheckStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnCheckStatus.FlatAppearance.BorderSize = 0;
            this.btnCheckStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCheckStatus.ForeColor = System.Drawing.Color.White;
            this.btnCheckStatus.Location = new System.Drawing.Point(20, 85);
            this.btnCheckStatus.Name = "btnCheckStatus";
            this.btnCheckStatus.Size = new System.Drawing.Size(435, 35);
            this.btnCheckStatus.TabIndex = 2;
            this.btnCheckStatus.Text = "2. Check Subscription Status";
            this.btnCheckStatus.UseVisualStyleBackColor = false;
            this.btnCheckStatus.Click += new System.EventHandler(this.btnCheckStatus_Click);
            // 
            // gbLog
            // 
            this.gbLog.BackColor = System.Drawing.Color.White;
            this.gbLog.Controls.Add(this.txtLog);
            this.gbLog.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbLog.Location = new System.Drawing.Point(15, 535);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(950, 190);
            this.gbLog.TabIndex = 3;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Execution Output / Log";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.txtLog.Location = new System.Drawing.Point(3, 25);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(944, 162);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // frmABDMM3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(980, 740);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gbSubscriptionStatus);
            this.Controls.Add(this.gbSubscriptionReq);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmABDMM3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "M3 Health Information Subscription Flow";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.gbSubscriptionReq.ResumeLayout(false);
            this.gbSubscriptionReq.PerformLayout();
            this.gbSubscriptionStatus.ResumeLayout(false);
            this.gbSubscriptionStatus.PerformLayout();
            this.gbLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox gbSubscriptionReq;
        private System.Windows.Forms.Label lblAbhaAddress;
        private System.Windows.Forms.TextBox txtAbhaAddress;
        private System.Windows.Forms.Label lblPurposeCode;
        private System.Windows.Forms.TextBox txtPurposeCode;
        private System.Windows.Forms.Label lblCategories;
        private System.Windows.Forms.CheckBox chkCategoryLink;
        private System.Windows.Forms.CheckBox chkCategoryData;
        private System.Windows.Forms.Label lblDateFrom;
        private System.Windows.Forms.TextBox txtDateFrom;
        private System.Windows.Forms.Label lblDateTo;
        private System.Windows.Forms.TextBox txtDateTo;
        private System.Windows.Forms.Button btnInitiateSubscription;
        private System.Windows.Forms.GroupBox gbSubscriptionStatus;
        private System.Windows.Forms.Label lblSubscriptionReqId;
        private System.Windows.Forms.TextBox txtSubscriptionReqId;
        private System.Windows.Forms.Button btnCheckStatus;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.RichTextBox txtLog;
    }
}
