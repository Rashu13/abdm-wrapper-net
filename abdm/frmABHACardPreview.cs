using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABHACardPreview : Form
    {
        private readonly string _healthIdNumber;
        private readonly byte[] _cardData;

        public frmABHACardPreview(string healthIdNumber, string base64Content)
        {
            InitializeComponent();
            _healthIdNumber = healthIdNumber;
            _cardData = Convert.FromBase64String(base64Content);
        }

        private void frmABHACardPreview_Load(object sender, EventArgs e)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(_cardData))
                {
                    picCard.Image = Image.FromStream(ms);
                }
                lblHealthId.Text = "ABHA Number: " + _healthIdNumber;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying card: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select location to save ABHA Card";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fileName = $"ABHA_Card_{_healthIdNumber.Replace("-", "")}.png";
                        string filePath = Path.Combine(fbd.SelectedPath, fileName);
                        File.WriteAllBytes(filePath, _cardData);
                        
                        MessageBox.Show($"ABHA Card saved to:\n{filePath}", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
