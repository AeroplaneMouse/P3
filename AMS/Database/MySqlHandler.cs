using System;
using MySql.Data.MySqlClient;
using AMS.Models;
using System.Threading.Tasks;
using AMS.Authentication;
using AMS.ConfigurationHandler;
using AMS.Events;
using AMS.ViewModels;

namespace AMS.Database
{
    public class MySqlHandler
    {
        private readonly MySqlConnection _connection;

        public static event SqlConnectionEventHandler ConnectionFailed;

        public MySqlHandler()
        {
            _connection = new MySqlConnection(Session.GetDBKey());
        }

        /// <summary>
        /// Opens a connection to the database.
        /// </summary>
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
        }

        /// <summary>
        /// Gets the current connection.
        /// </summary>
        public MySqlConnection GetConnection()
        {
            return _connection;
        }

        /// <summary>
        /// Check if a connection to the database is available to use.
        /// </summary>
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

        /// <summary>
        /// Gets the name of the current database or the database to be used after a connection is opened.
        /// </summary>
        public string GetDatabaseName()
        {
            return _connection.Database;
        }
    }
}