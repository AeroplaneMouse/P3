using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Media;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Drawing;

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
                    string connstring = "Server=192.38.49.9; database=ds303e19; UID=ds303e19; password=Cisptf8CuT4hLj4T; Pooling=true; Min Pool Size=0; Max Pool Size=100; Connection Lifetime=0; Charset=utf8;";
                    connection = new MySqlConnection(connstring);
                }

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return true;
            }
            catch (MySqlException)
            {
                //Console.WriteLine(e);
                connection = null;
                SqlConnectionFailed?.Invoke(new Notification("Error! SQL connection failed to connect", Notification.ERROR));
                return false;
            }
        }

        public void Close()
        {
            if(_instance != null){
                connection.Close();
                _instance = null;
            }
        }
    }
}
