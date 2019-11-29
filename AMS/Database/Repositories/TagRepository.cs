﻿using System;
using System.Collections.Generic;
using AMS.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Database.Repositories.Interfaces;
using AMS.ViewModels;
using AMS.Logging.Interfaces;
using AMS.Logging;
using Org.BouncyCastle.Utilities;

namespace AMS.Database.Repositories
{
    public class TagRepository : ITagRepository
    {
        private Ilogger logger { get; set; } = new Logger(new LogRepository());

        public ulong GetCount()
        {
            var con = new MySqlHandler().GetConnection();
            ulong count = 0;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT COUNT(*) FROM tags;";
                
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

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "INSERT INTO tags (label, color, options, department_id, parent_id, updated_at) " +
                                         "VALUES (@label, @color, @options, @department_id, @parent_id, CURRENT_TIMESTAMP())";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@label", MySqlDbType.String);
                        cmd.Parameters["@label"].Value = entity;

                        cmd.Parameters.Add("@color", MySqlDbType.String);
                        cmd.Parameters["@color"].Value = entity.Color;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields == null ? "[]" : entity.SerializedFields;

                        if (entity.DepartmentID == 0)
                        {
                            cmd.Parameters.Add("@department_id", MySqlDbType.String);
                            cmd.Parameters["@department_id"].Value = null;
                        }
                        else
                        {
                            cmd.Parameters.Add("@department_id", MySqlDbType.UInt64);
                            cmd.Parameters["@department_id"].Value = entity.DepartmentID;
                        }
                        
                        cmd.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                        cmd.Parameters["@parent_id"].Value = entity.ParentID;

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
            //MySqlTransaction trans = con.BeginTransaction();
            
            bool querySuccess = false;
            
            // Check if current tag is a child and changed parent
            if (entity.ParentId > 0 && entity.Changes.ContainsKey("ParentID"))
            {
                // Is child and parent_id changed
                Tag newParent = GetById((ulong) entity.Changes["ParentID"]);

                if (newParent.DepartmentID != entity.DepartmentID)
                {
                    entity.DepartmentID = newParent.DepartmentID;
                    ClearConnections(entity);
                }
            }

            // Opening connection
            if (MySqlHandler.Open(ref con) && entity.IsDirty())
            {
                try
                {
                    const string query = "UPDATE tags SET label=@label, color=@color, options=@options, department_id=@department_id, parent_id=@parent_id, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@label", MySqlDbType.String);
                        cmd.Parameters["@label"].Value = entity;

                        cmd.Parameters.Add("@color", MySqlDbType.String);
                        cmd.Parameters["@color"].Value = entity.Color;

                        if (entity.DepartmentID == 0)
                        {
                            cmd.Parameters.Add("@department_id", MySqlDbType.String);
                            cmd.Parameters["@department_id"].Value = null;
                        }
                        else
                        {
                            cmd.Parameters.Add("@department_id", MySqlDbType.UInt64);
                            cmd.Parameters["@department_id"].Value = entity.DepartmentID;
                        }

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields == null ? "[]" : entity.SerializedFields;

                        cmd.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                        cmd.Parameters["@parent_id"].Value = entity.ParentID;

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
                
                // If we are updating a parent, make sure that to
                // update the children tags department_id if needed
                if (entity.ParentID == 0 
                    && entity.ChildrenCount > 0 
                    && entity.Changes.ContainsKey("DepartmentID")){
                    UpdateChildrenDepartmentId(entity);
                }
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
            if (entity.ID == 1)
                return false;
            
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "DELETE FROM tags WHERE id=@id";

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Tag GetById(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            Tag tag = null;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, " +
                                     "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren " +
                                     "FROM tags AS t WHERE t.id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@ID", MySqlDbType.Int64);
                        cmd.Parameters["@ID"].Value = id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tag = DataMapper(reader);
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

            return tag;
        }

        public IEnumerable<Tag> GetTagsForAsset(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            List<Tag> tags = new List<Tag>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, " +
                                     "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren " +
                                     "FROM tags AS t " +
                                     "INNER JOIN asset_tags AS at ON at.tag_id = t.id " +
                                     "WHERE at.asset_id = @id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(DataMapper(reader));
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

            return tags;
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

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                { 
                    const string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, " +
                                     "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren " +
                                     "FROM tags AS t WHERE t.parent_id=@id  ORDER BY countChildren DESC, t.label ASC";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int64);
                        cmd.Parameters["@id"].Value = parentId;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(DataMapper(reader));
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

            return tags;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<Tag> Search(string keyword,List<ulong> tag_list=null, List<ulong> users=null, bool strict=false)
        {
            var con = new MySqlHandler().GetConnection();
            List<Tag> tags = new List<Tag>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try 
                {
                    string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, " +
                                     "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren " +
                                     "FROM tags AS t WHERE t.label LIKE @keyword";

                    if (Features.Main.CurrentDepartment.ID > 0)
                        query += $" AND t.department_id={ Features.Main.CurrentDepartment.ID.ToString() } OR t.department_id IS NULL";

                    if (!keyword.Contains('%'))
                        keyword = $"%{keyword}%";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@keyword", MySqlDbType.String);
                        cmd.Parameters["@keyword"].Value = keyword;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(DataMapper(reader));
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

            return tags;
        }

        public IEnumerable<Tag> GetAll()
        {
            var con = new MySqlHandler().GetConnection();
            List<Tag> tags = new List<Tag>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                // Sending sql query
                try
                {
                    string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, t.options, " +
                                     "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren " +
                                     "FROM tags AS t";
                    
                    if (Features.Main.CurrentDepartment.ID > 0)
                        query += $" WHERE t.department_id={ Features.Main.CurrentDepartment.ID.ToString() } OR t.department_id IS NULL";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(DataMapper(reader));
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
                
            return tags;
        }

        private void UpdateChildrenDepartmentId(Tag tag)
        {
            var con = new MySqlHandler().GetConnection();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                // Sending sql query
                try
                {
                    const string query = "UPDATE tags SET department_id=@department_id WHERE parent_id=@parent_id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        if (tag.DepartmentID == 0)
                        {
                            cmd.Parameters.Add("@department_id", MySqlDbType.String);
                            cmd.Parameters["@department_id"].Value = null;
                        }
                        else
                        {
                            cmd.Parameters.Add("@department_id", MySqlDbType.UInt64);
                            cmd.Parameters["@department_id"].Value = tag.DepartmentID;
                        }
                        
                        cmd.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                        cmd.Parameters["@parent_id"].Value = tag.ID;
                        
                        cmd.ExecuteNonQuery();
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
        }

        private void ClearConnections(Tag tag)
        {
            var con = new MySqlHandler().GetConnection();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "DELETE FROM asset_tags WHERE tag_id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = tag.ID;
                        
                        cmd.ExecuteNonQuery();
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
        }

        public IEnumerable<Tag> GetTreeViewDataList(string keyword="")
        {
            if (!keyword.Contains('%'))
                keyword = $"%{keyword}%";
            
            var con = new MySqlHandler().GetConnection();
            Dictionary<ulong, Tag> tags_placeholder = new Dictionary<ulong, Tag>();
            
            if (MySqlHandler.Open(ref con))
            {
                ulong department = Features.Main.CurrentDepartment.ID;
                
                // Sending sql query
                try
                {
                    string query = "SELECT DISTINCT tp.id AS id, tp.label, tp.parent_id, tp.department_id, tp.color, " +
                                   "IF((SELECT COUNT(id) FROM tags WHERE parent_id = tp.id) OR tp.id = 1, 1, 0) AS contains_children " +
                                   "FROM tags AS tp " +
                                   "LEFT JOIN tags AS tc ON " +
                                   "    tp.id = tc.parent_id " +
                                   "    "+(department > 0 ? "AND (tc.department_id = @department OR tc.department_id IS NULL) " : "") +
                                   "    AND tc.label LIKE @keyword "+
                                   "WHERE (tp.label LIKE @keyword OR tc.label LIKE @keyword) " +
                                   "    "+(department > 0 ? "AND (tp.department_id = @department OR tp.department_id IS NULL)" : "") +
                                   "ORDER BY tp.parent_id ASC, contains_children DESC, tp.label ASC";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@keyword", MySqlDbType.String);
                        cmd.Parameters["@keyword"].Value = keyword;

                        if (department > 0)
                        {
                            cmd.Parameters.Add("@department", MySqlDbType.UInt64);
                            cmd.Parameters["@department"].Value = department;
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ulong rowId = reader.GetUInt64("id");
                                String rowLabel = reader.GetString("label");
                                ulong rowParentId = reader.GetUInt64("parent_id");
                                var ordinal = reader.GetOrdinal("department_id");
                                ulong rowDepartmentId = (reader.IsDBNull(ordinal) ? 0 : reader.GetUInt64("department_id"));
                                string rowColor = reader.GetString("color");
                                int rowContainsChildren = reader.GetInt32("contains_children");

                                Tag tag = (Tag) Activator.CreateInstance(typeof(Tag),
                                    BindingFlags.Instance | BindingFlags.NonPublic, null,
                                    new object[] { rowId, rowLabel, rowDepartmentId, rowParentId, rowColor, rowContainsChildren, null, null, "[]" }, null,
                                    null);

                                if (tag.ParentId > 0 && tags_placeholder.ContainsKey(tag.ParentId))
                                    tags_placeholder[tag.ParentId].Children.Add(tag);
                                else
                                    tags_placeholder.Add(tag.ID, tag);
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
            
            List<Tag> tags = new List<Tag>();

            foreach (var item in tags_placeholder)
            {
                tags.Add(item.Value);
            }

            return tags;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Tag DataMapper(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            String rowLabel = reader.GetString("label");
            ulong rowParentId = reader.GetUInt64("parent_id");
            var ordinal = reader.GetOrdinal("department_id");
            ulong rowDepartmentId = (reader.IsDBNull(ordinal) ? 0 : reader.GetUInt64("department_id"));
            string rowColor = reader.GetString("color");
            int rowNumOfChildren = reader.GetInt32("countChildren");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");
            string rowOptions = reader.GetString("options");

            return (Tag) Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowLabel, rowDepartmentId, rowParentId, rowColor, rowNumOfChildren, rowCreatedAt, rowUpdatedAt, rowOptions }, null,
                null);
        }
    }
}