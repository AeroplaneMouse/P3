using System;
using System.Collections.Generic;
using AMS.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.ObjectModel;
using AMS.Database.Repositories.Interfaces;
using AMS.ViewModels;
using AMS.Logging.Interfaces;

namespace AMS.Database.Repositories
{
    public class TagRepository : ITagRepository
    {
        public ILogger logger { get; set; }

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
                        cmd.Parameters["@color"].Value = entity.TagColor;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields;

                        cmd.Parameters.Add("@department_id", MySqlDbType.UInt64);
                        cmd.Parameters["@department_id"].Value = entity.DepartmentID;

                        cmd.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                        cmd.Parameters["@parent_id"].Value = entity.ParentID;

                        querySuccess = cmd.ExecuteNonQuery() > 0;

                        id = (ulong)cmd.LastInsertedId;
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
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(Tag entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con) && entity.IsDirty())
            {
                try
                {
                    const string query = "UPDATE tags SET label=@label, color=@color, options=@options, parent_id=@parent_id, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@label", MySqlDbType.String);
                        cmd.Parameters["@label"].Value = entity;

                        cmd.Parameters.Add("@color", MySqlDbType.String);
                        cmd.Parameters["@color"].Value = entity.TagColor;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields;

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
        public ObservableCollection<Tag> Search(string keyword,List<ulong> tag_list=null, List<ulong> users=null, bool strict=false)
        {
            var con = new MySqlHandler().GetConnection();
            ObservableCollection<Tag> tags = new ObservableCollection<Tag>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try 
                {
                    string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, " +
                                     "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren " +
                                     "FROM tags AS t WHERE t.label LIKE @keyword";

                    if (Features.Main.CurrentDepartment.ID > 0)
                        query += $" AND t.department_id={ Features.Main.CurrentDepartment.ID.ToString() } OR t.department_id=0";

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
                    const string query = "SELECT t.id, t.label, t.parent_id, t.department_id, t.color, t.options, t.created_at, t.updated_at, t.options, " +
                                     "(SELECT COUNT(ct.id) FROM tags AS ct WHERE t.id = ct.parent_id) AS countChildren " +
                                     "FROM tags AS t";

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
            ulong rowDepartmentId = reader.GetUInt64("department_id");
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