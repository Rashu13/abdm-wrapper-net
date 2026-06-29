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
            pnlHeader = new Panel();
            btnClose = new Button();
            lblTitle = new Label();
            gbPatient = new GroupBox();
            lblRecordType = new Label();
            cmbRecordType = new ComboBox();
            lblCareCtxDisplay = new Label();
            txtCareContextDisplay = new TextBox();
            lblCareCtxRef = new Label();
            txtCareContextRef = new TextBox();
            lblPatRef = new Label();
            txtPatientRef = new TextBox();
            lblMobile = new Label();
            txtPatientMobile = new TextBox();
            lblDob = new Label();
            txtDob = new TextBox();
            lblGender = new Label();
            cmbGender = new ComboBox();
            lblName = new Label();
            txtPatientName = new TextBox();
            lblAbha = new Label();
            txtAbhaAddress = new TextBox();
            gbPrescribe = new GroupBox();
            lvMedicines = new ListView();
            chMedicine = new ColumnHeader();
            chDosage = new ColumnHeader();
            btnRemoveMedicine = new Button();
            btnAddMedicine = new Button();
            lblDosage = new Label();
            txtDosage = new TextBox();
            lblMedicine = new Label();
            txtMedicineName = new TextBox();
            gbPdf = new GroupBox();
            lblPdfStatus = new Label();
            btnUploadPdf = new Button();
            gbActions = new GroupBox();
            btnAutoFlow = new Button();
            btnInitiateLink = new Button();
            btnSaveAndPush = new Button();
            txtLogs = new TextBox();
            lblLogs = new Label();
            pnlHeader.SuspendLayout();
            gbPatient.SuspendLayout();
            gbPrescribe.SuspendLayout();
            gbPdf.SuspendLayout();
            gbActions.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(55, 115, 200);
            pnlHeader.Controls.Add(btnClose);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(3, 4, 3, 4);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1200, 62);
            pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(1149, 1);
            btnClose.Margin = new Padding(3, 4, 3, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(50, 62);
            btnClose.TabIndex = 1;
            btnClose.Text = "X";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(15, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(409, 28);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Write Prescription & Upload Document (PDF)";
            // 
            // gbPatient
            // 
            gbPatient.Controls.Add(lblRecordType);
            gbPatient.Controls.Add(cmbRecordType);
            gbPatient.Controls.Add(lblCareCtxDisplay);
            gbPatient.Controls.Add(txtCareContextDisplay);
            gbPatient.Controls.Add(lblCareCtxRef);
            gbPatient.Controls.Add(txtCareContextRef);
            gbPatient.Controls.Add(lblPatRef);
            gbPatient.Controls.Add(txtPatientRef);
            gbPatient.Controls.Add(lblMobile);
            gbPatient.Controls.Add(txtPatientMobile);
            gbPatient.Controls.Add(lblDob);
            gbPatient.Controls.Add(txtDob);
            gbPatient.Controls.Add(lblGender);
            gbPatient.Controls.Add(cmbGender);
            gbPatient.Controls.Add(lblName);
            gbPatient.Controls.Add(txtPatientName);
            gbPatient.Controls.Add(lblAbha);
            gbPatient.Controls.Add(txtAbhaAddress);
            gbPatient.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            gbPatient.Location = new Point(15, 81);
            gbPatient.Margin = new Padding(3, 4, 3, 4);
            gbPatient.Name = "gbPatient";
            gbPatient.Padding = new Padding(3, 4, 3, 4);
            gbPatient.Size = new Size(430, 494);
            gbPatient.TabIndex = 1;
            gbPatient.TabStop = false;
            gbPatient.Text = "Patient & Care Context Details";
            // 
            // lblRecordType
            // 
            lblRecordType.AutoSize = true;
            lblRecordType.Location = new Point(15, 438);
            lblRecordType.Name = "lblRecordType";
            lblRecordType.Size = new Size(102, 21);
            lblRecordType.TabIndex = 16;
            lblRecordType.Text = "Record Type";
            // 
            // cmbRecordType
            // 
            cmbRecordType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRecordType.FormattingEnabled = true;
            cmbRecordType.Items.AddRange(new object[] { "PrescriptionRecord", "OPConsultationRecord", "HealthDocumentRecord", "DiagnosticReport", "DischargeSummary" });
            cmbRecordType.Location = new Point(170, 434);
            cmbRecordType.Margin = new Padding(3, 4, 3, 4);
            cmbRecordType.Name = "cmbRecordType";
            cmbRecordType.Size = new Size(240, 29);
            cmbRecordType.TabIndex = 17;
            // 
            // lblCareCtxDisplay
            // 
            lblCareCtxDisplay.AutoSize = true;
            lblCareCtxDisplay.Location = new Point(15, 388);
            lblCareCtxDisplay.Name = "lblCareCtxDisplay";
            lblCareCtxDisplay.Size = new Size(152, 21);
            lblCareCtxDisplay.TabIndex = 14;
            lblCareCtxDisplay.Text = "Care Context Disp *";
            // 
            // txtCareContextDisplay
            // 
            txtCareContextDisplay.Location = new Point(170, 384);
            txtCareContextDisplay.Margin = new Padding(3, 4, 3, 4);
            txtCareContextDisplay.Name = "txtCareContextDisplay";
            txtCareContextDisplay.Size = new Size(240, 29);
            txtCareContextDisplay.TabIndex = 15;
            // 
            // lblCareCtxRef
            // 
            lblCareCtxRef.AutoSize = true;
            lblCareCtxRef.Location = new Point(15, 338);
            lblCareCtxRef.Name = "lblCareCtxRef";
            lblCareCtxRef.Size = new Size(145, 21);
            lblCareCtxRef.TabIndex = 12;
            lblCareCtxRef.Text = "Care Context Ref *";
            // 
            // txtCareContextRef
            // 
            txtCareContextRef.Location = new Point(170, 334);
            txtCareContextRef.Margin = new Padding(3, 4, 3, 4);
            txtCareContextRef.Name = "txtCareContextRef";
            txtCareContextRef.Size = new Size(240, 29);
            txtCareContextRef.TabIndex = 13;
            // 
            // lblPatRef
            // 
            lblPatRef.AutoSize = true;
            lblPatRef.Location = new Point(15, 288);
            lblPatRef.Name = "lblPatRef";
            lblPatRef.Size = new Size(140, 21);
            lblPatRef.TabIndex = 10;
            lblPatRef.Text = "Patient Reference";
            // 
            // txtPatientRef
            // 
            txtPatientRef.Location = new Point(170, 284);
            txtPatientRef.Margin = new Padding(3, 4, 3, 4);
            txtPatientRef.Name = "txtPatientRef";
            txtPatientRef.Size = new Size(240, 29);
            txtPatientRef.TabIndex = 11;
            // 
            // lblMobile
            // 
            lblMobile.AutoSize = true;
            lblMobile.Location = new Point(15, 238);
            lblMobile.Name = "lblMobile";
            lblMobile.Size = new Size(62, 21);
            lblMobile.TabIndex = 8;
            lblMobile.Text = "Mobile";
            // 
            // txtPatientMobile
            // 
            txtPatientMobile.Location = new Point(170, 234);
            txtPatientMobile.Margin = new Padding(3, 4, 3, 4);
            txtPatientMobile.Name = "txtPatientMobile";
            txtPatientMobile.Size = new Size(240, 29);
            txtPatientMobile.TabIndex = 9;
            txtPatientMobile.Text = "9876543210";
            // 
            // lblDob
            // 
            lblDob.AutoSize = true;
            lblDob.Location = new Point(15, 188);
            lblDob.Name = "lblDob";
            lblDob.Size = new Size(157, 21);
            lblDob.TabIndex = 6;
            lblDob.Text = "DOB (YYYY-MM-DD)";
            // 
            // txtDob
            // 
            txtDob.Location = new Point(170, 184);
            txtDob.Margin = new Padding(3, 4, 3, 4);
            txtDob.Name = "txtDob";
            txtDob.Size = new Size(240, 29);
            txtDob.TabIndex = 7;
            txtDob.Text = "1990-01-01";
            // 
            // lblGender
            // 
            lblGender.AutoSize = true;
            lblGender.Location = new Point(15, 138);
            lblGender.Name = "lblGender";
            lblGender.Size = new Size(64, 21);
            lblGender.TabIndex = 4;
            lblGender.Text = "Gender";
            // 
            // cmbGender
            // 
            cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGender.FormattingEnabled = true;
            cmbGender.Items.AddRange(new object[] { "M", "F", "O" });
            cmbGender.Location = new Point(170, 134);
            cmbGender.Margin = new Padding(3, 4, 3, 4);
            cmbGender.Name = "cmbGender";
            cmbGender.Size = new Size(240, 29);
            cmbGender.TabIndex = 5;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(15, 88);
            lblName.Name = "lblName";
            lblName.Size = new Size(119, 21);
            lblName.TabIndex = 2;
            lblName.Text = "Patient Name *";
            // 
            // txtPatientName
            // 
            txtPatientName.Location = new Point(170, 84);
            txtPatientName.Margin = new Padding(3, 4, 3, 4);
            txtPatientName.Name = "txtPatientName";
            txtPatientName.Size = new Size(240, 29);
            txtPatientName.TabIndex = 3;
            // 
            // lblAbha
            // 
            lblAbha.AutoSize = true;
            lblAbha.Location = new Point(15, 38);
            lblAbha.Name = "lblAbha";
            lblAbha.Size = new Size(129, 21);
            lblAbha.TabIndex = 0;
            lblAbha.Text = "ABHA Address *";
            // 
            // txtAbhaAddress
            // 
            txtAbhaAddress.Location = new Point(170, 34);
            txtAbhaAddress.Margin = new Padding(3, 4, 3, 4);
            txtAbhaAddress.Name = "txtAbhaAddress";
            txtAbhaAddress.Size = new Size(240, 29);
            txtAbhaAddress.TabIndex = 1;
            // 
            // gbPrescribe
            // 
            gbPrescribe.Controls.Add(lvMedicines);
            gbPrescribe.Controls.Add(btnRemoveMedicine);
            gbPrescribe.Controls.Add(btnAddMedicine);
            gbPrescribe.Controls.Add(lblDosage);
            gbPrescribe.Controls.Add(txtDosage);
            gbPrescribe.Controls.Add(lblMedicine);
            gbPrescribe.Controls.Add(txtMedicineName);
            gbPrescribe.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            gbPrescribe.Location = new Point(460, 81);
            gbPrescribe.Margin = new Padding(3, 4, 3, 4);
            gbPrescribe.Name = "gbPrescribe";
            gbPrescribe.Padding = new Padding(3, 4, 3, 4);
            gbPrescribe.Size = new Size(725, 362);
            gbPrescribe.TabIndex = 2;
            gbPrescribe.TabStop = false;
            gbPrescribe.Text = "Write Medicine (Dwai)";
            // 
            // lvMedicines
            // 
            lvMedicines.Columns.AddRange(new ColumnHeader[] { chMedicine, chDosage });
            lvMedicines.FullRowSelect = true;
            lvMedicines.GridLines = true;
            lvMedicines.Location = new Point(20, 138);
            lvMedicines.Margin = new Padding(3, 4, 3, 4);
            lvMedicines.Name = "lvMedicines";
            lvMedicines.Size = new Size(474, 205);
            lvMedicines.TabIndex = 6;
            lvMedicines.UseCompatibleStateImageBehavior = false;
            lvMedicines.View = View.Details;
            // 
            // chMedicine
            // 
            chMedicine.Text = "Medicine Name";
            chMedicine.Width = 260;
            // 
            // chDosage
            // 
            chDosage.Text = "Dosage";
            chDosage.Width = 150;
            // 
            // btnRemoveMedicine
            // 
            btnRemoveMedicine.BackColor = Color.DarkRed;
            btnRemoveMedicine.FlatStyle = FlatStyle.Flat;
            btnRemoveMedicine.ForeColor = Color.White;
            btnRemoveMedicine.Location = new Point(340, 81);
            btnRemoveMedicine.Margin = new Padding(3, 4, 3, 4);
            btnRemoveMedicine.Name = "btnRemoveMedicine";
            btnRemoveMedicine.Size = new Size(154, 40);
            btnRemoveMedicine.TabIndex = 5;
            btnRemoveMedicine.Text = "Remove";
            btnRemoveMedicine.UseVisualStyleBackColor = false;
            // 
            // btnAddMedicine
            // 
            btnAddMedicine.BackColor = Color.FromArgb(55, 115, 200);
            btnAddMedicine.FlatStyle = FlatStyle.Flat;
            btnAddMedicine.ForeColor = Color.White;
            btnAddMedicine.Location = new Point(340, 31);
            btnAddMedicine.Margin = new Padding(3, 4, 3, 4);
            btnAddMedicine.Name = "btnAddMedicine";
            btnAddMedicine.Size = new Size(154, 40);
            btnAddMedicine.TabIndex = 4;
            btnAddMedicine.Text = "Add Medicine";
            btnAddMedicine.UseVisualStyleBackColor = false;
            // 
            // lblDosage
            // 
            lblDosage.AutoSize = true;
            lblDosage.Location = new Point(15, 88);
            lblDosage.Name = "lblDosage";
            lblDosage.Size = new Size(65, 21);
            lblDosage.TabIndex = 2;
            lblDosage.Text = "Dosage";
            // 
            // txtDosage
            // 
            txtDosage.Location = new Point(140, 84);
            txtDosage.Margin = new Padding(3, 4, 3, 4);
            txtDosage.Name = "txtDosage";
            txtDosage.Size = new Size(180, 29);
            txtDosage.TabIndex = 3;
            txtDosage.Text = "1-0-1";
            // 
            // lblMedicine
            // 
            lblMedicine.AutoSize = true;
            lblMedicine.Location = new Point(15, 38);
            lblMedicine.Name = "lblMedicine";
            lblMedicine.Size = new Size(125, 21);
            lblMedicine.TabIndex = 0;
            lblMedicine.Text = "Medicine Name";
            // 
            // txtMedicineName
            // 
            txtMedicineName.Location = new Point(140, 34);
            txtMedicineName.Margin = new Padding(3, 4, 3, 4);
            txtMedicineName.Name = "txtMedicineName";
            txtMedicineName.Size = new Size(180, 29);
            txtMedicineName.TabIndex = 1;
            // 
            // gbPdf
            // 
            gbPdf.Controls.Add(lblPdfStatus);
            gbPdf.Controls.Add(btnUploadPdf);
            gbPdf.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            gbPdf.Location = new Point(460, 450);
            gbPdf.Margin = new Padding(3, 4, 3, 4);
            gbPdf.Name = "gbPdf";
            gbPdf.Padding = new Padding(3, 4, 3, 4);
            gbPdf.Size = new Size(725, 125);
            gbPdf.TabIndex = 3;
            gbPdf.TabStop = false;
            gbPdf.Text = "Attach Prescription PDF File";
            // 
            // lblPdfStatus
            // 
            lblPdfStatus.AutoSize = true;
            lblPdfStatus.ForeColor = Color.Gray;
            lblPdfStatus.Location = new Point(220, 59);
            lblPdfStatus.Name = "lblPdfStatus";
            lblPdfStatus.Size = new Size(140, 21);
            lblPdfStatus.TabIndex = 1;
            lblPdfStatus.Text = "No PDF Uploaded";
            // 
            // btnUploadPdf
            // 
            btnUploadPdf.BackColor = Color.White;
            btnUploadPdf.FlatAppearance.BorderColor = Color.FromArgb(230, 100, 50);
            btnUploadPdf.FlatAppearance.BorderSize = 2;
            btnUploadPdf.FlatStyle = FlatStyle.Flat;
            btnUploadPdf.ForeColor = Color.FromArgb(230, 100, 50);
            btnUploadPdf.Location = new Point(20, 44);
            btnUploadPdf.Margin = new Padding(3, 4, 3, 4);
            btnUploadPdf.Name = "btnUploadPdf";
            btnUploadPdf.Size = new Size(180, 56);
            btnUploadPdf.TabIndex = 0;
            btnUploadPdf.Text = "Upload PDF";
            btnUploadPdf.UseVisualStyleBackColor = false;
            btnUploadPdf.Click += btnUploadPdf_Click;
            // 
            // gbActions
            // 
            gbActions.Controls.Add(btnAutoFlow);
            gbActions.Controls.Add(btnInitiateLink);
            gbActions.Controls.Add(btnSaveAndPush);
            gbActions.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            gbActions.Location = new Point(15, 588);
            gbActions.Margin = new Padding(3, 4, 3, 4);
            gbActions.Name = "gbActions";
            gbActions.Padding = new Padding(3, 4, 3, 4);
            gbActions.Size = new Size(430, 262);
            gbActions.TabIndex = 4;
            gbActions.TabStop = false;
            gbActions.Text = "ABDM Integration Actions";
            // 
            // btnAutoFlow
            // 
            btnAutoFlow.BackColor = Color.FromArgb(55, 115, 200);
            btnAutoFlow.FlatStyle = FlatStyle.Flat;
            btnAutoFlow.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAutoFlow.ForeColor = Color.White;
            btnAutoFlow.Location = new Point(15, 181);
            btnAutoFlow.Margin = new Padding(3, 4, 3, 4);
            btnAutoFlow.Name = "btnAutoFlow";
            btnAutoFlow.Size = new Size(395, 62);
            btnAutoFlow.TabIndex = 2;
            btnAutoFlow.Text = "Auto Link & Push (All in One)";
            btnAutoFlow.UseVisualStyleBackColor = false;
            btnAutoFlow.Click += btnAutoFlow_Click;
            // 
            // btnInitiateLink
            // 
            btnInitiateLink.BackColor = Color.White;
            btnInitiateLink.FlatAppearance.BorderColor = Color.FromArgb(55, 115, 200);
            btnInitiateLink.FlatAppearance.BorderSize = 2;
            btnInitiateLink.FlatStyle = FlatStyle.Flat;
            btnInitiateLink.ForeColor = Color.FromArgb(55, 115, 200);
            btnInitiateLink.Location = new Point(15, 106);
            btnInitiateLink.Margin = new Padding(3, 4, 3, 4);
            btnInitiateLink.Name = "btnInitiateLink";
            btnInitiateLink.Size = new Size(395, 56);
            btnInitiateLink.TabIndex = 1;
            btnInitiateLink.Text = "2. Link Care Context on Gateway";
            btnInitiateLink.UseVisualStyleBackColor = false;
            btnInitiateLink.Click += btnInitiateLink_Click;
            // 
            // btnSaveAndPush
            // 
            btnSaveAndPush.BackColor = Color.White;
            btnSaveAndPush.FlatAppearance.BorderColor = Color.FromArgb(55, 115, 200);
            btnSaveAndPush.FlatAppearance.BorderSize = 2;
            btnSaveAndPush.FlatStyle = FlatStyle.Flat;
            btnSaveAndPush.ForeColor = Color.FromArgb(55, 115, 200);
            btnSaveAndPush.Location = new Point(15, 38);
            btnSaveAndPush.Margin = new Padding(3, 4, 3, 4);
            btnSaveAndPush.Name = "btnSaveAndPush";
            btnSaveAndPush.Size = new Size(395, 56);
            btnSaveAndPush.TabIndex = 0;
            btnSaveAndPush.Text = "1. Save Patient & Push Health Data";
            btnSaveAndPush.UseVisualStyleBackColor = false;
            btnSaveAndPush.Click += btnSaveAndPush_Click;
            // 
            // txtLogs
            // 
            txtLogs.BackColor = Color.FromArgb(30, 30, 35);
            txtLogs.Font = new Font("Consolas", 9F);
            txtLogs.ForeColor = Color.LightGray;
            txtLogs.Location = new Point(460, 625);
            txtLogs.Margin = new Padding(3, 4, 3, 4);
            txtLogs.Multiline = true;
            txtLogs.Name = "txtLogs";
            txtLogs.ReadOnly = true;
            txtLogs.ScrollBars = ScrollBars.Vertical;
            txtLogs.Size = new Size(725, 224);
            txtLogs.TabIndex = 5;
            // 
            // lblLogs
            // 
            lblLogs.AutoSize = true;
            lblLogs.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            lblLogs.Location = new Point(460, 594);
            lblLogs.Name = "lblLogs";
            lblLogs.Size = new Size(148, 21);
            lblLogs.TabIndex = 6;
            lblLogs.Text = "Execution Console:";
            // 
            // frmPrescription
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1200, 875);
            Controls.Add(lblLogs);
            Controls.Add(txtLogs);
            Controls.Add(gbActions);
            Controls.Add(gbPdf);
            Controls.Add(gbPrescribe);
            Controls.Add(gbPatient);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "frmPrescription";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Write Prescription & Upload Document";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            gbPatient.ResumeLayout(false);
            gbPatient.PerformLayout();
            gbPrescribe.ResumeLayout(false);
            gbPrescribe.PerformLayout();
            gbPdf.ResumeLayout(false);
            gbPdf.PerformLayout();
            gbActions.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

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
