using System;
using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;

namespace AMS.Database.Repositories
{
    public class LogRepository : ILogRepository
    {
        public bool Insert(LogEntry entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "INSERT INTO log (user_id, entry_type, description, changes) " +
                                         "VALUES (@user_id, @entry_type, @description, @changes)";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@userId", MySqlDbType.String);
                        cmd.Parameters["@userId"].Value = entity.UserId;

                        cmd.Parameters.Add("@entry_type", MySqlDbType.String);
                        cmd.Parameters["@entry_type"].Value = entity.EntryType;

                        cmd.Parameters.Add("@description", MySqlDbType.Text);
                        cmd.Parameters["@description"].Value = entity.Description;

                        cmd.Parameters.Add("@changes", MySqlDbType.JSON);
                        cmd.Parameters["@changes"].Value = entity.Changes;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
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
            }

            return querySuccess;
        }

        public IEnumerable<LogEntry> GetLogEntries(ulong logableId, Type logableType)
        {
            return GetLogEntries(logableId, logableType, null);
        }

        public IEnumerable<LogEntry> GetLogEntries(ulong logableId, Type logableType, string username)
        {
            var con = new MySqlHandler().GetConnection();
            List<LogEntry> entries = new List<LogEntry>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    string query = "SELECT id, logable_id, logable_type, username, description, options, created_at " +
                                   "FROM log WHERE logable_id=@logable_id AND logable_type=@logable_type" +
                                   (username != null ? " AND username=@username" : "");

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@logable_id", MySqlDbType.UInt64);
                        cmd.Parameters["@logable_id"].Value = logableId;

                        cmd.Parameters.Add("@logable_type", MySqlDbType.String);
                        cmd.Parameters["@logable_type"].Value = logableType.ToString();

                        if (username != null)
                        {
                            cmd.Parameters.Add("@username", MySqlDbType.String);
                            cmd.Parameters["@username"].Value = username;
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entries.Add(DBOToModelConvert(reader));
                            }
                            reader.Close();
                        }
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
            }
            
            return entries;
        }

        public ObservableCollection<LogEntry> Search(string keyword, List<ulong> tags=null, List<ulong> users=null, bool strict=false)
        {
            var con = new MySqlHandler().GetConnection();
            ObservableCollection<LogEntry> entries = new ObservableCollection<LogEntry>();
            int limit = 100;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    string query = "SELECT id, logable_id, logable_type, username, description, options, created_at " +
                                   "FROM log WHERE username LIKE @keyword OR description LIKE @keyword " +
                                   "ORDER BY id desc LIMIT " + limit;

                    if (!keyword.Contains("%"))
                        keyword = "%" + keyword + "%";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@keyword", MySqlDbType.String);
                        cmd.Parameters["@keyword"].Value = keyword;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entries.Add(DBOToModelConvert(reader));
                            }
                            reader.Close();
                        }
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
            }

            return entries;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public LogEntry DBOToModelConvert(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            ulong rowLogableId = reader.GetUInt64("logable_id");
            Type rowLogableType = Type.GetType(reader.GetString("logable_type"));
            string rowUsername = reader.GetString("username");
            string rowDescription = reader.GetString("description");
            string rowOptions = reader.GetString("options");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");

            return (LogEntry) Activator.CreateInstance(typeof(LogEntry), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { rowId, rowLogableId, rowLogableType, rowDescription, rowUsername, rowOptions, rowCreatedAt }, 
                null, null);
        }
    }
}