using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.ObjectModel;

using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging.Interfaces;
using AMS.ViewModels;
using AMS.Logging;

namespace AMS.Database.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private QueryGenerator _query { get; set; }
        private Ilogger _logger { get; set; }

        public AssetRepository()
        {
            _query = new QueryGenerator();
            _logger = new Logger(Features.LogRepository);
            
        }

        /// <summary>
        /// Returns the number of assets in the database
        /// </summary>
        /// <returns>Number of assets in database</returns>
        public ulong GetCount()
        {
            var con = new MySqlHandler().GetConnection();
            ulong count = 0;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT COUNT(*) FROM assets WHERE deleted_at IS NULL";

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
        /// Inserts the given asset into the database
        /// </summary>
        /// <param name="entity">The asset to be inserted</param>
        /// <param name="id"></param>
        /// <returns>Whether the insertion was successful or not</returns>
        public Asset Insert(Asset entity, out ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;
            id = 0;
            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "INSERT INTO assets (name, identifier, description, department_id, options, updated_at) " +
                                         "VALUES (@name, @identifier, @description, @department, @options, CURRENT_TIMESTAMP())";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;

                        cmd.Parameters.Add("@description", MySqlDbType.String);
                        cmd.Parameters["@description"].Value = entity.Description == null ? "" : entity.Description;

                        cmd.Parameters.Add("@identifier", MySqlDbType.String);
                        cmd.Parameters["@identifier"].Value = entity.Identifier == null ? "" : entity.Identifier;

                        cmd.Parameters.Add("@department", MySqlDbType.UInt64);
                        cmd.Parameters["@department"].Value = entity.DepartmentdId;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields == null ? "[]" : entity.SerializedFields;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
                        id = (ulong)cmd.LastInsertedId;
                    }

                    _logger.AddEntry(entity, Features.GetCurrentSession().user.ID, id);
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
        /// Updates the given asset in the database
        /// </summary>
        /// <param name="entity">The locally updated asset</param>
        /// <returns>Rather the update was successful or not</returns>
        public bool Update(Asset entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con) && entity.IsDirty())
            {
                try
                {
                    const string query = "UPDATE assets SET name=@name, identifier=@identifier, description=@description, options=@options, updated_at=CURRENT_TIMESTAMP() " +
                                         "WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;

                        cmd.Parameters.Add("@description", MySqlDbType.String);
                        cmd.Parameters["@description"].Value = entity.Description;

                        cmd.Parameters.Add("@identifier", MySqlDbType.String);
                        cmd.Parameters["@identifier"].Value = entity.Identifier;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields;

                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = entity.ID;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
                    }
                    
                    _logger.AddEntry(entity, Features.GetCurrentSession().user.ID);
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

            return querySuccess;
        }

        /// <summary>
        /// Remvoes the given asset from the database
        /// </summary>
        /// <param name="entity">The asset to be deleted</param>
        /// <returns>Rather the deletion was successful or not</returns>
        public bool Delete(Asset entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "UPDATE assets SET deleted_at=CURRENT_TIMESTAMP() WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = entity.ID;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
                    }
                    
                    _logger.AddEntry(entity, Features.GetCurrentSession().user.ID);
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

            return querySuccess;
        }

        /// <summary>
        /// Fetches the asset with the given id from the database
        /// </summary>
        /// <param name="id">ID of the desired asset</param>
        /// <returns>The asset or null, if the asset was not found in the database</returns>
        public Asset GetById(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            Asset asset = null;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT id, name, description, identifier, department_id, options, created_at, updated_at " +
                                         "FROM assets WHERE id=@id AND deleted_at IS NULL";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                asset = DataMapper(reader);
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

            return asset;
        }

        public List<Asset> Search(string keyword)
        {
            List<ulong> tags = new List<ulong>();
            tags.Add(9);
            return Search(keyword, tags);
        }

        /// <summary>
        /// Searches for assets in the database based on the keyword.
        /// If the keyword starts or ends with '%', they will be used in the query.
        /// A '%' at the start of the keyword will search for entries ending on the rest of the keyword.
        /// A '%' at the end of the keyword will search for entries starting with the rest of the keyword.
        /// If no '%' is added, the search will return every entry containing the keyword (slower).
        /// Using own query generator.
        /// </summary>
        /// <param name="keyword">The search query to search by</param>
        /// <param name="tags">List of tag ids</param>
        /// <param name="users">List of user ids</param>
        /// <param name="strict">Enable strict search</param>
        /// <returns>An ObservableCollection of assets, containing the found assets (empty if no assets were found)</returns>
        public List<Asset> Search(string keyword, List<ulong> tags=null, List<ulong> users=null, bool strict=false, bool searchInFields=false)
        {
            var con = new MySqlHandler().GetConnection();
            List<Asset> assets = new List<Asset>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                // Clear query and add primary table and columns to fetch.
                _query.Reset();
                _query.AddTable("assets AS a");
                _query.Columns.AddRange(new[] { "a.id", "a.name", "a.description", "a.identifier", "a.department_id", "a.options", "a.created_at", "a.updated_at" });

                try
                {
                    if (Features.GetCurrentDepartment().ID > 0)
                        _query.Where("a.department_id", Features.GetCurrentDepartment().ID.ToString());
                    
                    // Do not include deleted items
                    _query.Where("a.deleted_at", "IS NULL", "");

                    // Prepare keyword for search
                    if (keyword.Length > 0 && !keyword.Equals("%"))
                    {
                        if (!keyword.StartsWith("%") && !keyword.EndsWith("%"))
                            keyword = "%" + keyword + "%";
                        
                        // Make free text search on id, name and identifier
                        Statement statement = new Statement();
                        if(int.TryParse(keyword.Replace("%", ""), out int assetIdKeyword))
                            statement.AddOrStatement("a.id", assetIdKeyword.ToString());
                        statement.AddOrStatement("a.name", keyword, "LIKE");
                        statement.AddOrStatement("a.identifier", keyword, "LIKE");
                        
                        // If search in fields include JSON search in query
                        if (searchInFields)
                            statement.AddOrStatement("JSON_EXTRACT(a.options, '$[*].Content')", keyword, "LIKE");
                        
                        _query.Where(statement);
                    }
                    
                    // If current search include tags to base the search on add query
                    if (tags != null && tags.Count > 0)
                    {
                        var pivot = new Table("asset_tags AS at", "INNER JOIN");
                        pivot.AddConnection("at.asset_id", "a.id");
                        _query.Tables.Add(pivot);
                        _query.Where("at.tag_id", "(" + string.Join(",", tags) + ")", "IN");
                        
                        // If strict all applied tags are required on the asset
                        if (strict)
                            _query.HavingStatements.Add(new Statement("COUNT(DISTINCT at.tag_id)", tags.Count.ToString()));
                    }
                    
                    // If current search include users to base the search on add query
                    if (users != null && users.Count > 0)
                    {
                        var pivot = new Table("asset_users AS au", "INNER JOIN");
                        pivot.AddConnection("au.asset_id", "a.id");
                        _query.Tables.Add(pivot);
                        _query.Where("au.user_id", "(" + string.Join(",", users) + ")", "IN");
                        
                        // If strict all applied users are required on the asset
                        if (strict)
                            _query.HavingStatements.Add(new Statement("COUNT(DISTINCT au.user_id)", users.Count.ToString()));
                    }
                
                    _query.OrderBy("a.id", false);
                    _query.GroupBy = "a.id";
                    _query.Limit = 1000;
                   
                    using (var cmd = new MySqlCommand(_query.PrepareSelect(), con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                assets.Add(DataMapper(reader));
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

            return assets;
        }

        /// <summary>
        /// Attaches a list of tags to the given asset
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="listOfTags">The list of tags to attach to the given asset</param>
        /// <returns></returns>
        public bool AttachTags(Asset asset, List<ITagable> listOfTags)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;
            
            List<User> users = listOfTags.OfType<User>().ToList();
            List<Tag> tags = listOfTags.OfType<Tag>().ToList();
            int userCounter = users.Count;
            int tagCounter = tags.Count;
            
            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                MySqlCommand command = con.CreateCommand();
                MySqlTransaction transaction = con.BeginTransaction();
                command.Transaction = transaction;
                command.Connection = con;
                
                try
                {
                    ClearTags(asset, command);
                    
                    string tagLabels = "\"" + String.Join("\", \"", tags);

                    StringBuilder userQuery = new StringBuilder("INSERT INTO asset_users (asset_id, user_id) VALUES ");

                    for (int i = 0; i < userCounter; i++)
                    {
                        userQuery.AppendFormat("({0},{1})", asset.ID, users[i].ID);

                        if (i != userCounter - 1)
                            userQuery.Append(",");
                    }

                    StringBuilder tagQuery = new StringBuilder("INSERT INTO asset_tags (asset_id, tag_id) VALUES ");

                    for (int i = 0; i < tagCounter; i++)
                    {
                        tagQuery.AppendFormat("({0},{1})", asset.ID, tags[i].ID);

                        if (i != tagCounter - 1)
                            tagQuery.Append(",");
                    }
                    
                    if (users.Count > 0)
                    {
                        command.CommandText = userQuery.ToString();
                        command.ExecuteNonQuery();
                    }
                    
                    if (tags.Count > 0)
                    {
                        command.CommandText = tagQuery.ToString();
                        command.ExecuteNonQuery();
                    }
                    
                    _logger.AddEntry("Tag attached", tagLabels + " was attached to the asset with ID: "
                        + asset.ID + " and name: " + asset.Name + ". Other tags have been removed.", Features.GetCurrentSession().user.ID);
                    
                    transaction.Commit();
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

        /// <summary>
        /// Returns the tags attached to the given asset
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public IEnumerable<ITagable> GetTags(Asset asset)
        {
            var taggedWith = new List<ITagable>();
            
            taggedWith.AddRange(Features.TagRepository.GetTagsForAsset(asset.ID));
            taggedWith.AddRange(Features.UserRepository.GetUsersForAsset(asset.ID));

            return taggedWith;
        }

        /// <summary>
        /// Removes all tags from the given asset
        /// </summary>
        /// <param name="asset"></param>
        private void ClearTags(Asset asset, MySqlCommand command)
        {
            command.CommandText = "DELETE FROM asset_tags WHERE asset_id = @id";
            command.Parameters.Add("@id", MySqlDbType.UInt64);
            command.Parameters["@id"].Value = asset.ID;
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            
            command.CommandText = "DELETE FROM asset_users WHERE asset_id = @id";
            command.Parameters.Add("@id", MySqlDbType.UInt64);
            command.Parameters["@id"].Value = asset.ID;
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }

        /// <summary>
        /// Convert a database entry of an asset to an asset in the system
        /// </summary>
        /// <param name="reader">A MySQLDataReader containing the data for the asset</param>
        /// <returns>The asset made from the given data</returns>
        public Asset DataMapper(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            string rowName = reader.GetString("name");
            string rowDescription = reader.GetString("description");
            string rowIdentifier = reader.GetString("identifier");
            ulong rowDepartmentId = reader.GetUInt64("department_id");
            string rowOptions = reader.GetString("options");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");
            
            return (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { rowId, rowName, rowDescription, rowIdentifier, rowDepartmentId, rowOptions, rowCreatedAt, rowUpdatedAt }, null, null);
        }
    }
}