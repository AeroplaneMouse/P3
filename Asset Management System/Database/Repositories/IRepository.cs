using System;

namespace Asset_Management_System.Database.Repositories
{
    interface IRepository<T>
    {
        //event EventHandler RepositoryError;

        bool Insert(T entity);
        bool Update(T entity);
        bool Delete(T entity);

        T GetById(ulong id);
    }
}
