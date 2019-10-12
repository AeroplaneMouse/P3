using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using Asset_Management_System.Helpers;
using System.Collections.ObjectModel;

namespace Asset_Management_System.Database.Repositories
{
    public class AssetRepository : IAssetRepository
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
                try{
                    const string query = "INSERT INTO assets (name, description, department_id, options) "+ 
                		                 "VALUES (@name, @description, @department, @options)";
                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;

                        cmd.Parameters.Add("@description", MySqlDbType.String);
                        cmd.Parameters["@description"].Value = entity.Description;

                        cmd.Parameters.Add("@department", MySqlDbType.UInt64);
                        cmd.Parameters["@department"].Value = entity.DepartmentID;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields;

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

        public bool Update(Asset entity)
        {
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "UPDATE assets SET name=@name, description=@description, options=@options WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;

                        cmd.Parameters.Add("@description", MySqlDbType.String);
                        cmd.Parameters["@description"].Value = entity.Description;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields;

                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = entity.ID;

                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch(MySqlException e){ 
                    Console.WriteLine(e);
                }finally{
                    dbcon.Close();
                }
            }

            return query_success;
        }

        public bool Delete(Asset entity)
        {
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "DELETE FROM assets WHERE id=@id";

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

        public Asset GetById(ulong id)
        {
            Asset asset = null;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, name, description, department_id, created_at,options FROM assets WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                asset = DBOToModelConvert(reader);
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

            return asset;
        }

        public List<Asset> SearchByTags(List<int> tags_ids)
        {
            List<Asset> assets = new List<Asset>();

            if (dbcon.IsConnect())
            {
                //"WHERE atr.tag_id IN (@ids) GROUP BY a.id";
                try
                {
                    const string query = "SELECT a.* FROM assets AS a " +
                                         "INNER JOIN asset_tags AS atr ON (a.id = atr.asset_id) " +
                                         "WHERE atr.tag_id IN (@ids) GROUP BY a.id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
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

            return assets;
        }

        public ObservableCollection<Asset> Search(string keyword)
        {
            ObservableCollection<Asset> assets = new ObservableCollection<Asset>();

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, name, description, department_id, created_at,options FROM assets WHERE name LIKE @keyword";

                    if (!keyword.Contains("%"))
                    {
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

            return assets;
        }

        public Asset DBOToModelConvert(MySqlDataReader reader)
        {
            ulong row_id = reader.GetUInt64("id");
            string row_label = reader.GetString("name");
            string row_description = reader.GetString("description");
            ulong row_department_id = reader.GetUInt64("department_id");
            DateTime row_created_at = reader.GetDateTime("created_at");
            string row_options = reader.GetString("options");
            Console.WriteLine(row_options);

            
            return (Asset) Activator.CreateInstance(typeof(Asset), BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {row_id, row_label, row_description, row_department_id, row_created_at,row_options}, null,
                null);
        }
    }
}