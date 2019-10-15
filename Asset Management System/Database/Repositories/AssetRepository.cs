using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using Asset_Management_System.Helpers;

namespace Asset_Management_System.Database.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        public bool Insert(Asset entity)
        {
            DBConnection dbcon = DBConnection.Instance();
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
            DBConnection dbcon = DBConnection.Instance();
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
            DBConnection dbcon = DBConnection.Instance();
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
            DBConnection dbcon = DBConnection.Instance();
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
            DBConnection dbcon = DBConnection.Instance();
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

        public List<Asset> Search(string keyword)
        {
            DBConnection dbcon = DBConnection.Instance();
            List<Asset> assets = new List<Asset>();

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


            return (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { row_id, row_label, row_description, row_department_id, row_created_at, row_options }, null, null);
        }
        
        
        /// <summary>
        /// Adds a list of tags to an asset
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="tag_list"></param>
        /// <returns></returns>
        public bool AttachTagsToAsset(Asset asset, List<Tag> tags)
        {
            return RemoveTags(asset, tags) && AddTags(asset, tags);
        }

        /// <summary>
        /// Removes the tags on the asset that are not in teh list of tags to be added
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private bool RemoveTags(Asset asset, List<Tag> tags)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            // Makes a list of the ids of the tags to be added to the asset
            List<ulong> tag_ids = tags.Select(p => p.ID).ToList();
            // Makes a list of the ids of the tags already on the asset
            List<ulong> asset_tag_ids = GetAssetTags(asset).Select(p => p.ID).ToList();

            // Removes the ids of the tags that are supposed to stilll be on the asset
            // resulting in a list of ids og tags to be removed from the asset
            asset_tag_ids = asset_tag_ids.Except(tag_ids).ToList();


            StringBuilder query = new StringBuilder("DELETE FROM asset_tags WHERE asset_id = ");
            List<string> inserts = new List<string>();

            query.Append(asset.ID);
            query.Append(" AND tag_id IN (");

            foreach (var tag_id in asset_tag_ids)
            {
                inserts.Add(tag_id.ToString());
            }

            query.Append(string.Join(",", inserts));
            query.Append(")");

            try
            {
                if (dbcon.IsConnect() && asset_tag_ids.Count > 0)
                {
                    using (var cmd = new MySqlCommand(query.ToString(), dbcon.Connection))
                    {
                        Console.WriteLine(cmd.CommandText);
                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                dbcon.Close();
            }

            return query_success;
        }
       
        /// <summary>
        /// Adds the tags that are not already on the asset
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private bool AddTags(Asset asset, List<Tag> tags)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            // Makes a list of the ids of the tags to be added to the asset
            List<ulong> tag_ids = tags.Select(p => p.ID).ToList();
            // Makes a list of the ids of the tags already on the asset
            List<ulong> asset_tag_ids = GetAssetTags(asset).Select(p => p.ID).ToList();

            // Removes the ids of the tags that are already on the asset
            // resulting in a list of ids of tags to still to be added to the asset
            tag_ids = tag_ids.Except(asset_tag_ids).ToList();

            StringBuilder query = new StringBuilder("INSERT INTO asset_tags (asset_id, tag_id) VALUES ");
            List<string> inserts = new List<string>();

            foreach (var tag_id in tag_ids)
            {
                inserts.Add(string.Format("({0},{1})", asset.ID, tag_id));
            }

            query.Append(string.Join(",", inserts));

            try
            {
                if (dbcon.IsConnect() && tag_ids.Count > 0)
                {
                    using (var cmd = new MySqlCommand(query.ToString(), dbcon.Connection))
                    {
                        Console.WriteLine(cmd.CommandText);
                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                dbcon.Close();
            }

            return query_success;
        }

        #region Alternative methods for adding tags to fields
        //private bool RemoveTags2(Asset asset)
        //{
        //    DBConnection dbcon = DBConnection.Instance();
        //    bool query_success = false;

        //    List<ulong> asset_tag_ids = GetAssetTags(asset).Select(p => p.ID).ToList();

        //    StringBuilder query = new StringBuilder("DELETE FROM asset_tags WHERE asset_id = ");
        //    List<string> inserts = new List<string>();

        //    query.Append(asset.ID);
        //    query.Append(" AND tag_id IN (");

        //    foreach (var tag_id in asset_tag_ids)
        //    {
        //        inserts.Add(tag_id.ToString());
        //    }

        //    query.Append(string.Join(",", inserts));
        //    query.Append(")");

        //    try
        //    {
        //        if (dbcon.IsConnect() && asset_tag_ids.Count > 0)
        //        {
        //            using (var cmd = new MySqlCommand(query.ToString(), dbcon.Connection))
        //            {
        //                Console.WriteLine(cmd.CommandText);
        //                query_success = cmd.ExecuteNonQuery() > 0;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    finally
        //    {
        //        dbcon.Close();
        //    }

        //    return query_success;
        //}

        //private bool AddTags2 (Asset asset, List<Tag> tags)
        //{
        //    DBConnection dbcon = DBConnection.Instance();
        //    bool query_success = false;

        //    List<ulong> tag_ids = tags.Select(p => p.ID).ToList();

        //    StringBuilder query = new StringBuilder("INSERT INTO asset_tags (asset_id, tag_id) VALUES ");
        //    List<string> inserts = new List<string>();

        //    foreach (var tag_id in tag_ids)
        //    {
        //        inserts.Add(string.Format("({0},{1})", asset.ID, tag_id));
        //    }

        //    query.Append(string.Join(",", inserts));

        //    try
        //    {
        //        if (dbcon.IsConnect() && tag_ids.Count > 0)
        //        {
        //            using (var cmd = new MySqlCommand(query.ToString(), dbcon.Connection))
        //            {
        //                Console.WriteLine(cmd.CommandText);
        //                query_success = cmd.ExecuteNonQuery() > 0;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //    finally
        //    {
        //        dbcon.Close();
        //    }

        //    return query_success;
        //}
        #endregion

        /// <summary>
        /// Gets a list of all tags on an asset
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public List<Tag> GetAssetTags(Asset asset)
        {
            DBConnection dbcon = DBConnection.Instance();
            List<Tag> tags = new List<Tag>();

            TagRepository tag_rep = new TagRepository();

            if (dbcon.IsConnect())
            {
                try
                {
                    string query = "SELECT * FROM tags AS t " +
                                   "INNER JOIN asset_tags ON tag_id = t.id " +
                                   "WHERE asset_id = @asset_id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@asset_id", MySqlDbType.UInt64);
                        cmd.Parameters["@asset_id"].Value = asset.ID;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(tag_rep.DBOToModelConvert(reader));
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
    }
}