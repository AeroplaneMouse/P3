﻿using System;
using System.Collections.Generic;
using AMS.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using AMS.Database.Repositories.Interfaces;
using AMS.ViewModels;
using AMS.Logging.Interfaces;
using AMS.Logging;

namespace AMS.Database.Repositories
{
    public class TagRepository : ITagRepository
    {
        private Ilogger _logger { get; set; }

        public TagRepository()
        {
            _logger = new Logger(Features.LogRepository);
        }

        /// <summary>
        /// Gets the number of tags in the database
        /// </summary>
        /// <returns></returns>
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
                    _logger.AddEntry(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return count;
        }

        /// <summary>
        /// Inserts a tag into the database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns>The instance added to the database, with its databse ID</returns>
        public Tag Insert(Tag entity, out ulong id)
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

                        if (entity.ParentId == 0)
                        {
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
                        }
                        else
                        {
                            ulong parentDepartmentID = GetById(entity.ParentId).DepartmentID;
                            if (parentDepartmentID == 0)
                            {
                                cmd.Parameters.Add("@department_id", MySqlDbType.String);
                                cmd.Parameters["@department_id"].Value = null;
                            }
                            else
                            {
                                cmd.Parameters.Add("@department_id", MySqlDbType.UInt64);
                                cmd.Parameters["@department_id"].Value = parentDepartmentID;
                            }
                        }

                        cmd.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                        cmd.Parameters["@parent_id"].Value = entity.ParentId;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
                        id = (ulong)cmd.LastInsertedId;
                    }

                    _logger.AddEntry(entity, Features.GetCurrentSession().User.ID, id);
                }
                catch (MySqlException e)
                {
                    _logger.AddEntry(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return querySuccess ? GetById(id) : null;
        }

        /// <summary>
        /// Updates a tag in the database with the same ID as the input tag
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Rather the update was successful or not</returns>
        public bool Update(Tag entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con) && entity.IsDirty())
            {
                MySqlCommand command = con.CreateCommand();
                MySqlTransaction transaction = con.BeginTransaction();
                command.Transaction = transaction;
                command.Connection = con;
                
                try
                {
                    // Check if current tag is a child and changed parent
                    if (entity.ParentId > 0 && entity.Changes.ContainsKey("ParentId"))
                    {
                        // Is child and parent_id changed
                        Tag newParent = GetById((ulong) entity.Changes["ParentId"]);

                        if (newParent != null && newParent.DepartmentID != entity.DepartmentID)
                        {
                            entity.DepartmentID = newParent.DepartmentID;
                            ClearConnections(entity, command);
                        }
                    }
                    
                    command.CommandText = "UPDATE tags SET label=@label, color=@color, options=@options, department_id=@department_id, parent_id=@parent_id, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";
                    command.Parameters.Add("@label", MySqlDbType.String);
                    command.Parameters["@label"].Value = entity;
                    command.Parameters.Add("@color", MySqlDbType.String);
                    command.Parameters["@color"].Value = entity.Color;

                    if (entity.ParentId == 0)
                    {
                        if (entity.DepartmentID == 0)
                        {
                            command.Parameters.Add("@department_id", MySqlDbType.String);
                            command.Parameters["@department_id"].Value = null;
                        }
                        else
                        {
                            command.Parameters.Add("@department_id", MySqlDbType.UInt64);
                            command.Parameters["@department_id"].Value = entity.DepartmentID;
                        }
                    }
                    else
                    {
                        ulong parentDepartmentID = GetById(entity.ParentId).DepartmentID;
                        if(parentDepartmentID == 0)
                        {
                            command.Parameters.Add("@department_id", MySqlDbType.String);
                            command.Parameters["@department_id"].Value = null;
                        }
                        else
                        {
                            command.Parameters.Add("@department_id", MySqlDbType.UInt64);
                            command.Parameters["@department_id"].Value = parentDepartmentID;
                        }
                    }

                    command.Parameters.Add("@options", MySqlDbType.JSON);
                    command.Parameters["@options"].Value = entity.SerializedFields == null ? "[]" : entity.SerializedFields;

                    command.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                    command.Parameters["@parent_id"].Value = entity.ParentId;

                    command.Parameters.Add("@id", MySqlDbType.UInt64);
                    command.Parameters["@id"].Value = entity.ID;

                    querySuccess = command.ExecuteNonQuery() > 0;
                    
                    // If we are updating a parent, make sure that to
                    // update the children tags department_id if needed
                    if (entity.ParentId == 0 
                        && entity.NumberOfChildren > 0 
                        && entity.Changes.ContainsKey("DepartmentdId")){
                        UpdateChildrenDepartmentId(entity, command);
                    }
                    
                    transaction.Commit();
                    
                    _logger.AddEntry(entity, Features.GetCurrentSession().User.ID);
                }
                catch (MySqlException e)
                {
                    transaction.Rollback();
                    _logger.AddEntry(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return querySuccess;
        }

        public bool Delete(Tag entity)
        {
            return Delete(entity, false);
        }

        /// <summary>
        /// Deletes a tag from the database that corresponds with the input tag
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Rather the deletion was successful or not</returns>
        public bool Delete(Tag entity, bool removeChildren)
        {
            if (entity.ID == 1)
                return false;

            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                MySqlCommand command = con.CreateCommand();
                MySqlTransaction transaction = con.BeginTransaction();
                command.Transaction = transaction;
                command.Connection = con;
                
                try
                {
                    // Removes a parent children
                    if (removeChildren && entity.ParentId == 0)
                        DeleteChildren(entity.ID, command);
                    
                    // Update children's parent id to zero
                    else if (!removeChildren && entity.ParentId == 0)
                    {
                        command.CommandText = "UPDATE tags SET parent_id=0 WHERE parent_id=@pid";
                        command.Parameters.Add("@pid", MySqlDbType.UInt64);
                        command.Parameters["@pid"].Value = entity.ID;
                        command.ExecuteNonQuery();
                    }

                    command.CommandText = "DELETE FROM tags WHERE id=@id";
                    command.Parameters.Add("@id", MySqlDbType.UInt64);
                    command.Parameters["@id"].Value = entity.ID;
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    
                    _logger.AddEntry(entity, Features.GetCurrentSession().User.ID);
                    querySuccess = true;
                }
                catch (MySqlException e)
                {
                    transaction.Rollback();
                    _logger.AddEntry(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return querySuccess;
        }

        public bool DeleteChildren(ulong parentID)
        {
            throw new NotImplementedException();
        }

        private void DeleteChildren(ulong parentID, MySqlCommand command)
        {
            command.CommandText = "DELETE FROM tags WHERE parent_id=@id";
            command.Parameters.Add("@id", MySqlDbType.UInt64);
            command.Parameters["@id"].Value = parentID;
            command.ExecuteNonQuery();
            command.Parameters.Clear();

                // TODO: What to do here?!
            //logger.AddEntry(entity, Features.GetCurrentSession().user.ID);
        }

        /// <summary>
        /// Returns the tag with the given ID
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
                                         "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren, " +
                                         "IF(t.parent_id > 0, (SELECT p.label FROM tags AS p WHERE p.id = t.parent_id), '') AS parent_label " +
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
                    _logger.AddEntry(e);
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
                                         "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren, " +
                                         "IF(t.parent_id > 0, (SELECT p.label FROM tags AS p WHERE p.id = t.parent_id), '') AS parent_label " +
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
                    _logger.AddEntry(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return tags;
        }

        /// <summary>
        /// Returns every parent tag from the database (tags with parentID = 0)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tag> GetParentTags()
        {
            return this.GetChildTags(0);
        }

        /// <summary>
        /// Returns every child with the given parentID from the database
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public IEnumerable<Tag> GetChildTags(ulong parentID)
        {
            var con = new MySqlHandler().GetConnection();
            List<Tag> tags = new List<Tag>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                { 
                    ulong department = Features.GetCurrentDepartment().ID;
                    
                    string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, " +
                                         "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren, " +
                                         "IF(t.parent_id > 0, (SELECT p.label FROM tags AS p WHERE p.id = t.parent_id), '') AS parent_label " +
                                         "FROM tags AS t WHERE t.parent_id=@id " + (department > 0 ? "AND (t.department_id = @department OR t.department_id IS NULL)" : "")+
                                         "ORDER BY countChildren DESC, t.label ASC";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int64);
                        cmd.Parameters["@id"].Value = parentID;
                        
                        if (department > 0)
                        {
                            cmd.Parameters.Add("@department", MySqlDbType.UInt64);
                            cmd.Parameters["@department"].Value = department;
                        }

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
                    _logger.AddEntry(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return tags;
        }

        /// <summary>
        /// Returns a list of tags matching the search keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<Tag> Search(string keyword,List<ulong> tag_list=null, List<ulong> users=null, bool strict=false, bool searchInFields=false)
        {
            var con = new MySqlHandler().GetConnection();
            List<Tag> tags = new List<Tag>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try 
                {
                    string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, " +
                                   "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren, " +
                                   "IF(t.parent_id > 0, (SELECT p.label FROM tags AS p WHERE p.id = t.parent_id), '') AS parent_label " +
                                   "FROM tags AS t WHERE t.label LIKE @keyword";

                    if (Features.GetCurrentDepartment().ID > 0)
                        query += $" AND t.department_id={ Features.GetCurrentDepartment().ID.ToString() } OR t.department_id IS NULL";

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
                    _logger.AddEntry(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return tags;
        }

        /// <summary>
        /// Returns all tags from the database
        /// </summary>
        /// <returns></returns>
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
                                   "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren, " +
                                   "IF(t.parent_id > 0, (SELECT p.label FROM tags AS p WHERE p.id = t.parent_id), '') AS parent_label " +
                                   "FROM tags AS t";
                    
                    if (Features.GetCurrentDepartment().ID > 0)
                        query += $" WHERE t.department_id={ Features.GetCurrentDepartment().ID.ToString() } OR t.department_id IS NULL";

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
                    _logger.AddEntry(e);
                }
                finally
                {
                    con.Close();
                }
            }
                
            return tags;
        }

        /// <summary>
        /// Updates the departmentId of the children of teh given tag
        /// </summary>
        /// <param name="tag">Parent tag of the children to update</param>
        private void UpdateChildrenDepartmentId(Tag tag, MySqlCommand command)
        {
            command.Parameters.Clear();
            command.CommandText = "UPDATE tags SET department_id=@department_id WHERE parent_id=@parent_id";
            
            if (tag.DepartmentID == 0)
            {
                command.Parameters.Add("@department_id", MySqlDbType.String);
                command.Parameters["@department_id"].Value = null;
            }
            else
            {
                command.Parameters.Add("@department_id", MySqlDbType.UInt64);
                command.Parameters["@department_id"].Value = tag.DepartmentID;
            }
            
            command.Parameters.Add("@parent_id", MySqlDbType.UInt64);
            command.Parameters["@parent_id"].Value = tag.ID;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Removes all connections to a tag from the database
        /// </summary>
        /// <param name="tag">The tag that that all connections to should be removed</param>
        private void ClearConnections(Tag tag, MySqlCommand command)
        {
            command.CommandText = "DELETE FROM asset_tags WHERE tag_id=@id";
            command.Parameters.Add("@id", MySqlDbType.UInt64);
            command.Parameters["@id"].Value = tag.ID;
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }

        /// <summary>
        /// Returns a dataset, which can be used for a TreeView of the tags matching the keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<Tag> GetTreeViewDataList(string keyword="")
        {
            if (!keyword.Contains('%'))
                keyword = $"%{keyword}%";
            
            var con = new MySqlHandler().GetConnection();
            Dictionary<ulong, Tag> tags_placeholder = new Dictionary<ulong, Tag>();
            
            if (MySqlHandler.Open(ref con))
            {
                ulong department = Features.GetCurrentDepartment().ID;
                
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
                                ulong rowParentID = reader.GetUInt64("parent_id");
                                var ordinal = reader.GetOrdinal("department_id");
                                ulong rowDepartmentId = (reader.IsDBNull(ordinal) ? 0 : reader.GetUInt64("department_id"));
                                string rowColor = reader.GetString("color");
                                int rowContainsChildren = reader.GetInt32("contains_children");

                                Tag tag = (Tag) Activator.CreateInstance(typeof(Tag),
                                    BindingFlags.Instance | BindingFlags.NonPublic, null,
                                    new object[] { rowId, rowLabel, rowDepartmentId, rowParentID, rowColor, rowContainsChildren, "[]", "", null, null }, null,
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
                    _logger.AddEntry(e);
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
        /// Maps the data from the database to a tag instance
        /// </summary>
        /// <param name="reader">The reader to read the database data</param>
        /// <returns></returns>
        public Tag DataMapper(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            String rowLabel = reader.GetString("label");
            ulong rowParentID = reader.GetUInt64("parent_id");
            var ordinal = reader.GetOrdinal("department_id");
            ulong rowDepartmentId = (reader.IsDBNull(ordinal) ? 0 : reader.GetUInt64("department_id"));
            string rowColor = reader.GetString("color");
            int rowNumberOfChildren = reader.GetInt32("countChildren");
            string rowOptions = reader.GetString("options");
            string rowFullLabel = reader.GetString("parent_label");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");

            if (rowFullLabel.Length > 0)
                rowFullLabel += char.ConvertFromUtf32(0x202F) + char.ConvertFromUtf32(0x1f852) +
                                char.ConvertFromUtf32(0x202F) + rowLabel;
            else
                rowFullLabel = rowLabel;

            if (rowId == 1)
                rowFullLabel = char.ConvertFromUtf32(0x1f465) + " " + rowLabel;

            return (Tag) Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowLabel, rowDepartmentId, rowParentID, rowColor, rowNumberOfChildren, rowOptions, rowFullLabel, rowCreatedAt, rowUpdatedAt }, null,
                null);
        }
    }
}