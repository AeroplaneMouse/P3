using System;
using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using AMS.ViewModels;

namespace AMS.Database.Repositories
{
    public class LogRepository : ILogRepository
    {
        /// <summary>
        /// Insert a LogEntry into the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Rather the insertion was successful</returns>
        public bool Insert(LogEntry entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "INSERT INTO log (user_id, entry_type, description, logged_item_id, logged_item_type, changes) " +
                                         "VALUES (@user_id, @entry_type, @description, @logged_item_id, @logged_item_type, @changes)";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@user_id", MySqlDbType.UInt64);
                        cmd.Parameters["@user_id"].Value = entity.UserId;

                        cmd.Parameters.Add("@entry_type", MySqlDbType.String);
                        cmd.Parameters["@entry_type"].Value = entity.EntryType;

                        cmd.Parameters.Add("@description", MySqlDbType.Text);
                        cmd.Parameters["@description"].Value = entity.Description;

                        cmd.Parameters.Add("@changes", MySqlDbType.JSON);
                        cmd.Parameters["@changes"].Value = entity.Changes;

                        cmd.Parameters.Add("@logged_item_id", MySqlDbType.UInt64);
                        cmd.Parameters["@logged_item_id"].Value = entity.LoggedItemType == null ? 0 : entity.LoggedItemId;

                        cmd.Parameters.Add("@logged_item_type", MySqlDbType.String);
                        cmd.Parameters["@logged_item_type"].Value = entity.LoggedItemType == null ? null : entity.LoggedItemType.ToString();

                        querySuccess = cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    con.Close();
                }
            }

            return querySuccess;
        }

        /// <summary>
        /// Returns entries matching the loggedItemId and type.
        /// </summary>
        /// <param name="loggedItemId"></param>
        /// <param name="loggedItemType"></param>
        /// <returns></returns>
        public IEnumerable<LogEntry> GetLogEntries(ulong loggedItemId, Type loggedItemType)
        {
            return GetLogEntries(loggedItemId, loggedItemType, null);
        }

        /// <summary>
        /// Returns entries matching the loggedItemId, type, and userId. If userId is null, all entries matching the remaining parameters are returned.
        /// </summary>
        /// <param name="loggedItemId"></param>
        /// <param name="loggedItemType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<LogEntry> GetLogEntries(ulong loggedItemId, Type loggedItemType, string userId)
        {
            var con = new MySqlHandler().GetConnection();
            List<LogEntry> entries = new List<LogEntry>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    string query = "SELECT l.id, u.username, u.domain, l.entry_type, l.description, l.logged_item_id, l.logged_item_type, l.changes, l.created_at " +
                                   "FROM log AS l INNER JOIN users AS u ON(l.user_id = u.id) " +
                                   "WHERE logged_item_id=@logged_item_id AND logged_item_type=@logged_item_type" +
                                   (userId != null ? " AND user_id=@user_id" : "")+" LIMIT 1000";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@logged_item_id", MySqlDbType.UInt64);
                        cmd.Parameters["@logged_item_id"].Value = loggedItemId;

                        cmd.Parameters.Add("@logged_item_type", MySqlDbType.String);
                        cmd.Parameters["@logged_item_type"].Value = loggedItemType.ToString();

                        if (userId != null)
                        {
                            cmd.Parameters.Add("@username", MySqlDbType.String);
                            cmd.Parameters["@username"].Value = userId;
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entries.Add(DataMapper(reader));
                            }
                            reader.Close();
                        }
                    }
                }
                catch (MySqlException e)
                {
                    
                }
                finally
                {
                    con.Close();
                }
            }
            
            return entries;
        }

        /// <summary>
        /// Returns all LogEntries matching the keyword from the database.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<LogEntry> Search(string keyword)
        {
            var con = new MySqlHandler().GetConnection();
            List<LogEntry> entries = new List<LogEntry>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    string query = "SELECT l.id, u.username, u.domain, l.entry_type, l.description, " +
                                   "l.logged_item_id, l.logged_item_type, l.changes, l.created_at " +
                                   "FROM log AS l INNER JOIN users AS u ON(l.user_id = u.id) " +
                                   "WHERE l.user_id LIKE @keyword OR l.description LIKE @keyword OR l.entry_type LIKE @keyword OR l.created_at LIKE @keyword " +
                                   "ORDER BY l.id desc LIMIT 1000";

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
                                entries.Add(DataMapper(reader));
                            }
                            reader.Close();
                        }
                    }
                }
                catch (MySqlException e)
                {
                    
                }
                finally
                {
                    con.Close();
                }
            }

            return entries;
        }
        
        /// <summary>
        /// Constructs an instance of the LogEntry
        /// </summary>
        /// <param name="reader">The MySqlDataReader to be used for reading the columns</param>
        /// <returns></returns>
        public LogEntry DataMapper(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            ulong rowLoggedItemId = reader.GetUInt64("logged_item_id");

            Type rowLoggedItemType;
            var ordinal = reader.GetOrdinal("logged_item_type");
            rowLoggedItemType = reader.IsDBNull(ordinal) ? null : Type.GetType(reader.GetString("logged_item_type"));
            string rowEntryType = reader.GetString("entry_type");
            string rowDescription = reader.GetString("description");
            string rowChanges = reader.GetString("changes");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            string rowUserDoamin = reader.GetString("domain");
            string rowUserName = reader.GetString("username");

            return (LogEntry) Activator.CreateInstance(typeof(LogEntry), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { rowId, rowEntryType, rowDescription, rowLoggedItemId, rowLoggedItemType, rowChanges, rowCreatedAt, rowUserDoamin, rowUserName },
                null, null);
        }
    }
}