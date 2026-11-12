using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Suds_NBubbleLaundryShop
{
    public partial class LogInForm : Form
    {
        public LogInForm()
        {
            InitializeComponent();
        }

        int attemptsLeft = 3;
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = txtAdmin.Text;
            string password = txtPassword.Text;

            if (userName == "Admin" && password == "admin123")
            {
                MessageBox.Show("Login Successful!");

                //Admin form = new Admin();
                //form.Show();
                //this.Hide();
            }
            else
            {
                attemptsLeft--;

                MessageBox.Show("Invalid credentials!" + "\nAttempts Left: " + attemptsLeft);
                MessageBox.Show("Access Denied!");

                txtAdmin.Clear();
                txtPassword.Clear();

                if (attemptsLeft == 0)
                {
                    MessageBox.Show("No more attempts. System Locked!");

                    btnLogin.Enabled = false;
                    txtAdmin.Enabled = false;
                    txtPassword.Enabled = false;
                }
            }
        }
    }
}
