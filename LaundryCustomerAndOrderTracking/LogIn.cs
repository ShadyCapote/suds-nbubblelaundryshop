using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace LaundryCustomerAndOrderTracking
{
    public partial class LogIn : Form
    {
        // The table that holds the login accounts. It is created automatically.
        private const string UsersTable = "app_users";

        private const int MaxAttempts = 3;
        private int _failedAttempts = 0;

        private TextBox _txtUsername, _txtPassword;
        private Button _btnLogin;

        public LogIn()
        {
            InitializeComponent();
            SetupForm();
        }

        // ------------------------------------------------------------------
        // Finds the 2 text boxes (top = username, bottom = password) and the Login button
        // ------------------------------------------------------------------
        private void SetupForm()
        {
            List<TextBox> boxes = FindAll<TextBox>(this).OrderBy(t => AbsTop(t)).ToList();
            Button login = FindAll<Button>(this).FirstOrDefault(
                b => b.Text.Trim().Equals("Login", StringComparison.OrdinalIgnoreCase));

            if (boxes.Count != 2 || login == null)
            {
                MessageBox.Show(
                    "The Login form needs 2 text boxes (Username, Password) and a button with the text \"Login\".",
                    "Form Layout",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _txtUsername = boxes[0];
            _txtPassword = boxes[1];
            _btnLogin = login;

            // Single-line boxes, so the password shows as dots and Enter does not add a new line
            _txtUsername.Multiline = false;
            _txtPassword.Multiline = false;
            _txtPassword.UseSystemPasswordChar = true;

            // "-=" first, so the button runs only once even if the designer also connected it
            _btnLogin.Click -= btnLogin_Click;
            _btnLogin.Click += btnLogin_Click;
            AcceptButton = _btnLogin;   // pressing Enter clicks Login

            Load += (s, ev) =>
            {
                PrepareUsersTable();
                _txtUsername.Focus();
            };
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
        // Creates the accounts table, and the first two accounts if it is empty
        // ------------------------------------------------------------------
        private void PrepareUsersTable()
        {
            try
            {
                ConnectDB.executeQuery(
                    "CREATE TABLE IF NOT EXISTS " + UsersTable + " (" +
                    "user_id INT AUTO_INCREMENT PRIMARY KEY, " +
                    "username VARCHAR(50) NOT NULL UNIQUE, " +
                    "password_hash VARCHAR(200) NOT NULL, " +
                    "role VARCHAR(20) NOT NULL DEFAULT 'Cashier')");

                DataTable count = ConnectDB.getData("SELECT COUNT(*) AS n FROM " + UsersTable);
                if (Convert.ToInt32(count.Rows[0]["n"]) == 0)
                {
                    AddUser("admin", "admin123", "Admin");
                    AddUser("cashier", "cashier123", "Cashier");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not prepare the accounts table!\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AddUser(string username, string password, string role)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@u", username },
                { "@p", PasswordHasher.Hash(password) },
                { "@r", role }
            };
            ConnectDB.executeQuery(
                "INSERT INTO " + UsersTable + " (username, password_hash, role) VALUES (@u, @p, @r)",
                parameters);
        }

        // ------------------------------------------------------------------
        // The Login button
        // ------------------------------------------------------------------
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = _txtUsername.Text.Trim();
            string password = _txtPassword.Text;

            if (username == "" || password == "")
            {
                Warn("Please enter your username and password.");
                return;
            }

            try
            {
                var parameters = new Dictionary<string, object> { { "@u", username } };
                DataTable dt = ConnectDB.getData(
                    "SELECT username, password_hash, role FROM " + UsersTable + " WHERE username = @u",
                    parameters);

                bool ok = dt.Rows.Count == 1 &&
                          PasswordHasher.Verify(password, Convert.ToString(dt.Rows[0]["password_hash"]));

                if (!ok)
                {
                    _failedAttempts++;
                    int left = MaxAttempts - _failedAttempts;

                    if (left <= 0)
                    {
                        MessageBox.Show(
                            "Too many failed attempts. The system is now closing.",
                            "Login Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }

                    // The same message for a wrong username or a wrong password
                    Warn("Incorrect username or password. Attempts left: " + left);
                    _txtPassword.Clear();
                    _txtPassword.Focus();
                    return;
                }

                Session.Username = Convert.ToString(dt.Rows[0]["username"]);
                Session.Role = Convert.ToString(dt.Rows[0]["role"]);

                _txtPassword.Clear();
                OpenDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not log in!\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Finds the Dashboard form by its class name, shows it and hides this one
        private void OpenDashboard()
        {
            Type target = null;
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (string keyword in new[] { "dashboard", "form1" })
            {
                target = types.FirstOrDefault(t =>
                    typeof(Form).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    t != GetType() &&
                    t.Name.ToLower().Contains(keyword));

                if (target != null)
                {
                    break;
                }
            }

            if (target == null)
            {
                MessageBox.Show(
                    "Login successful, but the Dashboard form was not found.",
                    "Form Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Form next = (Form)Activator.CreateInstance(target);
            next.FormClosed += (s, ev) => Application.Exit();
            next.Show();
            Hide();
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // ----------------------------------------------------------------------
    // Remembers who is logged in. Other forms can read Session.Username and Session.Role
    // ----------------------------------------------------------------------
    public static class Session
    {
        public static string Username = "";
        public static string Role = "";
    }

    // ----------------------------------------------------------------------
    // Password hashing: PBKDF2 with a random salt. The real password is never stored.
    // ----------------------------------------------------------------------
    public static class PasswordHasher
    {
        private const int Iterations = 100000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        // Returns "iterations:salt:hash", ready to save in the password_hash column
        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            return Iterations + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        // Checks a typed password against a saved hash
        public static bool Verify(string password, string stored)
        {
            try
            {
                string[] parts = stored.Split(':');
                if (parts.Length != 3)
                {
                    return false;
                }

                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] expected = Convert.FromBase64String(parts[2]);

                byte[] actual;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    actual = pbkdf2.GetBytes(expected.Length);
                }

                // Compare every byte, so the time taken does not reveal where they differ
                int diff = 0;
                for (int i = 0; i < expected.Length; i++)
                {
                    diff |= expected[i] ^ actual[i];
                }
                return diff == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}