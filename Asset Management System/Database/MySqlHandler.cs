using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database
{
    public class MySqlHandler
    {
        private MySqlConnection dbcon;

        public MySqlHandler(MySqlConnection connection)
        {
            this.dbcon = connection;
        }

        public bool RawQuery(string raw_query, MySqlParameterCollection par=null)
        {
            bool result = false;
            
            using (var cmd = new MySqlCommand(raw_query, this.dbcon))
            {
                if (par != null)
                {
                    foreach (var param in par)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                result = cmd.ExecuteNonQuery() > 0;
            }

            return result;
        }
        
        public void Close()
        {
            this.dbcon.Close();
        }
    }
}
