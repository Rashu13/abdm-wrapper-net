namespace HMS.abdm
{
    partial class frmPrescription
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
            this.gbPatient = new System.Windows.Forms.GroupBox();
            this.lblAbha = new System.Windows.Forms.Label();
            this.txtAbhaAddress = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtPatientName = new System.Windows.Forms.TextBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.lblDob = new System.Windows.Forms.Label();
            this.txtDob = new System.Windows.Forms.TextBox();
            this.lblMobile = new System.Windows.Forms.Label();
            this.txtPatientMobile = new System.Windows.Forms.TextBox();
            this.lblPatRef = new System.Windows.Forms.Label();
            this.txtPatientRef = new System.Windows.Forms.TextBox();
            this.lblCareCtxRef = new System.Windows.Forms.Label();
            this.txtCareContextRef = new System.Windows.Forms.TextBox();
            this.lblCareCtxDisplay = new System.Windows.Forms.Label();
            this.txtCareContextDisplay = new System.Windows.Forms.TextBox();
            this.lblRecordType = new System.Windows.Forms.Label();
            this.cmbRecordType = new System.Windows.Forms.ComboBox();
            this.gbPrescribe = new System.Windows.Forms.GroupBox();
            this.lblMedicine = new System.Windows.Forms.Label();
            this.txtMedicineName = new System.Windows.Forms.TextBox();
            this.lblDosage = new System.Windows.Forms.Label();
            this.txtDosage = new System.Windows.Forms.TextBox();
            this.btnAddMedicine = new System.Windows.Forms.Button();
            this.btnRemoveMedicine = new System.Windows.Forms.Button();
            this.lvMedicines = new System.Windows.Forms.ListView();
            this.chMedicine = new System.Windows.Forms.ColumnHeader();
            this.chDosage = new System.Windows.Forms.ColumnHeader();
            this.gbPdf = new System.Windows.Forms.GroupBox();
            this.btnUploadPdf = new System.Windows.Forms.Button();
            this.lblPdfStatus = new System.Windows.Forms.Label();
            this.gbActions = new System.Windows.Forms.GroupBox();
            this.btnSaveAndPush = new System.Windows.Forms.Button();
            this.btnInitiateLink = new System.Windows.Forms.Button();
            this.btnAutoFlow = new System.Windows.Forms.Button();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.lblLogs = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.gbPatient.SuspendLayout();
            this.gbPrescribe.SuspendLayout();
            this.gbPdf.SuspendLayout();
            this.gbActions.SuspendLayout();
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
            this.pnlHeader.Size = new System.Drawing.Size(950, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(900, 0);
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
            this.lblTitle.Size = new System.Drawing.Size(374, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Write Prescription & Upload Document (PDF)";
            // 
            // gbPatient
            // 
            this.gbPatient.Controls.Add(this.lblRecordType);
            this.gbPatient.Controls.Add(this.cmbRecordType);
            this.gbPatient.Controls.Add(this.lblCareCtxDisplay);
            this.gbPatient.Controls.Add(this.txtCareContextDisplay);
            this.gbPatient.Controls.Add(this.lblCareCtxRef);
            this.gbPatient.Controls.Add(this.txtCareContextRef);
            this.gbPatient.Controls.Add(this.lblPatRef);
            this.gbPatient.Controls.Add(this.txtPatientRef);
            this.gbPatient.Controls.Add(this.lblMobile);
            this.gbPatient.Controls.Add(this.txtPatientMobile);
            this.gbPatient.Controls.Add(this.lblDob);
            this.gbPatient.Controls.Add(this.txtDob);
            this.gbPatient.Controls.Add(this.lblGender);
            this.gbPatient.Controls.Add(this.cmbGender);
            this.gbPatient.Controls.Add(this.lblName);
            this.gbPatient.Controls.Add(this.txtPatientName);
            this.gbPatient.Controls.Add(this.lblAbha);
            this.gbPatient.Controls.Add(this.txtAbhaAddress);
            this.gbPatient.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.gbPatient.Location = new System.Drawing.Point(15, 65);
            this.gbPatient.Name = "gbPatient";
            this.gbPatient.Size = new System.Drawing.Size(430, 395);
            this.gbPatient.TabIndex = 1;
            this.gbPatient.TabStop = false;
            this.gbPatient.Text = "Patient & Care Context Details";
            // 
            // lblAbha
            // 
            this.lblAbha.AutoSize = true;
            this.lblAbha.Location = new System.Drawing.Point(15, 30);
            this.lblAbha.Name = "lblAbha";
            this.lblAbha.Size = new System.Drawing.Size(126, 21);
            this.lblAbha.TabIndex = 0;
            this.lblAbha.Text = "ABHA Address *";
            // 
            // txtAbhaAddress
            // 
            this.txtAbhaAddress.Location = new System.Drawing.Point(170, 27);
            this.txtAbhaAddress.Name = "txtAbhaAddress";
            this.txtAbhaAddress.Size = new System.Drawing.Size(240, 29);
            this.txtAbhaAddress.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(15, 70);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(117, 21);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Patient Name *";
            // 
            // txtPatientName
            // 
            this.txtPatientName.Location = new System.Drawing.Point(170, 67);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.Size = new System.Drawing.Size(240, 29);
            this.txtPatientName.TabIndex = 3;
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(15, 110);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(64, 21);
            this.lblGender.TabIndex = 4;
            this.lblGender.Text = "Gender";
            // 
            // cmbGender
            // 
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Items.AddRange(new object[] {
            "M",
            "F",
            "O"});
            this.cmbGender.Location = new System.Drawing.Point(170, 107);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(240, 29);
            this.cmbGender.TabIndex = 5;
            // 
            // lblDob
            // 
            this.lblDob.AutoSize = true;
            this.lblDob.Location = new System.Drawing.Point(15, 150);
            this.lblDob.Name = "lblDob";
            this.lblDob.Size = new System.Drawing.Size(126, 21);
            this.lblDob.TabIndex = 6;
            this.lblDob.Text = "DOB (YYYY-MM-DD)";
            // 
            // txtDob
            // 
            this.txtDob.Location = new System.Drawing.Point(170, 147);
            this.txtDob.Name = "txtDob";
            this.txtDob.Size = new System.Drawing.Size(240, 29);
            this.txtDob.TabIndex = 7;
            this.txtDob.Text = "1990-01-01";
            // 
            // lblMobile
            // 
            this.lblMobile.AutoSize = true;
            this.lblMobile.Location = new System.Drawing.Point(15, 190);
            this.lblMobile.Name = "lblMobile";
            this.lblMobile.Size = new System.Drawing.Size(61, 21);
            this.lblMobile.TabIndex = 8;
            this.lblMobile.Text = "Mobile";
            // 
            // txtPatientMobile
            // 
            this.txtPatientMobile.Location = new System.Drawing.Point(170, 187);
            this.txtPatientMobile.Name = "txtPatientMobile";
            this.txtPatientMobile.Size = new System.Drawing.Size(240, 29);
            this.txtPatientMobile.TabIndex = 9;
            this.txtPatientMobile.Text = "9876543210";
            // 
            // lblPatRef
            // 
            this.lblPatRef.AutoSize = true;
            this.lblPatRef.Location = new System.Drawing.Point(15, 230);
            this.lblPatRef.Name = "lblPatRef";
            this.lblPatRef.Size = new System.Drawing.Size(140, 21);
            this.lblPatRef.TabIndex = 10;
            this.lblPatRef.Text = "Patient Reference";
            // 
            // txtPatientRef
            // 
            this.txtPatientRef.Location = new System.Drawing.Point(170, 227);
            this.txtPatientRef.Name = "txtPatientRef";
            this.txtPatientRef.Size = new System.Drawing.Size(240, 29);
            this.txtPatientRef.TabIndex = 11;
            // 
            // lblCareCtxRef
            // 
            this.lblCareCtxRef.AutoSize = true;
            this.lblCareCtxRef.Location = new System.Drawing.Point(15, 270);
            this.lblCareCtxRef.Name = "lblCareCtxRef";
            this.lblCareCtxRef.Size = new System.Drawing.Size(141, 21);
            this.lblCareCtxRef.TabIndex = 12;
            this.lblCareCtxRef.Text = "Care Context Ref *";
            // 
            // txtCareContextRef
            // 
            this.txtCareContextRef.Location = new System.Drawing.Point(170, 267);
            this.txtCareContextRef.Name = "txtCareContextRef";
            this.txtCareContextRef.Size = new System.Drawing.Size(240, 29);
            this.txtCareContextRef.TabIndex = 13;
            // 
            // lblCareCtxDisplay
            // 
            this.lblCareCtxDisplay.AutoSize = true;
            this.lblCareCtxDisplay.Location = new System.Drawing.Point(15, 310);
            this.lblCareCtxDisplay.Name = "lblCareCtxDisplay";
            this.lblCareCtxDisplay.Size = new System.Drawing.Size(148, 21);
            this.lblCareCtxDisplay.TabIndex = 14;
            this.lblCareCtxDisplay.Text = "Care Context Disp *";
            // 
            // txtCareContextDisplay
            // 
            this.txtCareContextDisplay.Location = new System.Drawing.Point(170, 307);
            this.txtCareContextDisplay.Name = "txtCareContextDisplay";
            this.txtCareContextDisplay.Size = new System.Drawing.Size(240, 29);
            this.txtCareContextDisplay.TabIndex = 15;
            // 
            // lblRecordType
            // 
            this.lblRecordType.AutoSize = true;
            this.lblRecordType.Location = new System.Drawing.Point(15, 350);
            this.lblRecordType.Name = "lblRecordType";
            this.lblRecordType.Size = new System.Drawing.Size(100, 21);
            this.lblRecordType.TabIndex = 16;
            this.lblRecordType.Text = "Record Type";
            // 
            // cmbRecordType
            // 
            this.cmbRecordType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordType.FormattingEnabled = true;
            this.cmbRecordType.Items.AddRange(new object[] {
            "PrescriptionRecord",
            "OPConsultationRecord",
            "HealthDocumentRecord"});
            this.cmbRecordType.Location = new System.Drawing.Point(170, 347);
            this.cmbRecordType.Name = "cmbRecordType";
            this.cmbRecordType.Size = new System.Drawing.Size(240, 29);
            this.cmbRecordType.TabIndex = 17;
            // 
            // gbPrescribe
            // 
            this.gbPrescribe.Controls.Add(this.lvMedicines);
            this.gbPrescribe.Controls.Add(this.btnRemoveMedicine);
            this.gbPrescribe.Controls.Add(this.btnAddMedicine);
            this.gbPrescribe.Controls.Add(this.lblDosage);
            this.gbPrescribe.Controls.Add(this.txtDosage);
            this.gbPrescribe.Controls.Add(this.lblMedicine);
            this.gbPrescribe.Controls.Add(this.txtMedicineName);
            this.gbPrescribe.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.gbPrescribe.Location = new System.Drawing.Point(460, 65);
            this.gbPrescribe.Name = "gbPrescribe";
            this.gbPrescribe.Size = new System.Drawing.Size(475, 290);
            this.gbPrescribe.TabIndex = 2;
            this.gbPrescribe.TabStop = false;
            this.gbPrescribe.Text = "Write Medicine (Dwai)";
            // 
            // lblMedicine
            // 
            this.lblMedicine.AutoSize = true;
            this.lblMedicine.Location = new System.Drawing.Point(15, 30);
            this.lblMedicine.Name = "lblMedicine";
            this.lblMedicine.Size = new System.Drawing.Size(122, 21);
            this.lblMedicine.TabIndex = 0;
            this.lblMedicine.Text = "Medicine Name";
            // 
            // txtMedicineName
            // 
            this.txtMedicineName.Location = new System.Drawing.Point(140, 27);
            this.txtMedicineName.Name = "txtMedicineName";
            this.txtMedicineName.Size = new System.Drawing.Size(180, 29);
            this.txtMedicineName.TabIndex = 1;
            // 
            // lblDosage
            // 
            this.lblDosage.AutoSize = true;
            this.lblDosage.Location = new System.Drawing.Point(15, 70);
            this.lblDosage.Name = "lblDosage";
            this.lblDosage.Size = new System.Drawing.Size(63, 21);
            this.lblDosage.TabIndex = 2;
            this.lblDosage.Text = "Dosage";
            // 
            // txtDosage
            // 
            this.txtDosage.Location = new System.Drawing.Point(140, 67);
            this.txtDosage.Name = "txtDosage";
            this.txtDosage.Size = new System.Drawing.Size(180, 29);
            this.txtDosage.TabIndex = 3;
            this.txtDosage.Text = "1-0-1";
            // 
            // btnAddMedicine
            // 
            this.btnAddMedicine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnAddMedicine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddMedicine.ForeColor = System.Drawing.Color.White;
            this.btnAddMedicine.Location = new System.Drawing.Point(340, 25);
            this.btnAddMedicine.Name = "btnAddMedicine";
            this.btnAddMedicine.Size = new System.Drawing.Size(120, 32);
            this.btnAddMedicine.TabIndex = 4;
            this.btnAddMedicine.Text = "Add Medicine";
            this.btnAddMedicine.UseVisualStyleBackColor = false;
            this.btnAddMedicine.Click += new System.EventHandler(this.btnAddMedicine_Click);
            // 
            // btnRemoveMedicine
            // 
            this.btnRemoveMedicine.BackColor = System.Drawing.Color.DarkRed;
            this.btnRemoveMedicine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveMedicine.ForeColor = System.Drawing.Color.White;
            this.btnRemoveMedicine.Location = new System.Drawing.Point(340, 65);
            this.btnRemoveMedicine.Name = "btnRemoveMedicine";
            this.btnRemoveMedicine.Size = new System.Drawing.Size(120, 32);
            this.btnRemoveMedicine.TabIndex = 5;
            this.btnRemoveMedicine.Text = "Remove";
            this.btnRemoveMedicine.UseVisualStyleBackColor = false;
            this.btnRemoveMedicine.Click += new System.EventHandler(this.btnRemoveMedicine_Click);
            // 
            // lvMedicines
            // 
            this.lvMedicines.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chMedicine,
            this.chDosage});
            this.lvMedicines.FullRowSelect = true;
            this.lvMedicines.GridLines = true;
            this.lvMedicines.HideSelection = false;
            this.lvMedicines.Location = new System.Drawing.Point(20, 110);
            this.lvMedicines.Name = "lvMedicines";
            this.lvMedicines.Size = new System.Drawing.Size(440, 165);
            this.lvMedicines.TabIndex = 6;
            this.lvMedicines.UseCompatibleStateImageBehavior = false;
            this.lvMedicines.View = System.Windows.Forms.View.Details;
            // 
            // chMedicine
            // 
            this.chMedicine.Text = "Medicine Name";
            this.chMedicine.Width = 260;
            // 
            // chDosage
            // 
            this.chDosage.Text = "Dosage";
            this.chDosage.Width = 150;
            // 
            // gbPdf
            // 
            this.gbPdf.Controls.Add(this.lblPdfStatus);
            this.gbPdf.Controls.Add(this.btnUploadPdf);
            this.gbPdf.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.gbPdf.Location = new System.Drawing.Point(460, 360);
            this.gbPdf.Name = "gbPdf";
            this.gbPdf.Size = new System.Drawing.Size(475, 100);
            this.gbPdf.TabIndex = 3;
            this.gbPdf.TabStop = false;
            this.gbPdf.Text = "Attach Prescription PDF File";
            // 
            // btnUploadPdf
            // 
            this.btnUploadPdf.BackColor = System.Drawing.Color.White;
            this.btnUploadPdf.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnUploadPdf.FlatAppearance.BorderSize = 2;
            this.btnUploadPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUploadPdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnUploadPdf.Location = new System.Drawing.Point(20, 35);
            this.btnUploadPdf.Name = "btnUploadPdf";
            this.btnUploadPdf.Size = new System.Drawing.Size(180, 45);
            this.btnUploadPdf.TabIndex = 0;
            this.btnUploadPdf.Text = "Upload PDF";
            this.btnUploadPdf.UseVisualStyleBackColor = false;
            this.btnUploadPdf.Click += new System.EventHandler(this.btnUploadPdf_Click);
            // 
            // lblPdfStatus
            // 
            this.lblPdfStatus.AutoSize = true;
            this.lblPdfStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblPdfStatus.Location = new System.Drawing.Point(220, 47);
            this.lblPdfStatus.Name = "lblPdfStatus";
            this.lblPdfStatus.Size = new System.Drawing.Size(139, 21);
            this.lblPdfStatus.TabIndex = 1;
            this.lblPdfStatus.Text = "No PDF Uploaded";
            // 
            // gbActions
            // 
            this.gbActions.Controls.Add(this.btnAutoFlow);
            this.gbActions.Controls.Add(this.btnInitiateLink);
            this.gbActions.Controls.Add(this.btnSaveAndPush);
            this.gbActions.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.gbActions.Location = new System.Drawing.Point(15, 470);
            this.gbActions.Name = "gbActions";
            this.gbActions.Size = new System.Drawing.Size(430, 210);
            this.gbActions.TabIndex = 4;
            this.gbActions.TabStop = false;
            this.gbActions.Text = "ABDM Integration Actions";
            // 
            // btnSaveAndPush
            // 
            this.btnSaveAndPush.BackColor = System.Drawing.Color.White;
            this.btnSaveAndPush.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnSaveAndPush.FlatAppearance.BorderSize = 2;
            this.btnSaveAndPush.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAndPush.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnSaveAndPush.Location = new System.Drawing.Point(15, 30);
            this.btnSaveAndPush.Name = "btnSaveAndPush";
            this.btnSaveAndPush.Size = new System.Drawing.Size(395, 45);
            this.btnSaveAndPush.TabIndex = 0;
            this.btnSaveAndPush.Text = "1. Save Patient & Push Health Data";
            this.btnSaveAndPush.UseVisualStyleBackColor = false;
            this.btnSaveAndPush.Click += new System.EventHandler(this.btnSaveAndPush_Click);
            // 
            // btnInitiateLink
            // 
            this.btnInitiateLink.BackColor = System.Drawing.Color.White;
            this.btnInitiateLink.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnInitiateLink.FlatAppearance.BorderSize = 2;
            this.btnInitiateLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInitiateLink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnInitiateLink.Location = new System.Drawing.Point(15, 85);
            this.btnInitiateLink.Name = "btnInitiateLink";
            this.btnInitiateLink.Size = new System.Drawing.Size(395, 45);
            this.btnInitiateLink.TabIndex = 1;
            this.btnInitiateLink.Text = "2. Link Care Context on Gateway";
            this.btnInitiateLink.UseVisualStyleBackColor = false;
            this.btnInitiateLink.Click += new System.EventHandler(this.btnInitiateLink_Click);
            // 
            // btnAutoFlow
            // 
            this.btnAutoFlow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.btnAutoFlow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAutoFlow.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAutoFlow.ForeColor = System.Drawing.Color.White;
            this.btnAutoFlow.Location = new System.Drawing.Point(15, 145);
            this.btnAutoFlow.Name = "btnAutoFlow";
            this.btnAutoFlow.Size = new System.Drawing.Size(395, 50);
            this.btnAutoFlow.TabIndex = 2;
            this.btnAutoFlow.Text = "Auto Link & Push (All in One)";
            this.btnAutoFlow.UseVisualStyleBackColor = false;
            this.btnAutoFlow.Click += new System.EventHandler(this.btnAutoFlow_Click);
            // 
            // txtLogs
            // 
            this.txtLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(35)))));
            this.txtLogs.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLogs.ForeColor = System.Drawing.Color.LightGray;
            this.txtLogs.Location = new System.Drawing.Point(460, 500);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogs.Size = new System.Drawing.Size(475, 180);
            this.txtLogs.TabIndex = 5;
            // 
            // lblLogs
            // 
            this.lblLogs.AutoSize = true;
            this.lblLogs.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblLogs.Location = new System.Drawing.Point(460, 475);
            this.lblLogs.Name = "lblLogs";
            this.lblLogs.Size = new System.Drawing.Size(147, 21);
            this.lblLogs.TabIndex = 6;
            this.lblLogs.Text = "Execution Console:";
            // 
            // frmPrescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(950, 700);
            this.Controls.Add(this.lblLogs);
            this.Controls.Add(this.txtLogs);
            this.Controls.Add(this.gbActions);
            this.Controls.Add(this.gbPdf);
            this.Controls.Add(this.gbPrescribe);
            this.Controls.Add(this.gbPatient);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPrescription";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Write Prescription & Upload Document";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.gbPatient.ResumeLayout(false);
            this.gbPatient.PerformLayout();
            this.gbPrescribe.ResumeLayout(false);
            this.gbPrescribe.PerformLayout();
            this.gbPdf.ResumeLayout(false);
            this.gbPdf.PerformLayout();
            this.gbActions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbPatient;
        private System.Windows.Forms.Label lblAbha;
        private System.Windows.Forms.TextBox txtAbhaAddress;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtPatientName;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.ComboBox cmbGender;
        private System.Windows.Forms.Label lblDob;
        private System.Windows.Forms.TextBox txtDob;
        private System.Windows.Forms.Label lblMobile;
        private System.Windows.Forms.TextBox txtPatientMobile;
        private System.Windows.Forms.Label lblPatRef;
        private System.Windows.Forms.TextBox txtPatientRef;
        private System.Windows.Forms.Label lblCareCtxRef;
        private System.Windows.Forms.TextBox txtCareContextRef;
        private System.Windows.Forms.Label lblCareCtxDisplay;
        private System.Windows.Forms.TextBox txtCareContextDisplay;
        private System.Windows.Forms.Label lblRecordType;
        private System.Windows.Forms.ComboBox cmbRecordType;
        private System.Windows.Forms.GroupBox gbPrescribe;
        private System.Windows.Forms.Label lblMedicine;
        private System.Windows.Forms.TextBox txtMedicineName;
        private System.Windows.Forms.Label lblDosage;
        private System.Windows.Forms.TextBox txtDosage;
        private System.Windows.Forms.Button btnAddMedicine;
        private System.Windows.Forms.Button btnRemoveMedicine;
        private System.Windows.Forms.ListView lvMedicines;
        private System.Windows.Forms.ColumnHeader chMedicine;
        private System.Windows.Forms.ColumnHeader chDosage;
        private System.Windows.Forms.GroupBox gbPdf;
        private System.Windows.Forms.Button btnUploadPdf;
        private System.Windows.Forms.Label lblPdfStatus;
        private System.Windows.Forms.GroupBox gbActions;
        private System.Windows.Forms.Button btnSaveAndPush;
        private System.Windows.Forms.Button btnInitiateLink;
        private System.Windows.Forms.Button btnAutoFlow;
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.Label lblLogs;
    }
}
