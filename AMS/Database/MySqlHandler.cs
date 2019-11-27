using System;
using MySql.Data.MySqlClient;
using AMS.Models;
using System.Threading.Tasks;
using AMS.ConfigurationHandler;
using AMS.Events;
using AMS.ViewModels;

namespace AMS.Database
{
    public class MySqlHandler
    {
        private readonly MySqlConnection _connection;
        //private static MySqlConnection _con;
        private FileConfigurationHandler _fileConfigurationHandler;
        
        public static event SqlConnectionEventHandler ConnectionFailed;

        public MySqlHandler()
        {
            // "Server=192.38.49.9; database=ds303e19_dev; UID=ds303e19; password=Cisptf8CuT4hLj4T; Charset=utf8; Connect Timeout=5"
            _fileConfigurationHandler = new FileConfigurationHandler(Features.GetCurrentSession());
            _connection = new MySqlConnection(_fileConfigurationHandler.GetConfigValue());
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
                // TODO: Open MySQL connection error handling
                // Maybe something like
                // e.Number == MysqlConnection.ErrorNumber
                if (e.Message == "Something connection failed...")
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
            var con = GetConnection();
            if (Open(ref con))
            {
                con.Close();
                return true;
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