namespace HMS.abdm
{
    partial class frmBridgeConfig
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblBridgeUrl = new System.Windows.Forms.Label();
            this.txtBridgeUrl = new System.Windows.Forms.TextBox();
            this.btnRegisterBridge = new System.Windows.Forms.Button();
            this.btnCheckServices = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            
            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(262, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ABDM Bridge Configuration";
            
            // lblBridgeUrl
            this.lblBridgeUrl.AutoSize = true;
            this.lblBridgeUrl.Location = new System.Drawing.Point(20, 70);
            this.lblBridgeUrl.Name = "lblBridgeUrl";
            this.lblBridgeUrl.Size = new System.Drawing.Size(86, 20);
            this.lblBridgeUrl.TabIndex = 1;
            this.lblBridgeUrl.Text = "Bridge URL:";
            
            // txtBridgeUrl
            this.txtBridgeUrl.Location = new System.Drawing.Point(120, 67);
            this.txtBridgeUrl.Name = "txtBridgeUrl";
            this.txtBridgeUrl.Size = new System.Drawing.Size(400, 27);
            this.txtBridgeUrl.TabIndex = 2;
            this.txtBridgeUrl.Text = "https://sbx.wati.digital";
            
            // btnRegisterBridge
            this.btnRegisterBridge.Location = new System.Drawing.Point(120, 110);
            this.btnRegisterBridge.Name = "btnRegisterBridge";
            this.btnRegisterBridge.Size = new System.Drawing.Size(190, 35);
            this.btnRegisterBridge.TabIndex = 3;
            this.btnRegisterBridge.Text = "1. Register Bridge URL";
            this.btnRegisterBridge.UseVisualStyleBackColor = true;
            this.btnRegisterBridge.Click += new System.EventHandler(this.btnRegisterBridge_Click);
            
            // btnCheckServices
            this.btnCheckServices.Location = new System.Drawing.Point(330, 110);
            this.btnCheckServices.Name = "btnCheckServices";
            this.btnCheckServices.Size = new System.Drawing.Size(190, 35);
            this.btnCheckServices.TabIndex = 4;
            this.btnCheckServices.Text = "2. Check Bridge Services";
            this.btnCheckServices.UseVisualStyleBackColor = true;
            this.btnCheckServices.Click += new System.EventHandler(this.btnCheckServices_Click);
            
            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblStatus.Location = new System.Drawing.Point(20, 160);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(57, 20);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Status: Idle";
            
            // txtLog
            this.txtLog.Location = new System.Drawing.Point(20, 190);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(500, 200);
            this.txtLog.TabIndex = 6;
            
            // frmBridgeConfig
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 420);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCheckServices);
            this.Controls.Add(this.btnRegisterBridge);
            this.Controls.Add(this.txtBridgeUrl);
            this.Controls.Add(this.lblBridgeUrl);
            this.Controls.Add(this.lblTitle);
            this.Name = "frmBridgeConfig";
            this.Text = "ABDM Configuration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBridgeUrl;
        private System.Windows.Forms.TextBox txtBridgeUrl;
        private System.Windows.Forms.Button btnRegisterBridge;
        private System.Windows.Forms.Button btnCheckServices;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtLog;
    }
}
