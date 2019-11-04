using System;
using Asset_Management_System.Events;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database
{
    public class MySqlHandler
    {
        private readonly MySqlConnection _connection;
        public event NotificationEventHandler SqlConnectionFailed;
        
        public MySqlHandler()
        {
            _connection = new MySqlConnection("Server=192.38.49.9; database=ds303e19; UID=ds303e19; password=Cisptf8CuT4hLj4T; Charset=utf8; Connect Timeout=5");
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
            catch (Exception)
            {
                return false;
            }
        }

        public bool RawQuery(string rawQuery, MySqlParameterCollection par = null)
        {
            var con = GetConnection();
            var result = false;

            try
            {
                con.Open();
                using (var cmd = new MySqlCommand(rawQuery, con))
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