using System;
using System.Drawing;
using System.Windows.Forms;

namespace HMS.abdm
{
    public partial class ucImmunizationRecord : UserControl
    {
        public ucImmunizationRecord()
        {
            InitializeComponent();
            StyleControls();
        }

        // Expose controls for seamless integration with frmPrescription
        public ListView lvMedicines_Imm => lvMedicines;
        public TextBox txtVaccineName_Imm => txtVaccineName;
        public TextBox txtLotNumber_Imm => txtLotNumber;
        public TextBox txtDoseNumber_Imm => txtDoseNumber;

        private void StyleControls()
        {
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // Styling buttons
            btnAdd.BackColor = Color.FromArgb(41, 128, 185);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = Cursors.Hand;

            btnRemove.BackColor = Color.FromArgb(192, 57, 43);
            btnRemove.ForeColor = Color.White;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Cursor = Cursors.Hand;

            // Styling inputs
            txtVaccineName.BorderStyle = BorderStyle.FixedSingle;
            txtLotNumber.BorderStyle = BorderStyle.FixedSingle;
            txtDoseNumber.BorderStyle = BorderStyle.FixedSingle;

            // Styling ListView
            lvMedicines.GridLines = true;
            lvMedicines.FullRowSelect = true;
            lvMedicines.View = View.Details;
            lvMedicines.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtVaccineName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a vaccine name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string lot = txtLotNumber.Text.Trim();
            if (string.IsNullOrEmpty(lot))
            {
                MessageBox.Show("Please enter a lot number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string dose = txtDoseNumber.Text.Trim();
            if (string.IsNullOrEmpty(dose))
            {
                MessageBox.Show("Please enter a dose number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = new ListViewItem(name);
            item.SubItems.Add(lot);
            item.SubItems.Add(dose);
            lvMedicines.Items.Add(item);

            // Clear vaccine input and reset focus
            txtVaccineName.Clear();
            txtVaccineName.Focus();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvMedicines.SelectedItems.Count > 0)
            {
                lvMedicines.Items.Remove(lvMedicines.SelectedItems[0]);
            }
            else
            {
                MessageBox.Show("Please select a vaccine item to remove.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
