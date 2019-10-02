using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database.Repositories
{
    class AssetRepository : IAssetRepository
    {
        private DBConnection dbcon;

        public AssetRepository()
        {
            this.dbcon = DBConnection.Instance();
        }
        public void Delete(Asset entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Asset> GetAll()
        {
            throw new NotImplementedException();
        }

        public Asset GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Asset entity)
        {
            void Insert(Department entity)
            {
                if (dbcon.IsConnect())
                {
                    string query = "INSERT INTO assets (name) VALUES (@name)";
                    var cmd = new MySqlCommand(query, dbcon.Connection);
                    cmd.Parameters.Add("@name", MySqlDbType.String);
                    cmd.Parameters["@name"].Value = entity.Name;
                    cmd.ExecuteNonQuery();
                    dbcon.Close();
                }
            }
        }

        public List<Asset> SearchByTags(List<int> tags_ids)
        {
            if (dbcon.IsConnect())
            {
                //suppose col0 and col1 are defined as VARCHAR in the DB
                string query = "SELECT id, label, parent_id, department_id, color, options FROM tags WHERE id=@id";
                var cmd = new MySqlCommand(query, dbcon.Connection);
                cmd.Parameters.Add("@ID", MySqlDbType.Int64);
                cmd.Parameters["@ID"].Value = id;
                var reader = cmd.ExecuteReader();

                Tag tag = null;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        long row_id = reader.GetInt64("id");
                        String row_label = reader.GetString("label");
                        long row_parent_id = reader.GetInt64("parent_id");
                        long row_department_id = reader.GetInt64("department_id");

                        tag = new Tag(row_id, row_label, row_department_id, row_parent_id);
                    }
                }

                dbcon.Close();
                return tag;
            }
            else
            {
                return null;
            }
        }

        public IQueryable<Asset> SearchFor(Expression<Func<Asset, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
