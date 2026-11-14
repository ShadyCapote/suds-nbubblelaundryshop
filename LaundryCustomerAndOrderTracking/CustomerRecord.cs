using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LaundryCustomerAndOrderTracking
{
    public partial class CustomerRecord : Form
    {
        // One row per order. Customers with no orders yet still show up.
        // Edit the column names here if your tables are named differently.
        private const string RecordsSql =
            "SELECT p.personal_id AS `Customer ID`, " +
            "       p.lastname    AS `Last Name`, " +
            "       p.firstname   AS `First Name`, " +
            "       p.middlename  AS `Middle Name`, " +
            "       p.contact     AS `Contact`, " +
            "       o.service     AS `Service`, " +
            "       o.weight      AS `Weight (kg)`, " +
            "       o.total       AS `Total`, " +
            "       o.order_date  AS `Order Date` " +
            "FROM personal_info p " +
            "LEFT JOIN orders o ON o.personal_id = p.personal_id " +
            "WHERE p.firstname LIKE @s OR p.middlename LIKE @s OR p.lastname LIKE @s " +
            "   OR p.contact LIKE @s OR o.service LIKE @s " +
            "   OR CAST(p.personal_id AS CHAR) LIKE @s " +
            "ORDER BY p.personal_id DESC, o.order_id DESC";

        private TextBox _txtSearch;

        public CustomerRecord()
        {
            InitializeComponent();
            SetupForm();
        }

        // ------------------------------------------------------------------
        // Finds the search box and connects every button by its text
        // ------------------------------------------------------------------
        private void SetupForm()
        {
            _txtSearch = FindAll<TextBox>(this).FirstOrDefault();
            if (_txtSearch != null)
            {
                _txtSearch.KeyDown += SearchBox_KeyDown;
            }

            foreach (Button b in FindAll<Button>(this))
            {
                switch (b.Text.Trim().ToLower())
                {
                    case "search":
                        b.Click += btnSearch_Click;
                        break;
                    case "add":
                        b.Click += btnAdd_Click;
                        break;
                    case "dashboard":
                        b.Click += btnDashboard_Click;
                        break;
                    case "customer":
                        b.Click += btnCustomer_Click;
                        break;
                    case "service":
                        b.Click += btnService_Click;
                        break;
                    case "order tracking":
                        b.Click += btnOrderTracking_Click;
                        break;
                    case "payments":
                        b.Click += btnPayments_Click;
                        break;
                    case "reports":
                        b.Click += btnReports_Click;
                        break;
                }
            }

            Load += CustomerRecord_Load;
        }

        private static List<T> FindAll<T>(Control parent) where T : Control
        {
            var found = new List<T>();
            foreach (Control c in parent.Controls)
            {
                if (c is T)
                {
                    found.Add((T)c);
                }
                found.AddRange(FindAll<T>(c));
            }
            return found;
        }

        // ------------------------------------------------------------------
        // Loads the records into the grid
        // ------------------------------------------------------------------
        private void CustomerRecord_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private string CurrentSearch()
        {
            return (_txtSearch == null) ? "" : _txtSearch.Text.Trim();
        }

        private void LoadData(string search = "")
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@s", "%" + search + "%" }
                };

                dgvCustomers.DataSource = ConnectDB.getData(RecordsSql, parameters);
                FormatGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load the records!\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            dgvCustomers.ReadOnly = true;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            SetFormat("Total", "C2");
            SetFormat("Order Date", "yyyy-MM-dd HH:mm");
        }

        private void SetFormat(string column, string format)
        {
            if (dgvCustomers.Columns.Contains(column))
            {
                dgvCustomers.Columns[column].DefaultCellStyle.Format = format;
            }
        }

        // ------------------------------------------------------------------
        // Search and Add buttons
        // ------------------------------------------------------------------
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData(CurrentSearch());
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadData(CurrentSearch());
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenForm("neworder");
        }

        // ------------------------------------------------------------------
        // Sidebar buttons
        // ------------------------------------------------------------------
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            OpenForm("dashboard", "form1");
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            LoadData(CurrentSearch());   // you are already on this form, so just refresh
        }

        private void btnService_Click(object sender, EventArgs e)
        {
            OpenForm("service");
        }

        private void btnOrderTracking_Click(object sender, EventArgs e)
        {
            OpenForm("ordertracking", "tracking");
        }

        private void btnPayments_Click(object sender, EventArgs e)
        {
            OpenForm("payment");
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            OpenForm("report");
        }

        // Finds a form in this project by part of its class name, shows it and hides this one.
        // Closing the new form closes the whole program.
        private void OpenForm(params string[] keywords)
        {
            if (!Visible)
            {
                return;   // already navigating, so ignore a double click
            }

            Type target = null;
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (string keyword in keywords)
            {
                target = types.FirstOrDefault(t =>
                    typeof(Form).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    t != GetType() &&
                    t.Name.Replace("_", "").ToLower().Contains(keyword));

                if (target != null)
                {
                    break;
                }
            }

            if (target == null)
            {
                MessageBox.Show(
                    "Could not find a form for \"" + keywords[0] + "\".",
                    "Form Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Form next = (Form)Activator.CreateInstance(target);
                next.FormClosed += (s, ev) => Application.Exit();
                next.Show();
                Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not open the form!\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}