using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Asset_Management_System.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        public bool Insert(User entity)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "INSERT INTO users (name, username, admin) " +
                                         "VALUES (@name, @username, @admin)";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;

                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = entity.Username;

                        cmd.Parameters.Add("@admin", MySqlDbType.String);
                        cmd.Parameters["@admin"].Value = entity.IsAdmin ? 1 : 0;
                        
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

        public bool Update(User entity)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "UPDATE users SET name=@name, username=@username, admin=@admin WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;

                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = entity.Username;

                        cmd.Parameters.Add("@admin", MySqlDbType.UInt16);
                        cmd.Parameters["@admin"].Value = entity.IsAdmin ? 1 : 0;
                        
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

        public bool Delete(User entity)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "DELETE FROM users WHERE id=@id";

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

        public User GetById(ulong id)
        {
            DBConnection dbcon = DBConnection.Instance();
            User user = null;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query =
                        "SELECT id, name, username, admin, created_at FROM users WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = id;

                        using var reader = cmd.ExecuteReader();
                        
                        while (reader.Read())
                        {
                            user = DBOToModelConvert(reader);
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

            return user;
        }

        public User GetByUsername(string username)
        {
            DBConnection dbcon = DBConnection.Instance();
            User user = null;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "SELECT id, name, username, admin, created_at FROM users WHERE username=@username";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@username", MySqlDbType.String);
                        cmd.Parameters["@username"].Value = username;

                        using var reader = cmd.ExecuteReader();
                        
                        while (reader.Read())
                        {
                            user = DBOToModelConvert(reader);
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

            return user;
        }
        
        public User DBOToModelConvert(MySqlDataReader reader)
        {
            ulong row_id = reader.GetUInt64("id");
            String row_name = reader.GetString("name");
            String row_username = reader.GetString("username");
            bool row_admin = reader.GetBoolean("admin");
            DateTime row_created_at = reader.GetDateTime("created_at");

            return (User) Activator.CreateInstance(typeof(User),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {row_id, row_name, row_username, row_admin, row_created_at}, null,
                null);
        }
    }
}