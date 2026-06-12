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
