using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database.Repositories
{
    class AssetRepository : IRepository<Asset>
    {
        private DBConnection dbcon;

        public AssetRepository()
        {
            this.dbcon = DBConnection.Instance();
        }
        public void Delete(Asset entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Asset> GetAll()
        {
            throw new NotImplementedException();
        }

        public Asset GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Asset entity)
        {
            void Insert(Department entity)
            {
                if (dbcon.IsConnect())
                {
                    string query = "INSERT INTO assets (name) VALUES (@name)";
                    var cmd = new MySqlCommand(query, dbcon.Connection);
                    cmd.Parameters.Add("@name", MySqlDbType.String);
                    cmd.Parameters["@name"].Value = entity.Name;
                    cmd.ExecuteNonQuery();
                    dbcon.Close();
                }
            }
        }

        public IQueryable<Asset> SearchFor(Expression<Func<Asset, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
