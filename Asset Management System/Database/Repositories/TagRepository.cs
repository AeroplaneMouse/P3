using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Asset_Management_System.Database.Repositories
{
    public class TagRepository : ITagRepository
    {
        public int GetCount()
        {
            DBConnection dbcon = DBConnection.Instance();
            int count = 0;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "SELECT COUNT(*) FROM tags;";
                    using var cmd = new MySqlCommand(query, dbcon.Connection);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                        count = reader.GetInt32("COUNT(*)");
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    dbcon.Close();
                }
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Insert(Tag entity, out ulong id)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            id = 0;

            entity.SerializeFields();

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "INSERT INTO tags (label, color, options, department_id, parent_id, updated_at) " +
                                         "VALUES (@label, @color, @options, @department_id, @parent_id, CURRENT_TIMESTAMP())";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
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
                    dbcon.Close();
                }
            }

            return query_success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(Tag entity)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            entity.SerializeFields();

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "UPDATE tags SET label=@label, color=@color, options=@options, parent_id=@parent_id, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
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

                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    dbcon.Close();
                }
            }

            return query_success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(Tag entity)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "DELETE FROM tags WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = entity.ID;

                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    dbcon.Close();
                }
            }

            return query_success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Tag GetById(ulong id)
        {
            DBConnection dbcon = DBConnection.Instance();
            Tag tag = null;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, label, parent_id, department_id, color, options, created_at, updated_at FROM tags WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@ID", MySqlDbType.Int64);
                        cmd.Parameters["@ID"].Value = id;

                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tag = DBOToModelConvert(reader);
                        }
                    }
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    dbcon.Close();
                }
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
        /// <param name="parent_id"></param>
        /// <returns></returns>
        public IEnumerable<Tag> GetChildTags(ulong parent_id)
        {
            DBConnection dbcon = DBConnection.Instance();
            List<Tag> tags = new List<Tag>();

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, label, parent_id, department_id, color, options, created_at, updated_at FROM tags WHERE parent_id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int64);
                        cmd.Parameters["@id"].Value = parent_id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(DBOToModelConvert(reader));
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
                    dbcon.Close();
                }
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
            DBConnection dbcon = DBConnection.Instance();
            ObservableCollection<Tag> tags = new ObservableCollection<Tag>();

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, label, parent_id, department_id, color, options, created_at, updated_at FROM tags WHERE label LIKE @keyword";

                    if (!keyword.Contains('%'))
                        keyword = $"%{keyword}%";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@keyword", MySqlDbType.String);
                        cmd.Parameters["@keyword"].Value = keyword;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(DBOToModelConvert(reader));
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
                    dbcon.Close();
                }
            }

            return tags;
        }

        public IEnumerable<Tag> GetAll()
        {
            DBConnection dbcon = DBConnection.Instance();
            List<Tag> tags = new List<Tag>();

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "SELECT id, label, parent_id, department_id, color, options, created_at, updated_at,options FROM tags";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(DBOToModelConvert(reader));
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
                    dbcon.Close();
                }
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
            ulong row_id = reader.GetUInt64("id");
            String row_label = reader.GetString("label");
            ulong row_parent_id = reader.GetUInt64("parent_id");
            ulong row_department_id = reader.GetUInt64("department_id");
            string row_color = reader.GetString("color");
            DateTime row_created_at = reader.GetDateTime("created_at");
            DateTime row_updated_at = reader.GetDateTime("updated_at");
            string row_options = reader.GetString("options");

            return (Tag) Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {row_id, row_label, row_department_id, row_parent_id, row_color, row_created_at, row_updated_at,row_options}, null,
                null);
        }
    }
}