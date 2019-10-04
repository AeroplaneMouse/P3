using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Asset_Management_System.Events;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database
{
    public class DBConnection
    {
        public event NotificationEventHandler SqlConnectionFailed;

        private DBConnection()
        {
        }

        private string databaseName = string.Empty;
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            try
            {
                if (Connection == null)
                {
                    string connstring = "Server=192.38.49.9; database=ds303e19; UID=ds303e19; password=Cisptf8CuT4hLj4T";
                    connection = new MySqlConnection(connstring);
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException)
            {
                connection = null;
                if (SqlConnectionFailed != null)
                    SqlConnectionFailed(this, new NotificationEventArgs("Unable to connect to SQL database.", Brushes.Red));
                return false;
            }
        }

        public void Close()
        {
            connection.Close();
            _instance = null;
        }
    }
}
