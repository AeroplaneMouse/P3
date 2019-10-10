using System;
using System.Collections.Generic;
using System.Reflection;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Database;
using MySql.Data.MySqlClient;
using Asset_Management_System.Logging;

namespace Asset_Management_System.Database.Repositories
{
    public class LogRepository : ILogRepository<Entry>
    {
        private DBConnection dbcon;

        public LogRepository()
        {
            this.dbcon = DBConnection.Instance();
        }
        
        public bool Insert(Entry entity)
        {
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "INSERT INTO log (username, description, options, logable_id, logable_type) " +
                                         "VALUES (@username, @description, @options, @logable_id, @logable_type)";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = entity.Username;

                        cmd.Parameters.Add("@description", MySqlDbType.Text);
                        cmd.Parameters["@description"].Value = entity.Description;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = "{}";

                        cmd.Parameters.Add("@logable_id", MySqlDbType.UInt64);
                        cmd.Parameters["@logable_id"].Value = entity.LogableId;

                        cmd.Parameters.Add("@logable_type", MySqlDbType.String);
                        cmd.Parameters["@logable_type"].Value = entity.LogableType.ToString();

                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    dbcon.Close();
                }
            }

            return query_success;
        }

        public List<Entry> GetLogEntries(ulong logable_id, Type logable_type)
        {
            return GetLogEntries(logable_id, logable_type, null);
        }

        public List<Entry> GetLogEntries(ulong logable_id, Type logable_type, string username)
        {
            List<Entry> entries = new List<Entry>();

            if (dbcon.IsConnect())
            {
                try
                {
                    string query = "SELECT id, logable_id, logable_type, username, description, options, created_at " + 
                                   "FROM logs WHERE logable_id=@logable_id AND logable_type=logable_type"+
                                   (username != null ? " AND username=@username" : "");

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@logable_id", MySqlDbType.UInt64);
                        cmd.Parameters["@logable_id"].Value = logable_id;
                        
                        cmd.Parameters.Add("@logable_type", MySqlDbType.String);
                        cmd.Parameters["@logable_type"].Value = logable_type.ToString();

                        if (username != null)
                        {
                            cmd.Parameters.Add("@username", MySqlDbType.String);
                            cmd.Parameters["@username"].Value = username;
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ulong row_id = reader.GetUInt64("id");
                                ulong row_logable_id = reader.GetUInt64("logable_id");
                                string row_logable_type = reader.GetString("logable_type");
                                string row_username = reader.GetString("username");
                                string row_description = reader.GetString("description");
                                string row_options = reader.GetString("options");
                                DateTime row_created_at = reader.GetDateTime("created_at");

                                Entry entry = (Entry)Activator.CreateInstance(typeof(Entry), 
                                    BindingFlags.Instance | BindingFlags.NonPublic, null, 
                                    new object[] { row_id, row_logable_id, row_logable_type, row_description, row_username, row_options, row_created_at }, 
                                    null, null);
                                
                                entries.Add(entry);
                            }
                        }
                    }
                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    dbcon.Close();
                }
            }

            return entries;
        }
    }
}