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
        private QueryGenerator _query;
        
        private Ilogger logger { get; set; } = new Logger(new LogRepository());

        public AssetRepository()
        {
            _query = new QueryGenerator();
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
        /// Inserts the given asset into the database
        /// </summary>
        /// <param name="entity">The asset to be inserted</param>
        /// <param name="id"></param>
        /// <returns>Whether the insertion was successful or not</returns>
        public bool Insert(Asset entity, out ulong id)
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
                        cmd.Parameters["@department"].Value = entity.DepartmentID;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields == null ? "[]" : entity.SerializedFields;

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
                    Console.WriteLine(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return asset;
        }

        /// <summary>
        /// Searches the database for assets with one or more of the given tags
        /// </summary>
        /// <param name="tagsIds">A list of IDs of the tags</param>
        /// <returns>An ObservableCollection of assets, containing the found assets (empty if no assets were found)</returns>
        public ObservableCollection<Asset> SearchByTags(List<int> tagsIds)
        {
            var con = new MySqlHandler().GetConnection();
            ObservableCollection<Asset> assets = new ObservableCollection<Asset>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                //"WHERE atr.tag_id IN (@ids) GROUP BY a.id";
                try
                {
                    const string query = "SELECT a.* FROM assets AS a " +
                                         "INNER JOIN asset_tags AS atr ON (a.id = atr.asset_id) " +
                                         "WHERE atr.tag_id IN (@ids) AND deleted_at IS NULL GROUP BY a.id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@ids", MySqlDbType.String);
                        cmd.Parameters["@ids"].Value = string.Join(",", tagsIds);

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
                    Console.WriteLine(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return assets;
        }

        public ObservableCollection<Asset> Search(string keyword)
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
        /// </summary>
        /// <param name="keyword">The search query to search by</param>
        /// <returns>An ObservableCollection of assets, containing the found assets (empty if no assets were found)</returns>
        public ObservableCollection<Asset> Search(string keyword, List<ulong> tags=null, List<ulong> users=null, bool strict=false)
        {
            var con = new MySqlHandler().GetConnection();
            ObservableCollection<Asset> assets = new ObservableCollection<Asset>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                _query.Reset();
                _query.AddTable("assets AS a");
                _query.Columns.AddRange(new[] { "a.id", "a.name", "a.description", "a.identifier", "a.department_id", "a.options", "a.created_at", "a.updated_at" });

                try
                {
                    if (Features.Main.CurrentDepartment.ID > 0)
                        _query.Where("a.department_id", Features.Main.CurrentDepartment.ID.ToString());
                    _query.Where("a.deleted_at", "IS NULL", "");

                    if (keyword.Length > 0)
                    {
                        if (!keyword.StartsWith("%") && !keyword.EndsWith("%"))
                            keyword = "%" + keyword + "%";

                        Statement statement = new Statement();
                        statement.AddOrStatement("a.name", keyword, "LIKE");
                        statement.AddOrStatement("a.identifier", keyword, "LIKE");
                        statement.AddOrStatement("JSON_EXTRACT(a.options, '$[*].Content')", keyword, "LIKE");
                        _query.Where(statement);
                    }

                    if (tags != null && tags.Count > 0)
                    {
                        var pivot = new Table("asset_tags AS at", "INNER JOIN");
                        pivot.AddConnection("at.asset_id", "a.id");
                        _query.Tables.Add(pivot);
                        _query.Where("at.tag_id", "(" + string.Join(",", tags) + ")", "IN");

                        if (strict)
                            _query.HavingStatements.Add(new Statement("COUNT(DISTINCT at.tag_id)", tags.Count.ToString()));
                    }

                    if (users != null && users.Count > 0)
                    {
                        var pivot = new Table("asset_users AS au", "INNER JOIN");
                        pivot.AddConnection("au.asset_id", "a.id");
                        _query.Tables.Add(pivot);
                        _query.Where("au.user_id", "(" + string.Join(",", users) + ")", "IN");

                        if (strict)
                            _query.HavingStatements.Add(new Statement("COUNT(DISTINCT au.user_id)", users.Count.ToString()));
                    }

                    _query.GroupBy = "a.id";

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
                    Console.WriteLine(e);
                }
                finally
                {
                    con.Close();
                }
            }

            return assets;
        }

        public bool AttachTags(Asset asset, List<ITagable> tagged)
        {
            if (!tagged.Any())
            {
                return true;
            }

            List<User> users = tagged.OfType<User>().ToList();
            List<Tag> tags = tagged.OfType<Tag>().ToList();

            ClearTags(asset);
            
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    string tagLabels = "\"" + String.Join("\", \"", tags);

                    StringBuilder userQuery = new StringBuilder("INSERT INTO asset_users VALUES ");
                    int counter = users.Count;

                    for (int i = 0; i < counter; i++)
                    {
                        userQuery.AppendFormat("({0},{1})", asset.ID, users[i].ID);

                        if (i != counter - 1)
                            userQuery.Append(",");
                    }

                    StringBuilder tagQuery = new StringBuilder("INSERT INTO asset_tags VALUES ");
                    counter = tags.Count;

                    for (int i = 0; i < counter; i++)
                    {
                        tagQuery.AppendFormat("({0},{1})", asset.ID, tags[i].ID);

                        if (i != counter - 1)
                            tagQuery.Append(",");
                    }

                    using (var cmd = new MySqlCommand(userQuery.ToString(), con))
                    {
                        querySuccess = cmd.ExecuteNonQuery() > 0;
                    }

                    using (var cmd = new MySqlCommand(tagQuery.ToString(), con))
                    {
                        querySuccess = cmd.ExecuteNonQuery() > 0 && querySuccess;
                    }

                    logger.AddEntry("Tag attached", tagLabels + " was attached to the asset with ID: "
                        + asset.ID + " and name: " + asset.Name + ". Other tags have been removed.", Features.GetCurrentSession().user.ID);

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

        public IEnumerable<ITagable> GetTags(Asset asset)
        {
            // TODO: Ingen new repositories!
            var taggedWith = new List<ITagable>();
            var tagRepository = new TagRepository();
            var userRepository = new UserRepository();
            
            taggedWith.AddRange(tagRepository.GetTagsForAsset(asset.ID));
            taggedWith.AddRange(userRepository.GetUsersForAsset(asset.ID));

            return taggedWith;
        }

        private bool ClearTags(Asset asset)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "DELETE FROM asset_tags WHERE asset_id = @id; " +
                                         "DELETE FROM asset_users WHERE asset_id = @id;";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = asset.ID;

                        querySuccess = cmd.ExecuteNonQuery() > 0;
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

            return querySuccess;
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
            string rowOptions = Encoding.Unicode.GetString(                
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.Unicode,
                    Encoding.UTF8.GetBytes(reader.GetString("options"))));
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");
            
            return (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { rowId, rowName, rowDescription, rowIdentifier, rowDepartmentId, rowOptions, rowCreatedAt, rowUpdatedAt }, null, null);
        }
    }
}