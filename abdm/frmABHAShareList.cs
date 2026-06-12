using System;
using System.Data;
using System.Windows.Forms;
using ABDM.Models;

namespace HMS.abdm
{
    public partial class frmABHAShareList : Form
    {
        public AbhaProfile SelectedProfile { get; private set; }

        public frmABHAShareList()
        {
            InitializeComponent();
        }

        private void frmABHAShareList_Load(object sender, EventArgs e)
        {
            LoadRequests();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRequests();
        }

        private void LoadRequests()
        {
            try
            {
                DataTable dt = MyClass.GetDataTable("Select * from tblAbdmScanShare Where Status='Pending' Order By ShareTime Desc", CommandType.Text, null);
                dgvRequests.DataSource = dt;
                
                if (dgvRequests.Columns.Count > 0)
                {
                    dgvRequests.Columns["Id"].Visible = false;
                    dgvRequests.Columns["Status"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading requests: " + ex.Message);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dgvRequests.CurrentRow != null)
            {
                var row = dgvRequests.CurrentRow;
                SelectedProfile = new AbhaProfile
                {
                    HealthIdNumber = row.Cells["AbhaNumber"].Value.ToString(),
                    AbhaAddress = row.Cells["AbhaAddress"].Value.ToString(),
                    Name = row.Cells["Name"].Value.ToString(),
                    Gender = row.Cells["Gender"].Value.ToString() == "Male" ? "M" : "F",
                    YearOfBirth = row.Cells["DOB"].Value.ToString(),
                    Mobile = row.Cells["Mobile"].Value.ToString(),
                    Address = row.Cells["Address"].Value.ToString(),
                    City = dgvRequests.Columns.Contains("City") ? (row.Cells["City"].Value?.ToString() ?? "") : "",
                    State = dgvRequests.Columns.Contains("State") ? (row.Cells["State"].Value?.ToString() ?? "") : "",
                    FatherName = dgvRequests.Columns.Contains("FatherName") ? (row.Cells["FatherName"].Value?.ToString() ?? "") : ""
                };

                // Save to session so frmOPD can pick it up
                var session = SessionStore.Create(null, "SCAN_SHARE_TOKEN", SelectedProfile);
                SessionStore.Save(session);
                
                // Mark as processing (optional: could delete or mark as Registered later)
                // MyClass.Execute("Update tblAbdmScanShare Set Status='Processing' Where Id=" + row.Cells["Id"].Value, CommandType.Text, null);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a request from the list.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
