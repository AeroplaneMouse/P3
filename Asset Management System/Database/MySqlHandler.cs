using System;
using MySql.Data.MySqlClient;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using System.Threading.Tasks;

namespace Asset_Management_System.Database
{
    public class MySqlHandler
    {
        private readonly MySqlConnection _connection;
        //private static MySqlConnection _con;
        public static event SqlConnectionEventHandler ConnectionFailed;
        

        public MySqlHandler()
        {
            _connection = new MySqlConnection("Server=192.38.49.9; database=ds303e19_dev; UID=ds303e19; password=Cisptf8CuT4hLj4T; Charset=utf8; Connect Timeout=5");
        }

        public static bool Open(ref MySqlConnection con)
        {
            try
            {
                con.Open();
                return true;
            }
            catch (MySqlException e)
            {
                ConnectionFailed?.Invoke();
            }
            return false;
            //_con = con;
            //return await Task.Run(_open);
        }

        //private static bool _open()
        //{
        //    try
        //    {
        //        _con.Open();
        //        return true;
        //    }
        //    catch (MySqlException e)
        //    {
        //        ConnectionFailed?.Invoke();
        //    }
        //    return false;
        //}

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
            catch (MySqlException)
            {
                Console.WriteLine("Connection failed...");
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