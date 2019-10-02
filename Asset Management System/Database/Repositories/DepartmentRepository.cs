﻿using System;
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

        public void Insert(Department entity)
        {
            if (dbcon.IsConnect())
            {
                string query = "INSERT INTO departments (name) VALUES (@name)";

                using (var cmd = new MySqlCommand(query, dbcon.Connection))
                {
                    cmd.Parameters.Add("@name", MySqlDbType.String);
                    cmd.Parameters["@name"].Value = entity.Name;
                    cmd.ExecuteNonQuery();
                }

                dbcon.Close();
            }
        }

        public Department GetById(long id)
        {
            Department department = null;

            if (dbcon.IsConnect())
            {
                string query = "SELECT id, name, FROM departments WHERE id=@id";

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

                dbcon.Close();
            }

            return department;
        }

        public void Delete(Department entity)
        {
            if (dbcon.IsConnect())
            {
                string query = "DELETE FROM departments WHERE id=@id";

                using (var cmd = new MySqlCommand(query, dbcon.Connection))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int64);
                    cmd.Parameters["@id"].Value = entity.ID;
                    cmd.ExecuteNonQuery();
                }

                dbcon.Close();
            }
        }

        public List<Department> GetAll()
        {
            List<Department> departments = new List<Department>();

            if (dbcon.IsConnect())
            {
                string query = "SELECT id, name FROM departments ORDER BY name ASC";

                using (var cmd = new MySqlCommand(query, dbcon.Connection)){
                    using (var reader = cmd.ExecuteReader()){
                        while (reader.Read())
                        {
                            Department dep = DBOToModelConvert(reader);
                            departments.Add(dep);
                        }
                    } 
                }

                dbcon.Close();
            }

            return departments;
        }

        public Department DBOToModelConvert(MySqlDataReader reader)
        {
            long row_id = reader.GetInt64("id");
            string row_name = reader.GetString("name");

            Department f = (Department)Activator.CreateInstance(typeof(Department), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { row_id, row_name }, null, null);
            return f;
        }
    }
}
