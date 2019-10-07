﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace Asset_Management_System.Database.Repositories
{
    class AssetRepository : IAssetRepository
    {
        private DBConnection dbcon;

        public AssetRepository()
        {
            this.dbcon = DBConnection.Instance();
        }

        public bool Insert(Asset entity)
        {
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                string query = "INSERT INTO assets (name, description, department_id) VALUES (@name, @description, @department)";

                using (var cmd = new MySqlCommand(query, dbcon.Connection))
                {
                    cmd.Parameters.Add("@name", MySqlDbType.String);
                    cmd.Parameters["@name"].Value = entity.Label;

                    cmd.Parameters.Add("@description", MySqlDbType.String);
                    cmd.Parameters["@description"].Value = entity.Description;

                    cmd.Parameters.Add("@department", MySqlDbType.UInt64);
                    cmd.Parameters["@department"].Value = 1;

                    query_success = cmd.ExecuteNonQuery() > 0 ? true : false;
                }

                dbcon.Close();
            }

            return query_success;
        }

        public bool Update(Asset entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Asset entity)
        {
            throw new NotImplementedException();
        }

        public Asset GetById(long id)
        {
            Asset asset = null;

            if (dbcon.IsConnect())
            {
                string query = "SELECT id, name, description FROM assets WHERE id=@id";

                using (var cmd = new MySqlCommand(query, dbcon.Connection))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int64);
                    cmd.Parameters["@id"].Value = id;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            asset = DBOToModelConvert(reader);
                        }
                    }

                    dbcon.Close();
                }
            }

            return asset;
        }

        public List<Asset> SearchByTags(List<int> tags_ids)
        {
            List<Asset> assets = new List<Asset>();

            if (dbcon.IsConnect())
            {
                string query = "SELECT a.* FROM assets AS a " +
                    "INNER JOIN asset_tags AS atr ON (a.id = atr.asset_id) " +
                    "WHERE atr.tag_id IN (@ids) GROUP BY a.id";

                using (var cmd = new MySqlCommand(query, dbcon.Connection)){
                    cmd.Parameters.Add("@ids", MySqlDbType.String);
                    cmd.Parameters["@ids"].Value = string.Join(",", tags_ids);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Asset asset = DBOToModelConvert(reader);
                            assets.Add(asset);
                        }
                    }

                    dbcon.Close();
                }
            }

            return assets;
        }

        public List<Asset> Search(string keyword)
        {
            List<Asset> assets = new List<Asset>();

            if (dbcon.IsConnect())
            {
                string query = "SELECT id, name, description, department_id FROM assets WHERE name LIKE @keyword";

                if(!keyword.Contains("%")){
                    keyword = "%" + keyword + "%";
                }

                using (var cmd = new MySqlCommand(query, dbcon.Connection))
                {
                    cmd.Parameters.Add("@keyword", MySqlDbType.String);
                    cmd.Parameters["@keyword"].Value = keyword;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Asset asset = DBOToModelConvert(reader);
                            assets.Add(asset);
                        }
                    }

                    dbcon.Close();
                }
            }

            return assets;
        }

        public Asset DBOToModelConvert(MySqlDataReader reader)
        {
            long row_id = reader.GetInt64("id");
            string row_label = reader.GetString("name");
            string row_description = reader.GetString("description");
            long row_department_id = reader.GetInt64("department_id");

            return (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { row_id, row_label, row_description, row_department_id }, null, null);
        }
    }
}
