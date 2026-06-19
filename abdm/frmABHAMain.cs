using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABHAMain : Form
    {
        private readonly AbdmApiClient _client;
        public AbhaProfile SelectedProfile { get; private set; }

        public frmABHAMain()
        {
            InitializeComponent();
            var settings = new AbdmSettings
            {
                BaseUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:BaseUrl"] ?? "https://dev.abdm.gov.in/api/hiecm/gateway",
                AbhaServiceUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:AbhaServiceUrl"] ?? "https://abhasbx.abdm.gov.in/abha/api/v3",
                ClientId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientId"],
                ClientSecret = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientSecret"],
                HipId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipId"],
                HipName = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipName"],
                CmId = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:CmId"] ?? "sbx",
                Environment = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:Environment"] ?? "Sandbox"
            };
            _client = new AbdmApiClient(settings);
            this.Load += new System.EventHandler(this.frmABHAMain_Load);
        }

        private async void frmABHAMain_Load(object sender, EventArgs e)
        {
            try
            {
                // Pre-fetch token to ensure session is active
                await _client.GetAccessTokenAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ABDM Session Initialization Failed: " + ex.Message, "ABDM Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHA())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (this.Modal)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHALogin())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    this.SelectedProfile = frm.LoggedInProfile;
                    if (this.Modal)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnScanShare_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHAQR())
            {
                frm.ShowDialog();
            }
        }

        private void btnViewRequests_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHAShareList())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    this.SelectedProfile = frm.SelectedProfile;
                    if (this.Modal)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnAccountOps_Click(object sender, EventArgs e)
        {
            var session = ABDM.Models.SessionStore.Load();
            if (session == null || string.IsNullOrEmpty(session.UserToken))
            {
                MessageBox.Show("Please login first to access account operations.", "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                using (var loginFrm = new frmABHALogin())
                {
                    if (loginFrm.ShowDialog() == DialogResult.OK)
                    {
                        session = ABDM.Models.SessionStore.Load();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (session != null && !string.IsNullOrEmpty(session.UserToken))
            {
                using (var opsFrm = new frmABHAAccountOps(session))
                {
                    opsFrm.ShowDialog();
                }
            }
        }

        private void btnBioEnroll_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHABio(_client))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (this.Modal)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnDlEnroll_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHADrivingLicense(_client))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (this.Modal)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnScanUserQR_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHAScanUserQR())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (this.Modal)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnCreateAddress_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHAAddressCreationDirect(_client))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (this.Modal)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnDownloadCard_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABHACardDownloadDirect(_client))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (this.Modal)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private void btnM2Testing_Click(object sender, EventArgs e)
        {
            using (var frm = new frmABDMM2())
            {
                frm.ShowDialog();
            }
        }

        private void btnBridgeConfig_Click(object sender, EventArgs e)
        {
            using (var frm = new frmBridgeConfig())
            {
                frm.ShowDialog();
            }
        }

        private async void btnTestAbdm_Click(object sender, EventArgs e)
        {
            try
            {
                var parchiJson = new
                {
                    bundleType = "PrescriptionRecord",
                    careContextReference = "OPD-1001",
                    authoredOn = DateTime.UtcNow.ToString("o"),
                    patient = new
                    {
                        name = "Test Patient",
                        patientReference = "PT-001",
                        gender = "male",
                        birthDate = "1990-01-01"
                    },
                    practitioners = new[] { new { name = "Dr. Midha", practitionerId = "DOC-01" } },
                    organisation = new { facilityName = "MIDHA HOSPITAL", facilityId = "IN0610090658" },
                    prescriptions = new[]
                    {
                        new { medicine = "Paracetamol 500mg", dosage = "1-1-1" }
                    }
                };

                var recordData = new
                {
                    AbhaAddress = "testpatient@sbx",
                    CareContextReference = "OPD-1001",
                    RecordType = "PrescriptionRecord",
                    FhirJsonPayload = System.Text.Json.JsonSerializer.Serialize(parchiJson)
                };

                using (var client = new System.Net.Http.HttpClient())
                {
                    // Yahan par "localhost:5221" ko hatakar apne Wrapper ka asal domain/URL daal dein! 
                    // Yahan par humne aapka naya online Wrapper URL daal diya hai!
                    string wrapperBaseUrl = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:WrapperUrl"] ?? "https://sbx.wati.digital";
                    string apiUrl = $"{wrapperBaseUrl.TrimEnd('/')}/v3/patient/health-data"; 
                    var content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(recordData), System.Text.Encoding.UTF8, "application/json");
                    
                    var response = await client.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Parchi Wrapper Database mein chali gayi! ABDM Push tayyar hai.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Error saving data: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
