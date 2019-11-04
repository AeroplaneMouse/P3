﻿using System;
using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using Asset_Management_System.Logging;
using System.Collections.ObjectModel;

namespace Asset_Management_System.Database.Repositories
{
    public class LogRepository : ILogRepository<Entry>
    {
        public bool Insert(Entry entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;
            
            try
            {
                const string query = "INSERT INTO log (username, description, options, logable_id, logable_type) " +
                                     "VALUES (@username, @description, @options, @logable_id, @logable_type)";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.String);
                    cmd.Parameters["@username"].Value = entity.Username;

                    cmd.Parameters.Add("@description", MySqlDbType.Text);
                    cmd.Parameters["@description"].Value = entity.Description;

                    cmd.Parameters.Add("@options", MySqlDbType.JSON);
                    cmd.Parameters["@options"].Value = entity.Options;

                    cmd.Parameters.Add("@logable_id", MySqlDbType.UInt64);
                    cmd.Parameters["@logable_id"].Value = entity.LogableId;

                    cmd.Parameters.Add("@logable_type", MySqlDbType.String);
                    cmd.Parameters["@logable_type"].Value = entity.LogableType.ToString();

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
            
            return querySuccess;
        }

        public IEnumerable<Entry> GetLogEntries(ulong logableId, Type logableType)
        {
            return GetLogEntries(logableId, logableType, null);
        }

        public IEnumerable<Entry> GetLogEntries(ulong logableId, Type logableType, string username)
        {
            var con = new MySqlHandler().GetConnection();
            List<Entry> entries = new List<Entry>();

            try
            {
                string query = "SELECT id, logable_id, logable_type, username, description, options, created_at " + 
                               "FROM log WHERE logable_id=@logable_id AND logable_type=@logable_type"+
                               (username != null ? " AND username=@username" : "");
                
                con.Open();
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
            
            return entries;
        }

        public ObservableCollection<Entry> Search(string keyword, List<ulong> tags_list=null)
        {
            var con = new MySqlHandler().GetConnection();
            ObservableCollection<Entry> entries = new ObservableCollection<Entry>();
            int limit = 100;

            try
            {
                string query = "SELECT id, logable_id, logable_type, username, description, options, created_at " +
                               "FROM log WHERE username LIKE @keyword OR description LIKE @keyword " +
                               "ORDER BY id desc LIMIT "+limit;
                
                if (!keyword.Contains("%"))
                {
                    keyword = "%" + keyword + "%";
                }
                
                con.Open();
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
            
            return entries;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Entry DBOToModelConvert(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            ulong rowLogableId = reader.GetUInt64("logable_id");
            Type rowLogableType = Type.GetType(reader.GetString("logable_type"));
            string rowUsername = reader.GetString("username");
            string rowDescription = reader.GetString("description");
            string rowOptions = reader.GetString("options");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");

            return (Entry) Activator.CreateInstance(typeof(Entry), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { rowId, rowLogableId, rowLogableType, rowDescription, rowUsername, rowOptions, rowCreatedAt }, 
                null, null);
        }
    }
}