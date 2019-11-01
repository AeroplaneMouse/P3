﻿using System;
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
            var querySuccess = false;
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

                    querySuccess = cmd.ExecuteNonQuery() > 0;
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
            
            return querySuccess;
        }

        public bool Update(Comment comment)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

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

                    querySuccess = cmd.ExecuteNonQuery() > 0;
                }
            }catch (MySqlException e){
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return querySuccess;
        }

        public bool Delete(Comment comment)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            try
            {
                const string query = "DELETE FROM comments WHERE id=@id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                    cmd.Parameters["@id"].Value = comment.ID;

                    querySuccess = cmd.ExecuteNonQuery() > 0;
                }
            }catch (MySqlException e){
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return querySuccess;
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

        public ObservableCollection<Comment> GetByAssetId(ulong assetId)
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
                    cmd.Parameters["@asset_id"].Value = assetId;

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
