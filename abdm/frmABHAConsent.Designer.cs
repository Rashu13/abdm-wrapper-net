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
            pnlHeader = new Panel();
            pictureBox1 = new PictureBox();
            btnClose = new Button();
            lblTitle = new Label();
            pnlContent = new Panel();
            lblNote = new Label();
            pnlComplex2 = new Panel();
            lblExplainSuffix = new Label();
            txtExplain = new TextBox();
            chk7 = new CheckBox();
            pnlComplex1 = new Panel();
            lblBeneficiarySuffix = new Label();
            txtBeneficiary = new TextBox();
            chk6 = new CheckBox();
            chk5 = new CheckBox();
            chk4 = new CheckBox();
            chk3 = new CheckBox();
            chk2 = new CheckBox();
            chk1 = new CheckBox();
            lblDeclare = new Label();
            pnlFooter = new Panel();
            btnCancel = new Button();
            btnAgree = new Button();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlContent.SuspendLayout();
            pnlComplex2.SuspendLayout();
            pnlComplex1.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(55, 115, 200);
            pnlHeader.Controls.Add(pictureBox1);
            pnlHeader.Controls.Add(btnClose);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(900, 85);
            pnlHeader.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(703, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(154, 63);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 17;
            pictureBox1.TabStop = false;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.Red;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(855, 1);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(45, 45);
            btnClose.TabIndex = 1;
            btnClose.Text = "X";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(12, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(599, 23);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Creating a nudge: Making ABDM implementation easier - Consent Language";
            // 
            // pnlContent
            // 
            pnlContent.AutoScroll = true;
            pnlContent.BackColor = Color.White;
            pnlContent.Controls.Add(lblNote);
            pnlContent.Controls.Add(pnlComplex2);
            pnlContent.Controls.Add(pnlComplex1);
            pnlContent.Controls.Add(chk5);
            pnlContent.Controls.Add(chk4);
            pnlContent.Controls.Add(chk3);
            pnlContent.Controls.Add(chk2);
            pnlContent.Controls.Add(chk1);
            pnlContent.Controls.Add(lblDeclare);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 85);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(20);
            pnlContent.Size = new Size(900, 465);
            pnlContent.TabIndex = 1;
            // 
            // lblNote
            // 
            lblNote.Dock = DockStyle.Top;
            lblNote.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblNote.Location = new Point(20, 340);
            lblNote.Name = "lblNote";
            lblNote.Padding = new Padding(10, 10, 0, 0);
            lblNote.Size = new Size(860, 60);
            lblNote.TabIndex = 8;
            lblNote.Text = resources.GetString("lblNote.Text");
            // 
            // pnlComplex2
            // 
            pnlComplex2.Controls.Add(lblExplainSuffix);
            pnlComplex2.Controls.Add(txtExplain);
            pnlComplex2.Controls.Add(chk7);
            pnlComplex2.Dock = DockStyle.Top;
            pnlComplex2.Location = new Point(20, 300);
            pnlComplex2.Name = "pnlComplex2";
            pnlComplex2.Padding = new Padding(40, 5, 0, 5);
            pnlComplex2.Size = new Size(860, 40);
            pnlComplex2.TabIndex = 7;
            // 
            // lblExplainSuffix
            // 
            lblExplainSuffix.AutoSize = true;
            lblExplainSuffix.Dock = DockStyle.Left;
            lblExplainSuffix.Location = new Point(240, 5);
            lblExplainSuffix.Name = "lblExplainSuffix";
            lblExplainSuffix.Padding = new Padding(5, 5, 0, 0);
            lblExplainSuffix.Size = new Size(597, 25);
            lblExplainSuffix.TabIndex = 2;
            lblExplainSuffix.Text = "have been explained about the consent as stated above and hereby provide my consent.";
            // 
            // txtExplain
            // 
            txtExplain.Dock = DockStyle.Left;
            txtExplain.Location = new Point(82, 5);
            txtExplain.Name = "txtExplain";
            txtExplain.Size = new Size(158, 27);
            txtExplain.TabIndex = 1;
            // 
            // chk7
            // 
            chk7.AutoSize = true;
            chk7.Dock = DockStyle.Left;
            chk7.Location = new Point(40, 5);
            chk7.Name = "chk7";
            chk7.Size = new Size(42, 30);
            chk7.TabIndex = 0;
            chk7.Text = "I, ";
            chk7.UseVisualStyleBackColor = true;
            // 
            // pnlComplex1
            // 
            pnlComplex1.Controls.Add(lblBeneficiarySuffix);
            pnlComplex1.Controls.Add(txtBeneficiary);
            pnlComplex1.Controls.Add(chk6);
            pnlComplex1.Dock = DockStyle.Top;
            pnlComplex1.Location = new Point(20, 260);
            pnlComplex1.Name = "pnlComplex1";
            pnlComplex1.Padding = new Padding(40, 5, 0, 5);
            pnlComplex1.Size = new Size(860, 40);
            pnlComplex1.TabIndex = 6;
            // 
            // lblBeneficiarySuffix
            // 
            lblBeneficiarySuffix.AutoSize = true;
            lblBeneficiarySuffix.Dock = DockStyle.Left;
            lblBeneficiarySuffix.Location = new Point(240, 5);
            lblBeneficiarySuffix.Name = "lblBeneficiarySuffix";
            lblBeneficiarySuffix.Padding = new Padding(5, 5, 0, 0);
            lblBeneficiarySuffix.Size = new Size(614, 25);
            lblBeneficiarySuffix.TabIndex = 2;
            lblBeneficiarySuffix.Text = "confirm that I have duly informed and explained the beneficiary of the contents of consent.";
            // 
            // txtBeneficiary
            // 
            txtBeneficiary.Dock = DockStyle.Left;
            txtBeneficiary.Location = new Point(82, 5);
            txtBeneficiary.Name = "txtBeneficiary";
            txtBeneficiary.Size = new Size(158, 27);
            txtBeneficiary.TabIndex = 1;
            // 
            // chk6
            // 
            chk6.AutoSize = true;
            chk6.Dock = DockStyle.Left;
            chk6.Location = new Point(40, 5);
            chk6.Name = "chk6";
            chk6.Size = new Size(42, 30);
            chk6.TabIndex = 0;
            chk6.Text = "I, ";
            chk6.UseVisualStyleBackColor = true;
            // 
            // chk5
            // 
            chk5.Dock = DockStyle.Top;
            chk5.Font = new Font("Segoe UI", 8.5F);
            chk5.Location = new Point(20, 220);
            chk5.Name = "chk5";
            chk5.Padding = new Padding(10, 0, 0, 10);
            chk5.Size = new Size(860, 40);
            chk5.TabIndex = 5;
            chk5.Text = "I consent to the anonymization and subsequent use of my government health records for public health purposes.";
            chk5.TextAlign = ContentAlignment.TopLeft;
            chk5.UseVisualStyleBackColor = true;
            // 
            // chk4
            // 
            chk4.Dock = DockStyle.Top;
            chk4.Font = new Font("Segoe UI", 8.5F);
            chk4.Location = new Point(20, 180);
            chk4.Name = "chk4";
            chk4.Padding = new Padding(10, 0, 0, 10);
            chk4.Size = new Size(860, 40);
            chk4.TabIndex = 4;
            chk4.Text = "I authorize the sharing of all my health records with healthcare provider(s) for the purpose of providing healthcare services to me during this encounter.";
            chk4.TextAlign = ContentAlignment.TopLeft;
            chk4.UseVisualStyleBackColor = true;
            // 
            // chk3
            // 
            chk3.Dock = DockStyle.Top;
            chk3.Font = new Font("Segoe UI", 8.5F);
            chk3.Location = new Point(20, 135);
            chk3.Name = "chk3";
            chk3.Padding = new Padding(10, 0, 0, 10);
            chk3.Size = new Size(860, 45);
            chk3.TabIndex = 3;
            chk3.Text = "I consent to usage of my ABHA address and ABHA number for linking of my legacy (past) government health records and those which will be generated during this encounter.";
            chk3.TextAlign = ContentAlignment.TopLeft;
            chk3.UseVisualStyleBackColor = true;
            // 
            // chk2
            // 
            chk2.Dock = DockStyle.Top;
            chk2.Font = new Font("Segoe UI", 8.5F);
            chk2.Location = new Point(20, 90);
            chk2.Name = "chk2";
            chk2.Padding = new Padding(10, 0, 0, 10);
            chk2.Size = new Size(860, 45);
            chk2.TabIndex = 2;
            chk2.Text = "I intend to create Ayushman Bharat Health Account Number (\"ABHA number\") and Ayushman Bharat Health Account address (\"ABHA Address\") using document other than Aadhaar.";
            chk2.TextAlign = ContentAlignment.TopLeft;
            chk2.UseVisualStyleBackColor = true;
            // 
            // chk1
            // 
            chk1.Dock = DockStyle.Top;
            chk1.Font = new Font("Segoe UI", 8.5F);
            chk1.Location = new Point(20, 20);
            chk1.Name = "chk1";
            chk1.Padding = new Padding(10, 0, 0, 10);
            chk1.Size = new Size(860, 70);
            chk1.TabIndex = 1;
            chk1.Text = resources.GetString("chk1.Text");
            chk1.TextAlign = ContentAlignment.TopLeft;
            chk1.UseVisualStyleBackColor = true;
            // 
            // lblDeclare
            // 
            lblDeclare.AutoSize = true;
            lblDeclare.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblDeclare.Location = new Point(23, 20);
            lblDeclare.Name = "lblDeclare";
            lblDeclare.Size = new Size(154, 20);
            lblDeclare.TabIndex = 0;
            lblDeclare.Text = "I hereby declare that:";
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(240, 240, 240);
            pnlFooter.Controls.Add(btnCancel);
            pnlFooter.Controls.Add(btnAgree);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 550);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(900, 60);
            pnlFooter.TabIndex = 2;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(200, 200, 200);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.Black;
            btnCancel.Location = new Point(450, 10);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnAgree
            // 
            btnAgree.BackColor = Color.FromArgb(230, 100, 50);
            btnAgree.FlatAppearance.BorderSize = 0;
            btnAgree.FlatStyle = FlatStyle.Flat;
            btnAgree.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnAgree.ForeColor = Color.White;
            btnAgree.Location = new Point(320, 10);
            btnAgree.Name = "btnAgree";
            btnAgree.Size = new Size(120, 40);
            btnAgree.TabIndex = 0;
            btnAgree.Text = "I Agree";
            btnAgree.UseVisualStyleBackColor = false;
            btnAgree.Click += btnAgree_Click;
            // 
            // frmABHAConsent
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(900, 610);
            Controls.Add(pnlContent);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmABHAConsent";
            StartPosition = FormStartPosition.CenterScreen;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            pnlComplex2.ResumeLayout(false);
            pnlComplex2.PerformLayout();
            pnlComplex1.ResumeLayout(false);
            pnlComplex1.PerformLayout();
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);

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
