using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database
{
    class MySqlHandler : ISqlHandler
    {
        private MySqlConnection connection = null;
        private string connstring = "Server=192.38.49.9; database=ds303e19; UID=ds303e19; password=Cisptf8CuT4hLj4T; Pooling=true; Min Pool Size=0; Max Pool Size=100; Connection Lifetime=0";

        private MySqlHandler()
        {
            connection = new MySqlConnection(connstring);
        }

        public MySqlConnection Connection
        {
            get { return connection; }
        }

        public bool IsConnect()
        {
            connection.Open();

            try
            {
                if (Connection == null)
                {
                    
                    
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public void Close()
        {
            connection.Close();
   
        }
    }
}
