﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace Asset_Management_System.Database.Repositories
{
    class TagRepository : ITagRepository
    {
        private DBConnection dbcon;

        public TagRepository(){ 
            this.dbcon = DBConnection.Instance();
        }

        public bool Insert(Tag entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(Tag entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Tag entity)
        {
            throw new NotImplementedException();
        }

        public Tag GetById(long id)
        {
            Tag tag = null;

            if (dbcon.IsConnect())
            {
                string query = "SELECT id, label, parent_id, department_id, color, options FROM tags WHERE id=@id";

                using (var cmd = new MySqlCommand(query, dbcon.Connection))
                {
                    cmd.Parameters.Add("@ID", MySqlDbType.Int64);
                    cmd.Parameters["@ID"].Value = id;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tag = DBOToModelConvert(reader);
                        }
                    }
                }

                dbcon.Close();
            }

            return tag;
        }

        public List<Tag> GetChildTags(long parent_id)
        {
            throw new NotImplementedException();
        }

        public Department GetDepartment()
        {
            throw new NotImplementedException();
        }

        public Tag GetParentTag()
        {
            throw new NotImplementedException();
        }

        public List<Tag> Search(string keyword)
        {
            List<Tag> tags = new List<Tag>();

            if (dbcon.IsConnect())
            {
                string query = "SELECT id, label, parent_id, department_id, color, options FROM tags WHERE label LIKE @keyword";

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
                            Tag tag = DBOToModelConvert(reader);
                            tags.Add(tag);
                        }
                    }

                    dbcon.Close();
                }
            }

            return tags;
        }

        public Tag DBOToModelConvert(MySqlDataReader reader)
        {
            long row_id = reader.GetInt64("id");
            String row_label = reader.GetString("label");
            long row_parent_id = reader.GetInt64("parent_id");
            long row_department_id = reader.GetInt64("department_id");

            return (Tag)Activator.CreateInstance(typeof(Tag), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { row_id, row_label, row_department_id, row_parent_id }, null, null);
        }
    }
}
