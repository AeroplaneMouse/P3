using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Asset_Management_System.Models;
using System.Reflection;

namespace Asset_Management_System.Database.Repositories
{
    class DepartmentRepository : IDepartmentRepository
    {
        private readonly DBConnection dbcon;

        public DepartmentRepository()
        {
            this.dbcon = DBConnection.Instance();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>True if entity was successfully inserted.</returns>
        public bool Insert(Department entity)
        {
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "INSERT INTO departments (name) VALUES (@name)";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@name", MySqlDbType.String);
                        cmd.Parameters["@name"].Value = entity.Name;
                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (MySqlException e)
                { 
                    
                }finally{
                    dbcon.Close();
                }
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
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "UPDATE departments SET name=@name WHERE id=id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
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

                }
                finally
                {
                    dbcon.Close();
                }
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
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "DELETE FROM departments WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.AddWithValue  ("@id", MySqlDbType.Int64);
                        cmd.Parameters["@id"].Value = entity.ID;
                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }catch(MySqlException e){ 
                
                }finally{
                    dbcon.Close();
                }
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
            Department department = null;

            if (dbcon.IsConnect())
            {
                try{
                    const string query = "SELECT id, name, FROM departments WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int64);
                        cmd.Parameters["@id"].Value = id;

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                department = DBOToModelConvert(reader);
                            }
                        }
                    }
                }catch(MySqlException e){ 
                    
                }finally{
                    dbcon.Close();
                }
            }

            return department;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Department> GetAll()
        {
            List<Department> departments = new List<Department>();

            if (dbcon.IsConnect())
            {
                try{
                    const string query = "SELECT id, name FROM departments ORDER BY name ASC";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Department dep = DBOToModelConvert(reader);
                                departments.Add(dep);
                            }
                        }
                    }
                }catch(MySqlException e){ 
                    
                }finally{
                    dbcon.Close();
                }
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

            return (Department)Activator.CreateInstance(typeof(Department), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { row_id, row_name }, null, null);
        }
    }
}
