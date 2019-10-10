using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace Asset_Management_System.Database.Repositories
{
    class CommentRepository : ICommentRepository
    {

        public bool Insert(Comment entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(Comment entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Comment entity)
        {
            throw new NotImplementedException();
        }

        public Comment GetById(long id)
        {
            throw new NotImplementedException();
        }

        public Comment DBOToModelConvert(MySqlDataReader reader)
        {
            ulong row_id = reader.GetUInt64("id");
            String row_content = reader.GetString("content");
            ulong row_asset_id = reader.GetUInt64("asset_id");
            DateTime row_created_at = reader.GetDateTime("created_at");

            return (Comment)Activator.CreateInstance(typeof(Comment), 
                BindingFlags.Instance | BindingFlags.NonPublic, null, 
                new object[] { row_id, row_content, row_asset_id, row_created_at }, null, null);
        }
    }
}
