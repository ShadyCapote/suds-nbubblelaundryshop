using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LaundryCustomerAndOrderTracking
{
    public partial class NewOrder : Form
    {
        // ------------------------------------------------------------------
        // The two queries that save a new order
        // ------------------------------------------------------------------
        private const string InsertCustomerSql =
            "INSERT INTO personal_info (personal_id, firstname, middlename, lastname, contact, address) " +
            "VALUES (@id, @first, @middle, @last, @contact, @address)";

        private const string InsertOrderSql =
            "INSERT INTO orders (order_id, personal_id, weight, price_per_kilo, service, total, " +
            "order_date, pickup_date, payment_method, amount_paid, balance) " +
            "VALUES (@oid, @id, @weight, @price, @service, @total, " +
            "@date, @pickup, @method, @paid, @balance)";

        // The controls on the form (found automatically by FindControls)
        private GroupBox _payBox, _sumBox;
        private TextBox _txtName, _txtContact, _txtAddress;
        private ComboBox _cmbService, _cmbPrice;
        private NumericUpDown _nudWeight, _nudPaid;
        private DateTimePicker _dtpOrder, _dtpPickup;
        private List<RadioButton> _radios;
        private Label _lblCustomerId, _lblTotal, _lblBalance, _lblOrderId;
        private Label _sumCustomer, _sumTotal, _sumPaid, _sumBalance;

        private int _nextCustomerId, _nextOrderId;

        public NewOrder()
        {
            InitializeComponent();

            if (FindControls())
            {
                SetupForm();
            }
        }

        // ------------------------------------------------------------------
        // Finds the controls by their type, text and position
        // ------------------------------------------------------------------
        private bool FindControls()
        {
            var problems = new List<string>();

            List<GroupBox> groups = FindAll<GroupBox>(this);
            _payBox = groups.FirstOrDefault(
                g => g.Text.Trim().StartsWith("Payment", StringComparison.OrdinalIgnoreCase));
            _sumBox = groups.FirstOrDefault(
                g => g.Text.Trim().StartsWith("Order Summary", StringComparison.OrdinalIgnoreCase));

            if (_payBox == null)
            {
                problems.Add("a group box with the text \"Payment Method\".");
            }
            if (_sumBox == null)
            {
                problems.Add("a group box with the text \"Order Summary\".");
            }
            if (problems.Count > 0)
            {
                return ShowProblems(problems);
            }

            List<TextBox> boxes = FindAll<TextBox>(this)
                .Where(c => Outside(c)).OrderBy(c => AbsTop(c)).ToList();
            List<ComboBox> combos = FindAll<ComboBox>(this)
                .OrderBy(c => AbsTop(c)).ToList();
            List<DateTimePicker> dates = FindAll<DateTimePicker>(this)
                .OrderBy(c => AbsTop(c)).ToList();
            List<NumericUpDown> weightBoxes = FindAll<NumericUpDown>(this)
                .Where(c => Outside(c)).ToList();
            List<NumericUpDown> paidBoxes = FindAll<NumericUpDown>(_payBox);
            List<RadioButton> radios = FindAll<RadioButton>(_payBox)
                .OrderBy(c => AbsTop(c)).ToList();

            List<Label> topValues = FindAll<Label>(this)
                .Where(l => IsValueLabel(l) && Outside(l)).OrderBy(l => AbsTop(l)).ToList();
            List<Label> payValues = FindAll<Label>(_payBox)
                .Where(l => IsValueLabel(l)).ToList();
            List<Label> sumValues = FindAll<Label>(_sumBox)
                .Where(l => IsValueLabel(l)).OrderBy(l => AbsTop(l)).ToList();
            List<Label> sumCaptions = FindAll<Label>(_sumBox)
                .Where(l => !IsValueLabel(l)).OrderBy(l => AbsTop(l)).ToList();
            Label orderIdCaption = FindAll<Label>(this).FirstOrDefault(
                l => l.Text.Trim().StartsWith("Order ID", StringComparison.OrdinalIgnoreCase));

            List<Button> buttons = FindAll<Button>(this);
            bool hasSave = buttons.Any(
                b => b.Text.Trim().Equals("Save Order", StringComparison.OrdinalIgnoreCase));

            if (boxes.Count != 3)
            {
                problems.Add("3 text boxes (Customer Name, Contact, Address). Found " + boxes.Count + ".");
            }
            if (combos.Count != 2)
            {
                problems.Add("2 combo boxes (Service, Price Per Kilo). Found " + combos.Count + ".");
            }
            if (dates.Count != 2)
            {
                problems.Add("2 date pickers (Order Date, Pickup Date). Found " + dates.Count + ".");
            }
            if (weightBoxes.Count != 1)
            {
                problems.Add("1 number box for Weight. Found " + weightBoxes.Count + ".");
            }
            if (paidBoxes.Count != 1)
            {
                problems.Add("1 number box for Amount Paid inside Payment Method. Found " + paidBoxes.Count + ".");
            }
            if (radios.Count != 3)
            {
                problems.Add("3 radio buttons (Cash, Gcash, Other). Found " + radios.Count + ".");
            }
            if (topValues.Count != 2)
            {
                problems.Add("2 labels that still say \"label\" (Customer ID and Total). Found " + topValues.Count + ".");
            }
            if (payValues.Count != 1)
            {
                problems.Add("1 label that says \"label\" for Balance inside Payment Method. Found " + payValues.Count + ".");
            }
            if (sumValues.Count != 4)
            {
                problems.Add("4 labels that say \"label\" inside Order Summary. Found " + sumValues.Count + ".");
            }
            if (!hasSave)
            {
                problems.Add("a button with the text \"Save Order\".");
            }
            if (problems.Count > 0)
            {
                return ShowProblems(problems);
            }

            _txtName = boxes[0];
            _txtContact = boxes[1];
            _txtAddress = boxes[2];
            _cmbService = combos[0];
            _cmbPrice = combos[1];
            _dtpOrder = dates[0];
            _dtpPickup = dates[1];
            _nudWeight = weightBoxes[0];
            _nudPaid = paidBoxes[0];
            _radios = radios;
            _lblCustomerId = topValues[0];
            _lblTotal = topValues[1];
            _lblBalance = payValues[0];
            _sumCustomer = sumValues[0];
            _sumTotal = sumValues[1];
            _sumPaid = sumValues[2];
            _sumBalance = sumValues[3];
            _lblOrderId = orderIdCaption;

            // The Order Summary captions say "Balance:" three times. Give them proper names.
            if (sumCaptions.Count == 4)
            {
                sumCaptions[0].Text = "Customer:";
                sumCaptions[1].Text = "Total:";
                sumCaptions[2].Text = "Amount Paid:";
                sumCaptions[3].Text = "Balance:";
            }

            return true;
        }

        private bool ShowProblems(List<string> problems)
        {
            MessageBox.Show(
                "The New Order form needs:\n\n- " + string.Join("\n- ", problems),
                "Form Layout",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        // A value label is one that still has the default text "label"
        private static bool IsValueLabel(Label l)
        {
            return l.Text.StartsWith("label", StringComparison.OrdinalIgnoreCase);
        }

        // True for controls that are not inside the Payment Method or Order Summary boxes
        private bool Outside(Control c)
        {
            return !IsInside(c, _payBox) && !IsInside(c, _sumBox);
        }

        private static bool IsInside(Control c, Control container)
        {
            for (Control p = c.Parent; p != null; p = p.Parent)
            {
                if (p == container)
                {
                    return true;
                }
            }
            return false;
        }

        // Lists every control of one type, including the ones inside panels and group boxes
        private static List<T> FindAll<T>(Control parent) where T : Control
        {
            var found = new List<T>();
            foreach (Control c in parent.Controls)
            {
                if (c is T)
                {
                    found.Add((T)c);
                }

                // A number box hides a text box inside it. Do not look in there.
                if (!(c is UpDownBase))
                {
                    found.AddRange(FindAll<T>(c));
                }
            }
            return found;
        }

        // Distance from the top of the form, even for controls inside a box
        private int AbsTop(Control c)
        {
            int top = 0;
            while (c != null && c != this)
            {
                top += c.Top;
                c = c.Parent;
            }
            return top;
        }

        // ------------------------------------------------------------------
        // Sets up the lists, number boxes, typing rules and buttons
        // ------------------------------------------------------------------
        private void SetupForm()
        {
            EnsureTables();

            _cmbService.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbPrice.DropDownStyle = ComboBoxStyle.DropDownList;

            // Sample values. Replace them with your own services and prices.
            _cmbService.Items.AddRange(new object[] { "Wash", "Dry", "Wash & Dry", "Wash, Dry & Fold" });
            _cmbPrice.Items.AddRange(new object[] { "25", "30", "35", "40" });

            _nudWeight.DecimalPlaces = 2;
            _nudWeight.Minimum = 0;
            _nudWeight.Maximum = 1000;
            _nudWeight.Increment = 0.5m;

            _nudPaid.DecimalPlaces = 2;
            _nudPaid.Minimum = 0;
            _nudPaid.Maximum = 1000000;
            _nudPaid.Increment = 10m;

            _txtContact.MaxLength = 11;
            _txtContact.KeyPress += OnlyDigits_KeyPress;

            // The total, balance and summary update as you type
            _txtName.TextChanged += (s, ev) => UpdateTotals();
            _nudWeight.ValueChanged += (s, ev) => UpdateTotals();
            _nudPaid.ValueChanged += (s, ev) => UpdateTotals();
            _cmbPrice.SelectedIndexChanged += (s, ev) => UpdateTotals();

            // Connect every button by its text
            foreach (Button b in FindAll<Button>(this))
            {
                switch (b.Text.Trim().ToLower())
                {
                    case "save order":
                        Wire(b, btnSaveOrder_Click);
                        break;
                    case "clear form":
                        Wire(b, btnClearForm_Click);
                        break;
                    case "cancel":
                        Wire(b, btnCancel_Click);
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
                    case "logout":
                        Wire(b, btnLogout_Click);
                        break;
                }
            }

            ClearForm();
        }

        // "-=" first, so a button never runs twice even if the designer also connected it
        private static void Wire(Button b, EventHandler handler)
        {
            b.Click -= handler;
            b.Click += handler;
        }

        // ------------------------------------------------------------------
        // Makes sure the tables and columns this form needs exist
        // ------------------------------------------------------------------
        private void EnsureTables()
        {
            try
            {
                ConnectDB.executeQuery(
                    "CREATE TABLE IF NOT EXISTS personal_info (" +
                    "personal_id INT AUTO_INCREMENT PRIMARY KEY, " +
                    "firstname VARCHAR(50) NOT NULL, " +
                    "middlename VARCHAR(100) NULL, " +
                    "lastname VARCHAR(50) NOT NULL, " +
                    "birthdate DATE NULL, " +
                    "contact VARCHAR(15) NULL, " +
                    "address VARCHAR(150) NULL)");

                ConnectDB.executeQuery(
                    "CREATE TABLE IF NOT EXISTS orders (" +
                    "order_id INT AUTO_INCREMENT PRIMARY KEY, " +
                    "personal_id INT NOT NULL, " +
                    "weight DECIMAL(6,2) NOT NULL, " +
                    "price_per_kilo DECIMAL(8,2) NOT NULL, " +
                    "service VARCHAR(50) NOT NULL, " +
                    "total DECIMAL(10,2) NOT NULL, " +
                    "order_date DATETIME NOT NULL, " +
                    "pickup_date DATE NULL, " +
                    "payment_method VARCHAR(20) NULL, " +
                    "amount_paid DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                    "balance DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                    "status VARCHAR(20) NOT NULL DEFAULT 'Pending', " +
                    "FOREIGN KEY (personal_id) REFERENCES personal_info(personal_id))");

                // If the tables already existed, add what is missing
                AddColumnIfMissing("personal_info", "contact", "VARCHAR(15) NULL");
                AddColumnIfMissing("personal_info", "address", "VARCHAR(150) NULL");
                AddColumnIfMissing("orders", "pickup_date", "DATE NULL");
                AddColumnIfMissing("orders", "payment_method", "VARCHAR(20) NULL");
                AddColumnIfMissing("orders", "amount_paid", "DECIMAL(10,2) NOT NULL DEFAULT 0");
                AddColumnIfMissing("orders", "balance", "DECIMAL(10,2) NOT NULL DEFAULT 0");

                // This form does not ask for these, so they must be allowed to stay empty
                MakeOptional("personal_info", "birthdate", "DATE NULL");
                MakeOptional("personal_info", "middlename", "VARCHAR(100) NULL");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not prepare the database tables!\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool ColumnExists(string table, string column)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@t", table },
                { "@c", column }
            };
            DataTable dt = ConnectDB.getData(
                "SELECT COUNT(*) AS n FROM information_schema.COLUMNS " +
                "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t AND COLUMN_NAME = @c",
                parameters);
            return Convert.ToInt32(dt.Rows[0]["n"]) > 0;
        }

        private void AddColumnIfMissing(string table, string column, string definition)
        {
            if (!ColumnExists(table, column))
            {
                ConnectDB.executeQuery("ALTER TABLE " + table + " ADD COLUMN " + column + " " + definition);
            }
        }

        private void MakeOptional(string table, string column, string definition)
        {
            if (ColumnExists(table, column))
            {
                ConnectDB.executeQuery("ALTER TABLE " + table + " MODIFY COLUMN " + column + " " + definition);
            }
        }

        // ------------------------------------------------------------------
        // Customer ID and Order ID = the next free numbers
        // ------------------------------------------------------------------
        private int NextId(string table, string column)
        {
            DataTable dt = ConnectDB.getData("SELECT MAX(" + column + ") AS max_id FROM " + table);
            object max = dt.Rows[0]["max_id"];
            return (max == DBNull.Value) ? 1 : Convert.ToInt32(max) + 1;
        }

        private void ShowNextIds()
        {
            try
            {
                _nextCustomerId = NextId("personal_info", "personal_id");
                _nextOrderId = NextId("orders", "order_id");

                _lblCustomerId.Text = _nextCustomerId.ToString();
                if (_lblOrderId != null)
                {
                    _lblOrderId.Text = "Order ID:  " + _nextOrderId;
                }
            }
            catch (Exception ex)
            {
                _nextCustomerId = 0;
                _nextOrderId = 0;
                _lblCustomerId.Text = "";
                MessageBox.Show(
                    "Could not load the customer and order numbers!\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ------------------------------------------------------------------
        // Total, balance and the Order Summary
        // ------------------------------------------------------------------
        private void UpdateTotals()
        {
            decimal price;
            decimal.TryParse(_cmbPrice.Text, out price);

            decimal total = _nudWeight.Value * price;
            decimal paid = _nudPaid.Value;
            decimal balance = Math.Max(total - paid, 0m);

            _lblTotal.Text = total.ToString("C2");
            _lblBalance.Text = balance.ToString("C2");
            _lblBalance.BackColor = (balance > 0) ? Color.LightCoral : Color.LightGreen;

            string name = _txtName.Text.Trim();
            _sumCustomer.Text = (name == "") ? "-" : name;
            _sumTotal.Text = total.ToString("C2");
            _sumPaid.Text = paid.ToString("C2");
            _sumBalance.Text = balance.ToString("C2");
        }

        private void OnlyDigits_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // ------------------------------------------------------------------
        // Save Order
        // ------------------------------------------------------------------
        private void btnSaveOrder_Click(object sender, EventArgs e)
        {
            string name = _txtName.Text.Trim();
            string contact = _txtContact.Text.Trim();
            string address = _txtAddress.Text.Trim();
            string service = _cmbService.Text;
            decimal price;
            RadioButton method = _radios.FirstOrDefault(r => r.Checked);

            if (_nextCustomerId < 1 || _nextOrderId < 1)
            {
                Warn("The customer and order numbers could not be loaded. Check the database connection.");
                return;
            }
            if (name == "")
            {
                Warn("Please enter the customer's name.");
                _txtName.Focus();
                return;
            }
            if (contact.Length < 7)
            {
                Warn("Please enter a valid contact number.");
                _txtContact.Focus();
                return;
            }
            if (service == "")
            {
                Warn("Please choose a service.");
                return;
            }
            if (!decimal.TryParse(_cmbPrice.Text, out price))
            {
                Warn("Please choose the price per kilo.");
                return;
            }
            if (_nudWeight.Value <= 0)
            {
                Warn("Please enter the weight in kilos. It must be more than 0.");
                return;
            }
            if (method == null)
            {
                Warn("Please choose a payment method.");
                return;
            }
            if (_dtpPickup.Value.Date < _dtpOrder.Value.Date)
            {
                Warn("The pickup date cannot be before the order date.");
                return;
            }

            decimal weight = _nudWeight.Value;
            decimal total = weight * price;
            decimal paid = _nudPaid.Value;
            decimal balance = Math.Max(total - paid, 0m);
            decimal change = Math.Max(paid - total, 0m);

            // "Maria Dela Cruz" is saved as first name "Maria Dela" and last name "Cruz"
            string first;
            string middle;
            string last;
            SplitName(name, out first, out middle, out last);



            try
            {
                var customer = new Dictionary<string, object>
                {
                    { "@id", _nextCustomerId },
                    { "@first", first },
                    { "@middle", middle },
                    { "@last", last },
                    { "@contact", contact },
                    { "@address", address }
                };
                ConnectDB.executeQuery(InsertCustomerSql, customer);

                var order = new Dictionary<string, object>
                {
                    { "@oid", _nextOrderId },
                    { "@id", _nextCustomerId },
                    { "@weight", weight },
                    { "@price", price },
                    { "@service", service },
                    { "@total", total },
                    { "@date", _dtpOrder.Value.Date + DateTime.Now.TimeOfDay },
                    { "@pickup", _dtpPickup.Value.Date },
                    { "@method", method.Text.Trim() },
                    { "@paid", paid },
                    { "@balance", balance }
                };
                ConnectDB.executeQuery(InsertOrderSql, order);

                string payLine = (change > 0)
                    ? "Change: " + change.ToString("C2")
                    : "Balance: " + balance.ToString("C2");

                MessageBox.Show(
                    "Order successfully saved!\n\n" +
                    "Order ID: " + _nextOrderId + "\n" +
                    "Customer: " + name + "\n" +
                    "Service: " + service + "\n" +
                    "Weight: " + weight + " kg\n" +
                    "Price per kilo: " + price.ToString("C2") + "\n" +
                    "Total: " + total.ToString("C2") + "\n" +
                    "Payment: " + method.Text.Trim() + "\n" +
                    "Amount paid: " + paid.ToString("C2") + "\n" +
                    payLine + "\n" +
                    "Pickup date: " + _dtpPickup.Value.ToString("MMMM d, yyyy"),
                    "Order Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearForm();
                OpenForm("customer");   // shows the new record. Delete this line to stay on this form.
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to save order!\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void SplitName(string full, out string first, out string middle, out string last)
        {
            string[] parts = full.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length <= 1)
            {
                first = full.Trim();
                middle = "";
                last = "";
            }
            else if (parts.Length == 2)
            {
                first = parts[0];
                middle = "";
                last = parts[1];
            }
            else
            {
                first = parts[0];
                last = parts[parts.Length - 1];
                middle = string.Join(" ", parts.Skip(1).Take(parts.Length - 2));
            }
        }

        // ------------------------------------------------------------------
        // Clear Form and Cancel
        // ------------------------------------------------------------------
        private void btnClearForm_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bool hasInput = _txtName.Text.Trim() != "" || _txtContact.Text.Trim() != "" ||
                            _txtAddress.Text.Trim() != "" || _nudWeight.Value > 0;

            if (hasInput)
            {
                DialogResult answer = MessageBox.Show(
                    "Discard this order?",
                    "Cancel",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (answer != DialogResult.Yes)
                {
                    return;
                }
            }

            ClearForm();
            OpenForm("dashboard", "form1");
        }

        private void ClearForm()
        {
            _txtName.Clear();
            _txtContact.Clear();
            _txtAddress.Clear();
            _cmbService.SelectedIndex = -1;
            _cmbPrice.SelectedIndex = -1;
            _nudWeight.Value = 0;
            _nudPaid.Value = 0;
            _dtpOrder.Value = DateTime.Today;
            _dtpPickup.Value = DateTime.Today.AddDays(2);
            _radios[0].Checked = true;   // Cash

            ShowNextIds();
            UpdateTotals();
            _txtName.Focus();
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
            OpenForm("customer");
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

        private void btnLogout_Click(object sender, EventArgs e)
        {
            OpenForm("login");
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

        private void Warn(string message)
        {
            MessageBox.Show(message, "Missing Information",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}