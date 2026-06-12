using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace HMS.abdm
{
    public partial class frmABHAQR : Form
    {
        public frmABHAQR()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmABHAQR_Load(object sender, EventArgs e)
        {
            LoadFacilityDetails();
            GenerateQR();
        }

        private void LoadFacilityDetails()
        {
            try
            {
                string hipId = ConfigurationManager.AppSettings["AbdmSettings:HipId"] ?? "IN0610090658";
                string hipName = ConfigurationManager.AppSettings["AbdmSettings:HipName"] ?? "MIDHA HOSPITAL";

                lblFacilityId.Text = "HIP ID: " + hipId;
                lblFacilityName.Text = hipName;
            }
            catch { }
        }

        private void GenerateQR()
        {
            try
            {
                string hipId = ConfigurationManager.AppSettings["AbdmSettings:HipId"] ?? "IN0610090658";
                string hipName = ConfigurationManager.AppSettings["AbdmSettings:HipName"] ?? "MIDHA HOSPITAL";
                // ABDM Sandbox Scan & Share URL Format
                // Note: hip-id and counter-id use hyphens as per latest sandbox documentation
                string qrData = $"https://phrsbx.abdm.gov.in/share-profile?hip-id={hipId}&counter-id=1";
                
                // Using a public QR generator API for demonstration
                string qrApiUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={Uri.EscapeDataString(qrData)}";
                
                picQR.ImageLocation = qrApiUrl;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load QR code: " + ex.Message);
            }
        }
    }
}
