using System;
using AMS.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using AMS.Database.Repositories.Interfaces;
using System.Collections.Generic;
using AMS.ViewModels;
using AMS.Logging.Interfaces;
using AMS.Logging;

namespace AMS.Database.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private Ilogger logger { get; set; } = new Logger(new LogRepository());

        public Comment Insert(Comment entity, out ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            var querySuccess = false;
            id = 0;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "INSERT INTO comments (asset_id, username, content, updated_at) " +
                                         "VALUES (@asset_id, @username, @content, CURRENT_TIMESTAMP())";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@asset_id", MySqlDbType.UInt64);
                        cmd.Parameters["@asset_id"].Value = entity.AssetID;

                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = entity.Username;

                        cmd.Parameters.Add("@content", MySqlDbType.String);
                        cmd.Parameters["@content"].Value = entity.Content;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
                        id = (ulong)cmd.LastInsertedId;
                    }

                    logger.AddEntry(entity, Features.GetCurrentSession().user.ID, id);
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
            
            return querySuccess ? GetById(id) : null;
        }

        public bool Update(Comment entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con) && entity.IsDirty())
            {
                try
                {
                    const string query = "UPDATE comments SET asset_id=@asset_id, username=@username, content=@content, updated_at=CURRENT_TIMESTAMP() " +
                                         "WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@asset_id", MySqlDbType.UInt64);
                        cmd.Parameters["@asset_id"].Value = entity.AssetID;

                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = entity.Username;

                        cmd.Parameters.Add("@content", MySqlDbType.String);
                        cmd.Parameters["@content"].Value = entity.Content;

                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = entity.ID;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
                    }
                    
                    logger.AddEntry(entity, Features.GetCurrentSession().user.ID);
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

        public bool Delete(Comment entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "UPDATE comments SET deleted_at=CURRENT_TIMESTAMP() WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = entity.ID;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
                    }

                    logger.AddEntry(entity, Features.GetCurrentSession().user.ID);
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

        public Comment GetById(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            Comment comment = null;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT id, asset_id, username, content, created_at, updated_at, deleted_at " +
                                         "FROM comments WHERE id=@id AND deleted_at IS NULL";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comment = DataMapper(reader);
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
            
            return comment;
        }

        public List<Comment> GetByAssetId(ulong assetId)
        {
            var con = new MySqlHandler().GetConnection();
            List<Comment> comments = new List<Comment>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT id, asset_id, username, content, created_at, updated_at, deleted_at " +
                                         "FROM comments WHERE asset_id=@asset_id AND deleted_at IS NULL";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@asset_id", MySqlDbType.UInt64);
                        cmd.Parameters["@asset_id"].Value = assetId;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comments.Add(DataMapper(reader));
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
            
            return comments;
        }

        public List<Comment> GetAll(bool includeDeleted = false, int limit=100)
        {
            var con = new MySqlHandler().GetConnection();
            List<Comment> comments = new List<Comment>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                // Sending sql query
                try
                {
                    string query = "SELECT c.id, c.asset_id, c.username, c.content, c.created_at, c.updated_at FROM comments as c "+ 
                                   "INNER JOIN assets a on c.asset_id = a.id "+ 
                                   "WHERE a.deleted_at IS NULL "+ 
                                   (!includeDeleted ? "AND c.deleted_at IS NULL " : "")+
                                   "ORDER BY created_at DESC LIMIT "+limit;

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comments.Add(DataMapper(reader));
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

            return comments;
        }

        public Comment DataMapper(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            ulong rowAssetId = reader.GetUInt64("asset_id");
            String rowUsername = reader.GetString("username");
            String rowContent = reader.GetString("content");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");

            return (Comment)Activator.CreateInstance(typeof(Comment),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowUsername, rowContent, rowAssetId, rowCreatedAt, rowUpdatedAt }, null, null);
        }
    }
}
