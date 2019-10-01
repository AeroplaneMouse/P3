using System;
using System.Linq;
using System.Linq.Expressions;

namespace Asset_Management_System.Database.Repositories
{
    public interface IRepository<T>
    {
        public void Insert(T entity);
        public T GetById(int id);
        public void Delete(T entity);
    }
}
