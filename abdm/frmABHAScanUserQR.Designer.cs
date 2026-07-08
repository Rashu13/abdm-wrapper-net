using System.Windows.Forms;

namespace HMS.abdm
{
    partial class frmABHAScanUserQR
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.pnlCameraFrame = new System.Windows.Forms.Panel();
            this.lblCameraOverlay = new System.Windows.Forms.Label();
            this.timerScanLine = new System.Windows.Forms.Timer();
            this.btnScan = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlCameraFrame.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(55, 115, 200);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(500, 50);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(248, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Scan Patient ABHA Card QR (M1)";
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(450, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(50, 50);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // lblInstruction
            // 
            this.lblInstruction.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblInstruction.ForeColor = System.Drawing.Color.FromArgb(70, 70, 70);
            this.lblInstruction.Location = new System.Drawing.Point(20, 65);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(460, 45);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = "Position the patient\'s ABHA QR code in front of the scanner webcam.";
            this.lblInstruction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlCameraFrame
            // 
            this.pnlCameraFrame.BackColor = System.Drawing.Color.Black;
            this.pnlCameraFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCameraFrame.Controls.Add(this.lblCameraOverlay);
            this.pnlCameraFrame.Location = new System.Drawing.Point(80, 120);
            this.pnlCameraFrame.Name = "pnlCameraFrame";
            this.pnlCameraFrame.Size = new System.Drawing.Size(340, 240);
            this.pnlCameraFrame.TabIndex = 2;
            // 
            // lblCameraOverlay
            // 
            this.lblCameraOverlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCameraOverlay.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCameraOverlay.ForeColor = System.Drawing.Color.LightGray;
            this.lblCameraOverlay.Location = new System.Drawing.Point(0, 0);
            this.lblCameraOverlay.Name = "lblCameraOverlay";
            this.lblCameraOverlay.Size = new System.Drawing.Size(338, 238);
            this.lblCameraOverlay.TabIndex = 0;
            this.lblCameraOverlay.Text = "📷 Camera View Active\r\n[Waiting for ABHA QR Code]";
            this.lblCameraOverlay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerScanLine
            // 
            this.timerScanLine.Interval = 30;
            // 
            // btnScan
            // 
            this.btnScan.BackColor = System.Drawing.Color.FromArgb(46, 117, 89);
            this.btnScan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnScan.FlatAppearance.BorderSize = 0;
            this.btnScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScan.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnScan.ForeColor = System.Drawing.Color.White;
            this.btnScan.Location = new System.Drawing.Point(150, 395);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(200, 45);
            this.btnScan.TabIndex = 3;
            this.btnScan.Text = "Simulate Scan Card QR";
            this.btnScan.UseVisualStyleBackColor = false;
            // 
            // frmABHAScanUserQR
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(500, 480);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.pnlCameraFrame);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmABHAScanUserQR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scan Patient ABHA Card QR (M1)";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlCameraFrame.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlCameraFrame;
        private System.Windows.Forms.Label lblCameraOverlay;
        private System.Windows.Forms.Timer timerScanLine;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Label lblInstruction;
    }
}
