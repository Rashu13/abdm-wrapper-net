namespace HMS.abdm
{
    partial class frmABHAConsent
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmABHAConsent));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblNote = new System.Windows.Forms.Label();
            this.pnlComplex2 = new System.Windows.Forms.Panel();
            this.lblExplainSuffix = new System.Windows.Forms.Label();
            this.txtExplain = new System.Windows.Forms.TextBox();
            this.chk7 = new System.Windows.Forms.CheckBox();
            this.pnlComplex1 = new System.Windows.Forms.Panel();
            this.lblBeneficiarySuffix = new System.Windows.Forms.Label();
            this.txtBeneficiary = new System.Windows.Forms.TextBox();
            this.chk6 = new System.Windows.Forms.CheckBox();
            this.chk5 = new System.Windows.Forms.CheckBox();
            this.chk4 = new System.Windows.Forms.CheckBox();
            this.chk3 = new System.Windows.Forms.CheckBox();
            this.chk2 = new System.Windows.Forms.CheckBox();
            this.chk1 = new System.Windows.Forms.CheckBox();
            this.lblDeclare = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAgree = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlHeader.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlComplex2.SuspendLayout();
            this.pnlComplex1.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(115)))), ((int)(((byte)(200)))));
            this.pnlHeader.Controls.Add(this.pictureBox1);
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(900, 85);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(855, 1);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 45);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(599, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Creating a nudge: Making ABDM implementation easier - Consent Language";
            // 
            // pnlContent
            // 
            this.pnlContent.AutoScroll = true;
            this.pnlContent.BackColor = System.Drawing.Color.White;
            this.pnlContent.Controls.Add(this.lblNote);
            this.pnlContent.Controls.Add(this.pnlComplex2);
            this.pnlContent.Controls.Add(this.pnlComplex1);
            this.pnlContent.Controls.Add(this.chk5);
            this.pnlContent.Controls.Add(this.chk4);
            this.pnlContent.Controls.Add(this.chk3);
            this.pnlContent.Controls.Add(this.chk2);
            this.pnlContent.Controls.Add(this.chk1);
            this.pnlContent.Controls.Add(this.lblDeclare);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 85);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(20);
            this.pnlContent.Size = new System.Drawing.Size(900, 465);
            this.pnlContent.TabIndex = 1;
            // 
            // lblNote
            // 
            this.lblNote.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblNote.Location = new System.Drawing.Point(20, 340);
            this.lblNote.Name = "lblNote";
            this.lblNote.Padding = new System.Windows.Forms.Padding(10, 10, 0, 0);
            this.lblNote.Size = new System.Drawing.Size(860, 60);
            this.lblNote.TabIndex = 8;
            this.lblNote.Text = resources.GetString("lblNote.Text");
            // 
            // pnlComplex2
            // 
            this.pnlComplex2.Controls.Add(this.lblExplainSuffix);
            this.pnlComplex2.Controls.Add(this.txtExplain);
            this.pnlComplex2.Controls.Add(this.chk7);
            this.pnlComplex2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlComplex2.Location = new System.Drawing.Point(20, 300);
            this.pnlComplex2.Name = "pnlComplex2";
            this.pnlComplex2.Padding = new System.Windows.Forms.Padding(40, 5, 0, 5);
            this.pnlComplex2.Size = new System.Drawing.Size(860, 40);
            this.pnlComplex2.TabIndex = 7;
            // 
            // lblExplainSuffix
            // 
            this.lblExplainSuffix.AutoSize = true;
            this.lblExplainSuffix.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblExplainSuffix.Location = new System.Drawing.Point(240, 5);
            this.lblExplainSuffix.Name = "lblExplainSuffix";
            this.lblExplainSuffix.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.lblExplainSuffix.Size = new System.Drawing.Size(597, 25);
            this.lblExplainSuffix.TabIndex = 2;
            this.lblExplainSuffix.Text = "have been explained about the consent as stated above and hereby provide my conse" +
    "nt.";
            // 
            // txtExplain
            // 
            this.txtExplain.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtExplain.Location = new System.Drawing.Point(82, 5);
            this.txtExplain.Name = "txtExplain";
            this.txtExplain.Size = new System.Drawing.Size(158, 27);
            this.txtExplain.TabIndex = 1;
            // 
            // chk7
            // 
            this.chk7.AutoSize = true;
            this.chk7.Dock = System.Windows.Forms.DockStyle.Left;
            this.chk7.Location = new System.Drawing.Point(40, 5);
            this.chk7.Name = "chk7";
            this.chk7.Size = new System.Drawing.Size(42, 30);
            this.chk7.TabIndex = 0;
            this.chk7.Text = "I, ";
            this.chk7.UseVisualStyleBackColor = true;
            // 
            // pnlComplex1
            // 
            this.pnlComplex1.Controls.Add(this.lblBeneficiarySuffix);
            this.pnlComplex1.Controls.Add(this.txtBeneficiary);
            this.pnlComplex1.Controls.Add(this.chk6);
            this.pnlComplex1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlComplex1.Location = new System.Drawing.Point(20, 260);
            this.pnlComplex1.Name = "pnlComplex1";
            this.pnlComplex1.Padding = new System.Windows.Forms.Padding(40, 5, 0, 5);
            this.pnlComplex1.Size = new System.Drawing.Size(860, 40);
            this.pnlComplex1.TabIndex = 6;
            // 
            // lblBeneficiarySuffix
            // 
            this.lblBeneficiarySuffix.AutoSize = true;
            this.lblBeneficiarySuffix.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblBeneficiarySuffix.Location = new System.Drawing.Point(240, 5);
            this.lblBeneficiarySuffix.Name = "lblBeneficiarySuffix";
            this.lblBeneficiarySuffix.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.lblBeneficiarySuffix.Size = new System.Drawing.Size(614, 25);
            this.lblBeneficiarySuffix.TabIndex = 2;
            this.lblBeneficiarySuffix.Text = "confirm that I have duly informed and explained the beneficiary of the contents o" +
    "f consent.";
            // 
            // txtBeneficiary
            // 
            this.txtBeneficiary.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtBeneficiary.Location = new System.Drawing.Point(82, 5);
            this.txtBeneficiary.Name = "txtBeneficiary";
            this.txtBeneficiary.Size = new System.Drawing.Size(158, 27);
            this.txtBeneficiary.TabIndex = 1;
            // 
            // chk6
            // 
            this.chk6.AutoSize = true;
            this.chk6.Dock = System.Windows.Forms.DockStyle.Left;
            this.chk6.Location = new System.Drawing.Point(40, 5);
            this.chk6.Name = "chk6";
            this.chk6.Size = new System.Drawing.Size(42, 30);
            this.chk6.TabIndex = 0;
            this.chk6.Text = "I, ";
            this.chk6.UseVisualStyleBackColor = true;
            // 
            // chk5
            // 
            this.chk5.Dock = System.Windows.Forms.DockStyle.Top;
            this.chk5.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk5.Location = new System.Drawing.Point(20, 220);
            this.chk5.Name = "chk5";
            this.chk5.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
            this.chk5.Size = new System.Drawing.Size(860, 40);
            this.chk5.TabIndex = 5;
            this.chk5.Text = "I consent to the anonymization and subsequent use of my government health records" +
    " for public health purposes.";
            this.chk5.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chk5.UseVisualStyleBackColor = true;
            // 
            // chk4
            // 
            this.chk4.Dock = System.Windows.Forms.DockStyle.Top;
            this.chk4.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk4.Location = new System.Drawing.Point(20, 180);
            this.chk4.Name = "chk4";
            this.chk4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
            this.chk4.Size = new System.Drawing.Size(860, 40);
            this.chk4.TabIndex = 4;
            this.chk4.Text = "I authorize the sharing of all my health records with healthcare provider(s) for " +
    "the purpose of providing healthcare services to me during this encounter.";
            this.chk4.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chk4.UseVisualStyleBackColor = true;
            // 
            // chk3
            // 
            this.chk3.Dock = System.Windows.Forms.DockStyle.Top;
            this.chk3.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk3.Location = new System.Drawing.Point(20, 135);
            this.chk3.Name = "chk3";
            this.chk3.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
            this.chk3.Size = new System.Drawing.Size(860, 45);
            this.chk3.TabIndex = 3;
            this.chk3.Text = "I consent to usage of my ABHA address and ABHA number for linking of my legacy (p" +
    "ast) government health records and those which will be generated during this enc" +
    "ounter.";
            this.chk3.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chk3.UseVisualStyleBackColor = true;
            // 
            // chk2
            // 
            this.chk2.Dock = System.Windows.Forms.DockStyle.Top;
            this.chk2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk2.Location = new System.Drawing.Point(20, 90);
            this.chk2.Name = "chk2";
            this.chk2.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
            this.chk2.Size = new System.Drawing.Size(860, 45);
            this.chk2.TabIndex = 2;
            this.chk2.Text = "I intend to create Ayushman Bharat Health Account Number (\"ABHA number\") and Ayus" +
    "hman Bharat Health Account address (\"ABHA Address\") using document other than Aa" +
    "dhaar.";
            this.chk2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chk2.UseVisualStyleBackColor = true;
            // 
            // chk1
            // 
            this.chk1.Dock = System.Windows.Forms.DockStyle.Top;
            this.chk1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk1.Location = new System.Drawing.Point(20, 20);
            this.chk1.Name = "chk1";
            this.chk1.Padding = new System.Windows.Forms.Padding(10, 0, 0, 10);
            this.chk1.Size = new System.Drawing.Size(860, 70);
            this.chk1.TabIndex = 1;
            this.chk1.Text = resources.GetString("chk1.Text");
            this.chk1.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chk1.UseVisualStyleBackColor = true;
            // 
            // lblDeclare
            // 
            this.lblDeclare.AutoSize = true;
            this.lblDeclare.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblDeclare.Location = new System.Drawing.Point(23, 20);
            this.lblDeclare.Name = "lblDeclare";
            this.lblDeclare.Size = new System.Drawing.Size(154, 20);
            this.lblDeclare.TabIndex = 0;
            this.lblDeclare.Text = "I hereby declare that:";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlFooter.Controls.Add(this.btnCancel);
            this.pnlFooter.Controls.Add(this.btnAgree);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 550);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(900, 60);
            this.pnlFooter.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(450, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 40);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAgree
            // 
            this.btnAgree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnAgree.FlatAppearance.BorderSize = 0;
            this.btnAgree.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgree.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnAgree.ForeColor = System.Drawing.Color.White;
            this.btnAgree.Location = new System.Drawing.Point(320, 10);
            this.btnAgree.Name = "btnAgree";
            this.btnAgree.Size = new System.Drawing.Size(120, 40);
            this.btnAgree.TabIndex = 0;
            this.btnAgree.Text = "I Agree";
            this.btnAgree.UseVisualStyleBackColor = false;
            this.btnAgree.Click += new System.EventHandler(this.btnAgree_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HMS.Properties.Resources.nhalogo;
            this.pictureBox1.Location = new System.Drawing.Point(703, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(154, 63);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // frmABHAConsent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(900, 610);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmABHAConsent";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.pnlComplex2.ResumeLayout(false);
            this.pnlComplex2.PerformLayout();
            this.pnlComplex1.ResumeLayout(false);
            this.pnlComplex1.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblDeclare;
        private System.Windows.Forms.CheckBox chk1;
        private System.Windows.Forms.CheckBox chk2;
        private System.Windows.Forms.CheckBox chk3;
        private System.Windows.Forms.CheckBox chk4;
        private System.Windows.Forms.CheckBox chk5;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnAgree;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlComplex2;
        private System.Windows.Forms.Label lblExplainSuffix;
        private System.Windows.Forms.TextBox txtExplain;
        private System.Windows.Forms.CheckBox chk7;
        private System.Windows.Forms.Panel pnlComplex1;
        private System.Windows.Forms.Label lblBeneficiarySuffix;
        private System.Windows.Forms.TextBox txtBeneficiary;
        private System.Windows.Forms.CheckBox chk6;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
