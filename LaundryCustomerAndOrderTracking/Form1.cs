using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaundryCustomerAndOrderTracking
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }


        private void Dashboard_Load(object sender, EventArgs e)
        {
            LoadSummary();
            LoadRecentOrders();
        }


        private void btnCustomer_Click(object sender, EventArgs e)
        {
            CustomerRecord customerRecord = new CustomerRecord();
            customerRecord.Show();
            this.Hide();
        }

        private void btnService_Click(object sender, EventArgs e)
        {
            Service_Pricing service = new Service_Pricing();
            service.Show();
            this.Hide();
        }

        private void btnOrderTracking_Click(object sender, EventArgs e)
        {
            Order_Tracking orderTracking = new Order_Tracking();
            orderTracking.Show();
            this.Hide();
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            Payment_Record payment = new Payment_Record();
            payment.Show();
            this.Hide();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            Reports reports = new Reports();
            reports.Show();
            this.Hide();
        }

        private void btnButton_Click(object sender, EventArgs e)
        {

        }

   
       
        // Runs a query that returns one value (a count or a sum)
        private object Scalar(string sql)
        {
            DataTable dt = ConnectDB.getData(sql, new Dictionary<string, object>());
            return dt.Rows.Count > 0 ? dt.Rows[0][0] : null;
        }

        private void LoadSummary()
        {
            try
            {


                // personal_info comes from your existing code.
                // The orders and payments table and column names below are ASSUMPTIONS.
                // Change them to match your database.
                lblTotalCustomer.Text = Convert.ToString(Scalar(
                    "SELECT COUNT(*) FROM personal_info"));

                object income = Scalar("SELECT IFNULL(SUM(total), 0) FROM orders");
                lblTotalIncome.Text = Convert.ToDecimal(income).ToString("N2");

                lblPendingOrder.Text = Convert.ToString(Scalar(
                   "SELECT COUNT(*) FROM orders WHERE status = 'Pending'"));

                lblCompletedOrders.Text = Convert.ToString(Scalar(
                   "SELECT COUNT(*) FROM orders WHERE status = 'Completed'"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load dashboard totals.\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRecentOrders()
        {
            try
            {
                // Assumed table and columns. Change to match your database.
                dgvRecentOrders.DataSource = ConnectDB.getData(
                    "SELECT * FROM orders ORDER BY 1 DESC LIMIT 20",
                    new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load recent orders.\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Navigation: replace the form class names with your real ones,
        // then uncomment. Check Solution Explorer for the names.
        //
        // private void btnCustomer_Click(object sender, EventArgs e)
        // {
        //     new CustomerRecord().Show();
        //     this.Hide();
        // }
    }
}