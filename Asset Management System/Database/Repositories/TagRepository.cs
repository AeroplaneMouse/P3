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
    class TagRepository : ITagRepository
    {
        private DBConnection dbcon;

        public TagRepository()
        {
            this.dbcon = DBConnection.Instance();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Insert(Tag entity)
        {
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "INSERT INTO tags (label, color, options, department_id, parent_id) " +
                                         "VALUES (@label, @color, @options, @department_id, @parent_id)";

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
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "UPDATE tags SET label=@label, color=@color, options=@options, parent_id=@parent_id WHERE id=@id";

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
            Tag tag = null;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, label, parent_id, department_id, color, options, created_at FROM tags WHERE id=@id";

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
        public List<Tag> GetParentTags()
        {
            return this.GetChildTags(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent_id"></param>
        /// <returns></returns>
        public List<Tag> GetChildTags(ulong parent_id)
        {
            List<Tag> tags = new List<Tag>();

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, label, parent_id, department_id, color, options, created_at FROM tags WHERE parent_id=@id";

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
            ObservableCollection<Tag> tags = new ObservableCollection<Tag>();

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, label, parent_id, department_id, color, options, created_at FROM tags WHERE label LIKE @keyword";

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

            return (Tag) Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {row_id, row_label, row_department_id, row_parent_id, row_color, row_created_at}, null,
                null);
        }
    }
}