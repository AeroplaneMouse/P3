using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    class TagRepository : IRepository<Tag>
    {
        public void Delete(Tag entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Tag> GetAll()
        {
            throw new NotImplementedException();
        }

        public Tag GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Tag entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Tag> SearchFor(Expression<Func<Tag, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
