namespace HMS.abdm
{
    partial class ucImmunizationRecord
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.lblVaccineName = new System.Windows.Forms.Label();
            this.txtVaccineName = new System.Windows.Forms.TextBox();
            this.lblLotNumber = new System.Windows.Forms.Label();
            this.txtLotNumber = new System.Windows.Forms.TextBox();
            this.lblDoseNumber = new System.Windows.Forms.Label();
            this.txtDoseNumber = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lvMedicines = new System.Windows.Forms.ListView();
            this.colVaccineName = new System.Windows.Forms.ColumnHeader();
            this.colLotNumber = new System.Windows.Forms.ColumnHeader();
            this.colDoseNumber = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lblVaccineName
            // 
            this.lblVaccineName.AutoSize = true;
            this.lblVaccineName.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblVaccineName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblVaccineName.Location = new System.Drawing.Point(10, 10);
            this.lblVaccineName.Name = "lblVaccineName";
            this.lblVaccineName.Size = new System.Drawing.Size(93, 17);
            this.lblVaccineName.TabIndex = 0;
            this.lblVaccineName.Text = "Vaccine Name";
            // 
            // txtVaccineName
            // 
            this.txtVaccineName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtVaccineName.Location = new System.Drawing.Point(10, 32);
            this.txtVaccineName.Name = "txtVaccineName";
            this.txtVaccineName.Size = new System.Drawing.Size(200, 25);
            this.txtVaccineName.TabIndex = 1;
            this.txtVaccineName.Text = "Covishield";
            // 
            // lblLotNumber
            // 
            this.lblLotNumber.AutoSize = true;
            this.lblLotNumber.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblLotNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblLotNumber.Location = new System.Drawing.Point(220, 10);
            this.lblLotNumber.Name = "lblLotNumber";
            this.lblLotNumber.Size = new System.Drawing.Size(81, 17);
            this.lblLotNumber.TabIndex = 2;
            this.lblLotNumber.Text = "Lot Number";
            // 
            // txtLotNumber
            // 
            this.txtLotNumber.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtLotNumber.Location = new System.Drawing.Point(220, 32);
            this.txtLotNumber.Name = "txtLotNumber";
            this.txtLotNumber.Size = new System.Drawing.Size(130, 25);
            this.txtLotNumber.TabIndex = 3;
            this.txtLotNumber.Text = "LOT-1234";
            // 
            // lblDoseNumber
            // 
            this.lblDoseNumber.AutoSize = true;
            this.lblDoseNumber.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblDoseNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.lblDoseNumber.Location = new System.Drawing.Point(360, 10);
            this.lblDoseNumber.Name = "lblDoseNumber";
            this.lblDoseNumber.Size = new System.Drawing.Size(92, 17);
            this.lblDoseNumber.TabIndex = 4;
            this.lblDoseNumber.Text = "Dose Number";
            // 
            // txtDoseNumber
            // 
            this.txtDoseNumber.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDoseNumber.Location = new System.Drawing.Point(360, 32);
            this.txtDoseNumber.Name = "txtDoseNumber";
            this.txtDoseNumber.Size = new System.Drawing.Size(100, 25);
            this.txtDoseNumber.TabIndex = 5;
            this.txtDoseNumber.Text = "1";
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnAdd.Location = new System.Drawing.Point(480, 30);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(95, 28);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "+ Add Item";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnRemove.Location = new System.Drawing.Point(582, 30);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(95, 28);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lvMedicines
            // 
            this.lvMedicines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMedicines.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colVaccineName,
            this.colLotNumber,
            this.colDoseNumber});
            this.lvMedicines.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lvMedicines.HideSelection = false;
            this.lvMedicines.Location = new System.Drawing.Point(10, 72);
            this.lvMedicines.Name = "lvMedicines";
            this.lvMedicines.Size = new System.Drawing.Size(667, 210);
            this.lvMedicines.TabIndex = 8;
            this.lvMedicines.UseCompatibleStateImageBehavior = false;
            this.lvMedicines.View = System.Windows.Forms.View.Details;
            // 
            // colVaccineName
            // 
            this.colVaccineName.Text = "Vaccine Name";
            this.colVaccineName.Width = 300;
            // 
            // colLotNumber
            // 
            this.colLotNumber.Text = "Lot Number";
            this.colLotNumber.Width = 180;
            // 
            // colDoseNumber
            // 
            this.colDoseNumber.Text = "Dose Number";
            this.colDoseNumber.Width = 160;
            // 
            // ucImmunizationRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvMedicines);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtDoseNumber);
            this.Controls.Add(this.lblDoseNumber);
            this.Controls.Add(this.txtLotNumber);
            this.Controls.Add(this.lblLotNumber);
            this.Controls.Add(this.txtVaccineName);
            this.Controls.Add(this.lblVaccineName);
            this.Name = "ucImmunizationRecord";
            this.Size = new System.Drawing.Size(687, 292);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVaccineName;
        private System.Windows.Forms.TextBox txtVaccineName;
        private System.Windows.Forms.Label lblLotNumber;
        private System.Windows.Forms.TextBox txtLotNumber;
        private System.Windows.Forms.Label lblDoseNumber;
        private System.Windows.Forms.TextBox txtDoseNumber;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ListView lvMedicines;
        private System.Windows.Forms.ColumnHeader colVaccineName;
        private System.Windows.Forms.ColumnHeader colLotNumber;
        private System.Windows.Forms.ColumnHeader colDoseNumber;
    }
}
