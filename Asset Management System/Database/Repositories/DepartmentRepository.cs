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
            DBConnection dbcon = DBConnection.Instance();
            int count = 0;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "SELECT COUNT(*) FROM departments;";
                    using var cmd = new MySqlCommand(query, dbcon.Connection);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                        count = reader.GetInt32("COUNT(*)");
                }
                catch (MySqlException)
                {

                }
                finally
                {
                    dbcon.Close();
                }
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
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "INSERT INTO departments (name, updated_at) VALUES (@name, CURRENT_TIMESTAMP())";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
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
            DBConnection dbcon = DBConnection.Instance();
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "UPDATE departments SET name=@name, updated_at=CURRENT_TIMESTAMP() WHERE id=id";

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
                    Console.WriteLine(e);
                    //dbcon.SqlConnectionFailed?.Invoke(new Notification("ERROR! Unable to update department", Notification.ERROR));
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
            DBConnection dbcon = DBConnection.Instance();
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
                    Console.WriteLine(e);
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
            DBConnection dbcon = DBConnection.Instance();
            Department department = null;

            if (dbcon.IsConnect())
            {
                try{
                    const string query = "SELECT id, name, created_at, updated_at FROM departments WHERE id=@id";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.UInt64);
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
                    Console.WriteLine(e);
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
        public IEnumerable<Department> GetAll()
        {
            DBConnection dbcon = DBConnection.Instance();
            List<Department> departments = new List<Department>();

            if (dbcon.IsConnect())
            {
                try{
                    const string query = "SELECT id, name, created_at, updated_at FROM departments ORDER BY name ASC";

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
                    Console.WriteLine(e);
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
            DateTime row_created_at = reader.GetDateTime("created_at");
            DateTime row_updated_at = reader.GetDateTime("updated_at");

            return (Department)Activator.CreateInstance(typeof(Department), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { row_id, row_name, row_created_at, row_updated_at }, null, null);
        }
    }
}
