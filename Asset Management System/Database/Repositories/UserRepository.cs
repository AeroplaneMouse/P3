using System;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.Generic;

namespace Asset_Management_System.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
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
                const string query = "INSERT INTO users (username, domain, description, enabled, default_department, admin, updated_at) " +
                                     "VALUES (@username, @domain, @description, @enabled, @default_department, @admin, CURRENT_TIMESTAMP())";
                
                con.Open();
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
            
            return querySuccess;
        }

        public bool Update(User entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            try
            {
                const string query = "UPDATE users SET username=@username, domain=@domain, description=@description, enabled=@enabled, admin=@admin, default_department=@default_department, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                con.Open();
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
                const string query = "SELECT * FROM users WHERE id=@id";

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

        public User GetByUsername(string username)
        {
            var con = new MySqlHandler().GetConnection();
            User user = null;
            
            try
            {
                const string query = "SELECT * " +
                                     "FROM users WHERE username=@username AND enabled=1";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.String);
                    cmd.Parameters["@username"].Value = username;

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

        public IEnumerable<User> GetAll()
        {
            var con = new MySqlHandler().GetConnection();
            List<User> users = new List<User>();

            try
            {
                const string query = "SELECT * " +
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

        public User DBOToModelConvert(MySqlDataReader reader)
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