using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.Generic;

namespace Asset_Management_System.Database.Repositories
{
    public class UserRepository : IUserRepository
    {

        public IEnumerable<User> GetAll()
        {
            var con = new MySqlHandler().GetConnection();
            List<User> users = new List<User>();

            try
            {
                const string query = "SELECT id, name, description, domain, username, enabled, admin, default_department, created_at, updated_at " +
                                     "FROM users";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(DBOToModelConvert(reader));
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
                
            return users;
        }

        public IEnumerable<User> GetUsersForAsset(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            List<User> users = new List<User>();

            try
            {
                const string query = "SELECT u.id, u.name, u.username, u.domain, u.description, u.enabled, u.admin, u.default_department, u.created_at, u.updated_at " +
                                     "FROM users AS u " +
                                     "INNER JOIN asset_users AS au ON au.user_id = u.id " +
                                     "WHERE au.asset_id = @id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
   
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(DBOToModelConvert(reader));
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

            return users;
        }

        public ulong GetCount()
        {
            var con = new MySqlHandler().GetConnection();
            ulong count = 0;
            
            try
            {
                const string query = "SELECT COUNT(*) FROM users;";
                
                con.Open();
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
          
            return count;
        }

        public bool Insert(User entity, out ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            id = 0;

            try
            {
                const string query = "INSERT INTO users (name, username, description, enabled, default_department, admin, updated_at) " +
                                     "VALUES (@name, @username, @description, @enabled, @default_department, @admin, CURRENT_TIMESTAMP())";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@name", MySqlDbType.String);
                    cmd.Parameters["@name"].Value = entity.Name;

                    cmd.Parameters.Add("@username", MySqlDbType.String);
                    cmd.Parameters["@username"].Value = entity.Username;

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
            
            return querySuccess;
        }

        public bool Update(User entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            try
            {
                const string query = "UPDATE users SET name=@name, username=@username, description=@description, enabled=@enabled, admin=@admin, default_department=@default_department, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                    cmd.Parameters["@id"].Value = entity.ID;

                    cmd.Parameters.Add("@name", MySqlDbType.String);
                    cmd.Parameters["@name"].Value = entity.Name;

                    cmd.Parameters.Add("@username", MySqlDbType.String);
                    cmd.Parameters["@username"].Value = entity.Username;

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
            
            return querySuccess;
        }

        public bool Delete(User entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            try
            {
                const string query = "DELETE FROM users WHERE id=@id";

                con.Open();
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

            return querySuccess;
        }

        public User GetById(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            User user = null;

            try
            {
                const string query = "SELECT id, name, username, domain, description, enabled, admin, default_department, created_at, updated_at FROM users WHERE id=@id";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                    cmd.Parameters["@id"].Value = id;
                    
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = DBOToModelConvert(reader);
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

            return user;
        }

        public User GetByIdentity(string identity)
        {
            var con = new MySqlHandler().GetConnection();
            User user = null;

            var parts = identity.Split('\\');
            
            try
            {
                const string query = "SELECT id, name, domain, description, enabled, username, admin, default_department, created_at, updated_at " +
                                     "FROM users WHERE username=@username AND domain=@domain AND enabled=1";

                con.Open();
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
                            user = DBOToModelConvert(reader);
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

            return user;
        }

        public User DBOToModelConvert(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            String rowName = reader.GetString("name");
            String rowUsername = reader.GetString("username");
            String rowDescription = reader.GetString("description");
            bool rowEnabled = reader.GetBoolean("enabled");
            ulong rowDefaultDepartment = reader.GetUInt64("default_department");
            bool rowAdmin = reader.GetBoolean("admin");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");

            return (User) Activator.CreateInstance(typeof(User),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowName, rowUsername, rowDescription, rowEnabled, rowDefaultDepartment, rowAdmin, rowCreatedAt, rowUpdatedAt }, null, null);
        }
    }
}