using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace Asset_Management_System.Database.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private DBConnection _dbcon;

        public CommentRepository()
        {
            _dbcon = DBConnection.Instance();
        }

        public bool Insert(Comment comment)
        {
            bool query_success = false;

            if (_dbcon.IsConnect())
            {
                try
                {
                    const string query = "INSERT INTO comments (asset_id, username, content, updated_at)" +
                                         "VALUES (@asset_id, @username, @content, CURRENT_TIMESTAMP())";

                    using (var cmd = new MySqlCommand(query, _dbcon.Connection))
                    {
                        cmd.Parameters.Add("@asset_id", MySqlDbType.UInt64);
                        cmd.Parameters["@asset_id"].Value = comment.AssetID;

                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = comment.Username;

                        cmd.Parameters.Add("@content", MySqlDbType.String);
                        cmd.Parameters["@content"].Value = comment.Content;

                        query_success = cmd.ExecuteNonQuery() > 0;
                    }

                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                }

                finally
                {
                    _dbcon.Close();
                }
            }

            return query_success;
        }

        public bool Update(Comment comment)
        {
            bool query_success = false;

            if (_dbcon.IsConnect())
            {
                try
                {
                    const string query = "UPDATE comments SET asset_id=@asset_id, username=@username, content=@content, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, _dbcon.Connection))
                    {
                        cmd.Parameters.Add("@asset_id", MySqlDbType.UInt64);
                        cmd.Parameters["@asset_id"].Value = comment.AssetID;

                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = comment.Username;

                        cmd.Parameters.Add("@content", MySqlDbType.String);
                        cmd.Parameters["@content"].Value = comment.Content;

                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = comment.ID;

                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }

                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                }

                finally
                {
                    _dbcon.Close();
                }
            }

            return query_success;
        }

        public bool Delete(Comment comment)
        {
            bool query_success = false;

            if (_dbcon.IsConnect())
            {
                try
                {
                    const string query = "DELETE FROM comments WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, _dbcon.Connection))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = comment.ID;

                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }

                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                }

                finally
                {
                    _dbcon.Close();
                }
            }

            return query_success;
        }

        public Comment GetById(ulong id)
        {
            Comment comment = null;

            if (_dbcon.IsConnect())
            {
                try
                {
                    const string query = "SELECT id, asset_id, username, content, created_at, updated_at, deleted_at" +
                                         "FROM comments WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, _dbcon.Connection))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comment = DBOToModelConvert(reader);
                            }
                        }
                    }
                }

                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                }

                finally
                {
                    _dbcon.Close();
                }
            }

            return comment;
        }

        public Comment DBOToModelConvert(MySqlDataReader reader)
        {
            ulong row_id = reader.GetUInt64("id");
            String row_content = reader.GetString("content");
            ulong row_asset_id = reader.GetUInt64("asset_id");
            DateTime row_created_at = reader.GetDateTime("created_at");

            return (Comment)Activator.CreateInstance(typeof(Comment), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { row_id, row_content, row_asset_id, row_created_at }, null, null);
        }
    }
}
