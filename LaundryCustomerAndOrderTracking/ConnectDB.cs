using LaundryCustomerAndOrderTracking;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Activity_FinalTerm
{
    public class ConnectDB
    {

        public static MySqlConnection GetConnection()
        {
            string connectionString =
                "Server=localhost;" +
                 "Port=3306;" +
                 "Database=personalDB;" +
                 "Uid=root;" +
                 "Pwd=;" +
                 "Allow Zero Datetime=True;";

            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "MYSQL Connection Error!\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                conn.Dispose();
                throw;
            }

            return conn;
        }


        public static DataTable getData(
     string query,
     Dictionary<string, object> parameters = null)
        {
            DataTable table = new DataTable();

            using (var con = GetConnection())
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = query;

                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = p.Key;
                        param.Value = p.Value ?? DBNull.Value;

                        cmd.Parameters.Add(param);
                    }
                }

                using (var reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }


        public static void getMaxID()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    MySqlCommand cmd = new MySqlCommand("Select Max(Personal_ID) as maxID from personal_info", conn);
                    //Execute the command ans get the result
                    object result = cmd.ExecuteScalar();

                    //Check if the result is not null and display the maximum personal_ID 
                    Instantiate.recordChecker.MaxID = result != DBNull.Value ? Convert.ToInt32(result) + 1 : 2024001;
                    conn.Close();
                }
            }
            catch (MySqlException ex)
            {
                //handle exception related to MySQL
                MessageBox.Show("Database Error:" + ex.Message);

            }
            catch (Exception ex)
            {
                //Handle any other exceptions
                MessageBox.Show("An error occurred:" + ex.Message);
            }
        }
        public static void saveUpdateDeleteData(string query, Dictionary<string, object> parameters)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    // Add parameters to the command
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    // Execute the command
                    int rowsAffected = cmd.ExecuteNonQuery();
                    // Optionally, you can check the number of rows affected
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Operation successful!");
                    }
                    else
                    {
                        MessageBox.Show("No rows affected.");
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle exception related to MySQL
                MessageBox.Show("Database Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


    }
}

