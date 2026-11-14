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
        // EDIT THESE TWO QUERIES IF YOUR TABLES OR COLUMNS ARE NAMED DIFFERENTLY
        // ------------------------------------------------------------------
        private const string InsertCustomerSql =
            "INSERT INTO personal_info (personal_id, firstname, middlename, lastname, contact) " +
            "VALUES (@id, @first, @middle, @last, @contact)";

        private const string InsertOrderSql =
            "INSERT INTO orders (personal_id, weight, price_per_kilo, service, total, order_date) " +
            "VALUES (@id, @weight, @price, @service, @total, @date)";

        // The controls on the form (filled in by FindControls)
        private TextBox _txtLastName, _txtFirstName, _txtMiddleName, _txtContact, _txtWeight;
        private ComboBox _cmbPricePerKilo, _cmbService;
        private Label _lblCustomerId, _lblTotal;
        private Button _btnPay;

        public NewOrder()
        {
            InitializeComponent();

            if (FindControls())
            {
                SetupForm();
            }
        }

        // ------------------------------------------------------------------
        // Finds the controls by position, and says what is missing if not found
        // ------------------------------------------------------------------
        private bool FindControls()
        {
            var textBoxes = new List<TextBox>();
            var combos = new List<ComboBox>();
            var valueLabels = new List<Label>();
            var buttons = new List<Button>();
            Collect(this, textBoxes, combos, valueLabels, buttons);

            textBoxes = textBoxes.OrderBy(t => AbsTop(t)).ToList();
            combos = combos.OrderBy(c => AbsTop(c)).ToList();
            valueLabels = valueLabels.OrderBy(l => AbsTop(l)).ToList();
            Button pay = buttons.FirstOrDefault(
                b => b.Text.Trim().Equals("Pay", StringComparison.OrdinalIgnoreCase));

            var problems = new List<string>();
            if (textBoxes.Count != 4 && textBoxes.Count != 5)
            {
                problems.Add("4 text boxes (Last Name, First Name, Middle Name, Contact). Found " + textBoxes.Count + ".");
            }
            if (combos.Count != 2)
            {
                problems.Add("2 combo boxes (Price Per Kilo, Service). Found " + combos.Count + ".");
            }
            if (valueLabels.Count != 2)
            {
                problems.Add("2 labels that still say \"label\" (Customer ID and Total). Found " + valueLabels.Count + ".");
            }
            if (pay == null)
            {
                problems.Add("a button with the text \"Pay\".");
            }

            if (problems.Count > 0)
            {
                MessageBox.Show(
                    "The New Order form needs:\n\n- " + string.Join("\n- ", problems),
                    "Form Layout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            _txtLastName = textBoxes[0];
            _txtFirstName = textBoxes[1];
            _txtMiddleName = textBoxes[2];
            _txtContact = textBoxes[3];

            // No Weight box on the form yet? Make one under the Service combo box.
            _txtWeight = (textBoxes.Count == 5) ? textBoxes[4] : CreateWeightBox(combos[1], combos[0]);
            _cmbPricePerKilo = combos[0];
            _cmbService = combos[1];
            _lblCustomerId = valueLabels[0];
            _lblTotal = valueLabels[1];
            _btnPay = pay;
            return true;
        }

        private void Collect(Control parent, List<TextBox> textBoxes, List<ComboBox> combos,
            List<Label> valueLabels, List<Button> buttons)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox)
                {
                    textBoxes.Add((TextBox)c);
                }
                else if (c is ComboBox)
                {
                    combos.Add((ComboBox)c);
                }
                else if (c is Button)
                {
                    buttons.Add((Button)c);
                }
                else if (c is Label && c.Text.StartsWith("label", StringComparison.OrdinalIgnoreCase))
                {
                    valueLabels.Add((Label)c);
                }

                Collect(c, textBoxes, combos, valueLabels, buttons);
            }
        }

        // Distance from the top of the form, even for controls inside a panel
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

        // Creates the "Weight (kg):" label and text box under the Service combo box
        private TextBox CreateWeightBox(ComboBox service, ComboBox price)
        {
            Control parent = service.Parent;

            int step = service.Top - price.Top;   // same row spacing as the rest of the form
            if (step < 30)
            {
                step = 40;
            }

            Label serviceLabel = parent.Controls.OfType<Label>().FirstOrDefault(
                l => l.Text.Trim().StartsWith("Service", StringComparison.OrdinalIgnoreCase));

            var label = new Label();
            label.Text = "Weight (kg):";
            label.AutoSize = true;
            label.Font = (serviceLabel != null) ? serviceLabel.Font : service.Font;
            label.ForeColor = (serviceLabel != null) ? serviceLabel.ForeColor : Color.Black;
            label.Left = (serviceLabel != null) ? serviceLabel.Left : service.Left - 100;
            label.Top = ((serviceLabel != null) ? serviceLabel.Top : service.Top) + step;

            var box = new TextBox();
            box.Font = service.Font;
            box.Left = service.Left;
            box.Top = service.Top + step;
            box.Width = service.Width;

            parent.Controls.Add(label);
            parent.Controls.Add(box);
            return box;
        }

        // ------------------------------------------------------------------
        // Sets up the combo boxes, typing rules and events
        // ------------------------------------------------------------------
        private void SetupForm()
        {
            _cmbPricePerKilo.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbService.DropDownStyle = ComboBoxStyle.DropDownList;

            // Sample values. Replace them with your own prices and services.
            _cmbPricePerKilo.Items.AddRange(new object[] { "25", "30", "35", "40" });
            _cmbService.Items.AddRange(new object[] { "Wash", "Dry", "Wash & Dry", "Wash, Dry & Fold" });

            _txtContact.MaxLength = 11;
            _txtContact.KeyPress += OnlyDigits_KeyPress;
            _txtWeight.KeyPress += Weight_KeyPress;

            // The Total updates as soon as the weight or the price changes
            _txtWeight.TextChanged += (s, ev) => UpdateTotal();
            _cmbPricePerKilo.SelectedIndexChanged += (s, ev) => UpdateTotal();

            _btnPay.Click += btnPay_Click;

            ClearForm();
        }

        // ------------------------------------------------------------------
        // Customer ID = the next free number in personal_info
        // ------------------------------------------------------------------
        private void ShowNextCustomerId()
        {
            try
            {
                DataTable dt = ConnectDB.getData(
                    "SELECT MAX(personal_id) AS max_id FROM personal_info",
                    new Dictionary<string, object>());

                object max = dt.Rows[0]["max_id"];
                int next = (max == DBNull.Value) ? 1 : Convert.ToInt32(max) + 1;
                _lblCustomerId.Text = next.ToString();
            }
            catch (Exception ex)
            {
                _lblCustomerId.Text = "";
                MessageBox.Show(
                    "Could not load the customer ID!\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ------------------------------------------------------------------
        // Total = weight x price per kilo
        // ------------------------------------------------------------------
        private void UpdateTotal()
        {
            decimal weight;
            decimal price;

            if (decimal.TryParse(_txtWeight.Text, out weight) &&
                decimal.TryParse(_cmbPricePerKilo.Text, out price))
            {
                _lblTotal.Text = (weight * price).ToString("C2");
            }
            else
            {
                _lblTotal.Text = 0m.ToString("C2");
            }
        }

        // ------------------------------------------------------------------
        // Typing rules: digits only for Contact, digits and one dot for Weight
        // ------------------------------------------------------------------
        private void OnlyDigits_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Weight_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isDot = e.KeyChar == '.';

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !isDot)
            {
                e.Handled = true;
            }

            if (isDot && _txtWeight.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        // ------------------------------------------------------------------
        // The Pay button: check the input, save the customer and the order
        // ------------------------------------------------------------------
        private void btnPay_Click(object sender, EventArgs e)
        {
            string last = _txtLastName.Text.Trim();
            string first = _txtFirstName.Text.Trim();
            string middle = _txtMiddleName.Text.Trim();
            string contact = _txtContact.Text.Trim();
            string service = _cmbService.Text;

            int customerId;
            decimal weight;
            decimal pricePerKilo;

            if (!int.TryParse(_lblCustomerId.Text, out customerId))
            {
                Warn("The customer ID could not be loaded. Check the database connection.");
                return;
            }
            if (last == "" || first == "")
            {
                Warn("Please enter the customer's last name and first name.");
                return;
            }
            if (contact.Length < 7)
            {
                Warn("Please enter a valid contact number.");
                return;
            }
            if (!decimal.TryParse(_txtWeight.Text, out weight) || weight <= 0)
            {
                Warn("Please enter the weight in kilos. It must be more than 0.");
                return;
            }
            if (!decimal.TryParse(_cmbPricePerKilo.Text, out pricePerKilo))
            {
                Warn("Please choose the price per kilo.");
                return;
            }
            if (service == "")
            {
                Warn("Please choose a service.");
                return;
            }

            decimal total = weight * pricePerKilo;

            try
            {
                var customer = new Dictionary<string, object>
                {
                    { "@id", customerId },
                    { "@first", first },
                    { "@middle", middle },
                    { "@last", last },
                    { "@contact", contact }
                };
                ConnectDB.executeQuery(InsertCustomerSql, customer);

                var order = new Dictionary<string, object>
                {
                    { "@id", customerId },
                    { "@weight", weight },
                    { "@price", pricePerKilo },
                    { "@service", service },
                    { "@total", total },
                    { "@date", DateTime.Now }
                };
                ConnectDB.executeQuery(InsertOrderSql, order);

                MessageBox.Show(
                    "Order successfully saved!\n\n" +
                    "Weight: " + weight + " kg\n" +
                    "Price per kilo: " + pricePerKilo.ToString("C2") + "\n" +
                    "Service: " + service + "\n" +
                    "Total Payment: " + total.ToString("C2"),
                    "Payment Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearForm();
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

        // ------------------------------------------------------------------
        // Sidebar buttons: open the other forms
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

        // Finds a form in this project by part of its class name, shows it and hides this one.
        // Closing the new form closes the whole program.
        private void OpenForm(params string[] keywords)
        {
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

        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------
        private void Warn(string message)
        {
            MessageBox.Show(message, "Missing Information",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ClearForm()
        {
            _txtLastName.Clear();
            _txtFirstName.Clear();
            _txtMiddleName.Clear();
            _txtContact.Clear();
            _txtWeight.Clear();
            _cmbPricePerKilo.SelectedIndex = -1;
            _cmbService.SelectedIndex = -1;
            _lblTotal.Text = 0m.ToString("C2");
            ShowNextCustomerId();
            _txtLastName.Focus();
        }
    }
}