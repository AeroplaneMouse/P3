using System;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using AMS.Models;
using AMS.Database.Repositories.Interfaces;

namespace AMS.Database.Repositories
{
    public class UserRepository : IUserRepository
    {

        public IEnumerable<User> GetAll(bool includeDisabled=false)
        {
            var con = new MySqlHandler().GetConnection();
            List<User> users = new List<User>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                // Sending sql query
                try
                {
                    string query = "SELECT id, name, description, domain, username, enabled, admin, default_department, created_at, updated_at " +
                                         "FROM users " + (!includeDisabled ? "WHERE enabled = 1" : "");

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(DataMapper(reader));
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
                
            return users;
        }

        public IEnumerable<User> GetUsersForAsset(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            List<User> users = new List<User>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT u.id, u.name, u.username, u.domain, u.description, u.enabled, u.admin, u.default_department, u.created_at, u.updated_at " +
                                         "FROM users AS u " +
                                         "INNER JOIN asset_users AS au ON au.user_id = u.id " +
                                         "WHERE au.asset_id = @id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(DataMapper(reader));
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

            return users;
        }

        public ulong GetCount()
        {
            var con = new MySqlHandler().GetConnection();
            ulong count = 0;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT COUNT(*) FROM users WHERE enabled = 1;";

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

        public User Insert(User entity, out ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;
            id = 0;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "INSERT INTO users (username, domain, description, enabled, default_department, admin, updated_at) " +
                                         "VALUES (@username, @domain, @description, @enabled, @default_department, @admin, CURRENT_TIMESTAMP())";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = entity.Username;

                        cmd.Parameters.Add("@domain", MySqlDbType.String);
                        cmd.Parameters["@domain"].Value = entity.Domain;

                        cmd.Parameters.Add("@description", MySqlDbType.String);
                        cmd.Parameters["@description"].Value = entity.Description;

                        cmd.Parameters.Add("@enabled", MySqlDbType.String);
                        cmd.Parameters["@enabled"].Value = entity.IsEnabled ? 1 : 0;

                        cmd.Parameters.Add("@default_department", MySqlDbType.UInt64);
                        cmd.Parameters["@default_department"].Value = entity.DefaultDepartment;

                        cmd.Parameters.Add("@admin", MySqlDbType.String);
                        cmd.Parameters["@admin"].Value = entity.IsAdmin ? 1 : 0;

                        querySuccess = cmd.ExecuteNonQuery() > 0;

                        id = (ulong)cmd.LastInsertedId;
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
            
            return querySuccess ? GetById(id) : null;
        }

        public bool Update(User entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "UPDATE users SET username=@username, domain=@domain, description=@description, enabled=@enabled, admin=@admin, default_department=@default_department, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = entity.ID;

                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = entity.Username;

                        cmd.Parameters.Add("@domain", MySqlDbType.String);
                        cmd.Parameters["@domain"].Value = entity.Domain;

                        cmd.Parameters.Add("@description", MySqlDbType.String);
                        cmd.Parameters["@description"].Value = entity.Description;

                        cmd.Parameters.Add("@enabled", MySqlDbType.String);
                        cmd.Parameters["@enabled"].Value = entity.IsEnabled ? 1 : 0;

                        cmd.Parameters.Add("@default_department", MySqlDbType.UInt64);
                        cmd.Parameters["@default_department"].Value = entity.DefaultDepartment;

                        cmd.Parameters.Add("@admin", MySqlDbType.String);
                        cmd.Parameters["@admin"].Value = entity.IsAdmin ? 1 : 0;

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

        public bool Delete(User entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "DELETE FROM users WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = entity.ID;

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

        public User GetById(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            User user = null;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT id, name, username, domain, description, enabled, admin, default_department, created_at, updated_at FROM users WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user = DataMapper(reader);
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

            return user;
        }

        public User GetByIdentity(string identity)
        {
            var con = new MySqlHandler().GetConnection();
            User user = null;

            var parts = identity.Split('\\');

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT id, name, domain, description, enabled, username, admin, default_department, created_at, updated_at " +
                                            "FROM users WHERE username=@username AND domain=@domain AND enabled=1";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@domain", MySqlDbType.String);
                        cmd.Parameters["@domain"].Value = parts[0];

                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = parts[1];

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user = DataMapper(reader);
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

            return user;
        }

        public User DataMapper(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            String rowUsername = reader.GetString("username");
            String rowDomain = reader.GetString("domain");
            String rowDescription = reader.GetString("description");
            bool rowEnabled = reader.GetBoolean("enabled");
            ulong rowDefaultDepartment = reader.GetUInt64("default_department");
            bool rowAdmin = reader.GetBoolean("admin");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");

            return (User) Activator.CreateInstance(typeof(User),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowUsername, rowDomain, rowDescription, rowEnabled, rowDefaultDepartment, rowAdmin, rowCreatedAt, rowUpdatedAt }, null, null);
        }
    }
}