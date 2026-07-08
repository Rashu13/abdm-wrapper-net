using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMS.abdm
{
    public partial class frmBridgeConfig : Form
    {
        private readonly HttpClient _httpClient;

        private string WrapperBaseUrl => txtBridgeUrl.Text.Trim().TrimEnd('/');

        public frmBridgeConfig()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            this.Load += new System.EventHandler(this.frmBridgeConfig_Load);
        }

        private void frmBridgeConfig_Load(object sender, EventArgs e)
        {
            try
            {
                txtBridgeUrl.Text = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:BaseUrl"] ?? "http://sbx.wati.digital";
                txtHipId.Text = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:HipId"] ?? "IN0610090658";
                txtClientId.Text = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientId"] ?? "";
                txtClientSecret.Text = System.Configuration.ConfigurationManager.AppSettings["AbdmSettings:ClientSecret"] ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHipId.Text) || !txtHipId.Text.StartsWith("IN"))
                {
                    MessageBox.Show("Please enter a valid Facility (HIP) ID starting with 'IN'.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBridgeUrl.Text))
                {
                    MessageBox.Show("Please enter a valid Bridge URL.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update App.config programmatically
                var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                config.AppSettings.Settings["AbdmSettings:BaseUrl"].Value = txtBridgeUrl.Text.Trim();
                config.AppSettings.Settings["AbdmSettings:HipId"].Value = txtHipId.Text.Trim();
                config.AppSettings.Settings["AbdmSettings:ClientId"].Value = txtClientId.Text.Trim();
                config.AppSettings.Settings["AbdmSettings:ClientSecret"].Value = txtClientSecret.Text.Trim();
                
                config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");

                MessageBox.Show("ABDM Settings saved and updated to App.config successfully!", "Configuration Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = "Status: Local Configuration Saved";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRegisterBridge_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBridgeUrl.Text))
            {
                MessageBox.Show("Please enter a Bridge URL.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnRegisterBridge.Enabled = false;
                lblStatus.Text = "Status: Registering...";

                var payload = new { url = txtBridgeUrl.Text.Trim() };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{WrapperBaseUrl}/v3/config/register-bridge", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Bridge URL Registered Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblStatus.Text = "Status: Registered Successfully";
                    txtLog.Text = responseString;
                }
                else
                {
                    MessageBox.Show("Failed to register bridge URL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Status: Registration Failed";
                    txtLog.Text = responseString;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Status: Error";
            }
            finally
            {
                btnRegisterBridge.Enabled = true;
            }
        }

        private async void btnCheckServices_Click(object sender, EventArgs e)
        {
            try
            {
                btnCheckServices.Enabled = false;
                lblStatus.Text = "Status: Checking Services...";

                var response = await _httpClient.GetAsync($"{WrapperBaseUrl}/v3/config/bridge-services");
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    lblStatus.Text = "Status: Services Checked Successfully";
                }
                else
                {
                    lblStatus.Text = "Status: Failed to check services";
                }

                // Format JSON for display
                try
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseString);
                    txtLog.Text = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
                }
                catch
                {
                    txtLog.Text = responseString;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Status: Error";
            }
            finally
            {
                btnCheckServices.Enabled = true;
            }
        }
    }
}
