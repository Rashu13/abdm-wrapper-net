namespace HMS.abdm
{
    partial class frmABHALogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmABHALogin));
            pnlHeader = new Panel();
            pictureBox1 = new PictureBox();
            btnClose = new Button();
            lblTitle = new Label();
            lblInstructions = new Label();
            lblLoginId = new Label();
            txtLoginId = new TextBox();
            btnSendOtp = new Button();
            pnlOtp = new Panel();
            lblOtp = new Label();
            txtOtp = new TextBox();
            btnVerify = new Button();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            pnlOtp.SuspendLayout();
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
            pnlHeader.Margin = new Padding(3, 4, 3, 4);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(500, 84);
            pnlHeader.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 1);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(154, 79);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 17;
            pictureBox1.TabStop = false;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(455, 0);
            btnClose.Margin = new Padding(3, 4, 3, 4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(45, 56);
            btnClose.TabIndex = 1;
            btnClose.Text = "X";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(201, 25);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(115, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ABHA Login";
            // 
            // lblInstructions
            // 
            lblInstructions.AutoSize = true;
            lblInstructions.Font = new Font("Segoe UI", 9F);
            lblInstructions.ForeColor = Color.Gray;
            lblInstructions.Location = new Point(30, 88);
            lblInstructions.Name = "lblInstructions";
            lblInstructions.Size = new Size(443, 20);
            lblInstructions.TabIndex = 1;
            lblInstructions.Text = "Enter your registered Mobile Number or Aadhaar Number";
            // 
            // lblLoginId
            // 
            lblLoginId.AutoSize = true;
            lblLoginId.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblLoginId.Location = new Point(30, 131);
            lblLoginId.Name = "lblLoginId";
            lblLoginId.Size = new Size(157, 23);
            lblLoginId.TabIndex = 2;
            lblLoginId.Text = "Mobile / Aadhaar No:";
            // 
            // txtLoginId
            // 
            txtLoginId.Font = new Font("Segoe UI", 12F);
            txtLoginId.Location = new Point(30, 169);
            txtLoginId.Margin = new Padding(3, 4, 3, 4);
            txtLoginId.Name = "txtLoginId";
            txtLoginId.Size = new Size(440, 34);
            txtLoginId.TabIndex = 3;
            // 
            // btnSendOtp
            // 
            btnSendOtp.BackColor = Color.FromArgb(55, 115, 200);
            btnSendOtp.FlatStyle = FlatStyle.Flat;
            btnSendOtp.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnSendOtp.ForeColor = Color.White;
            btnSendOtp.Location = new Point(30, 231);
            btnSendOtp.Margin = new Padding(3, 4, 3, 4);
            btnSendOtp.Name = "btnSendOtp";
            btnSendOtp.Size = new Size(440, 56);
            btnSendOtp.TabIndex = 4;
            btnSendOtp.Text = "Send OTP";
            btnSendOtp.UseVisualStyleBackColor = false;
            btnSendOtp.Click += btnSendOtp_Click;
            // 
            // pnlOtp
            // 
            pnlOtp.Controls.Add(lblOtp);
            pnlOtp.Controls.Add(txtOtp);
            pnlOtp.Controls.Add(btnVerify);
            pnlOtp.Location = new Point(0, 300);
            pnlOtp.Margin = new Padding(3, 4, 3, 4);
            pnlOtp.Name = "pnlOtp";
            pnlOtp.Size = new Size(500, 188);
            pnlOtp.TabIndex = 5;
            pnlOtp.Visible = false;
            // 
            // lblOtp
            // 
            lblOtp.AutoSize = true;
            lblOtp.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblOtp.Location = new Point(30, 12);
            lblOtp.Name = "lblOtp";
            lblOtp.Size = new Size(90, 23);
            lblOtp.TabIndex = 0;
            lblOtp.Text = "Enter OTP:";
            // 
            // txtOtp
            // 
            txtOtp.Font = new Font("Segoe UI", 12F);
            txtOtp.Location = new Point(30, 50);
            txtOtp.Margin = new Padding(3, 4, 3, 4);
            txtOtp.Name = "txtOtp";
            txtOtp.Size = new Size(440, 34);
            txtOtp.TabIndex = 1;
            // 
            // btnVerify
            // 
            btnVerify.BackColor = Color.FromArgb(230, 100, 50);
            btnVerify.FlatStyle = FlatStyle.Flat;
            btnVerify.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnVerify.ForeColor = Color.White;
            btnVerify.Location = new Point(30, 112);
            btnVerify.Margin = new Padding(3, 4, 3, 4);
            btnVerify.Name = "btnVerify";
            btnVerify.Size = new Size(440, 56);
            btnVerify.TabIndex = 2;
            btnVerify.Text = "Verify && Login";
            btnVerify.UseVisualStyleBackColor = false;
            btnVerify.Click += btnVerify_Click;
            // 
            // frmABHALogin
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(500, 500);
            Controls.Add(pnlOtp);
            Controls.Add(btnSendOtp);
            Controls.Add(txtLoginId);
            Controls.Add(lblLoginId);
            Controls.Add(lblInstructions);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "frmABHALogin";
            StartPosition = FormStartPosition.CenterParent;
            Text = "ABHA Login";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            pnlOtp.ResumeLayout(false);
            pnlOtp.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblInstructions;
        private System.Windows.Forms.Label lblLoginId;
        private System.Windows.Forms.TextBox txtLoginId;
        private System.Windows.Forms.Button btnSendOtp;
        private System.Windows.Forms.Panel pnlOtp;
        private System.Windows.Forms.Label lblOtp;
        private System.Windows.Forms.TextBox txtOtp;
        private System.Windows.Forms.Button btnVerify;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
