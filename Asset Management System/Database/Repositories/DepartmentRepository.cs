using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Asset_Management_System.Models;
using System.Reflection;

namespace Asset_Management_System.Database.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        public int GetCount()
        {
            var con = new MySqlHandler().GetConnection();
            int count = 0;

            try
            {
                const string query = "SELECT COUNT(*) FROM departments;";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            count = reader.GetInt32("COUNT(*)");
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>True if entity was successfully inserted.</returns>
        public bool Insert(Department entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool query_success = false;
            
            try
            {
                const string query = "INSERT INTO departments (name, updated_at) VALUES (@name, CURRENT_TIMESTAMP())";

                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@name", MySqlDbType.String);
                    cmd.Parameters["@name"].Value = entity.Name;
                    query_success = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException e)
            { 
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return query_success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(Department entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool query_success = false;

            try
            {
                const string query = "UPDATE departments SET name=@name, updated_at=CURRENT_TIMESTAMP() WHERE id=id";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@name", MySqlDbType.String);
                    cmd.Parameters["@name"].Value = entity.Name;
                    cmd.Parameters.Add("@id", MySqlDbType.Int64);
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
                con.Close();
            }
            
            return query_success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(Department entity)
        {
            var con = new MySqlHandler().GetConnection();
            bool query_success = false;

            try
            {
                const string query = "DELETE FROM departments WHERE id=@id";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue  ("@id", MySqlDbType.Int64);
                    cmd.Parameters["@id"].Value = entity.ID;
                    query_success = cmd.ExecuteNonQuery() > 0;
                }
            }catch(MySqlException e){ 
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return query_success;
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

            try{
                const string query = "SELECT id, name, created_at, updated_at " +
                                     "FROM departments WHERE id=@id";

                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.UInt64);
                    cmd.Parameters["@id"].Value = id;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            department = DBOToModelConvert(reader);
                        }
                        reader.Close();
                    }
                }
            }catch(MySqlException e){ 
                Console.WriteLine(e);
            }finally{
                con.Close();
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

            try{
                const string query = "SELECT id, name, created_at, updated_at " +
                                     "FROM departments ORDER BY name ASC";
                
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Department dep = DBOToModelConvert(reader);
                            departments.Add(dep);
                        }
                        reader.Close();
                    }
                }
            }catch(MySqlException e){ 
                Console.WriteLine(e);
            }finally{
                con.Close();
            }
            
            return departments;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Department DBOToModelConvert(MySqlDataReader reader)
        {
            ulong row_id = reader.GetUInt64("id");
            string row_name = reader.GetString("name");
            DateTime row_created_at = reader.GetDateTime("created_at");
            DateTime row_updated_at = reader.GetDateTime("updated_at");

            return (Department)Activator.CreateInstance(typeof(Department), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { row_id, row_name, row_created_at, row_updated_at }, null, null);
        }
    }
}
