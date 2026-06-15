using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMS.abdm
{
    public partial class frmABHAConsent : BaseForm
    {
        public bool IsAgreed { get; private set; } = false;

        public bool Chk1Checked => chk1.Checked;
        public bool Chk2Checked => chk2.Checked;
        public bool Chk3Checked => chk3.Checked;
        public bool Chk4Checked => chk4.Checked;
        public bool Chk5Checked => chk5.Checked;
        public bool Chk6Checked => chk6.Checked;
        public string BeneficiaryText => txtBeneficiary.Text;
        public bool Chk7Checked => chk7.Checked;
        public string ExplainText => txtExplain.Text;

        public frmABHAConsent()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgree_Click(object sender, EventArgs e)
        {
            IsAgreed = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
