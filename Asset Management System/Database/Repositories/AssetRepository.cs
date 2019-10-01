using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    class AssetRepository : IRepository<Asset>
    {
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
            throw new NotImplementedException();
        }

        public IQueryable<Asset> SearchFor(Expression<Func<Asset, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
