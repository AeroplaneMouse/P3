using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using AMS.Models;
using System.Reflection;
using AMS.Database.Repositories.Interfaces;

namespace AMS.Database.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        public ulong GetCount()
        {
            var con = new MySqlHandler().GetConnection();
            ulong count = 0;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT COUNT(*) FROM departments;";

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
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns>True if entity was successfully inserted.</returns>
        public bool Insert(Department entity, out ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;
            id = 0;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "INSERT INTO departments (name, updated_at) VALUES (@name, CURRENT_TIMESTAMP())";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;
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

            return querySuccess;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(Department entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "UPDATE departments SET name=@name, updated_at=CURRENT_TIMESTAMP() WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;
                        cmd.Parameters.Add("@id", MySqlDbType.Int64);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(Department entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool querySuccess = false;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "DELETE FROM departments WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", MySqlDbType.Int64);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Department GetById(ulong id)
        {
            var con = new MySqlHandler().GetConnection();
            Department department = null;

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT id, name, created_at, updated_at " +
                                         "FROM departments WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                        cmd.Parameters["@id"].Value = id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                department = DataMapper(reader);
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
            
            return department;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Department> GetAll()
        {
            var con = new MySqlHandler().GetConnection();
            List<Department> departments = new List<Department>();

            // Opening connection
            if (MySqlHandler.Open(ref con))
            {
                try
                {
                    const string query = "SELECT id, name, created_at, updated_at " +
                                         "FROM departments ORDER BY name ASC";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Department dep = DataMapper(reader);
                                departments.Add(dep);
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
            
            return departments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Department DataMapper(MySqlDataReader reader)
        {
            ulong rowId = reader.GetUInt64("id");
            string rowName = reader.GetString("name");
            DateTime rowCreatedAt = reader.GetDateTime("created_at");
            DateTime rowUpdatedAt = reader.GetDateTime("updated_at");

            return (Department)Activator.CreateInstance(typeof(Department), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { rowId, rowName, rowCreatedAt, rowUpdatedAt }, null, null);
        }
    }
}
