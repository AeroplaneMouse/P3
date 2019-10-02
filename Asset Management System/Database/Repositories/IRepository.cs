namespace Asset_Management_System.Database.Repositories
{
    public interface IRepository<T>
    {
        void Insert(T entity);
        T GetById(long id);
        void Delete(T entity);
    }
}
