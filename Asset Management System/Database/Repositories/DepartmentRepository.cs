using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    class DepartmentRepository : IDepartmentRepository
    {
        private DBConnection dbcon;

        public DepartmentRepository()
        {
            this.dbcon = DBConnection.Instance();
        }

        public void Insert(Department entity)
        {
            if (dbcon.IsConnect())
            {
                string query = "INSERT INTO departments (name) VALUES (@name)";
                var cmd = new MySqlCommand(query, dbcon.Connection);
                cmd.Parameters.Add("@name", MySqlDbType.String);
                cmd.Parameters["@name"].Value = entity.Name;
                cmd.ExecuteNonQuery();
                dbcon.Close();
            }
        }

        public Department GetById(long id)
        {
            if (dbcon.IsConnect())
            {
                //suppose col0 and col1 are defined as VARCHAR in the DB
                string query = "SELECT id, name, FROM departments WHERE id=@id";
                var cmd = new MySqlCommand(query, dbcon.Connection);
                cmd.Parameters.Add("@id", MySqlDbType.Int64);
                cmd.Parameters["@id"].Value = id;
                var reader = cmd.ExecuteReader();

                Department department = null;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        long row_id = reader.GetInt64("id");
                        String row_name = reader.GetString("name");

                        department = new Department(row_id, row_name);
                    }
                }

                dbcon.Close();
                return department;
            }
            else
            {
                return null;
            }
        }

        public void Delete(Department entity)
        {
            if (dbcon.IsConnect())
            {
                string query = "DELETE FROM departments WHERE id=@id";
                var cmd = new MySqlCommand(query, dbcon.Connection);
                cmd.Parameters.Add("@id", MySqlDbType.Int64);
                cmd.Parameters["@id"].Value = entity.ID;
                cmd.ExecuteNonQuery();
                dbcon.Close();
            }
        }

        public List<Department> GetAll()
        {
            if (dbcon.IsConnect())
            {
                string query = "SELECT id, name FROM departments ORDER BY name ASC";
                var cmd = new MySqlCommand(query, dbcon.Connection);
                var reader = cmd.ExecuteReader();

                List<Department> departments = new List<Department>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        long row_id = reader.GetInt64("id");
                        string row_name = reader.GetString("name");

                        Department dep = new Department(row_id, row_name);
                        departments.Add(dep);
                    }
                }

                dbcon.Close();
                return departments;
            }
            else
            {
                return null;
            }
        }
    }
}
