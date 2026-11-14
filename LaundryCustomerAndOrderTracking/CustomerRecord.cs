using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LaundryCustomerAndOrderTracking
{
    public partial class CustomerRecord : Form
    {
        // One grid row per order. Customers with no orders yet still show up.
        private const string SelectSql =
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
            "LEFT JOIN orders o ON o.personal_id = p.personal_id";

        private const string OrderBySql = " ORDER BY p.personal_id DESC, o.order_id DESC";

        private TextBox _txtSearch;
        private System.Windows.Forms.Timer _searchTimer;
        private string _baseTitle = "";

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
            _baseTitle = Text;

            // The search box is the text box on the same row as the Search button
            _txtSearch = FindAll<TextBox>(this)
                .OrderBy(t => Math.Abs((t.Top + t.Height / 2) - (btnSearch.Top + btnSearch.Height / 2)))
                .FirstOrDefault();
            if (_txtSearch != null)
            {
                _txtSearch.KeyDown -= SearchBox_KeyDown;
                _txtSearch.KeyDown += SearchBox_KeyDown;
                _txtSearch.TextChanged -= SearchBox_TextChanged;
                _txtSearch.TextChanged += SearchBox_TextChanged;
            }

            // Waits a moment after typing stops, then searches
            _searchTimer = new System.Windows.Forms.Timer();
            _searchTimer.Interval = 300;
            _searchTimer.Tick += SearchTimer_Tick;

            foreach (Button b in FindAll<Button>(this))
            {
                switch (b.Text.Trim().ToLower())
                {
                    case "search":
                        Wire(b, btnSearch_Click);
                        break;
                    case "add":
                    case "add user":
                        Wire(b, btnAdd_Click);
                        break;
                    case "dashboard":
                        Wire(b, btnDashboard_Click);
                        break;
                    case "customer":
                        Wire(b, btnCustomer_Click);
                        break;
                    case "service":
                        Wire(b, btnService_Click);
                        break;
                    case "order tracking":
                        Wire(b, btnOrderTracking_Click);
                        break;
                    case "payments":
                        Wire(b, btnPayments_Click);
                        break;
                    case "reports":
                        Wire(b, btnReports_Click);
                        break;
                }
            }

            Load -= CustomerRecord_Load;
            Load += CustomerRecord_Load;
        }

        // "-=" first, so a button never runs twice even if the designer also connected it
        private static void Wire(Button b, EventHandler handler)
        {
            b.Click -= handler;
            b.Click += handler;
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
        // Search
        // ------------------------------------------------------------------

        private void CustomerRecord_Load(object sender, EventArgs e)
        {
            // Connect the Search button by its name, whatever its text says
            Wire(btnSearch, btnSearch_Click);
            LoadData();

            if (_txtSearch != null)
            {
                _txtSearch.Focus();
            }
        }

        private string CurrentSearch()
        {
            return (_txtSearch == null) ? "" : _txtSearch.Text.Trim();
        }

        // Typing in the box searches by itself after a short pause
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            LoadData(CurrentSearch());
        }

        // The Search button, or pressing Enter in the box, searches right away
        private void btnSearch_Click(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            LoadData(CurrentSearch());
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                _searchTimer.Stop();
                LoadData(CurrentSearch());
            }
        }

        // Every word you type must match something in the row, in any order.
        // "slim capote" and "capote slim" both find the same customer.
        // A word can match a first name, middle name, last name, contact, service or customer ID.
        private void LoadData(string search = "")
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                var conditions = new List<string>();

                string[] words = search.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words.Length; i++)
                {
                    string name = "@w" + i;
                    parameters[name] = "%" + EscapeLike(words[i]) + "%";

                    conditions.Add(
                        "(p.firstname LIKE " + name +
                        " OR p.middlename LIKE " + name +
                        " OR p.lastname LIKE " + name +
                        " OR p.contact LIKE " + name +
                        " OR o.service LIKE " + name +
                        " OR CAST(p.personal_id AS CHAR) LIKE " + name + ")");
                }

                string sql = SelectSql;
                if (conditions.Count > 0)
                {
                    sql += " WHERE " + string.Join(" AND ", conditions);
                }
                sql += OrderBySql;

                DataTable table = ConnectDB.getData(sql, parameters);

                dgvCustomers.AutoGenerateColumns = true;
                dgvCustomers.DataSource = table;
                FormatGrid();

                // The window title shows how many rows were found
                Text = _baseTitle + " - " + table.Rows.Count + " record(s)";
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

        // A typed % or _ is searched as plain text, not as a wildcard
        private static string EscapeLike(string text)
        {
            return text.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
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
        // Add button: opens New Order
        // ------------------------------------------------------------------

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
            MessageBox.Show("Clicked. Searching for: [" + CurrentSearch() + "]");
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