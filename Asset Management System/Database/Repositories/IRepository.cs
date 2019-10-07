namespace Asset_Management_System.Database.Repositories
{
    interface IRepository<T>
    {
        bool Insert(T entity);
        bool Update(T entity);
        bool Delete(T entity);

        T GetById(long id);
    }
}
