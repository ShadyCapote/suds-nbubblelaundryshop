using Activity_FinalTerm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace LaundryCustomerAndOrderTracking
{
    public partial class NewOrder : Form
    {
        public NewOrder()
        {
            InitializeComponent();
            this.Load += NewOrder_Load;
            btnPay.Click += btnPay_Click;
            txtWeight.TextChanged += (s, e) => ComputeTotal();
            cmbPricePerKilo.SelectedIndexChanged += (s, e) => ComputeTotal();
        }

        private void NewOrder_Load(object sender, EventArgs e)
        {
            // Sample values. Replace with data loaded from your database if you have it.
            cmbPricePerKilo.Items.AddRange(new object[] { "30", "40", "50", "60" });
            cmbService.Items.AddRange(new object[] { "Wash", "Dry", "Wash & Dry", "Wash, Dry & Fold" });

            ClearForm();
            ShowNextCustomerId();
        }

        // Shows the next customer number in the "Customer ID" label
        private void ShowNextCustomerId()
        {
            try
            {
                DataTable dt = ConnectDB.getData(
                    "SELECT MAX(personal_id) AS max_id FROM personal_info",
                    new Dictionary<string, object>());

                object max = dt.Rows[0]["max_id"];
                int next = (max == DBNull.Value) ? 1 : Convert.ToInt32(max) + 1;
                lblCustomerID.Text = next.ToString();
            }
            catch (Exception ex)
            {
                lblCustomerID.Text = "";
                MessageBox.Show(
                    "Could not load the customer ID!\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Total = weight (kg) x price per kilo
        private void ComputeTotal()
        {
            if (decimal.TryParse(txtWeight.Text, out decimal weight) &&
                decimal.TryParse(cmbPricePerKilo.Text, out decimal price))
            {
                lblTotal.Text = (weight * price).ToString("N2");
            }
            else
            {
                lblTotal.Text = "0.00";
            }
        }

        private void ClearForm()
        {
            txtLName.Clear();
            txtFName.Clear();
            txtMName.Clear();
            txtContact.Clear();
            txtWeight.Clear();
            cmbPricePerKilo.SelectedIndex = -1;
            cmbService.SelectedIndex = -1;
            lblTotal.Text = "0.00";
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLName.Text) ||
                string.IsNullOrWhiteSpace(txtFName.Text) ||
                string.IsNullOrWhiteSpace(txtWeight.Text) ||
                cmbPricePerKilo.SelectedIndex < 0 ||
                cmbService.SelectedIndex < 0)
            {
                MessageBox.Show("Please fill in all required fields.", "Missing Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // TODO: insert the customer and order into your database here,
            // using the same ConnectDB class you already have.

            MessageBox.Show("Order saved. Total: " + lblTotal.Text, "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
            ShowNextCustomerId();
        }
    }
}