﻿using System;
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

        public IEnumerable<LogEntry> GetLogEntries(ulong loggedItemId, Type loggedItemType, string userId)
        {
            var con = new MySqlHandler().GetConnection();
            List<LogEntry> entries = new List<LogEntry>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    string query = "SELECT l.id, u.username, u.domain, l.entry_type, l.description, l.changes, l.created_at " +
                                   "FROM log AS l INNER JOIN users AS u ON(l.user_id = u.id) " +
                                   "WHERE logged_item_id=@logged_item_id AND logged_item_type=@logged_item_type" +
                                   (userId != null ? " AND user_id=@user_id" : "");

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@logged_item_id", MySqlDbType.UInt64);
                        cmd.Parameters["@logged_item_id"].Value = loggedItemId;

                        cmd.Parameters.Add("@logable_type", MySqlDbType.String);
                        cmd.Parameters["@logable_type"].Value = loggedItemType.ToString();

                        if (userId != null)
                        {
                            cmd.Parameters.Add("@username", MySqlDbType.String);
                            cmd.Parameters["@username"].Value = userId;
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
                    string query = "SELECT id, user_id, entry_type, description, changes, logged_item_id, logged_item_type, created_at " +
                                   "FROM log WHERE user_id LIKE @keyword OR description LIKE @keyword " +
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
            ulong rowLoggedItemId = reader.GetUInt64("logged_item_id");
            Type rowLoggedItemType = Type.GetType(reader.GetString("logged_item_type"));
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