namespace AMS.Database.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        T Insert(T entity, out ulong id);
        bool Update(T entity);
        bool Delete(T entity);
        T GetById(ulong id);
    }
}