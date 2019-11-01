using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Asset_Management_System.Database.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        public bool Insert(Comment comment, out ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            bool query_success = false;
            id = 0;

            try
            {
                const string query = "INSERT INTO comments (asset_id, username, content, updated_at) " +
                                     "VALUES (@asset_id, @username, @content, CURRENT_TIMESTAMP())";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@asset_id", MySqlDbType.UInt64);
                    cmd.Parameters["@asset_id"].Value = comment.AssetID;

                    cmd.Parameters.Add("@username", MySqlDbType.String);
                    cmd.Parameters["@username"].Value = comment.Username;

                    cmd.Parameters.Add("@content", MySqlDbType.String);
                    cmd.Parameters["@content"].Value = comment.Content;

                    query_success = cmd.ExecuteNonQuery() > 0;
                    id = (ulong)cmd.LastInsertedId;
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
            
            return query_success;
        }

        public bool Update(Comment comment)
        {
            var con = new MySqlHandler().GetConnection();
            bool query_success = false;

            try
            {
                const string query = "UPDATE comments SET asset_id=@asset_id, username=@username, content=@content, updated_at=CURRENT_TIMESTAMP() " +
                                     "WHERE id=@id";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
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
            }catch (MySqlException e){
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return query_success;
        }

        public bool Delete(Comment comment)
        {
            var con = new MySqlHandler().GetConnection();
            bool query_success = false;

            try
            {
                const string query = "DELETE FROM comments WHERE id=@id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                    cmd.Parameters["@id"].Value = comment.ID;

                    query_success = cmd.ExecuteNonQuery() > 0;
                }
            }catch (MySqlException e){
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return query_success;
        }

        public Comment GetById(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            Comment comment = null;

            try
            {
                const string query = "SELECT id, asset_id, username, content, created_at, updated_at, deleted_at " +
                                     "FROM comments WHERE id=@id";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                    cmd.Parameters["@id"].Value = id;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comment = DBOToModelConvert(reader);
                        }
                        reader.Close();
                    }
                }
            }catch (MySqlException e){
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return comment;
        }

        public ObservableCollection<Comment> GetByAssetId(ulong assetID)
        {
            var con = new MySqlHandler().GetConnection();
            ObservableCollection<Comment> comments = new ObservableCollection<Comment>();

            try
            {
                const string query = "SELECT id, asset_id, username, content, created_at, updated_at, deleted_at " +
                                     "FROM comments WHERE asset_id=@asset_id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@asset_id", MySqlDbType.UInt64);
                    cmd.Parameters["@asset_id"].Value = assetID;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(DBOToModelConvert(reader));
                        }
                        reader.Close();
                    }
                }
            }catch (MySqlException e){
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return comments;
        }

        public Comment DBOToModelConvert(MySqlDataReader reader)
        {
            ulong row_id = reader.GetUInt64("id");
            ulong row_asset_id = reader.GetUInt64("asset_id");
            String row_username = reader.GetString("username");
            String row_content = reader.GetString("content");
            DateTime row_created_at = reader.GetDateTime("created_at");
            DateTime row_updated_at = reader.GetDateTime("updated_at");

            return (Comment)Activator.CreateInstance(typeof(Comment), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { row_id, row_username, row_content, row_asset_id, row_created_at, row_updated_at }, null, null);
        }
    }
}
