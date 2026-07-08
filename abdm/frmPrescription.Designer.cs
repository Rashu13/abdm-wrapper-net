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
            tcRecordDetails = new TabControl();
            tpPrescription = new TabPage();
            txtMedicineName_Presc = new TextBox();
            txtDosage_Presc = new TextBox();
            txtTiming_Presc = new TextBox();
            txtRoute_Presc = new TextBox();
            txtMethod_Presc = new TextBox();
            txtInstructions_Presc = new TextBox();
            txtReason_Presc = new TextBox();
            btnAddMedicine_Presc = new Button();
            btnRemoveMedicine_Presc = new Button();
            lvMedicines_Presc = new ListView();
            lblMedicine_Presc = new Label();
            lblDosage_Presc = new Label();
            lblTiming_Presc = new Label();
            lblRoute_Presc = new Label();
            lblMethod_Presc = new Label();
            lblInstructions_Presc = new Label();
            lblReason_Presc = new Label();
            tpOPConsultation = new TabPage();
            txtMedicineName_OP = new TextBox();
            txtDosage_OP = new TextBox();
            txtTiming_OP = new TextBox();
            txtRoute_OP = new TextBox();
            txtMethod_OP = new TextBox();
            txtInstructions_OP = new TextBox();
            txtReason_OP = new TextBox();
            btnAddMedicine_OP = new Button();
            btnRemoveMedicine_OP = new Button();
            lvMedicines_OP = new ListView();
            lblMedicine_OP = new Label();
            lblDosage_OP = new Label();
            lblTiming_OP = new Label();
            lblRoute_OP = new Label();
            lblMethod_OP = new Label();
            lblInstructions_OP = new Label();
            lblReason_OP = new Label();
            tpDiagnosticReport = new TabPage();
            txtTestName_Diag = new TextBox();
            txtSpecimen_Diag = new TextBox();
            txtResult_Diag = new TextBox();
            txtUnit_Diag = new TextBox();
            txtRefRange_Diag = new TextBox();
            txtRemarks_Diag = new TextBox();
            txtInterpretation_Diag = new TextBox();
            btnAddMedicine_Diag = new Button();
            btnRemoveMedicine_Diag = new Button();
            lvMedicines_Diag = new ListView();
            lblTestName_Diag = new Label();
            lblSpecimen_Diag = new Label();
            lblResult_Diag = new Label();
            lblUnit_Diag = new Label();
            lblRefRange_Diag = new Label();
            lblRemarks_Diag = new Label();
            lblInterpretation_Diag = new Label();
            tpDischargeSummary = new TabPage();
            txtMedicineName_Disch = new TextBox();
            txtDosage_Disch = new TextBox();
            txtAdmDate_Disch = new TextBox();
            txtDischDate_Disch = new TextBox();
            txtCourse_Disch = new TextBox();
            txtAdvice_Disch = new TextBox();
            txtCondition_Disch = new TextBox();
            btnAddMedicine_Disch = new Button();
            btnRemoveMedicine_Disch = new Button();
            lvMedicines_Disch = new ListView();
            lblMedicine_Disch = new Label();
            lblDosage_Disch = new Label();
            lblAdmDate_Disch = new Label();
            lblDischDate_Disch = new Label();
            lblCourse_Disch = new Label();
            lblAdvice_Disch = new Label();
            lblCondition_Disch = new Label();
            tpHealthDocument = new TabPage();
            lblInfo = new Label();
            tpImmunization = new TabPage();
            txtVaccineName_Imm = new TextBox();
            txtLotNumber_Imm = new TextBox();
            txtDoseNumber_Imm = new TextBox();
            btnAddMedicine_Imm = new Button();
            btnRemoveMedicine_Imm = new Button();
            lvMedicines_Imm = new ListView();
            lblVaccineName_Imm = new Label();
            lblLotNumber_Imm = new Label();
            lblDoseNumber_Imm = new Label();
            tpWellness = new TabPage();
            txtObservation_Well = new TextBox();
            txtResult_Well = new TextBox();
            txtUnit_Well = new TextBox();
            btnAddMedicine_Well = new Button();
            btnRemoveMedicine_Well = new Button();
            lvObservations_Well = new ListView();
            lblObservation_Well = new Label();
            lblResult_Well = new Label();
            lblUnit_Well = new Label();
            tpInvoice = new TabPage();
            txtItemName_Inv = new TextBox();
            txtAmount_Inv = new TextBox();
            btnAddMedicine_Inv = new Button();
            btnRemoveMedicine_Inv = new Button();
            lvItems_Inv = new ListView();
            lblAmount_Inv = new Label();
            lblItemName_Inv = new Label();
            gbPdf = new GroupBox();
            lblPdfStatus = new Label();
            btnUploadPdf = new Button();
            gbActions = new GroupBox();
            btnAutoFlow = new Button();
            btnInitiateLink = new Button();
            btnSaveAndPush = new Button();
            chkAllTypes = new CheckBox();
            txtLogs = new TextBox();
            lblLogs = new Label();
            pnlHeader.SuspendLayout();
            gbPatient.SuspendLayout();
            gbPrescribe.SuspendLayout();
            tcRecordDetails.SuspendLayout();
            tpPrescription.SuspendLayout();
            tpOPConsultation.SuspendLayout();
            tpDiagnosticReport.SuspendLayout();
            tpDischargeSummary.SuspendLayout();
            tpHealthDocument.SuspendLayout();
            tpImmunization.SuspendLayout();
            tpWellness.SuspendLayout();
            tpInvoice.SuspendLayout();
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
            pnlHeader.Size = new Size(1319, 62);
            pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(1268, 1);
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
            gbPatient.Size = new Size(417, 494);
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
            cmbRecordType.Items.AddRange(new object[] { "PrescriptionRecord", "OPConsultationRecord", "HealthDocumentRecord", "DiagnosticReport", "DischargeSummary", "ImmunizationRecord", "WellnessRecord", "Invoice" });
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
            gbPrescribe.Controls.Add(tcRecordDetails);
            gbPrescribe.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            gbPrescribe.Location = new Point(460, 81);
            gbPrescribe.Margin = new Padding(3, 4, 3, 4);
            gbPrescribe.Name = "gbPrescribe";
            gbPrescribe.Padding = new Padding(3, 4, 3, 4);
            gbPrescribe.Size = new Size(847, 362);
            gbPrescribe.TabIndex = 2;
            gbPrescribe.TabStop = false;
            gbPrescribe.Text = "Write Medicine (Dwai)";
            // 
            // tcRecordDetails
            // 
            tcRecordDetails.Controls.Add(tpPrescription);
            tcRecordDetails.Controls.Add(tpOPConsultation);
            tcRecordDetails.Controls.Add(tpDiagnosticReport);
            tcRecordDetails.Controls.Add(tpDischargeSummary);
            tcRecordDetails.Controls.Add(tpHealthDocument);
            tcRecordDetails.Controls.Add(tpImmunization);
            tcRecordDetails.Controls.Add(tpWellness);
            tcRecordDetails.Controls.Add(tpInvoice);
            tcRecordDetails.Location = new Point(10, 20);
            tcRecordDetails.Name = "tcRecordDetails";
            tcRecordDetails.SelectedIndex = 0;
            tcRecordDetails.Size = new Size(831, 330);
            tcRecordDetails.TabIndex = 0;
            tcRecordDetails.SelectedIndexChanged += tcRecordDetails_SelectedIndexChanged;
            // 
            // tpPrescription
            // 
            tpPrescription.Controls.Add(txtMedicineName_Presc);
            tpPrescription.Controls.Add(txtDosage_Presc);
            tpPrescription.Controls.Add(txtTiming_Presc);
            tpPrescription.Controls.Add(txtRoute_Presc);
            tpPrescription.Controls.Add(txtMethod_Presc);
            tpPrescription.Controls.Add(txtInstructions_Presc);
            tpPrescription.Controls.Add(txtReason_Presc);
            tpPrescription.Controls.Add(btnAddMedicine_Presc);
            tpPrescription.Controls.Add(btnRemoveMedicine_Presc);
            tpPrescription.Controls.Add(lvMedicines_Presc);
            tpPrescription.Controls.Add(lblMedicine_Presc);
            tpPrescription.Controls.Add(lblDosage_Presc);
            tpPrescription.Controls.Add(lblTiming_Presc);
            tpPrescription.Controls.Add(lblRoute_Presc);
            tpPrescription.Controls.Add(lblMethod_Presc);
            tpPrescription.Controls.Add(lblInstructions_Presc);
            tpPrescription.Controls.Add(lblReason_Presc);
            tpPrescription.Location = new Point(4, 30);
            tpPrescription.Name = "tpPrescription";
            tpPrescription.Size = new Size(823, 296);
            tpPrescription.TabIndex = 0;
            tpPrescription.Text = "Prescription";
            // 
            // txtMedicineName_Presc
            // 
            txtMedicineName_Presc.Location = new Point(115, 5);
            txtMedicineName_Presc.Name = "txtMedicineName_Presc";
            txtMedicineName_Presc.Size = new Size(200, 29);
            txtMedicineName_Presc.TabIndex = 0;
            // 
            // txtDosage_Presc
            // 
            txtDosage_Presc.Location = new Point(415, 5);
            txtDosage_Presc.Name = "txtDosage_Presc";
            txtDosage_Presc.Size = new Size(150, 29);
            txtDosage_Presc.TabIndex = 1;
            txtDosage_Presc.Text = "1-0-1";
            // 
            // txtTiming_Presc
            // 
            txtTiming_Presc.Location = new Point(115, 35);
            txtTiming_Presc.Name = "txtTiming_Presc";
            txtTiming_Presc.Size = new Size(200, 29);
            txtTiming_Presc.TabIndex = 2;
            txtTiming_Presc.Text = "1-1-D";
            // 
            // txtRoute_Presc
            // 
            txtRoute_Presc.Location = new Point(415, 35);
            txtRoute_Presc.Name = "txtRoute_Presc";
            txtRoute_Presc.Size = new Size(150, 29);
            txtRoute_Presc.TabIndex = 3;
            txtRoute_Presc.Text = "Oral";
            // 
            // txtMethod_Presc
            // 
            txtMethod_Presc.Location = new Point(115, 65);
            txtMethod_Presc.Name = "txtMethod_Presc";
            txtMethod_Presc.Size = new Size(200, 29);
            txtMethod_Presc.TabIndex = 4;
            txtMethod_Presc.Text = "swallow";
            // 
            // txtInstructions_Presc
            // 
            txtInstructions_Presc.Location = new Point(415, 65);
            txtInstructions_Presc.Name = "txtInstructions_Presc";
            txtInstructions_Presc.Size = new Size(150, 29);
            txtInstructions_Presc.TabIndex = 5;
            txtInstructions_Presc.Text = "after food";
            // 
            // txtReason_Presc
            // 
            txtReason_Presc.Location = new Point(115, 95);
            txtReason_Presc.Name = "txtReason_Presc";
            txtReason_Presc.Size = new Size(450, 29);
            txtReason_Presc.TabIndex = 6;
            txtReason_Presc.Text = "Fever";
            // 
            // btnAddMedicine_Presc
            // 
            btnAddMedicine_Presc.BackColor = Color.FromArgb(41, 128, 185);
            btnAddMedicine_Presc.FlatStyle = FlatStyle.Flat;
            btnAddMedicine_Presc.ForeColor = Color.White;
            btnAddMedicine_Presc.Location = new Point(580, 5);
            btnAddMedicine_Presc.Name = "btnAddMedicine_Presc";
            btnAddMedicine_Presc.Size = new Size(110, 28);
            btnAddMedicine_Presc.TabIndex = 7;
            btnAddMedicine_Presc.Text = "Add Item";
            btnAddMedicine_Presc.UseVisualStyleBackColor = false;
            btnAddMedicine_Presc.Click += btnAddMedicine_Presc_Click;
            // 
            // btnRemoveMedicine_Presc
            // 
            btnRemoveMedicine_Presc.BackColor = Color.FromArgb(192, 57, 43);
            btnRemoveMedicine_Presc.FlatStyle = FlatStyle.Flat;
            btnRemoveMedicine_Presc.ForeColor = Color.White;
            btnRemoveMedicine_Presc.Location = new Point(580, 38);
            btnRemoveMedicine_Presc.Name = "btnRemoveMedicine_Presc";
            btnRemoveMedicine_Presc.Size = new Size(110, 28);
            btnRemoveMedicine_Presc.TabIndex = 8;
            btnRemoveMedicine_Presc.Text = "Remove";
            btnRemoveMedicine_Presc.UseVisualStyleBackColor = false;
            btnRemoveMedicine_Presc.Click += btnRemoveMedicine_Presc_Click;
            // 
            // lvMedicines_Presc
            // 
            lvMedicines_Presc.FullRowSelect = true;
            lvMedicines_Presc.GridLines = true;
            lvMedicines_Presc.Location = new Point(10, 130);
            lvMedicines_Presc.Name = "lvMedicines_Presc";
            lvMedicines_Presc.Size = new Size(680, 160);
            lvMedicines_Presc.TabIndex = 9;
            lvMedicines_Presc.UseCompatibleStateImageBehavior = false;
            lvMedicines_Presc.View = View.Details;
            // 
            // lblMedicine_Presc
            // 
            lblMedicine_Presc.Location = new Point(0, 0);
            lblMedicine_Presc.Name = "lblMedicine_Presc";
            lblMedicine_Presc.Size = new Size(100, 23);
            lblMedicine_Presc.TabIndex = 10;
            // 
            // lblDosage_Presc
            // 
            lblDosage_Presc.Location = new Point(0, 0);
            lblDosage_Presc.Name = "lblDosage_Presc";
            lblDosage_Presc.Size = new Size(100, 23);
            lblDosage_Presc.TabIndex = 11;
            // 
            // lblTiming_Presc
            // 
            lblTiming_Presc.Location = new Point(0, 0);
            lblTiming_Presc.Name = "lblTiming_Presc";
            lblTiming_Presc.Size = new Size(100, 23);
            lblTiming_Presc.TabIndex = 12;
            // 
            // lblRoute_Presc
            // 
            lblRoute_Presc.Location = new Point(0, 0);
            lblRoute_Presc.Name = "lblRoute_Presc";
            lblRoute_Presc.Size = new Size(100, 23);
            lblRoute_Presc.TabIndex = 13;
            // 
            // lblMethod_Presc
            // 
            lblMethod_Presc.Location = new Point(0, 0);
            lblMethod_Presc.Name = "lblMethod_Presc";
            lblMethod_Presc.Size = new Size(100, 23);
            lblMethod_Presc.TabIndex = 14;
            // 
            // lblInstructions_Presc
            // 
            lblInstructions_Presc.Location = new Point(0, 0);
            lblInstructions_Presc.Name = "lblInstructions_Presc";
            lblInstructions_Presc.Size = new Size(100, 23);
            lblInstructions_Presc.TabIndex = 15;
            // 
            // lblReason_Presc
            // 
            lblReason_Presc.Location = new Point(0, 0);
            lblReason_Presc.Name = "lblReason_Presc";
            lblReason_Presc.Size = new Size(100, 23);
            lblReason_Presc.TabIndex = 16;
            // 
            // tpOPConsultation
            // 
            tpOPConsultation.Controls.Add(txtMedicineName_OP);
            tpOPConsultation.Controls.Add(txtDosage_OP);
            tpOPConsultation.Controls.Add(txtTiming_OP);
            tpOPConsultation.Controls.Add(txtRoute_OP);
            tpOPConsultation.Controls.Add(txtMethod_OP);
            tpOPConsultation.Controls.Add(txtInstructions_OP);
            tpOPConsultation.Controls.Add(txtReason_OP);
            tpOPConsultation.Controls.Add(btnAddMedicine_OP);
            tpOPConsultation.Controls.Add(btnRemoveMedicine_OP);
            tpOPConsultation.Controls.Add(lvMedicines_OP);
            tpOPConsultation.Controls.Add(lblMedicine_OP);
            tpOPConsultation.Controls.Add(lblDosage_OP);
            tpOPConsultation.Controls.Add(lblTiming_OP);
            tpOPConsultation.Controls.Add(lblRoute_OP);
            tpOPConsultation.Controls.Add(lblMethod_OP);
            tpOPConsultation.Controls.Add(lblInstructions_OP);
            tpOPConsultation.Controls.Add(lblReason_OP);
            tpOPConsultation.Location = new Point(4, 30);
            tpOPConsultation.Name = "tpOPConsultation";
            tpOPConsultation.Size = new Size(823, 296);
            tpOPConsultation.TabIndex = 1;
            tpOPConsultation.Text = "OP Consultation";
            // 
            // txtMedicineName_OP
            // 
            txtMedicineName_OP.Location = new Point(115, 5);
            txtMedicineName_OP.Name = "txtMedicineName_OP";
            txtMedicineName_OP.Size = new Size(200, 29);
            txtMedicineName_OP.TabIndex = 0;
            // 
            // txtDosage_OP
            // 
            txtDosage_OP.Location = new Point(415, 5);
            txtDosage_OP.Name = "txtDosage_OP";
            txtDosage_OP.Size = new Size(150, 29);
            txtDosage_OP.TabIndex = 1;
            txtDosage_OP.Text = "1-0-1";
            // 
            // txtTiming_OP
            // 
            txtTiming_OP.Location = new Point(115, 35);
            txtTiming_OP.Name = "txtTiming_OP";
            txtTiming_OP.Size = new Size(200, 29);
            txtTiming_OP.TabIndex = 2;
            txtTiming_OP.Text = "1-1-D";
            // 
            // txtRoute_OP
            // 
            txtRoute_OP.Location = new Point(415, 35);
            txtRoute_OP.Name = "txtRoute_OP";
            txtRoute_OP.Size = new Size(150, 29);
            txtRoute_OP.TabIndex = 3;
            txtRoute_OP.Text = "Oral";
            // 
            // txtMethod_OP
            // 
            txtMethod_OP.Location = new Point(115, 65);
            txtMethod_OP.Name = "txtMethod_OP";
            txtMethod_OP.Size = new Size(200, 29);
            txtMethod_OP.TabIndex = 4;
            txtMethod_OP.Text = "swallow";
            // 
            // txtInstructions_OP
            // 
            txtInstructions_OP.Location = new Point(415, 65);
            txtInstructions_OP.Name = "txtInstructions_OP";
            txtInstructions_OP.Size = new Size(150, 29);
            txtInstructions_OP.TabIndex = 5;
            txtInstructions_OP.Text = "after food";
            // 
            // txtReason_OP
            // 
            txtReason_OP.Location = new Point(115, 95);
            txtReason_OP.Name = "txtReason_OP";
            txtReason_OP.Size = new Size(450, 29);
            txtReason_OP.TabIndex = 6;
            txtReason_OP.Text = "Fever";
            // 
            // btnAddMedicine_OP
            // 
            btnAddMedicine_OP.BackColor = Color.FromArgb(41, 128, 185);
            btnAddMedicine_OP.FlatStyle = FlatStyle.Flat;
            btnAddMedicine_OP.ForeColor = Color.White;
            btnAddMedicine_OP.Location = new Point(580, 5);
            btnAddMedicine_OP.Name = "btnAddMedicine_OP";
            btnAddMedicine_OP.Size = new Size(140, 40);
            btnAddMedicine_OP.TabIndex = 7;
            btnAddMedicine_OP.Text = "Add Item";
            btnAddMedicine_OP.UseVisualStyleBackColor = false;
            btnAddMedicine_OP.Click += btnAddMedicine_OP_Click;
            // 
            // btnRemoveMedicine_OP
            // 
            btnRemoveMedicine_OP.BackColor = Color.FromArgb(192, 57, 43);
            btnRemoveMedicine_OP.FlatStyle = FlatStyle.Flat;
            btnRemoveMedicine_OP.ForeColor = Color.White;
            btnRemoveMedicine_OP.Location = new Point(580, 51);
            btnRemoveMedicine_OP.Name = "btnRemoveMedicine_OP";
            btnRemoveMedicine_OP.Size = new Size(140, 43);
            btnRemoveMedicine_OP.TabIndex = 8;
            btnRemoveMedicine_OP.Text = "Remove";
            btnRemoveMedicine_OP.UseVisualStyleBackColor = false;
            btnRemoveMedicine_OP.Click += btnRemoveMedicine_OP_Click;
            // 
            // lvMedicines_OP
            // 
            lvMedicines_OP.FullRowSelect = true;
            lvMedicines_OP.GridLines = true;
            lvMedicines_OP.Location = new Point(10, 130);
            lvMedicines_OP.Name = "lvMedicines_OP";
            lvMedicines_OP.Size = new Size(680, 160);
            lvMedicines_OP.TabIndex = 9;
            lvMedicines_OP.UseCompatibleStateImageBehavior = false;
            lvMedicines_OP.View = View.Details;
            // 
            // lblMedicine_OP
            // 
            lblMedicine_OP.Location = new Point(3, 34);
            lblMedicine_OP.Name = "lblMedicine_OP";
            lblMedicine_OP.Size = new Size(100, 23);
            lblMedicine_OP.TabIndex = 10;
            // 
            // lblDosage_OP
            // 
            lblDosage_OP.Location = new Point(0, 0);
            lblDosage_OP.Name = "lblDosage_OP";
            lblDosage_OP.Size = new Size(100, 23);
            lblDosage_OP.TabIndex = 11;
            // 
            // lblTiming_OP
            // 
            lblTiming_OP.Location = new Point(0, 0);
            lblTiming_OP.Name = "lblTiming_OP";
            lblTiming_OP.Size = new Size(100, 23);
            lblTiming_OP.TabIndex = 12;
            // 
            // lblRoute_OP
            // 
            lblRoute_OP.Location = new Point(0, 0);
            lblRoute_OP.Name = "lblRoute_OP";
            lblRoute_OP.Size = new Size(100, 23);
            lblRoute_OP.TabIndex = 13;
            // 
            // lblMethod_OP
            // 
            lblMethod_OP.Location = new Point(0, 0);
            lblMethod_OP.Name = "lblMethod_OP";
            lblMethod_OP.Size = new Size(100, 23);
            lblMethod_OP.TabIndex = 14;
            // 
            // lblInstructions_OP
            // 
            lblInstructions_OP.Location = new Point(0, 0);
            lblInstructions_OP.Name = "lblInstructions_OP";
            lblInstructions_OP.Size = new Size(100, 23);
            lblInstructions_OP.TabIndex = 15;
            // 
            // lblReason_OP
            // 
            lblReason_OP.Location = new Point(0, 0);
            lblReason_OP.Name = "lblReason_OP";
            lblReason_OP.Size = new Size(100, 23);
            lblReason_OP.TabIndex = 16;
            // 
            // tpDiagnosticReport
            // 
            tpDiagnosticReport.Controls.Add(txtTestName_Diag);
            tpDiagnosticReport.Controls.Add(txtSpecimen_Diag);
            tpDiagnosticReport.Controls.Add(txtResult_Diag);
            tpDiagnosticReport.Controls.Add(txtUnit_Diag);
            tpDiagnosticReport.Controls.Add(txtRefRange_Diag);
            tpDiagnosticReport.Controls.Add(txtRemarks_Diag);
            tpDiagnosticReport.Controls.Add(txtInterpretation_Diag);
            tpDiagnosticReport.Controls.Add(btnAddMedicine_Diag);
            tpDiagnosticReport.Controls.Add(btnRemoveMedicine_Diag);
            tpDiagnosticReport.Controls.Add(lvMedicines_Diag);
            tpDiagnosticReport.Controls.Add(lblTestName_Diag);
            tpDiagnosticReport.Controls.Add(lblSpecimen_Diag);
            tpDiagnosticReport.Controls.Add(lblResult_Diag);
            tpDiagnosticReport.Controls.Add(lblUnit_Diag);
            tpDiagnosticReport.Controls.Add(lblRefRange_Diag);
            tpDiagnosticReport.Controls.Add(lblRemarks_Diag);
            tpDiagnosticReport.Controls.Add(lblInterpretation_Diag);
            tpDiagnosticReport.Location = new Point(4, 30);
            tpDiagnosticReport.Name = "tpDiagnosticReport";
            tpDiagnosticReport.Size = new Size(823, 296);
            tpDiagnosticReport.TabIndex = 2;
            tpDiagnosticReport.Text = "Diagnostic Report";
            // 
            // txtTestName_Diag
            // 
            txtTestName_Diag.Location = new Point(115, 5);
            txtTestName_Diag.Name = "txtTestName_Diag";
            txtTestName_Diag.Size = new Size(200, 29);
            txtTestName_Diag.TabIndex = 0;
            // 
            // txtSpecimen_Diag
            // 
            txtSpecimen_Diag.Location = new Point(415, 5);
            txtSpecimen_Diag.Name = "txtSpecimen_Diag";
            txtSpecimen_Diag.Size = new Size(150, 29);
            txtSpecimen_Diag.TabIndex = 1;
            txtSpecimen_Diag.Text = "Blood";
            // 
            // txtResult_Diag
            // 
            txtResult_Diag.Location = new Point(115, 35);
            txtResult_Diag.Name = "txtResult_Diag";
            txtResult_Diag.Size = new Size(200, 29);
            txtResult_Diag.TabIndex = 2;
            // 
            // txtUnit_Diag
            // 
            txtUnit_Diag.Location = new Point(415, 35);
            txtUnit_Diag.Name = "txtUnit_Diag";
            txtUnit_Diag.Size = new Size(150, 29);
            txtUnit_Diag.TabIndex = 3;
            txtUnit_Diag.Text = "mg/dL";
            // 
            // txtRefRange_Diag
            // 
            txtRefRange_Diag.Location = new Point(115, 65);
            txtRefRange_Diag.Name = "txtRefRange_Diag";
            txtRefRange_Diag.Size = new Size(200, 29);
            txtRefRange_Diag.TabIndex = 4;
            txtRefRange_Diag.Text = "70-100";
            // 
            // txtRemarks_Diag
            // 
            txtRemarks_Diag.Location = new Point(415, 65);
            txtRemarks_Diag.Name = "txtRemarks_Diag";
            txtRemarks_Diag.Size = new Size(150, 29);
            txtRemarks_Diag.TabIndex = 5;
            txtRemarks_Diag.Text = "Normal";
            // 
            // txtInterpretation_Diag
            // 
            txtInterpretation_Diag.Location = new Point(115, 95);
            txtInterpretation_Diag.Name = "txtInterpretation_Diag";
            txtInterpretation_Diag.Size = new Size(450, 29);
            txtInterpretation_Diag.TabIndex = 6;
            txtInterpretation_Diag.Text = "Clinically stable";
            // 
            // btnAddMedicine_Diag
            // 
            btnAddMedicine_Diag.BackColor = Color.FromArgb(41, 128, 185);
            btnAddMedicine_Diag.FlatStyle = FlatStyle.Flat;
            btnAddMedicine_Diag.ForeColor = Color.White;
            btnAddMedicine_Diag.Location = new Point(580, 5);
            btnAddMedicine_Diag.Name = "btnAddMedicine_Diag";
            btnAddMedicine_Diag.Size = new Size(110, 28);
            btnAddMedicine_Diag.TabIndex = 7;
            btnAddMedicine_Diag.Text = "Add Item";
            btnAddMedicine_Diag.UseVisualStyleBackColor = false;
            btnAddMedicine_Diag.Click += btnAddMedicine_Diag_Click;
            // 
            // btnRemoveMedicine_Diag
            // 
            btnRemoveMedicine_Diag.BackColor = Color.FromArgb(192, 57, 43);
            btnRemoveMedicine_Diag.FlatStyle = FlatStyle.Flat;
            btnRemoveMedicine_Diag.ForeColor = Color.White;
            btnRemoveMedicine_Diag.Location = new Point(580, 38);
            btnRemoveMedicine_Diag.Name = "btnRemoveMedicine_Diag";
            btnRemoveMedicine_Diag.Size = new Size(110, 28);
            btnRemoveMedicine_Diag.TabIndex = 8;
            btnRemoveMedicine_Diag.Text = "Remove";
            btnRemoveMedicine_Diag.UseVisualStyleBackColor = false;
            btnRemoveMedicine_Diag.Click += btnRemoveMedicine_Diag_Click;
            // 
            // lvMedicines_Diag
            // 
            lvMedicines_Diag.FullRowSelect = true;
            lvMedicines_Diag.GridLines = true;
            lvMedicines_Diag.Location = new Point(10, 130);
            lvMedicines_Diag.Name = "lvMedicines_Diag";
            lvMedicines_Diag.Size = new Size(680, 160);
            lvMedicines_Diag.TabIndex = 9;
            lvMedicines_Diag.UseCompatibleStateImageBehavior = false;
            lvMedicines_Diag.View = View.Details;
            // 
            // lblTestName_Diag
            // 
            lblTestName_Diag.Location = new Point(0, 0);
            lblTestName_Diag.Name = "lblTestName_Diag";
            lblTestName_Diag.Size = new Size(100, 23);
            lblTestName_Diag.TabIndex = 10;
            // 
            // lblSpecimen_Diag
            // 
            lblSpecimen_Diag.Location = new Point(0, 0);
            lblSpecimen_Diag.Name = "lblSpecimen_Diag";
            lblSpecimen_Diag.Size = new Size(100, 23);
            lblSpecimen_Diag.TabIndex = 11;
            // 
            // lblResult_Diag
            // 
            lblResult_Diag.Location = new Point(0, 0);
            lblResult_Diag.Name = "lblResult_Diag";
            lblResult_Diag.Size = new Size(100, 23);
            lblResult_Diag.TabIndex = 12;
            // 
            // lblUnit_Diag
            // 
            lblUnit_Diag.Location = new Point(0, 0);
            lblUnit_Diag.Name = "lblUnit_Diag";
            lblUnit_Diag.Size = new Size(100, 23);
            lblUnit_Diag.TabIndex = 13;
            // 
            // lblRefRange_Diag
            // 
            lblRefRange_Diag.Location = new Point(0, 0);
            lblRefRange_Diag.Name = "lblRefRange_Diag";
            lblRefRange_Diag.Size = new Size(100, 23);
            lblRefRange_Diag.TabIndex = 14;
            // 
            // lblRemarks_Diag
            // 
            lblRemarks_Diag.Location = new Point(0, 0);
            lblRemarks_Diag.Name = "lblRemarks_Diag";
            lblRemarks_Diag.Size = new Size(100, 23);
            lblRemarks_Diag.TabIndex = 15;
            // 
            // lblInterpretation_Diag
            // 
            lblInterpretation_Diag.Location = new Point(0, 0);
            lblInterpretation_Diag.Name = "lblInterpretation_Diag";
            lblInterpretation_Diag.Size = new Size(100, 23);
            lblInterpretation_Diag.TabIndex = 16;
            // 
            // tpDischargeSummary
            // 
            tpDischargeSummary.Controls.Add(txtMedicineName_Disch);
            tpDischargeSummary.Controls.Add(txtDosage_Disch);
            tpDischargeSummary.Controls.Add(txtAdmDate_Disch);
            tpDischargeSummary.Controls.Add(txtDischDate_Disch);
            tpDischargeSummary.Controls.Add(txtCourse_Disch);
            tpDischargeSummary.Controls.Add(txtAdvice_Disch);
            tpDischargeSummary.Controls.Add(txtCondition_Disch);
            tpDischargeSummary.Controls.Add(btnAddMedicine_Disch);
            tpDischargeSummary.Controls.Add(btnRemoveMedicine_Disch);
            tpDischargeSummary.Controls.Add(lvMedicines_Disch);
            tpDischargeSummary.Controls.Add(lblMedicine_Disch);
            tpDischargeSummary.Controls.Add(lblDosage_Disch);
            tpDischargeSummary.Controls.Add(lblAdmDate_Disch);
            tpDischargeSummary.Controls.Add(lblDischDate_Disch);
            tpDischargeSummary.Controls.Add(lblCourse_Disch);
            tpDischargeSummary.Controls.Add(lblAdvice_Disch);
            tpDischargeSummary.Controls.Add(lblCondition_Disch);
            tpDischargeSummary.Location = new Point(4, 30);
            tpDischargeSummary.Name = "tpDischargeSummary";
            tpDischargeSummary.Size = new Size(823, 296);
            tpDischargeSummary.TabIndex = 3;
            tpDischargeSummary.Text = "Discharge Summary";
            // 
            // txtMedicineName_Disch
            // 
            txtMedicineName_Disch.Location = new Point(115, 5);
            txtMedicineName_Disch.Name = "txtMedicineName_Disch";
            txtMedicineName_Disch.Size = new Size(200, 29);
            txtMedicineName_Disch.TabIndex = 0;
            // 
            // txtDosage_Disch
            // 
            txtDosage_Disch.Location = new Point(415, 5);
            txtDosage_Disch.Name = "txtDosage_Disch";
            txtDosage_Disch.Size = new Size(150, 29);
            txtDosage_Disch.TabIndex = 1;
            txtDosage_Disch.Text = "1-0-1";
            // 
            // txtAdmDate_Disch
            // 
            txtAdmDate_Disch.Location = new Point(115, 35);
            txtAdmDate_Disch.Name = "txtAdmDate_Disch";
            txtAdmDate_Disch.Size = new Size(200, 29);
            txtAdmDate_Disch.TabIndex = 2;
            txtAdmDate_Disch.Text = "2026-07-05";
            // 
            // txtDischDate_Disch
            // 
            txtDischDate_Disch.Location = new Point(415, 35);
            txtDischDate_Disch.Name = "txtDischDate_Disch";
            txtDischDate_Disch.Size = new Size(150, 29);
            txtDischDate_Disch.TabIndex = 3;
            txtDischDate_Disch.Text = "2026-07-08";
            // 
            // txtCourse_Disch
            // 
            txtCourse_Disch.Location = new Point(115, 65);
            txtCourse_Disch.Name = "txtCourse_Disch";
            txtCourse_Disch.Size = new Size(200, 29);
            txtCourse_Disch.TabIndex = 4;
            txtCourse_Disch.Text = "IV Fluids & Antibiotics";
            // 
            // txtAdvice_Disch
            // 
            txtAdvice_Disch.Location = new Point(415, 65);
            txtAdvice_Disch.Name = "txtAdvice_Disch";
            txtAdvice_Disch.Size = new Size(150, 29);
            txtAdvice_Disch.TabIndex = 5;
            txtAdvice_Disch.Text = "Take rest";
            // 
            // txtCondition_Disch
            // 
            txtCondition_Disch.Location = new Point(115, 95);
            txtCondition_Disch.Name = "txtCondition_Disch";
            txtCondition_Disch.Size = new Size(450, 29);
            txtCondition_Disch.TabIndex = 6;
            txtCondition_Disch.Text = "Stable";
            // 
            // btnAddMedicine_Disch
            // 
            btnAddMedicine_Disch.BackColor = Color.FromArgb(41, 128, 185);
            btnAddMedicine_Disch.FlatStyle = FlatStyle.Flat;
            btnAddMedicine_Disch.ForeColor = Color.White;
            btnAddMedicine_Disch.Location = new Point(580, 5);
            btnAddMedicine_Disch.Name = "btnAddMedicine_Disch";
            btnAddMedicine_Disch.Size = new Size(110, 28);
            btnAddMedicine_Disch.TabIndex = 7;
            btnAddMedicine_Disch.Text = "Add Item";
            btnAddMedicine_Disch.UseVisualStyleBackColor = false;
            btnAddMedicine_Disch.Click += btnAddMedicine_Disch_Click;
            // 
            // btnRemoveMedicine_Disch
            // 
            btnRemoveMedicine_Disch.BackColor = Color.FromArgb(192, 57, 43);
            btnRemoveMedicine_Disch.FlatStyle = FlatStyle.Flat;
            btnRemoveMedicine_Disch.ForeColor = Color.White;
            btnRemoveMedicine_Disch.Location = new Point(580, 38);
            btnRemoveMedicine_Disch.Name = "btnRemoveMedicine_Disch";
            btnRemoveMedicine_Disch.Size = new Size(110, 28);
            btnRemoveMedicine_Disch.TabIndex = 8;
            btnRemoveMedicine_Disch.Text = "Remove";
            btnRemoveMedicine_Disch.UseVisualStyleBackColor = false;
            btnRemoveMedicine_Disch.Click += btnRemoveMedicine_Disch_Click;
            // 
            // lvMedicines_Disch
            // 
            lvMedicines_Disch.FullRowSelect = true;
            lvMedicines_Disch.GridLines = true;
            lvMedicines_Disch.Location = new Point(10, 130);
            lvMedicines_Disch.Name = "lvMedicines_Disch";
            lvMedicines_Disch.Size = new Size(680, 160);
            lvMedicines_Disch.TabIndex = 9;
            lvMedicines_Disch.UseCompatibleStateImageBehavior = false;
            lvMedicines_Disch.View = View.Details;
            // 
            // lblMedicine_Disch
            // 
            lblMedicine_Disch.Location = new Point(0, 0);
            lblMedicine_Disch.Name = "lblMedicine_Disch";
            lblMedicine_Disch.Size = new Size(100, 23);
            lblMedicine_Disch.TabIndex = 10;
            // 
            // lblDosage_Disch
            // 
            lblDosage_Disch.Location = new Point(0, 0);
            lblDosage_Disch.Name = "lblDosage_Disch";
            lblDosage_Disch.Size = new Size(100, 23);
            lblDosage_Disch.TabIndex = 11;
            // 
            // lblAdmDate_Disch
            // 
            lblAdmDate_Disch.Location = new Point(0, 0);
            lblAdmDate_Disch.Name = "lblAdmDate_Disch";
            lblAdmDate_Disch.Size = new Size(100, 23);
            lblAdmDate_Disch.TabIndex = 12;
            // 
            // lblDischDate_Disch
            // 
            lblDischDate_Disch.Location = new Point(0, 0);
            lblDischDate_Disch.Name = "lblDischDate_Disch";
            lblDischDate_Disch.Size = new Size(100, 23);
            lblDischDate_Disch.TabIndex = 13;
            // 
            // lblCourse_Disch
            // 
            lblCourse_Disch.Location = new Point(0, 0);
            lblCourse_Disch.Name = "lblCourse_Disch";
            lblCourse_Disch.Size = new Size(100, 23);
            lblCourse_Disch.TabIndex = 14;
            // 
            // lblAdvice_Disch
            // 
            lblAdvice_Disch.Location = new Point(0, 0);
            lblAdvice_Disch.Name = "lblAdvice_Disch";
            lblAdvice_Disch.Size = new Size(100, 23);
            lblAdvice_Disch.TabIndex = 15;
            // 
            // lblCondition_Disch
            // 
            lblCondition_Disch.Location = new Point(0, 0);
            lblCondition_Disch.Name = "lblCondition_Disch";
            lblCondition_Disch.Size = new Size(100, 23);
            lblCondition_Disch.TabIndex = 16;
            // 
            // tpHealthDocument
            // 
            tpHealthDocument.Controls.Add(lblInfo);
            tpHealthDocument.Location = new Point(4, 30);
            tpHealthDocument.Name = "tpHealthDocument";
            tpHealthDocument.Size = new Size(823, 296);
            tpHealthDocument.TabIndex = 4;
            tpHealthDocument.Text = "Health Document";
            // 
            // lblInfo
            // 
            lblInfo.Location = new Point(0, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(100, 23);
            lblInfo.TabIndex = 0;
            // 
            // tpImmunization
            // 
            tpImmunization.Controls.Add(txtVaccineName_Imm);
            tpImmunization.Controls.Add(txtLotNumber_Imm);
            tpImmunization.Controls.Add(txtDoseNumber_Imm);
            tpImmunization.Controls.Add(btnAddMedicine_Imm);
            tpImmunization.Controls.Add(btnRemoveMedicine_Imm);
            tpImmunization.Controls.Add(lvMedicines_Imm);
            tpImmunization.Controls.Add(lblVaccineName_Imm);
            tpImmunization.Controls.Add(lblLotNumber_Imm);
            tpImmunization.Controls.Add(lblDoseNumber_Imm);
            tpImmunization.Location = new Point(4, 30);
            tpImmunization.Name = "tpImmunization";
            tpImmunization.Size = new Size(823, 296);
            tpImmunization.TabIndex = 5;
            tpImmunization.Text = "Immunization";
            // 
            // txtVaccineName_Imm
            // 
            txtVaccineName_Imm.Location = new Point(115, 5);
            txtVaccineName_Imm.Name = "txtVaccineName_Imm";
            txtVaccineName_Imm.Size = new Size(200, 29);
            txtVaccineName_Imm.TabIndex = 0;
            txtVaccineName_Imm.Text = "Covishield";
            // 
            // txtLotNumber_Imm
            // 
            txtLotNumber_Imm.Location = new Point(415, 5);
            txtLotNumber_Imm.Name = "txtLotNumber_Imm";
            txtLotNumber_Imm.Size = new Size(150, 29);
            txtLotNumber_Imm.TabIndex = 1;
            txtLotNumber_Imm.Text = "LOT-1234";
            // 
            // txtDoseNumber_Imm
            // 
            txtDoseNumber_Imm.Location = new Point(115, 35);
            txtDoseNumber_Imm.Name = "txtDoseNumber_Imm";
            txtDoseNumber_Imm.Size = new Size(200, 29);
            txtDoseNumber_Imm.TabIndex = 2;
            txtDoseNumber_Imm.Text = "1";
            // 
            // btnAddMedicine_Imm
            // 
            btnAddMedicine_Imm.BackColor = Color.FromArgb(41, 128, 185);
            btnAddMedicine_Imm.FlatStyle = FlatStyle.Flat;
            btnAddMedicine_Imm.ForeColor = Color.White;
            btnAddMedicine_Imm.Location = new Point(580, 5);
            btnAddMedicine_Imm.Name = "btnAddMedicine_Imm";
            btnAddMedicine_Imm.Size = new Size(110, 28);
            btnAddMedicine_Imm.TabIndex = 3;
            btnAddMedicine_Imm.Text = "Add Item";
            btnAddMedicine_Imm.UseVisualStyleBackColor = false;
            btnAddMedicine_Imm.Click += btnAddMedicine_Imm_Click;
            // 
            // btnRemoveMedicine_Imm
            // 
            btnRemoveMedicine_Imm.BackColor = Color.FromArgb(192, 57, 43);
            btnRemoveMedicine_Imm.FlatStyle = FlatStyle.Flat;
            btnRemoveMedicine_Imm.ForeColor = Color.White;
            btnRemoveMedicine_Imm.Location = new Point(580, 38);
            btnRemoveMedicine_Imm.Name = "btnRemoveMedicine_Imm";
            btnRemoveMedicine_Imm.Size = new Size(110, 28);
            btnRemoveMedicine_Imm.TabIndex = 4;
            btnRemoveMedicine_Imm.Text = "Remove";
            btnRemoveMedicine_Imm.UseVisualStyleBackColor = false;
            btnRemoveMedicine_Imm.Click += btnRemoveMedicine_Imm_Click;
            // 
            // lvMedicines_Imm
            // 
            lvMedicines_Imm.FullRowSelect = true;
            lvMedicines_Imm.GridLines = true;
            lvMedicines_Imm.Location = new Point(10, 130);
            lvMedicines_Imm.Name = "lvMedicines_Imm";
            lvMedicines_Imm.Size = new Size(680, 160);
            lvMedicines_Imm.TabIndex = 5;
            lvMedicines_Imm.UseCompatibleStateImageBehavior = false;
            lvMedicines_Imm.View = View.Details;
            // 
            // lblVaccineName_Imm
            // 
            lblVaccineName_Imm.Location = new Point(0, 0);
            lblVaccineName_Imm.Name = "lblVaccineName_Imm";
            lblVaccineName_Imm.Size = new Size(100, 23);
            lblVaccineName_Imm.TabIndex = 6;
            // 
            // lblLotNumber_Imm
            // 
            lblLotNumber_Imm.Location = new Point(0, 0);
            lblLotNumber_Imm.Name = "lblLotNumber_Imm";
            lblLotNumber_Imm.Size = new Size(100, 23);
            lblLotNumber_Imm.TabIndex = 7;
            // 
            // lblDoseNumber_Imm
            // 
            lblDoseNumber_Imm.Location = new Point(0, 0);
            lblDoseNumber_Imm.Name = "lblDoseNumber_Imm";
            lblDoseNumber_Imm.Size = new Size(100, 23);
            lblDoseNumber_Imm.TabIndex = 8;
            // 
            // tpWellness
            // 
            tpWellness.Controls.Add(txtObservation_Well);
            tpWellness.Controls.Add(txtResult_Well);
            tpWellness.Controls.Add(txtUnit_Well);
            tpWellness.Controls.Add(btnAddMedicine_Well);
            tpWellness.Controls.Add(btnRemoveMedicine_Well);
            tpWellness.Controls.Add(lvObservations_Well);
            tpWellness.Controls.Add(lblObservation_Well);
            tpWellness.Controls.Add(lblResult_Well);
            tpWellness.Controls.Add(lblUnit_Well);
            tpWellness.Location = new Point(4, 30);
            tpWellness.Name = "tpWellness";
            tpWellness.Size = new Size(823, 296);
            tpWellness.TabIndex = 6;
            tpWellness.Text = "Wellness";
            // 
            // txtObservation_Well
            // 
            txtObservation_Well.Location = new Point(115, 5);
            txtObservation_Well.Name = "txtObservation_Well";
            txtObservation_Well.Size = new Size(200, 29);
            txtObservation_Well.TabIndex = 0;
            txtObservation_Well.Text = "Heart rate";
            // 
            // txtResult_Well
            // 
            txtResult_Well.Location = new Point(415, 5);
            txtResult_Well.Name = "txtResult_Well";
            txtResult_Well.Size = new Size(150, 29);
            txtResult_Well.TabIndex = 1;
            txtResult_Well.Text = "72";
            // 
            // txtUnit_Well
            // 
            txtUnit_Well.Location = new Point(115, 35);
            txtUnit_Well.Name = "txtUnit_Well";
            txtUnit_Well.Size = new Size(200, 29);
            txtUnit_Well.TabIndex = 2;
            txtUnit_Well.Text = "beats/minute";
            // 
            // btnAddMedicine_Well
            // 
            btnAddMedicine_Well.BackColor = Color.FromArgb(41, 128, 185);
            btnAddMedicine_Well.FlatStyle = FlatStyle.Flat;
            btnAddMedicine_Well.ForeColor = Color.White;
            btnAddMedicine_Well.Location = new Point(580, 5);
            btnAddMedicine_Well.Name = "btnAddMedicine_Well";
            btnAddMedicine_Well.Size = new Size(110, 28);
            btnAddMedicine_Well.TabIndex = 3;
            btnAddMedicine_Well.Text = "Add Item";
            btnAddMedicine_Well.UseVisualStyleBackColor = false;
            btnAddMedicine_Well.Click += btnAddMedicine_Well_Click;
            // 
            // btnRemoveMedicine_Well
            // 
            btnRemoveMedicine_Well.BackColor = Color.FromArgb(192, 57, 43);
            btnRemoveMedicine_Well.FlatStyle = FlatStyle.Flat;
            btnRemoveMedicine_Well.ForeColor = Color.White;
            btnRemoveMedicine_Well.Location = new Point(580, 38);
            btnRemoveMedicine_Well.Name = "btnRemoveMedicine_Well";
            btnRemoveMedicine_Well.Size = new Size(110, 28);
            btnRemoveMedicine_Well.TabIndex = 4;
            btnRemoveMedicine_Well.Text = "Remove";
            btnRemoveMedicine_Well.UseVisualStyleBackColor = false;
            btnRemoveMedicine_Well.Click += btnRemoveMedicine_Well_Click;
            // 
            // lvObservations_Well
            // 
            lvObservations_Well.FullRowSelect = true;
            lvObservations_Well.GridLines = true;
            lvObservations_Well.Location = new Point(10, 130);
            lvObservations_Well.Name = "lvObservations_Well";
            lvObservations_Well.Size = new Size(680, 160);
            lvObservations_Well.TabIndex = 5;
            lvObservations_Well.UseCompatibleStateImageBehavior = false;
            lvObservations_Well.View = View.Details;
            // 
            // lblObservation_Well
            // 
            lblObservation_Well.Location = new Point(0, 0);
            lblObservation_Well.Name = "lblObservation_Well";
            lblObservation_Well.Size = new Size(100, 23);
            lblObservation_Well.TabIndex = 6;
            // 
            // lblResult_Well
            // 
            lblResult_Well.Location = new Point(0, 0);
            lblResult_Well.Name = "lblResult_Well";
            lblResult_Well.Size = new Size(100, 23);
            lblResult_Well.TabIndex = 7;
            // 
            // lblUnit_Well
            // 
            lblUnit_Well.Location = new Point(0, 0);
            lblUnit_Well.Name = "lblUnit_Well";
            lblUnit_Well.Size = new Size(100, 23);
            lblUnit_Well.TabIndex = 8;
            // 
            // tpInvoice
            // 
            tpInvoice.Controls.Add(txtItemName_Inv);
            tpInvoice.Controls.Add(txtAmount_Inv);
            tpInvoice.Controls.Add(btnAddMedicine_Inv);
            tpInvoice.Controls.Add(btnRemoveMedicine_Inv);
            tpInvoice.Controls.Add(lvItems_Inv);
            tpInvoice.Controls.Add(lblAmount_Inv);
            tpInvoice.Controls.Add(lblItemName_Inv);
            tpInvoice.Location = new Point(4, 30);
            tpInvoice.Name = "tpInvoice";
            tpInvoice.Size = new Size(823, 296);
            tpInvoice.TabIndex = 7;
            tpInvoice.Text = "Invoice";
            // 
            // txtItemName_Inv
            // 
            txtItemName_Inv.Location = new Point(115, 5);
            txtItemName_Inv.Name = "txtItemName_Inv";
            txtItemName_Inv.Size = new Size(294, 29);
            txtItemName_Inv.TabIndex = 0;
            txtItemName_Inv.Text = "Consultation & Clinical Services";
            // 
            // txtAmount_Inv
            // 
            txtAmount_Inv.Location = new Point(415, 5);
            txtAmount_Inv.Name = "txtAmount_Inv";
            txtAmount_Inv.Size = new Size(150, 29);
            txtAmount_Inv.TabIndex = 1;
            txtAmount_Inv.Text = "500";
            // 
            // btnAddMedicine_Inv
            // 
            btnAddMedicine_Inv.BackColor = Color.FromArgb(41, 128, 185);
            btnAddMedicine_Inv.FlatStyle = FlatStyle.Flat;
            btnAddMedicine_Inv.ForeColor = Color.White;
            btnAddMedicine_Inv.Location = new Point(580, 5);
            btnAddMedicine_Inv.Name = "btnAddMedicine_Inv";
            btnAddMedicine_Inv.Size = new Size(110, 28);
            btnAddMedicine_Inv.TabIndex = 2;
            btnAddMedicine_Inv.Text = "Add Item";
            btnAddMedicine_Inv.UseVisualStyleBackColor = false;
            btnAddMedicine_Inv.Click += btnAddMedicine_Inv_Click;
            // 
            // btnRemoveMedicine_Inv
            // 
            btnRemoveMedicine_Inv.BackColor = Color.FromArgb(192, 57, 43);
            btnRemoveMedicine_Inv.FlatStyle = FlatStyle.Flat;
            btnRemoveMedicine_Inv.ForeColor = Color.White;
            btnRemoveMedicine_Inv.Location = new Point(580, 38);
            btnRemoveMedicine_Inv.Name = "btnRemoveMedicine_Inv";
            btnRemoveMedicine_Inv.Size = new Size(110, 28);
            btnRemoveMedicine_Inv.TabIndex = 3;
            btnRemoveMedicine_Inv.Text = "Remove";
            btnRemoveMedicine_Inv.UseVisualStyleBackColor = false;
            btnRemoveMedicine_Inv.Click += btnRemoveMedicine_Inv_Click;
            // 
            // lvItems_Inv
            // 
            lvItems_Inv.FullRowSelect = true;
            lvItems_Inv.GridLines = true;
            lvItems_Inv.Location = new Point(10, 72);
            lvItems_Inv.Name = "lvItems_Inv";
            lvItems_Inv.Size = new Size(810, 218);
            lvItems_Inv.TabIndex = 4;
            lvItems_Inv.UseCompatibleStateImageBehavior = false;
            lvItems_Inv.View = View.Details;
            // 
            // lblAmount_Inv
            // 
            lblAmount_Inv.Location = new Point(465, 38);
            lblAmount_Inv.Name = "lblAmount_Inv";
            lblAmount_Inv.Size = new Size(100, 23);
            lblAmount_Inv.TabIndex = 6;
            // 
            // lblItemName_Inv
            // 
            lblItemName_Inv.Location = new Point(6, 0);
            lblItemName_Inv.Name = "lblItemName_Inv";
            lblItemName_Inv.Size = new Size(100, 23);
            lblItemName_Inv.TabIndex = 5;
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
            gbPdf.Size = new Size(847, 125);
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
            // 
            // gbActions
            // 
            gbActions.Controls.Add(chkAllTypes);
            gbActions.Controls.Add(btnAutoFlow);
            gbActions.Controls.Add(btnInitiateLink);
            gbActions.Controls.Add(btnSaveAndPush);
            gbActions.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            gbActions.Location = new Point(15, 588);
            gbActions.Margin = new Padding(3, 4, 3, 4);
            gbActions.Name = "gbActions";
            gbActions.Padding = new Padding(3, 4, 3, 4);
            gbActions.Size = new Size(417, 275);
            gbActions.TabIndex = 4;
            gbActions.TabStop = false;
            gbActions.Text = "ABDM Integration Actions";
            // 
            // chkAllTypes
            // 
            chkAllTypes.AutoSize = true;
            chkAllTypes.Location = new Point(15, 28);
            chkAllTypes.Name = "chkAllTypes";
            chkAllTypes.Size = new Size(205, 25);
            chkAllTypes.TabIndex = 3;
            chkAllTypes.Text = "Apply to All 8 HI Types";
            chkAllTypes.UseVisualStyleBackColor = true;
            // 
            // btnAutoFlow
            // 
            btnAutoFlow.BackColor = Color.FromArgb(55, 115, 200);
            btnAutoFlow.FlatStyle = FlatStyle.Flat;
            btnAutoFlow.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAutoFlow.ForeColor = Color.White;
            btnAutoFlow.Location = new Point(15, 195);
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
            btnInitiateLink.Location = new Point(15, 125);
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
            btnSaveAndPush.Location = new Point(15, 60);
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
            txtLogs.Size = new Size(847, 224);
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
            ClientSize = new Size(1319, 875);
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
            WindowState = FormWindowState.Maximized;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            gbPatient.ResumeLayout(false);
            gbPatient.PerformLayout();
            gbPrescribe.ResumeLayout(false);
            tcRecordDetails.ResumeLayout(false);
            tpPrescription.ResumeLayout(false);
            tpPrescription.PerformLayout();
            tpOPConsultation.ResumeLayout(false);
            tpOPConsultation.PerformLayout();
            tpDiagnosticReport.ResumeLayout(false);
            tpDiagnosticReport.PerformLayout();
            tpDischargeSummary.ResumeLayout(false);
            tpDischargeSummary.PerformLayout();
            tpHealthDocument.ResumeLayout(false);
            tpImmunization.ResumeLayout(false);
            tpImmunization.PerformLayout();
            tpWellness.ResumeLayout(false);
            tpWellness.PerformLayout();
            tpInvoice.ResumeLayout(false);
            tpInvoice.PerformLayout();
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
        private System.Windows.Forms.TabControl tcRecordDetails;
        private System.Windows.Forms.TabPage tpPrescription;
        private System.Windows.Forms.TabPage tpOPConsultation;
        private System.Windows.Forms.TabPage tpDiagnosticReport;
        private System.Windows.Forms.TabPage tpDischargeSummary;
        private System.Windows.Forms.TabPage tpHealthDocument;
        private System.Windows.Forms.TabPage tpImmunization;
        private System.Windows.Forms.TabPage tpWellness;
        private System.Windows.Forms.TabPage tpInvoice;

        private System.Windows.Forms.TextBox txtMedicineName_Presc;
        private System.Windows.Forms.TextBox txtDosage_Presc;
        private System.Windows.Forms.TextBox txtTiming_Presc;
        private System.Windows.Forms.TextBox txtRoute_Presc;
        private System.Windows.Forms.TextBox txtMethod_Presc;
        private System.Windows.Forms.TextBox txtInstructions_Presc;
        private System.Windows.Forms.TextBox txtReason_Presc;
        private System.Windows.Forms.Button btnAddMedicine_Presc;
        private System.Windows.Forms.Button btnRemoveMedicine_Presc;
        private System.Windows.Forms.ListView lvMedicines_Presc;

        private System.Windows.Forms.TextBox txtMedicineName_OP;
        private System.Windows.Forms.TextBox txtDosage_OP;
        private System.Windows.Forms.TextBox txtTiming_OP;
        private System.Windows.Forms.TextBox txtRoute_OP;
        private System.Windows.Forms.TextBox txtMethod_OP;
        private System.Windows.Forms.TextBox txtInstructions_OP;
        private System.Windows.Forms.TextBox txtReason_OP;
        private System.Windows.Forms.Button btnAddMedicine_OP;
        private System.Windows.Forms.Button btnRemoveMedicine_OP;
        private System.Windows.Forms.ListView lvMedicines_OP;

        private System.Windows.Forms.TextBox txtTestName_Diag;
        private System.Windows.Forms.TextBox txtSpecimen_Diag;
        private System.Windows.Forms.TextBox txtResult_Diag;
        private System.Windows.Forms.TextBox txtUnit_Diag;
        private System.Windows.Forms.TextBox txtRefRange_Diag;
        private System.Windows.Forms.TextBox txtRemarks_Diag;
        private System.Windows.Forms.TextBox txtInterpretation_Diag;
        private System.Windows.Forms.Button btnAddMedicine_Diag;
        private System.Windows.Forms.Button btnRemoveMedicine_Diag;
        private System.Windows.Forms.ListView lvMedicines_Diag;

        private System.Windows.Forms.TextBox txtMedicineName_Disch;
        private System.Windows.Forms.TextBox txtDosage_Disch;
        private System.Windows.Forms.TextBox txtAdmDate_Disch;
        private System.Windows.Forms.TextBox txtDischDate_Disch;
        private System.Windows.Forms.TextBox txtCourse_Disch;
        private System.Windows.Forms.TextBox txtAdvice_Disch;
        private System.Windows.Forms.TextBox txtCondition_Disch;
        private System.Windows.Forms.Button btnAddMedicine_Disch;
        private System.Windows.Forms.Button btnRemoveMedicine_Disch;
        private System.Windows.Forms.ListView lvMedicines_Disch;

        private System.Windows.Forms.TextBox txtVaccineName_Imm;
        private System.Windows.Forms.TextBox txtLotNumber_Imm;
        private System.Windows.Forms.TextBox txtDoseNumber_Imm;
        private System.Windows.Forms.Button btnAddMedicine_Imm;
        private System.Windows.Forms.Button btnRemoveMedicine_Imm;
        private System.Windows.Forms.ListView lvMedicines_Imm;

        private System.Windows.Forms.TextBox txtObservation_Well;
        private System.Windows.Forms.TextBox txtResult_Well;
        private System.Windows.Forms.TextBox txtUnit_Well;
        private System.Windows.Forms.Button btnAddMedicine_Well;
        private System.Windows.Forms.Button btnRemoveMedicine_Well;
        private System.Windows.Forms.ListView lvObservations_Well;

        private System.Windows.Forms.TextBox txtItemName_Inv;
        private System.Windows.Forms.TextBox txtAmount_Inv;
        private System.Windows.Forms.Button btnAddMedicine_Inv;
        private System.Windows.Forms.Button btnRemoveMedicine_Inv;
        private System.Windows.Forms.ListView lvItems_Inv;
        private System.Windows.Forms.GroupBox gbPdf;
        private System.Windows.Forms.Button btnUploadPdf;
        private System.Windows.Forms.Label lblPdfStatus;
        private System.Windows.Forms.GroupBox gbActions;
        private System.Windows.Forms.Button btnSaveAndPush;
        private System.Windows.Forms.Button btnInitiateLink;
        private System.Windows.Forms.Button btnAutoFlow;
        private System.Windows.Forms.CheckBox chkAllTypes;
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.Label lblLogs;
        private Label lblMedicine_Presc;
        private Label lblDosage_Presc;
        private Label lblTiming_Presc;
        private Label lblRoute_Presc;
        private Label lblMethod_Presc;
        private Label lblInstructions_Presc;
        private Label lblReason_Presc;
        private Label lblMedicine_OP;
        private Label lblDosage_OP;
        private Label lblTiming_OP;
        private Label lblRoute_OP;
        private Label lblMethod_OP;
        private Label lblInstructions_OP;
        private Label lblReason_OP;
        private Label lblTestName_Diag;
        private Label lblSpecimen_Diag;
        private Label lblResult_Diag;
        private Label lblUnit_Diag;
        private Label lblRefRange_Diag;
        private Label lblRemarks_Diag;
        private Label lblInterpretation_Diag;
        private Label lblMedicine_Disch;
        private Label lblDosage_Disch;
        private Label lblAdmDate_Disch;
        private Label lblDischDate_Disch;
        private Label lblCourse_Disch;
        private Label lblAdvice_Disch;
        private Label lblCondition_Disch;
        private Label lblInfo;
        private Label lblVaccineName_Imm;
        private Label lblLotNumber_Imm;
        private Label lblDoseNumber_Imm;
        private Label lblObservation_Well;
        private Label lblResult_Well;
        private Label lblUnit_Well;
        private Label lblAmount_Inv;
        private Label lblItemName_Inv;
    }
}
