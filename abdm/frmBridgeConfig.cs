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
        // Replace with your running Wrapper API Base URL
        private readonly string _wrapperBaseUrl = "https://sbx.wati.digital"; 
        private readonly HttpClient _httpClient;

        public frmBridgeConfig()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
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

                var response = await _httpClient.PostAsync($"{_wrapperBaseUrl}/v3/config/register-bridge", content);
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

                var response = await _httpClient.GetAsync($"{_wrapperBaseUrl}/v3/config/bridge-services");
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
