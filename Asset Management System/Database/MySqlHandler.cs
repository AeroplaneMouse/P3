using System;
using MySql.Data.MySqlClient;
using Asset_Management_System.Events;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database
{
    public class MySqlHandler
    {
        private readonly MySqlConnection _connection;
        public static event SqlConnectionEventHandler ConnectionFailed;
        
        public MySqlHandler()
        {
            _connection = new MySqlConnection("Server=192.38.49.9; database=ds303e19_dev; UID=ds303e19; password=Cisptf8CuT4hLj4T; Charset=utf8; Connect Timeout=5");
        }

        public MySqlConnection GetConnection()
        {
            if (IsAvailable())
                return _connection;
            else
                return null;
        }

        public bool IsAvailable()
        {
            try
            {
                var con = _connection;
                con.Open();
                con.Close();
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine($"SQL error message: { e.Message }");
                const string message = "ERROR! Unfortunately, the excellent connection to the database has been lost...";
                ConnectionFailed?.Invoke(new Notification(message, Notification.ERROR), true);
            }
            catch (Exception)
            {
                const string message = "ERROR! An unknown error has occured...";
                ConnectionFailed?.Invoke(new Notification(message, Notification.ERROR), false);
            }
            return false;
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