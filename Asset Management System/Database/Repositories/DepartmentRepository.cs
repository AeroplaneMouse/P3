using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    class DepartmentRepository : IRepository<Department>
    {
        public void Delete(Department entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Department> GetAll()
        {
            throw new NotImplementedException();
        }

        public Department GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Department entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Department> SearchFor(Expression<Func<Department, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
