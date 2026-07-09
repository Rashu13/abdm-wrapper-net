using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HMS.abdm
{
    public partial class ucInvoiceRecord : UserControl
    {
        public ucInvoiceRecord()
        {
            InitializeComponent();
            StyleControls();
        }

        // Expose internal controls as read-only properties for seamless integration with frmPrescription
        public ListView lvItems_Inv => lvItems;
        public TextBox txtItemName_Inv => txtItemName;
        public TextBox txtAmount_Inv => txtAmount;
        public TextBox txtQuantity_Inv => txtQuantity;
        public TextBox txtUnit_Inv => txtUnit;

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
            txtItemName.BorderStyle = BorderStyle.FixedSingle;
            txtAmount.BorderStyle = BorderStyle.FixedSingle;
            txtQuantity.BorderStyle = BorderStyle.FixedSingle;
            txtUnit.BorderStyle = BorderStyle.FixedSingle;

            // Styling ListView
            lvItems.GridLines = true;
            lvItems.FullRowSelect = true;
            lvItems.View = View.Details;
            lvItems.BorderStyle = BorderStyle.FixedSingle;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtItemName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter an item name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string amount = txtAmount.Text.Trim();
            if (string.IsNullOrEmpty(amount))
            {
                MessageBox.Show("Please enter an amount.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string qty = txtQuantity.Text.Trim();
            if (string.IsNullOrEmpty(qty) || !decimal.TryParse(qty, out _))
            {
                MessageBox.Show("Please enter a valid numeric quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string unit = txtUnit.Text.Trim();
            if (string.IsNullOrEmpty(unit))
            {
                unit = "unit";
            }

            var item = new ListViewItem(name);
            item.SubItems.Add(amount);
            item.SubItems.Add(qty);
            item.SubItems.Add(unit);
            lvItems.Items.Add(item);

            // Clear inputs and reset focus
            txtItemName.Text = "Consultation & Clinical Services";
            txtAmount.Text = "500";
            txtQuantity.Text = "1";
            txtUnit.Text = "unit";
            txtItemName.Focus();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvItems.SelectedItems.Count > 0)
            {
                lvItems.Items.Remove(lvItems.SelectedItems[0]);
            }
            else
            {
                MessageBox.Show("Please select an item to remove.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
