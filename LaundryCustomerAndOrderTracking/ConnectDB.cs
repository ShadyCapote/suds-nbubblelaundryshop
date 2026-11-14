using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace LaundryCustomerAndOrderTracking
{
    /// <summary>
    /// Talks to the MySQL database "laundryDB".
    /// </summary>
    public static class ConnectDB
    {
        // Same settings as your old ConnectDB: database laundryDB, user root, empty password.
        // If you set a MySQL password, type it after Pwd=
        private const string ConnectionString =
            "Server=localhost;" +
            "Port=3306;" +
            "Database=laundryDB;" +
            "Uid=root;" +
            "Pwd=;" +
            "Allow Zero Datetime=True;";

        /// <summary>
        /// Your original method: returns an OPEN connection, or shows an error and throws.
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(ConnectionString);

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
                    MessageBoxIcon.Error);

                conn.Dispose();
                throw;
            }

            return conn;
        }

        /// <summary>
        /// Runs a SELECT query and returns the rows.
        /// </summary>
        /// <param name="query">SQL with @name placeholders</param>
        /// <param name="parameters">values for the placeholders, or null if none</param>
        public static DataTable getData(string query, Dictionary<string, object> parameters = null)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                AddParameters(cmd, parameters);

                var table = new DataTable();
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(table);   // opens and closes the connection by itself
                }
                return table;
            }
        }

        /// <summary>
        /// Runs an INSERT, UPDATE or DELETE and returns how many rows changed.
        /// </summary>
        public static int executeQuery(string query, Dictionary<string, object> parameters = null)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                AddParameters(cmd, parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private static void AddParameters(MySqlCommand cmd, Dictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var p in parameters)
            {
                cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            }
        }
    }
}