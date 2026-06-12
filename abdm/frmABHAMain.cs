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
            _client = new AbdmApiClient();
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
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}
