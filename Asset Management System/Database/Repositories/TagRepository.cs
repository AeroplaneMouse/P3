﻿using System;
using System.Collections.Generic;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Asset_Management_System.Database.Repositories
{
    public class TagRepository : ITagRepository
    {
        public ulong GetCount()
        {
            var con = new MySqlHandler().GetConnection();
            ulong count = 0;

            try
            {
                const string query = "SELECT COUNT(*) FROM tags;";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            count = reader.GetUInt64("COUNT(*)");
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
            
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Insert(Tag entity, out ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            id = 0;

            entity.SerializeFields();

            try
            {
                const string query = "INSERT INTO tags (label, color, options, department_id, parent_id, updated_at) " +
                                     "VALUES (@label, @color, @options, @department_id, @parent_id, CURRENT_TIMESTAMP())";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@label", MySqlDbType.String);
                    cmd.Parameters["@label"].Value = entity;

                    cmd.Parameters.Add("@color", MySqlDbType.String);
                    cmd.Parameters["@color"].Value = entity.Color;

                    cmd.Parameters.Add("@options", MySqlDbType.JSON);
                    cmd.Parameters["@options"].Value = entity.SerializedFields;

                    cmd.Parameters.Add("@department_id", MySqlDbType.UInt64);
                    cmd.Parameters["@department_id"].Value = entity.DepartmentID;

                    cmd.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                    cmd.Parameters["@parent_id"].Value = entity.ParentID;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(Tag entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            entity.SerializeFields();

            try
            {
                const string query = "UPDATE tags SET label=@label, color=@color, options=@options, parent_id=@parent_id, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@label", MySqlDbType.String);
                    cmd.Parameters["@label"].Value = entity;

                    cmd.Parameters.Add("@color", MySqlDbType.String);
                    cmd.Parameters["@color"].Value = entity.Color;

                    cmd.Parameters.Add("@options", MySqlDbType.JSON);
                    cmd.Parameters["@options"].Value = entity.SerializedFields;
                    
                    cmd.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                    cmd.Parameters["@parent_id"].Value = entity.ParentID;

                    cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                    cmd.Parameters["@id"].Value = entity.ID;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(Tag entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            try
            {
                const string query = "DELETE FROM tags WHERE id=@id";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                    cmd.Parameters["@id"].Value = entity.ID;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Tag GetById(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            Tag tag = null;

            try
            {
                const string query = "SELECT id, label, parent_id, department_id, color, options, created_at, updated_at " +
                                     "FROM tags WHERE id=@id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@ID", MySqlDbType.Int64);
                    cmd.Parameters["@ID"].Value = id;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tag = DBOToModelConvert(reader);
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
            
            return tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tag> GetParentTags()
        {
            return this.GetChildTags(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public IEnumerable<Tag> GetChildTags(ulong parentId)
        {
            var con = new MySqlHandler().GetConnection();
            List<Tag> tags = new List<Tag>();

            try
            {
                const string query = "SELECT id, label, parent_id, department_id, color, options, created_at, updated_at " +
                                     "FROM tags WHERE parent_id=@id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int64);
                    cmd.Parameters["@id"].Value = parentId;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tags.Add(DBOToModelConvert(reader));
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

            return tags;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ObservableCollection<Tag> Search(string keyword)
        {
            var con = new MySqlHandler().GetConnection();
            ObservableCollection<Tag> tags = new ObservableCollection<Tag>();

            try
            {
                const string query =
                    "SELECT id, label, parent_id, department_id, color, options, created_at, updated_at FROM tags WHERE label LIKE @keyword";

                if (!keyword.Contains('%'))
                    keyword = $"%{keyword}%";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@keyword", MySqlDbType.String);
                    cmd.Parameters["@keyword"].Value = keyword;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tags.Add(DBOToModelConvert(reader));
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
            
            return tags;
        }

        public IEnumerable<Tag> GetAll()
        {
            var con = new MySqlHandler().GetConnection();
            List<Tag> tags = new List<Tag>();

            try
            {
                const string query = "SELECT id, label, parent_id, department_id, color, options, created_at, updated_at, options " +
                                     "FROM tags";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tags.Add(DBOToModelConvert(reader));
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
                
            return tags;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Tag DBOToModelConvert(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            String rowLabel = reader.GetString("label");
            ulong rowParentId = reader.GetUInt64("parent_id");
            ulong rowDepartmentId = reader.GetUInt64("department_id");
            string rowColor = reader.GetString("color");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");
            string rowOptions = reader.GetString("options");

            return (Tag) Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowLabel, rowDepartmentId, rowParentId, rowColor, rowCreatedAt, rowUpdatedAt, rowOptions }, null,
                null);
        }
    }
}