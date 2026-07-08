namespace HMS.abdm
{
    partial class frmCreatePrescription
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTop = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtPatientName = new System.Windows.Forms.TextBox();
            this.lblAbha = new System.Windows.Forms.Label();
            this.txtPatientABHA = new System.Windows.Forms.TextBox();
            this.lblDiagnosis = new System.Windows.Forms.Label();
            this.txtDiagnosis = new System.Windows.Forms.TextBox();
            this.pnlMiddle = new System.Windows.Forms.GroupBox();
            this.gridMedicines = new System.Windows.Forms.DataGridView();
            this.colMedicine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDosage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBtn = new System.Windows.Forms.Panel();
            this.btnGenerateFHIR = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.GroupBox();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.mainPanel.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMedicines)).BeginInit();
            this.pnlBtn.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Controls.Add(this.pnlTop, 0, 0);
            this.mainPanel.Controls.Add(this.pnlMiddle, 0, 1);
            this.mainPanel.Controls.Add(this.pnlBtn, 0, 2);
            this.mainPanel.Controls.Add(this.pnlBottom, 0, 3);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(20);
            this.mainPanel.RowCount = 4;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.mainPanel.Size = new System.Drawing.Size(1000, 750);
            this.mainPanel.TabIndex = 0;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.txtDiagnosis);
            this.pnlTop.Controls.Add(this.lblDiagnosis);
            this.pnlTop.Controls.Add(this.txtPatientABHA);
            this.pnlTop.Controls.Add(this.lblAbha);
            this.pnlTop.Controls.Add(this.txtPatientName);
            this.pnlTop.Controls.Add(this.lblName);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.pnlTop.Location = new System.Drawing.Point(23, 23);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(954, 114);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.TabStop = false;
            this.pnlTop.Text = "Step 1: Patient Details && Diagnosis";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblName.Location = new System.Drawing.Point(20, 35);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(94, 19);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Patient Name:";
            // 
            // txtPatientName
            // 
            this.txtPatientName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPatientName.Location = new System.Drawing.Point(130, 32);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.Size = new System.Drawing.Size(250, 25);
            this.txtPatientName.TabIndex = 1;
            this.txtPatientName.Text = "Ravi Kumar";
            // 
            // lblAbha
            // 
            this.lblAbha.AutoSize = true;
            this.lblAbha.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAbha.Location = new System.Drawing.Point(420, 35);
            this.lblAbha.Name = "lblAbha";
            this.lblAbha.Size = new System.Drawing.Size(99, 19);
            this.lblAbha.TabIndex = 2;
            this.lblAbha.Text = "ABHA Address:";
            // 
            // txtPatientABHA
            // 
            this.txtPatientABHA.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPatientABHA.Location = new System.Drawing.Point(540, 32);
            this.txtPatientABHA.Name = "txtPatientABHA";
            this.txtPatientABHA.Size = new System.Drawing.Size(250, 25);
            this.txtPatientABHA.TabIndex = 3;
            this.txtPatientABHA.Text = "user.5682@sbx";
            // 
            // lblDiagnosis
            // 
            this.lblDiagnosis.AutoSize = true;
            this.lblDiagnosis.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDiagnosis.Location = new System.Drawing.Point(20, 75);
            this.lblDiagnosis.Name = "lblDiagnosis";
            this.lblDiagnosis.Size = new System.Drawing.Size(70, 19);
            this.lblDiagnosis.TabIndex = 4;
            this.lblDiagnosis.Text = "Diagnosis:";
            // 
            // txtDiagnosis
            // 
            this.txtDiagnosis.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDiagnosis.Location = new System.Drawing.Point(130, 72);
            this.txtDiagnosis.Name = "txtDiagnosis";
            this.txtDiagnosis.Size = new System.Drawing.Size(660, 25);
            this.txtDiagnosis.TabIndex = 5;
            this.txtDiagnosis.Text = "Viral Fever with Body Ache";
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.gridMedicines);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.pnlMiddle.Location = new System.Drawing.Point(23, 143);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(954, 183);
            this.pnlMiddle.TabIndex = 1;
            this.pnlMiddle.TabStop = false;
            this.pnlMiddle.Text = "Step 2: Prescribed Medicines";
            // 
            // gridMedicines
            // 
            this.gridMedicines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridMedicines.BackgroundColor = System.Drawing.Color.White;
            this.gridMedicines.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridMedicines.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridMedicines.ColumnHeadersHeight = 35;
            this.gridMedicines.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMedicine,
            this.colDosage,
            this.colDuration});
            this.gridMedicines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridMedicines.EnableHeadersVisualStyles = false;
            this.gridMedicines.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridMedicines.GridColor = System.Drawing.Color.LightGray;
            this.gridMedicines.Location = new System.Drawing.Point(3, 21);
            this.gridMedicines.Margin = new System.Windows.Forms.Padding(10);
            this.gridMedicines.Name = "gridMedicines";
            this.gridMedicines.Size = new System.Drawing.Size(948, 159);
            this.gridMedicines.TabIndex = 0;
            // 
            // colMedicine
            // 
            this.colMedicine.HeaderText = "Medicine Name";
            this.colMedicine.Name = "colMedicine";
            // 
            // colDosage
            // 
            this.colDosage.HeaderText = "Dosage (e.g. 1-0-1)";
            this.colDosage.Name = "colDosage";
            // 
            // colDuration
            // 
            this.colDuration.HeaderText = "Duration (Days)";
            this.colDuration.Name = "colDuration";
            // 
            // pnlBtn
            // 
            this.pnlBtn.Controls.Add(this.btnGenerateFHIR);
            this.pnlBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBtn.Location = new System.Drawing.Point(23, 332);
            this.pnlBtn.Name = "pnlBtn";
            this.pnlBtn.Size = new System.Drawing.Size(954, 44);
            this.pnlBtn.TabIndex = 2;
            // 
            // btnGenerateFHIR
            // 
            this.btnGenerateFHIR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnGenerateFHIR.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerateFHIR.FlatAppearance.BorderSize = 0;
            this.btnGenerateFHIR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateFHIR.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGenerateFHIR.ForeColor = System.Drawing.Color.White;
            this.btnGenerateFHIR.Location = new System.Drawing.Point(0, 2);
            this.btnGenerateFHIR.Name = "btnGenerateFHIR";
            this.btnGenerateFHIR.Size = new System.Drawing.Size(350, 40);
            this.btnGenerateFHIR.TabIndex = 0;
            this.btnGenerateFHIR.Text = "Generate ABDM FHIR Bundle (JSON)";
            this.btnGenerateFHIR.UseVisualStyleBackColor = false;
            this.btnGenerateFHIR.Click += new System.EventHandler(this.BtnGenerateFHIR_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.txtOutput);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBottom.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.pnlBottom.Location = new System.Drawing.Point(23, 382);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(954, 345);
            this.pnlBottom.TabIndex = 3;
            this.pnlBottom.TabStop = false;
            this.pnlBottom.Text = "Step 3: FHIR Output (Ready to push to ABDM)";
            // 
            // txtOutput
            // 
            this.txtOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtOutput.ForeColor = System.Drawing.Color.LightGreen;
            this.txtOutput.Location = new System.Drawing.Point(3, 21);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(948, 321);
            this.txtOutput.TabIndex = 0;
            this.txtOutput.Text = "";
            // 
            // frmCreatePrescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1000, 750);
            this.Controls.Add(this.mainPanel);
            this.Name = "frmCreatePrescription";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create OPD Prescription (ABDM)";
            this.mainPanel.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridMedicines)).EndInit();
            this.pnlBtn.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.GroupBox pnlTop;
        private System.Windows.Forms.TextBox txtDiagnosis;
        private System.Windows.Forms.Label lblDiagnosis;
        private System.Windows.Forms.TextBox txtPatientABHA;
        private System.Windows.Forms.Label lblAbha;
        private System.Windows.Forms.TextBox txtPatientName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.GroupBox pnlMiddle;
        private System.Windows.Forms.DataGridView gridMedicines;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMedicine;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDosage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDuration;
        private System.Windows.Forms.Panel pnlBtn;
        private System.Windows.Forms.Button btnGenerateFHIR;
        private System.Windows.Forms.GroupBox pnlBottom;
        private System.Windows.Forms.RichTextBox txtOutput;
    }
}
