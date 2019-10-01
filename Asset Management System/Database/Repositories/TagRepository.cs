using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database.Repositories
{
    class TagRepository : ITagRepository
    {
        private DBConnection dbcon;

        public TagRepository(){ 
            this.dbcon = DBConnection.Instance();
        }

        public void Delete(Tag entity)
        {
            throw new NotImplementedException();
        }

        public Tag GetById(Int64 id)
        {
            if (this.dbcon.IsConnect())
            {
                //suppose col0 and col1 are defined as VARCHAR in the DB
                string query = "SELECT id, label, parent_id, department_id, color, options FROM tags WHERE id=@id";
                var cmd = new MySqlCommand(query, dbcon.Connection);
                cmd.Parameters.Add("@ID", MySqlDbType.Int64);
                cmd.Parameters["@ID"].Value = id;
                var reader = cmd.ExecuteReader();

                if(reader.HasRows){
                    while (reader.Read())
                    {
                        string someStringFromColumnZero = reader.GetString(0);
                        Console.WriteLine(someStringFromColumnZero);
                    }
                }
                
                dbcon.Close()

            }
        }

        public List<Tag> GetChildTags()
        {
            throw new NotImplementedException();
        }

        public Department GetDepartment()
        {
            throw new NotImplementedException();
        }

        public Tag GetParentTag()
        {
            throw new NotImplementedException();
        }

        public void Insert(Tag entity)
        {
            throw new NotImplementedException();

            
        }
    }
}
