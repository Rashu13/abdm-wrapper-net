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
            this.gbHiu = new System.Windows.Forms.GroupBox();
            this.clbHiTypes = new System.Windows.Forms.CheckedListBox();
            this.txtHiuPatientAbha = new System.Windows.Forms.TextBox();
            this.lblHiuPatientAbha = new System.Windows.Forms.Label();
            this.cmbPurpose = new System.Windows.Forms.ComboBox();
            this.lblPurpose = new System.Windows.Forms.Label();
            this.lblHiTypes = new System.Windows.Forms.Label();
            this.txtHiuDateFrom = new System.Windows.Forms.TextBox();
            this.lblHiuDateFrom = new System.Windows.Forms.Label();
            this.txtHiuDateTo = new System.Windows.Forms.TextBox();
            this.lblHiuDateTo = new System.Windows.Forms.Label();
            this.txtHiuEraseAt = new System.Windows.Forms.TextBox();
            this.lblHiuEraseAt = new System.Windows.Forms.Label();
            this.btnRequestConsent = new System.Windows.Forms.Button();
            this.txtHiuConsentReqId = new System.Windows.Forms.TextBox();
            this.lblHiuConsentReqId = new System.Windows.Forms.Label();
            this.btnCheckConsentStatus = new System.Windows.Forms.Button();
            this.txtHiuConsentId = new System.Windows.Forms.TextBox();
            this.lblHiuConsentId = new System.Windows.Forms.Label();
            this.btnFetchRecords = new System.Windows.Forms.Button();
            this.txtHiuTxnId = new System.Windows.Forms.TextBox();
            this.lblHiuTxnId = new System.Windows.Forms.Label();
            this.btnGetData = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();

            this.pnlHeader.SuspendLayout();
            this.gbSubscriptionReq.SuspendLayout();
            this.gbSubscriptionStatus.SuspendLayout();
            this.gbLog.SuspendLayout();
            this.gbHiu.SuspendLayout();
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
            this.pnlHeader.Size = new System.Drawing.Size(1460, 50);
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
            this.gbSubscriptionReq.Size = new System.Drawing.Size(460, 505);
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
            this.txtPurposeCode.Text = "CAREMGT";
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
            this.gbSubscriptionStatus.Size = new System.Drawing.Size(470, 505);
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
            // gbHiu
            // 
            this.gbHiu.Controls.Add(this.btnGetData);
            this.gbHiu.Controls.Add(this.txtHiuTxnId);
            this.gbHiu.Controls.Add(this.lblHiuTxnId);
            this.gbHiu.Controls.Add(this.btnFetchRecords);
            this.gbHiu.Controls.Add(this.txtHiuConsentId);
            this.gbHiu.Controls.Add(this.lblHiuConsentId);
            this.gbHiu.Controls.Add(this.btnCheckConsentStatus);
            this.gbHiu.Controls.Add(this.txtHiuConsentReqId);
            this.gbHiu.Controls.Add(this.lblHiuConsentReqId);
            this.gbHiu.Controls.Add(this.btnRequestConsent);
            this.gbHiu.Controls.Add(this.txtHiuEraseAt);
            this.gbHiu.Controls.Add(this.lblHiuEraseAt);
            this.gbHiu.Controls.Add(this.txtHiuDateTo);
            this.gbHiu.Controls.Add(this.lblHiuDateTo);
            this.gbHiu.Controls.Add(this.txtHiuDateFrom);
            this.gbHiu.Controls.Add(this.lblHiuDateFrom);
            this.gbHiu.Controls.Add(this.clbHiTypes);
            this.gbHiu.Controls.Add(this.lblHiTypes);
            this.gbHiu.Controls.Add(this.cmbPurpose);
            this.gbHiu.Controls.Add(this.lblPurpose);
            this.gbHiu.Controls.Add(this.txtHiuPatientAbha);
            this.gbHiu.Controls.Add(this.lblHiuPatientAbha);
            this.gbHiu.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbHiu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.gbHiu.BackColor = System.Drawing.Color.White;
            this.gbHiu.Location = new System.Drawing.Point(975, 65);
            this.gbHiu.Name = "gbHiu";
            this.gbHiu.Size = new System.Drawing.Size(470, 505);
            this.gbHiu.TabIndex = 2;
            this.gbHiu.TabStop = false;
            this.gbHiu.Text = "HIU Operations (Consent & Health Data Fetch)";
            // 
            // btnGetData
            // 
            this.btnGetData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnGetData.FlatAppearance.BorderSize = 0;
            this.btnGetData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetData.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGetData.ForeColor = System.Drawing.Color.White;
            this.btnGetData.Location = new System.Drawing.Point(20, 453);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(435, 35);
            this.btnGetData.TabIndex = 23;
            this.btnGetData.Text = "4. View Decrypted Data";
            this.btnGetData.UseVisualStyleBackColor = false;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // txtHiuTxnId
            // 
            this.txtHiuTxnId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuTxnId.Location = new System.Drawing.Point(145, 421);
            this.txtHiuTxnId.Name = "txtHiuTxnId";
            this.txtHiuTxnId.Size = new System.Drawing.Size(310, 27);
            this.txtHiuTxnId.TabIndex = 22;
            // 
            // lblHiuTxnId
            // 
            this.lblHiuTxnId.AutoSize = true;
            this.lblHiuTxnId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuTxnId.ForeColor = System.Drawing.Color.Black;
            this.lblHiuTxnId.Location = new System.Drawing.Point(20, 424);
            this.lblHiuTxnId.Name = "lblHiuTxnId";
            this.lblHiuTxnId.Size = new System.Drawing.Size(124, 20);
            this.lblHiuTxnId.TabIndex = 21;
            this.lblHiuTxnId.Text = "Health Request ID:";
            // 
            // btnFetchRecords
            // 
            this.btnFetchRecords.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.btnFetchRecords.FlatAppearance.BorderSize = 0;
            this.btnFetchRecords.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFetchRecords.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFetchRecords.ForeColor = System.Drawing.Color.White;
            this.btnFetchRecords.Location = new System.Drawing.Point(20, 381);
            this.btnFetchRecords.Name = "btnFetchRecords";
            this.btnFetchRecords.Size = new System.Drawing.Size(435, 35);
            this.btnFetchRecords.TabIndex = 20;
            this.btnFetchRecords.Text = "3. Fetch Health Records";
            this.btnFetchRecords.UseVisualStyleBackColor = false;
            this.btnFetchRecords.Click += new System.EventHandler(this.btnFetchRecords_Click);
            // 
            // txtHiuConsentId
            // 
            this.txtHiuConsentId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuConsentId.Location = new System.Drawing.Point(145, 349);
            this.txtHiuConsentId.Name = "txtHiuConsentId";
            this.txtHiuConsentId.Size = new System.Drawing.Size(310, 27);
            this.txtHiuConsentId.TabIndex = 19;
            // 
            // lblHiuConsentId
            // 
            this.lblHiuConsentId.AutoSize = true;
            this.lblHiuConsentId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuConsentId.ForeColor = System.Drawing.Color.Black;
            this.lblHiuConsentId.Location = new System.Drawing.Point(20, 352);
            this.lblHiuConsentId.Name = "lblHiuConsentId";
            this.lblHiuConsentId.Size = new System.Drawing.Size(83, 20);
            this.lblHiuConsentId.TabIndex = 18;
            this.lblHiuConsentId.Text = "Consent ID:";
            // 
            // btnCheckConsentStatus
            // 
            this.btnCheckConsentStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnCheckConsentStatus.FlatAppearance.BorderSize = 0;
            this.btnCheckConsentStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckConsentStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCheckConsentStatus.ForeColor = System.Drawing.Color.White;
            this.btnCheckConsentStatus.Location = new System.Drawing.Point(20, 309);
            this.btnCheckConsentStatus.Name = "btnCheckConsentStatus";
            this.btnCheckConsentStatus.Size = new System.Drawing.Size(435, 35);
            this.btnCheckConsentStatus.TabIndex = 17;
            this.btnCheckConsentStatus.Text = "2. Check Consent Status";
            this.btnCheckConsentStatus.UseVisualStyleBackColor = false;
            this.btnCheckConsentStatus.Click += new System.EventHandler(this.btnCheckConsentStatus_Click);
            // 
            // txtHiuConsentReqId
            // 
            this.txtHiuConsentReqId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuConsentReqId.Location = new System.Drawing.Point(145, 277);
            this.txtHiuConsentReqId.Name = "txtHiuConsentReqId";
            this.txtHiuConsentReqId.Size = new System.Drawing.Size(310, 27);
            this.txtHiuConsentReqId.TabIndex = 16;
            // 
            // lblHiuConsentReqId
            // 
            this.lblHiuConsentReqId.AutoSize = true;
            this.lblHiuConsentReqId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuConsentReqId.ForeColor = System.Drawing.Color.Black;
            this.lblHiuConsentReqId.Location = new System.Drawing.Point(20, 280);
            this.lblHiuConsentReqId.Name = "lblHiuConsentReqId";
            this.lblHiuConsentReqId.Size = new System.Drawing.Size(113, 20);
            this.lblHiuConsentReqId.TabIndex = 15;
            this.lblHiuConsentReqId.Text = "Consent Req ID:";
            // 
            // btnRequestConsent
            // 
            this.btnRequestConsent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnRequestConsent.FlatAppearance.BorderSize = 0;
            this.btnRequestConsent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRequestConsent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRequestConsent.ForeColor = System.Drawing.Color.White;
            this.btnRequestConsent.Location = new System.Drawing.Point(20, 237);
            this.btnRequestConsent.Name = "btnRequestConsent";
            this.btnRequestConsent.Size = new System.Drawing.Size(435, 35);
            this.btnRequestConsent.TabIndex = 14;
            this.btnRequestConsent.Text = "1. Initiate Consent Request";
            this.btnRequestConsent.UseVisualStyleBackColor = false;
            this.btnRequestConsent.Click += new System.EventHandler(this.btnRequestConsent_Click);
            // 
            // txtHiuEraseAt
            // 
            this.txtHiuEraseAt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuEraseAt.Location = new System.Drawing.Point(145, 204);
            this.txtHiuEraseAt.Name = "txtHiuEraseAt";
            this.txtHiuEraseAt.Size = new System.Drawing.Size(310, 27);
            this.txtHiuEraseAt.TabIndex = 13;
            this.txtHiuEraseAt.Text = "2026-12-31T00:00:00.000Z";
            // 
            // lblHiuEraseAt
            // 
            this.lblHiuEraseAt.AutoSize = true;
            this.lblHiuEraseAt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuEraseAt.ForeColor = System.Drawing.Color.Black;
            this.lblHiuEraseAt.Location = new System.Drawing.Point(20, 207);
            this.lblHiuEraseAt.Name = "lblHiuEraseAt";
            this.lblHiuEraseAt.Size = new System.Drawing.Size(65, 20);
            this.lblHiuEraseAt.TabIndex = 12;
            this.lblHiuEraseAt.Text = "Erase At:";
            // 
            // txtHiuDateTo
            // 
            this.txtHiuDateTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuDateTo.Location = new System.Drawing.Point(145, 177);
            this.txtHiuDateTo.Name = "txtHiuDateTo";
            this.txtHiuDateTo.Size = new System.Drawing.Size(310, 27);
            this.txtHiuDateTo.TabIndex = 11;
            this.txtHiuDateTo.Text = "2026-06-16T00:00:00.000Z";
            // 
            // lblHiuDateTo
            // 
            this.lblHiuDateTo.AutoSize = true;
            this.lblHiuDateTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuDateTo.ForeColor = System.Drawing.Color.Black;
            this.lblHiuDateTo.Location = new System.Drawing.Point(20, 180);
            this.lblHiuDateTo.Name = "lblHiuDateTo";
            this.lblHiuDateTo.Size = new System.Drawing.Size(65, 20);
            this.lblHiuDateTo.TabIndex = 10;
            this.lblHiuDateTo.Text = "Date To:";
            // 
            // txtHiuDateFrom
            // 
            this.txtHiuDateFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuDateFrom.Location = new System.Drawing.Point(145, 150);
            this.txtHiuDateFrom.Name = "txtHiuDateFrom";
            this.txtHiuDateFrom.Size = new System.Drawing.Size(310, 27);
            this.txtHiuDateFrom.TabIndex = 9;
            this.txtHiuDateFrom.Text = "2024-01-01T00:00:00.000Z";
            // 
            // lblHiuDateFrom
            // 
            this.lblHiuDateFrom.AutoSize = true;
            this.lblHiuDateFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuDateFrom.ForeColor = System.Drawing.Color.Black;
            this.lblHiuDateFrom.Location = new System.Drawing.Point(20, 153);
            this.lblHiuDateFrom.Name = "lblHiuDateFrom";
            this.lblHiuDateFrom.Size = new System.Drawing.Size(83, 20);
            this.lblHiuDateFrom.TabIndex = 8;
            this.lblHiuDateFrom.Text = "Date From:";
            // 
            // clbHiTypes
            // 
            this.clbHiTypes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clbHiTypes.CheckOnClick = true;
            this.clbHiTypes.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.clbHiTypes.FormattingEnabled = true;
            this.clbHiTypes.Items.AddRange(new object[] {
            "OPConsultation",
            "Prescription",
            "DiagnosticReport",
            "DischargeSummary",
            "ImmunizationRecord",
            "HealthDocumentRecord",
            "WellnessRecord",
            "Invoice"});
            this.clbHiTypes.Location = new System.Drawing.Point(145, 80);
            this.clbHiTypes.Name = "clbHiTypes";
            this.clbHiTypes.Size = new System.Drawing.Size(310, 68);
            this.clbHiTypes.TabIndex = 5;
            // 
            // lblHiTypes
            // 
            this.lblHiTypes.AutoSize = true;
            this.lblHiTypes.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiTypes.ForeColor = System.Drawing.Color.Black;
            this.lblHiTypes.Location = new System.Drawing.Point(20, 84);
            this.lblHiTypes.Name = "lblHiTypes";
            this.lblHiTypes.Size = new System.Drawing.Size(68, 20);
            this.lblHiTypes.TabIndex = 4;
            this.lblHiTypes.Text = "HI Types:";
            // 
            // cmbPurpose
            // 
            this.cmbPurpose.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPurpose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbPurpose.FormattingEnabled = true;
            this.cmbPurpose.Items.AddRange(new object[] {
            "CACT - Care Management",
            "PUBHLTH - Public Health",
            "PATRQT - Patient Request"});
            this.cmbPurpose.Location = new System.Drawing.Point(145, 50);
            this.cmbPurpose.Name = "cmbPurpose";
            this.cmbPurpose.Size = new System.Drawing.Size(310, 28);
            this.cmbPurpose.TabIndex = 3;
            this.cmbPurpose.SelectedIndex = 0;
            // 
            // lblPurpose
            // 
            this.lblPurpose.AutoSize = true;
            this.lblPurpose.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPurpose.ForeColor = System.Drawing.Color.Black;
            this.lblPurpose.Location = new System.Drawing.Point(20, 53);
            this.lblPurpose.Name = "lblPurpose";
            this.lblPurpose.Size = new System.Drawing.Size(65, 20);
            this.lblPurpose.TabIndex = 2;
            this.lblPurpose.Text = "Purpose:";
            // 
            // txtHiuPatientAbha
            // 
            this.txtHiuPatientAbha.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuPatientAbha.Location = new System.Drawing.Point(145, 20);
            this.txtHiuPatientAbha.Name = "txtHiuPatientAbha";
            this.txtHiuPatientAbha.Size = new System.Drawing.Size(310, 27);
            this.txtHiuPatientAbha.TabIndex = 1;
            this.txtHiuPatientAbha.Text = "testpatient123@sbx";
            // 
            // lblHiuPatientAbha
            // 
            this.lblHiuPatientAbha.AutoSize = true;
            this.lblHiuPatientAbha.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuPatientAbha.ForeColor = System.Drawing.Color.Black;
            this.lblHiuPatientAbha.Location = new System.Drawing.Point(20, 23);
            this.lblHiuPatientAbha.Name = "lblHiuPatientAbha";
            this.lblHiuPatientAbha.Size = new System.Drawing.Size(107, 20);
            this.lblHiuPatientAbha.TabIndex = 0;
            this.lblHiuPatientAbha.Text = "Patient ABHA:";
            
            // 
            // gbLog
            // 
            this.gbLog.BackColor = System.Drawing.Color.White;
            this.gbLog.Controls.Add(this.txtLog);
            this.gbLog.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbLog.Location = new System.Drawing.Point(15, 580);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(1430, 145);
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
            this.txtLog.Size = new System.Drawing.Size(1424, 162);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // frmABDMM3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(1460, 740);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gbHiu);
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
            this.gbHiu.ResumeLayout(false);
            this.gbHiu.PerformLayout();
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
        private System.Windows.Forms.GroupBox gbHiu;
        private System.Windows.Forms.TextBox txtHiuPatientAbha;
        private System.Windows.Forms.Label lblHiuPatientAbha;
        private System.Windows.Forms.Label lblPurpose;
        private System.Windows.Forms.ComboBox cmbPurpose;
        private System.Windows.Forms.Label lblHiTypes;
        private System.Windows.Forms.CheckedListBox clbHiTypes;
        private System.Windows.Forms.TextBox txtHiuDateFrom;
        private System.Windows.Forms.Label lblHiuDateFrom;
        private System.Windows.Forms.TextBox txtHiuDateTo;
        private System.Windows.Forms.Label lblHiuDateTo;
        private System.Windows.Forms.TextBox txtHiuEraseAt;
        private System.Windows.Forms.Label lblHiuEraseAt;
        private System.Windows.Forms.Button btnRequestConsent;
        private System.Windows.Forms.TextBox txtHiuConsentReqId;
        private System.Windows.Forms.Label lblHiuConsentReqId;
        private System.Windows.Forms.Button btnCheckConsentStatus;
        private System.Windows.Forms.TextBox txtHiuConsentId;
        private System.Windows.Forms.Label lblHiuConsentId;
        private System.Windows.Forms.Button btnFetchRecords;
        private System.Windows.Forms.TextBox txtHiuTxnId;
        private System.Windows.Forms.Label lblHiuTxnId;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.RichTextBox txtLog;
    }
}
