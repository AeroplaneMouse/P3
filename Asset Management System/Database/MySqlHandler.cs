using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Asset_Management_System.Events;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database
{
    public class MySqlHandler
    {
        private string _connectionString;
        private MySqlConnection _connection;
        public event NotificationEventHandler SqlConnectionFailed;
        
        public MySqlHandler()
        {
            _connectionString = "Server=192.38.49.9; database=ds303e19; UID=ds303e19; password=Cisptf8CuT4hLj4T; Charset=utf8";
            _connection = new MySqlConnection(_connectionString);
        }

        public MySqlConnection GetConnection()
        {
            return _connection;
        }

        public bool IsAvailable()
        {
            try
            {
                var con = GetConnection();
                con.Open();
                con.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool RawQuery(string raw_query, MySqlParameterCollection par = null)
        {
            var con = GetConnection();
            bool result = false;

            try
            {
                con.Open();
                using (var cmd = new MySqlCommand(raw_query, con))
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
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                con.Close();
            }
            
            return result;
        }
    }
}