using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ABDM.Api;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABHAAddress : Form
    {
        private readonly AbdmApiClient _client;
        private readonly string _txnId;
        private readonly string _userToken;
        private List<string> _suggestions = new List<string>();

        public string CreatedAbhaAddress { get; private set; }

        public frmABHAAddress(AbdmApiClient client, string txnId, string userToken)
        {
            InitializeComponent();
            _client = client;
            _txnId = txnId;
            _userToken = userToken;
        }

        private async void frmABHAAddress_Load(object sender, EventArgs e)
        {
            try
            {
                var resp = await _client.GetAbhaSuggestionsAsync(_txnId);
                if (resp.Success && resp.Data != null)
                {
                    _suggestions = resp.Data.AbhaAddressList ?? new List<string>();
                    PopulateSuggestions();
                }
            }
            catch (Exception ex)
            {
                // Silent fail for suggestions
            }
        }

        private void PopulateSuggestions()
        {
            pnlSuggestions.Controls.Clear();
            // Show maximum 3 suggestions as requested
            foreach (var suggestion in _suggestions.Take(3))
            {
                LinkLabel lnk = new LinkLabel();
                lnk.Text = suggestion + "@sbx";
                lnk.AutoSize = true;
                lnk.Margin = new Padding(0, 0, 10, 5);
                lnk.LinkClicked += (s, e) => {
                    txtAddress.Text = suggestion;
                };
                pnlSuggestions.Controls.Add(lnk);
            }
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            string address = txtAddress.Text.Trim();
            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Please enter or select an ABHA Address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnSubmit.Enabled = false;
                btnSubmit.Text = "Creating...";

                var resp = await _client.CreateAbhaAddressAsync(_txnId, address, _userToken);
                if (resp.Success && resp.Data != null)
                {
                    CreatedAbhaAddress = resp.Data.AbhaAddress;
                    
                    // 1. Show Success Alert
                    string msg = $"Your ABHA Number is {resp.Data.HealthIdNumber}\n" +
                                 $"Your ABHA Address is {resp.Data.AbhaAddress}\n" +
                                 $"and status of your ABHA is {resp.Data.Status}";
                    
                    MessageBox.Show(msg, "ABHA Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 2. Show Card Preview
                    btnSubmit.Text = "Fetching Card...";
                    var cardResp = await _client.GetAbhaCardAsync(_userToken);
                    if (cardResp.Success && cardResp.Data != null)
                    {
                        using (var preview = new frmABHACardPreview(resp.Data.HealthIdNumber, cardResp.Data.Content))
                        {
                            preview.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("ABHA Address created but failed to fetch card preview: " + cardResp.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(resp.Message ?? "Failed to create ABHA Address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSubmit.Enabled = true;
                btnSubmit.Text = "Submit";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
