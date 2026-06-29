namespace HMS.abdm
{
    partial class frmABDMM2
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
            this.gbHip = new System.Windows.Forms.GroupBox();
            this.txtHipDob = new System.Windows.Forms.TextBox();
            this.lblHipDob = new System.Windows.Forms.Label();
            this.cmbHipGender = new System.Windows.Forms.ComboBox();
            this.lblHipGender = new System.Windows.Forms.Label();
            this.txtHipPatientName = new System.Windows.Forms.TextBox();
            this.lblHipPatientName = new System.Windows.Forms.Label();
            this.btnSendSms = new System.Windows.Forms.Button();
            this.btnCheckLinkStatus = new System.Windows.Forms.Button();
            this.txtHipLinkId = new System.Windows.Forms.TextBox();
            this.lblHipLinkId = new System.Windows.Forms.Label();
            this.btnInitiateLink = new System.Windows.Forms.Button();
            this.btnRegisterPatient = new System.Windows.Forms.Button();
            this.txtHipCareContextDisplay = new System.Windows.Forms.TextBox();
            this.lblHipCareContextDisplay = new System.Windows.Forms.Label();
            this.txtHipCareContextRef = new System.Windows.Forms.TextBox();
            this.lblHipCareContextRef = new System.Windows.Forms.Label();
            this.txtHipPatientMobile = new System.Windows.Forms.TextBox();
            this.lblHipPatientMobile = new System.Windows.Forms.Label();
            this.txtHipPatientRef = new System.Windows.Forms.TextBox();
            this.lblHipPatientRef = new System.Windows.Forms.Label();
            this.txtHipAbhaAddress = new System.Windows.Forms.TextBox();
            this.lblHipAbhaAddress = new System.Windows.Forms.Label();
            this.gbHiu = new System.Windows.Forms.GroupBox();
            this.btnGetData = new System.Windows.Forms.Button();
            this.txtHiuTxnId = new System.Windows.Forms.TextBox();
            this.lblHiuTxnId = new System.Windows.Forms.Label();
            this.btnFetchRecords = new System.Windows.Forms.Button();
            this.txtHiuConsentId = new System.Windows.Forms.TextBox();
            this.lblHiuConsentId = new System.Windows.Forms.Label();
            this.btnCheckConsentStatus = new System.Windows.Forms.Button();
            this.txtHiuConsentReqId = new System.Windows.Forms.TextBox();
            this.lblHiuConsentReqId = new System.Windows.Forms.Label();
            this.btnRequestConsent = new System.Windows.Forms.Button();
            this.txtHiuEraseAt = new System.Windows.Forms.TextBox();
            this.lblHiuEraseAt = new System.Windows.Forms.Label();
            this.txtHiuDateTo = new System.Windows.Forms.TextBox();
            this.lblHiuDateTo = new System.Windows.Forms.Label();
            this.txtHiuDateFrom = new System.Windows.Forms.TextBox();
            this.lblHiuDateFrom = new System.Windows.Forms.Label();
            this.chkDiagnostic = new System.Windows.Forms.CheckBox();
            this.chkPrescription = new System.Windows.Forms.CheckBox();
            this.chkConsultation = new System.Windows.Forms.CheckBox();
            this.lblHiTypes = new System.Windows.Forms.Label();
            this.cmbPurpose = new System.Windows.Forms.ComboBox();
            this.lblPurpose = new System.Windows.Forms.Label();
            this.txtHiuPatientAbha = new System.Windows.Forms.TextBox();
            this.lblHiuPatientAbha = new System.Windows.Forms.Label();
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.pnlHeader.SuspendLayout();
            this.gbHip.SuspendLayout();
            this.gbHiu.SuspendLayout();
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
            this.lblTitle.Text = "Milestone 2 (M2) Verification: Consent & Data Exchange";
            // 
            // gbHip
            // 
            this.gbHip.Controls.Add(this.txtHipDob);
            this.gbHip.Controls.Add(this.lblHipDob);
            this.gbHip.Controls.Add(this.cmbHipGender);
            this.gbHip.Controls.Add(this.lblHipGender);
            this.gbHip.Controls.Add(this.txtHipPatientName);
            this.gbHip.Controls.Add(this.lblHipPatientName);
            this.gbHip.Controls.Add(this.btnSendSms);
            this.gbHip.Controls.Add(this.btnCheckLinkStatus);
            this.gbHip.Controls.Add(this.txtHipLinkId);
            this.gbHip.Controls.Add(this.lblHipLinkId);
            this.gbHip.Controls.Add(this.btnInitiateLink);
            this.gbHip.Controls.Add(this.btnRegisterPatient);
            this.gbHip.Controls.Add(this.txtHipCareContextDisplay);
            this.gbHip.Controls.Add(this.lblHipCareContextDisplay);
            this.gbHip.Controls.Add(this.txtHipCareContextRef);
            this.gbHip.Controls.Add(this.lblHipCareContextRef);
            this.gbHip.Controls.Add(this.txtHipPatientMobile);
            this.gbHip.Controls.Add(this.lblHipPatientMobile);
            this.gbHip.Controls.Add(this.txtHipPatientRef);
            this.gbHip.Controls.Add(this.lblHipPatientRef);
            this.gbHip.Controls.Add(this.txtHipAbhaAddress);
            this.gbHip.Controls.Add(this.lblHipAbhaAddress);
            this.gbHip.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbHip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(54)))));
            this.gbHip.BackColor = System.Drawing.Color.White;
            this.gbHip.Location = new System.Drawing.Point(15, 65);
            this.gbHip.Name = "gbHip";
            this.gbHip.Size = new System.Drawing.Size(460, 460);
            this.gbHip.TabIndex = 1;
            this.gbHip.TabStop = false;
            this.gbHip.Text = "HIP Operations (Care Context Linking)";
            // 
            // txtHipDob
            // 
            this.txtHipDob.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHipDob.Location = new System.Drawing.Point(135, 120);
            this.txtHipDob.Name = "txtHipDob";
            this.txtHipDob.Size = new System.Drawing.Size(305, 27);
            this.txtHipDob.TabIndex = 7;
            this.txtHipDob.Text = "2000-01-01";
            // 
            // lblHipDob
            // 
            this.lblHipDob.AutoSize = true;
            this.lblHipDob.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipDob.ForeColor = System.Drawing.Color.Black;
            this.lblHipDob.Location = new System.Drawing.Point(15, 123);
            this.lblHipDob.Name = "lblHipDob";
            this.lblHipDob.Size = new System.Drawing.Size(97, 20);
            this.lblHipDob.TabIndex = 6;
            this.lblHipDob.Text = "Date of Birth:";
            // 
            // cmbHipGender
            // 
            this.cmbHipGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHipGender.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbHipGender.FormattingEnabled = true;
            this.cmbHipGender.Items.AddRange(new object[] {
            "M",
            "F",
            "O"});
            this.cmbHipGender.Location = new System.Drawing.Point(135, 90);
            this.cmbHipGender.Name = "cmbHipGender";
            this.cmbHipGender.Size = new System.Drawing.Size(305, 28);
            this.cmbHipGender.TabIndex = 5;
            this.cmbHipGender.SelectedIndex = 0;
            // 
            // lblHipGender
            // 
            this.lblHipGender.AutoSize = true;
            this.lblHipGender.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipGender.ForeColor = System.Drawing.Color.Black;
            this.lblHipGender.Location = new System.Drawing.Point(15, 93);
            this.lblHipGender.Name = "lblHipGender";
            this.lblHipGender.Size = new System.Drawing.Size(60, 20);
            this.lblHipGender.TabIndex = 4;
            this.lblHipGender.Text = "Gender:";
            // 
            // txtHipPatientName
            // 
            this.txtHipPatientName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHipPatientName.Location = new System.Drawing.Point(135, 60);
            this.txtHipPatientName.Name = "txtHipPatientName";
            this.txtHipPatientName.Size = new System.Drawing.Size(305, 27);
            this.txtHipPatientName.TabIndex = 3;
            this.txtHipPatientName.Text = "John Doe";
            // 
            // lblHipPatientName
            // 
            this.lblHipPatientName.AutoSize = true;
            this.lblHipPatientName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipPatientName.ForeColor = System.Drawing.Color.Black;
            this.lblHipPatientName.Location = new System.Drawing.Point(15, 63);
            this.lblHipPatientName.Name = "lblHipPatientName";
            this.lblHipPatientName.Size = new System.Drawing.Size(101, 20);
            this.lblHipPatientName.TabIndex = 2;
            this.lblHipPatientName.Text = "Patient Name:";
            // 
            // btnSendSms
            // 
            this.btnSendSms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            
            this.btnSendSms.FlatAppearance.BorderSize = 0;
            this.btnSendSms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendSms.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSendSms.Location = new System.Drawing.Point(15, 412);
            this.btnSendSms.Name = "btnSendSms";
            this.btnSendSms.Size = new System.Drawing.Size(425, 35);
            this.btnSendSms.TabIndex = 21;
            this.btnSendSms.Text = "Send SMS Notification";
            this.btnSendSms.UseVisualStyleBackColor = false;
            this.btnSendSms.Click += new System.EventHandler(this.btnSendSms_Click);
            
            this.btnSendSms.ForeColor = System.Drawing.Color.White;// 
            // btnCheckLinkStatus
            // 
            this.btnCheckLinkStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            
            this.btnCheckLinkStatus.FlatAppearance.BorderSize = 0;
            this.btnCheckLinkStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckLinkStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCheckLinkStatus.Location = new System.Drawing.Point(15, 372);
            this.btnCheckLinkStatus.Name = "btnCheckLinkStatus";
            this.btnCheckLinkStatus.Size = new System.Drawing.Size(425, 35);
            this.btnCheckLinkStatus.TabIndex = 20;
            this.btnCheckLinkStatus.Text = "3. Check Link Status";
            this.btnCheckLinkStatus.UseVisualStyleBackColor = false;
            this.btnCheckLinkStatus.Click += new System.EventHandler(this.btnCheckLinkStatus_Click);
            
            this.btnCheckLinkStatus.ForeColor = System.Drawing.Color.White;// 
            // txtHipLinkId
            // 
            this.txtHipLinkId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHipLinkId.Location = new System.Drawing.Point(135, 340);
            this.txtHipLinkId.Name = "txtHipLinkId";
            this.txtHipLinkId.Size = new System.Drawing.Size(305, 27);
            this.txtHipLinkId.TabIndex = 19;
            // 
            // lblHipLinkId
            // 
            this.lblHipLinkId.AutoSize = true;
            this.lblHipLinkId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipLinkId.ForeColor = System.Drawing.Color.Black;
            this.lblHipLinkId.Location = new System.Drawing.Point(15, 343);
            this.lblHipLinkId.Name = "lblHipLinkId";
            this.lblHipLinkId.Size = new System.Drawing.Size(113, 20);
            this.lblHipLinkId.TabIndex = 18;
            this.lblHipLinkId.Text = "Link Request ID:";
            // 
            // btnInitiateLink
            // 
            this.btnInitiateLink.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            
            this.btnInitiateLink.FlatAppearance.BorderSize = 0;
            this.btnInitiateLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInitiateLink.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnInitiateLink.Location = new System.Drawing.Point(15, 300);
            this.btnInitiateLink.Name = "btnInitiateLink";
            this.btnInitiateLink.Size = new System.Drawing.Size(425, 35);
            this.btnInitiateLink.TabIndex = 17;
            this.btnInitiateLink.Text = "2. Link Care Contexts";
            this.btnInitiateLink.UseVisualStyleBackColor = false;
            this.btnInitiateLink.Click += new System.EventHandler(this.btnInitiateLink_Click);
            
            this.btnInitiateLink.ForeColor = System.Drawing.Color.White;// 
            // btnRegisterPatient
            // 
            this.btnRegisterPatient.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            
            this.btnRegisterPatient.FlatAppearance.BorderSize = 0;
            this.btnRegisterPatient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegisterPatient.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRegisterPatient.Location = new System.Drawing.Point(15, 260);
            this.btnRegisterPatient.Name = "btnRegisterPatient";
            this.btnRegisterPatient.Size = new System.Drawing.Size(425, 35);
            this.btnRegisterPatient.TabIndex = 16;
            this.btnRegisterPatient.Text = "1. Register Patient in DB";
            this.btnRegisterPatient.UseVisualStyleBackColor = false;
            this.btnRegisterPatient.Click += new System.EventHandler(this.btnRegisterPatient_Click);
            
            this.btnRegisterPatient.ForeColor = System.Drawing.Color.White;// 
            // txtHipCareContextDisplay
            // 
            this.txtHipCareContextDisplay.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHipCareContextDisplay.Location = new System.Drawing.Point(135, 227);
            this.txtHipCareContextDisplay.Name = "txtHipCareContextDisplay";
            this.txtHipCareContextDisplay.Size = new System.Drawing.Size(305, 27);
            this.txtHipCareContextDisplay.TabIndex = 15;
            this.txtHipCareContextDisplay.Text = "OPD Consultation - Ortho";
            // 
            // lblHipCareContextDisplay
            // 
            this.lblHipCareContextDisplay.AutoSize = true;
            this.lblHipCareContextDisplay.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipCareContextDisplay.ForeColor = System.Drawing.Color.Black;
            this.lblHipCareContextDisplay.Location = new System.Drawing.Point(15, 230);
            this.lblHipCareContextDisplay.Name = "lblHipCareContextDisplay";
            this.lblHipCareContextDisplay.Size = new System.Drawing.Size(91, 20);
            this.lblHipCareContextDisplay.TabIndex = 14;
            this.lblHipCareContextDisplay.Text = "CC Display :";
            // 
            // txtHipCareContextRef
            // 
            this.txtHipCareContextRef.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHipCareContextRef.Location = new System.Drawing.Point(135, 200);
            this.txtHipCareContextRef.Name = "txtHipCareContextRef";
            this.txtHipCareContextRef.Size = new System.Drawing.Size(305, 27);
            this.txtHipCareContextRef.TabIndex = 13;
            this.txtHipCareContextRef.Text = "CC-1001";
            // 
            // lblHipCareContextRef
            // 
            this.lblHipCareContextRef.AutoSize = true;
            this.lblHipCareContextRef.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipCareContextRef.ForeColor = System.Drawing.Color.Black;
            this.lblHipCareContextRef.Location = new System.Drawing.Point(15, 203);
            this.lblHipCareContextRef.Name = "lblHipCareContextRef";
            this.lblHipCareContextRef.Size = new System.Drawing.Size(89, 20);
            this.lblHipCareContextRef.TabIndex = 12;
            this.lblHipCareContextRef.Text = "CC Ref Num:";
            // 
            // txtHipPatientMobile
            // 
            this.txtHipPatientMobile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHipPatientMobile.Location = new System.Drawing.Point(135, 173);
            this.txtHipPatientMobile.Name = "txtHipPatientMobile";
            this.txtHipPatientMobile.Size = new System.Drawing.Size(305, 27);
            this.txtHipPatientMobile.TabIndex = 11;
            this.txtHipPatientMobile.Text = "9876543210";
            // 
            // lblHipPatientMobile
            // 
            this.lblHipPatientMobile.AutoSize = true;
            this.lblHipPatientMobile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipPatientMobile.ForeColor = System.Drawing.Color.Black;
            this.lblHipPatientMobile.Location = new System.Drawing.Point(15, 176);
            this.lblHipPatientMobile.Name = "lblHipPatientMobile";
            this.lblHipPatientMobile.Size = new System.Drawing.Size(59, 20);
            this.lblHipPatientMobile.TabIndex = 10;
            this.lblHipPatientMobile.Text = "Mobile:";
            // 
            // txtHipPatientRef
            // 
            this.txtHipPatientRef.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHipPatientRef.Location = new System.Drawing.Point(135, 146);
            this.txtHipPatientRef.Name = "txtHipPatientRef";
            this.txtHipPatientRef.Size = new System.Drawing.Size(305, 27);
            this.txtHipPatientRef.TabIndex = 9;
            this.txtHipPatientRef.Text = "HIP-PAT-1001";
            // 
            // lblHipPatientRef
            // 
            this.lblHipPatientRef.AutoSize = true;
            this.lblHipPatientRef.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipPatientRef.ForeColor = System.Drawing.Color.Black;
            this.lblHipPatientRef.Location = new System.Drawing.Point(15, 149);
            this.lblHipPatientRef.Name = "lblHipPatientRef";
            this.lblHipPatientRef.Size = new System.Drawing.Size(87, 20);
            this.lblHipPatientRef.TabIndex = 8;
            this.lblHipPatientRef.Text = "Patient Ref:";
            // 
            // txtHipAbhaAddress
            // 
            this.txtHipAbhaAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHipAbhaAddress.Location = new System.Drawing.Point(135, 30);
            this.txtHipAbhaAddress.Name = "txtHipAbhaAddress";
            this.txtHipAbhaAddress.Size = new System.Drawing.Size(305, 27);
            this.txtHipAbhaAddress.TabIndex = 1;
            this.txtHipAbhaAddress.Text = "testpatient123@sbx";
            // 
            // lblHipAbhaAddress
            // 
            this.lblHipAbhaAddress.AutoSize = true;
            this.lblHipAbhaAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHipAbhaAddress.ForeColor = System.Drawing.Color.Black;
            this.lblHipAbhaAddress.Location = new System.Drawing.Point(15, 33);
            this.lblHipAbhaAddress.Name = "lblHipAbhaAddress";
            this.lblHipAbhaAddress.Size = new System.Drawing.Size(107, 20);
            this.lblHipAbhaAddress.TabIndex = 0;
            this.lblHipAbhaAddress.Text = "ABHA Address:";
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
            this.gbHiu.Controls.Add(this.chkDiagnostic);
            this.gbHiu.Controls.Add(this.chkPrescription);
            this.gbHiu.Controls.Add(this.chkConsultation);
            this.gbHiu.Controls.Add(this.lblHiTypes);
            this.gbHiu.Controls.Add(this.cmbPurpose);
            this.gbHiu.Controls.Add(this.lblPurpose);
            this.gbHiu.Controls.Add(this.txtHiuPatientAbha);
            this.gbHiu.Controls.Add(this.lblHiuPatientAbha);
            this.gbHiu.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbHiu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.gbHiu.BackColor = System.Drawing.Color.White;
            this.gbHiu.Location = new System.Drawing.Point(495, 65);
            this.gbHiu.Name = "gbHiu";
            this.gbHiu.Size = new System.Drawing.Size(470, 460);
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
            this.btnGetData.Location = new System.Drawing.Point(20, 412);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(435, 35);
            this.btnGetData.TabIndex = 23;
            this.btnGetData.Text = "4. View Decrypted Data";
            this.btnGetData.UseVisualStyleBackColor = false;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            
            this.btnGetData.ForeColor = System.Drawing.Color.White;// 
            // txtHiuTxnId
            // 
            this.txtHiuTxnId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuTxnId.Location = new System.Drawing.Point(145, 380);
            this.txtHiuTxnId.Name = "txtHiuTxnId";
            this.txtHiuTxnId.Size = new System.Drawing.Size(310, 27);
            this.txtHiuTxnId.TabIndex = 22;
            // 
            // lblHiuTxnId
            // 
            this.lblHiuTxnId.AutoSize = true;
            this.lblHiuTxnId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuTxnId.ForeColor = System.Drawing.Color.Black;
            this.lblHiuTxnId.Location = new System.Drawing.Point(20, 383);
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
            this.btnFetchRecords.Location = new System.Drawing.Point(20, 340);
            this.btnFetchRecords.Name = "btnFetchRecords";
            this.btnFetchRecords.Size = new System.Drawing.Size(435, 35);
            this.btnFetchRecords.TabIndex = 20;
            this.btnFetchRecords.Text = "3. Fetch Health Records";
            this.btnFetchRecords.UseVisualStyleBackColor = false;
            this.btnFetchRecords.Click += new System.EventHandler(this.btnFetchRecords_Click);
            
            this.btnFetchRecords.ForeColor = System.Drawing.Color.White;// 
            // txtHiuConsentId
            // 
            this.txtHiuConsentId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuConsentId.Location = new System.Drawing.Point(145, 308);
            this.txtHiuConsentId.Name = "txtHiuConsentId";
            this.txtHiuConsentId.Size = new System.Drawing.Size(310, 27);
            this.txtHiuConsentId.TabIndex = 19;
            // 
            // lblHiuConsentId
            // 
            this.lblHiuConsentId.AutoSize = true;
            this.lblHiuConsentId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuConsentId.ForeColor = System.Drawing.Color.Black;
            this.lblHiuConsentId.Location = new System.Drawing.Point(20, 311);
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
            this.btnCheckConsentStatus.Location = new System.Drawing.Point(20, 268);
            this.btnCheckConsentStatus.Name = "btnCheckConsentStatus";
            this.btnCheckConsentStatus.Size = new System.Drawing.Size(435, 35);
            this.btnCheckConsentStatus.TabIndex = 17;
            this.btnCheckConsentStatus.Text = "2. Check Consent Status";
            this.btnCheckConsentStatus.UseVisualStyleBackColor = false;
            this.btnCheckConsentStatus.Click += new System.EventHandler(this.btnCheckConsentStatus_Click);
            
            this.btnCheckConsentStatus.ForeColor = System.Drawing.Color.White;// 
            // txtHiuConsentReqId
            // 
            this.txtHiuConsentReqId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuConsentReqId.Location = new System.Drawing.Point(145, 236);
            this.txtHiuConsentReqId.Name = "txtHiuConsentReqId";
            this.txtHiuConsentReqId.Size = new System.Drawing.Size(310, 27);
            this.txtHiuConsentReqId.TabIndex = 16;
            // 
            // lblHiuConsentReqId
            // 
            this.lblHiuConsentReqId.AutoSize = true;
            this.lblHiuConsentReqId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblHiuConsentReqId.ForeColor = System.Drawing.Color.Black;
            this.lblHiuConsentReqId.Location = new System.Drawing.Point(20, 239);
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
            this.btnRequestConsent.Location = new System.Drawing.Point(20, 196);
            this.btnRequestConsent.Name = "btnRequestConsent";
            this.btnRequestConsent.Size = new System.Drawing.Size(435, 35);
            this.btnRequestConsent.TabIndex = 14;
            this.btnRequestConsent.Text = "1. Initiate Consent Request";
            this.btnRequestConsent.UseVisualStyleBackColor = false;
            this.btnRequestConsent.Click += new System.EventHandler(this.btnRequestConsent_Click);
            
            this.btnRequestConsent.ForeColor = System.Drawing.Color.White;// 
            // txtHiuEraseAt
            // 
            this.txtHiuEraseAt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuEraseAt.Location = new System.Drawing.Point(145, 163);
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
            this.lblHiuEraseAt.Location = new System.Drawing.Point(20, 166);
            this.lblHiuEraseAt.Name = "lblHiuEraseAt";
            this.lblHiuEraseAt.Size = new System.Drawing.Size(65, 20);
            this.lblHiuEraseAt.TabIndex = 12;
            this.lblHiuEraseAt.Text = "Erase At:";
            // 
            // txtHiuDateTo
            // 
            this.txtHiuDateTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuDateTo.Location = new System.Drawing.Point(145, 136);
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
            this.lblHiuDateTo.Location = new System.Drawing.Point(20, 139);
            this.lblHiuDateTo.Name = "lblHiuDateTo";
            this.lblHiuDateTo.Size = new System.Drawing.Size(65, 20);
            this.lblHiuDateTo.TabIndex = 10;
            this.lblHiuDateTo.Text = "Date To:";
            // 
            // txtHiuDateFrom
            // 
            this.txtHiuDateFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHiuDateFrom.Location = new System.Drawing.Point(145, 109);
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
            this.lblHiuDateFrom.Location = new System.Drawing.Point(20, 112);
            this.lblHiuDateFrom.Name = "lblHiuDateFrom";
            this.lblHiuDateFrom.Size = new System.Drawing.Size(83, 20);
            this.lblHiuDateFrom.TabIndex = 8;
            this.lblHiuDateFrom.Text = "Date From:";
            // 
            // chkDiagnostic
            // 
            this.chkDiagnostic.AutoSize = true;
            this.chkDiagnostic.Checked = true;
            this.chkDiagnostic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDiagnostic.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chkDiagnostic.ForeColor = System.Drawing.Color.Black;
            this.chkDiagnostic.Location = new System.Drawing.Point(340, 83);
            this.chkDiagnostic.Name = "chkDiagnostic";
            this.chkDiagnostic.Size = new System.Drawing.Size(102, 24);
            this.chkDiagnostic.TabIndex = 7;
            this.chkDiagnostic.Text = "Diag Report";
            this.chkDiagnostic.UseVisualStyleBackColor = true;
            // 
            // chkPrescription
            // 
            this.chkPrescription.AutoSize = true;
            this.chkPrescription.Checked = true;
            this.chkPrescription.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPrescription.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chkPrescription.ForeColor = System.Drawing.Color.Black;
            this.chkPrescription.Location = new System.Drawing.Point(235, 83);
            this.chkPrescription.Name = "chkPrescription";
            this.chkPrescription.Size = new System.Drawing.Size(109, 24);
            this.chkPrescription.TabIndex = 6;
            this.chkPrescription.Text = "Prescription";
            this.chkPrescription.UseVisualStyleBackColor = true;
            // 
            // chkConsultation
            // 
            this.chkConsultation.AutoSize = true;
            this.chkConsultation.Checked = true;
            this.chkConsultation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkConsultation.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chkConsultation.ForeColor = System.Drawing.Color.Black;
            this.chkConsultation.Location = new System.Drawing.Point(145, 83);
            this.chkConsultation.Name = "chkConsultation";
            this.chkConsultation.Size = new System.Drawing.Size(89, 24);
            this.chkConsultation.TabIndex = 5;
            this.chkConsultation.Text = "OP Consult";
            this.chkConsultation.UseVisualStyleBackColor = true;
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
            this.gbLog.Controls.Add(this.txtLog);
            this.gbLog.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbLog.ForeColor = System.Drawing.Color.Black;
            this.gbLog.Location = new System.Drawing.Point(15, 530);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(950, 195);
            this.gbLog.TabIndex = 3;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Execution Output / Log";
            this.gbLog.BackColor = System.Drawing.Color.White;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.Location = new System.Drawing.Point(3, 25);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(944, 167);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "Ready to test Milestone 2...";
            // 
            // frmABDMM2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(980, 740);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gbHiu);
            this.Controls.Add(this.gbHip);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmABDMM2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "M2 Consent & Data Flow Verification";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.gbHip.ResumeLayout(false);
            this.gbHip.PerformLayout();
            this.gbHiu.ResumeLayout(false);
            this.gbHiu.PerformLayout();
            this.gbLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox gbHip;
        private System.Windows.Forms.TextBox txtHipAbhaAddress;
        private System.Windows.Forms.Label lblHipAbhaAddress;
        private System.Windows.Forms.TextBox txtHipPatientRef;
        private System.Windows.Forms.Label lblHipPatientRef;
        private System.Windows.Forms.TextBox txtHipPatientMobile;
        private System.Windows.Forms.Label lblHipPatientMobile;
        private System.Windows.Forms.TextBox txtHipCareContextRef;
        private System.Windows.Forms.Label lblHipCareContextRef;
        private System.Windows.Forms.TextBox txtHipCareContextDisplay;
        private System.Windows.Forms.Label lblHipCareContextDisplay;
        private System.Windows.Forms.Button btnRegisterPatient;
        private System.Windows.Forms.Button btnInitiateLink;
        private System.Windows.Forms.TextBox txtHipLinkId;
        private System.Windows.Forms.Label lblHipLinkId;
        private System.Windows.Forms.Button btnCheckLinkStatus;
        private System.Windows.Forms.Button btnSendSms;
        private System.Windows.Forms.GroupBox gbHiu;
        private System.Windows.Forms.TextBox txtHiuPatientAbha;
        private System.Windows.Forms.Label lblHiuPatientAbha;
        private System.Windows.Forms.Label lblPurpose;
        private System.Windows.Forms.ComboBox cmbPurpose;
        private System.Windows.Forms.Label lblHiTypes;
        private System.Windows.Forms.CheckBox chkConsultation;
        private System.Windows.Forms.CheckBox chkPrescription;
        private System.Windows.Forms.CheckBox chkDiagnostic;
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
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.TextBox txtHipDob;
        private System.Windows.Forms.Label lblHipDob;
        private System.Windows.Forms.ComboBox cmbHipGender;
        private System.Windows.Forms.Label lblHipGender;
        private System.Windows.Forms.TextBox txtHipPatientName;
        private System.Windows.Forms.Label lblHipPatientName;
    }
}
