using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using MySql.Data.MySqlClient;

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
            throw new NotImplementedException();
        }
    }
}
