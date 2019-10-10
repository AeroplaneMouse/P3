namespace Asset_Management_System.Database.Repositories
{
    interface ILogRepository<T>
    {    
        bool Insert(T entity);
        bool GetLogEntries(T entity);
    }
}