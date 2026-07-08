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
            this.lblHipId = new System.Windows.Forms.Label();
            this.txtHipId = new System.Windows.Forms.TextBox();
            this.lblClientId = new System.Windows.Forms.Label();
            this.txtClientId = new System.Windows.Forms.TextBox();
            this.lblClientSecret = new System.Windows.Forms.Label();
            this.txtClientSecret = new System.Windows.Forms.TextBox();
            this.btnSaveConfig = new System.Windows.Forms.Button();
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
            this.lblTitle.Text = "ABDM Bridge & HIP Settings";
            
            // lblBridgeUrl
            this.lblBridgeUrl.AutoSize = true;
            this.lblBridgeUrl.Location = new System.Drawing.Point(20, 70);
            this.lblBridgeUrl.Name = "lblBridgeUrl";
            this.lblBridgeUrl.Size = new System.Drawing.Size(86, 20);
            this.lblBridgeUrl.TabIndex = 1;
            this.lblBridgeUrl.Text = "Bridge URL:";
            
            // txtBridgeUrl
            this.txtBridgeUrl.Location = new System.Drawing.Point(150, 67);
            this.txtBridgeUrl.Name = "txtBridgeUrl";
            this.txtBridgeUrl.Size = new System.Drawing.Size(380, 27);
            this.txtBridgeUrl.TabIndex = 2;
            this.txtBridgeUrl.Text = "http://sbx.wati.digital";
            
            // lblHipId
            this.lblHipId.AutoSize = true;
            this.lblHipId.Location = new System.Drawing.Point(20, 110);
            this.lblHipId.Name = "lblHipId";
            this.lblHipId.Size = new System.Drawing.Size(117, 20);
            this.lblHipId.TabIndex = 7;
            this.lblHipId.Text = "Facility (HIP) ID:";
            
            // txtHipId
            this.txtHipId.Location = new System.Drawing.Point(150, 107);
            this.txtHipId.Name = "txtHipId";
            this.txtHipId.Size = new System.Drawing.Size(380, 27);
            this.txtHipId.TabIndex = 8;
            this.txtHipId.Text = "IN0610090658";
            
            // lblClientId
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(20, 150);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(69, 20);
            this.lblClientId.TabIndex = 9;
            this.lblClientId.Text = "Client ID:";
            
            // txtClientId
            this.txtClientId.Location = new System.Drawing.Point(150, 147);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.Size = new System.Drawing.Size(380, 27);
            this.txtClientId.TabIndex = 10;
            
            // lblClientSecret
            this.lblClientSecret.AutoSize = true;
            this.lblClientSecret.Location = new System.Drawing.Point(20, 190);
            this.lblClientSecret.Name = "lblClientSecret";
            this.lblClientSecret.Size = new System.Drawing.Size(94, 20);
            this.lblClientSecret.TabIndex = 11;
            this.lblClientSecret.Text = "Client Secret:";
            
            // txtClientSecret
            this.txtClientSecret.Location = new System.Drawing.Point(150, 187);
            this.txtClientSecret.Name = "txtClientSecret";
            this.txtClientSecret.Size = new System.Drawing.Size(380, 27);
            this.txtClientSecret.TabIndex = 12;
            
            // btnSaveConfig
            this.btnSaveConfig.Location = new System.Drawing.Point(150, 225);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(180, 35);
            this.btnSaveConfig.TabIndex = 13;
            this.btnSaveConfig.Text = "Save Local Settings";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            
            // btnRegisterBridge
            this.btnRegisterBridge.Location = new System.Drawing.Point(20, 275);
            this.btnRegisterBridge.Name = "btnRegisterBridge";
            this.btnRegisterBridge.Size = new System.Drawing.Size(245, 35);
            this.btnRegisterBridge.TabIndex = 3;
            this.btnRegisterBridge.Text = "1. Register Bridge URL";
            this.btnRegisterBridge.UseVisualStyleBackColor = true;
            this.btnRegisterBridge.Click += new System.EventHandler(this.btnRegisterBridge_Click);
            
            // btnCheckServices
            this.btnCheckServices.Location = new System.Drawing.Point(285, 275);
            this.btnCheckServices.Name = "btnCheckServices";
            this.btnCheckServices.Size = new System.Drawing.Size(245, 35);
            this.btnCheckServices.TabIndex = 4;
            this.btnCheckServices.Text = "2. Check Bridge Services";
            this.btnCheckServices.UseVisualStyleBackColor = true;
            this.btnCheckServices.Click += new System.EventHandler(this.btnCheckServices_Click);
            
            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblStatus.Location = new System.Drawing.Point(20, 325);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(57, 20);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Status: Idle";
            
            // txtLog
            this.txtLog.Location = new System.Drawing.Point(20, 355);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(510, 180);
            this.txtLog.TabIndex = 6;
            
            // frmBridgeConfig
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 560);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnCheckServices);
            this.Controls.Add(this.btnRegisterBridge);
            this.Controls.Add(this.btnSaveConfig);
            this.Controls.Add(this.txtClientSecret);
            this.Controls.Add(this.lblClientSecret);
            this.Controls.Add(this.txtClientId);
            this.Controls.Add(this.lblClientId);
            this.Controls.Add(this.txtHipId);
            this.Controls.Add(this.lblHipId);
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
        private System.Windows.Forms.Label lblHipId;
        private System.Windows.Forms.TextBox txtHipId;
        private System.Windows.Forms.Label lblClientId;
        private System.Windows.Forms.TextBox txtClientId;
        private System.Windows.Forms.Label lblClientSecret;
        private System.Windows.Forms.TextBox txtClientSecret;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnRegisterBridge;
        private System.Windows.Forms.Button btnCheckServices;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtLog;
    }
}
