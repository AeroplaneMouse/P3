using System;
using System.Collections.Generic;

namespace Asset_Management_System.Database.Repositories
{
    interface ILogRepository<T> : ISearchableRepository<T>
    {    
        bool Insert(T entity);
        
        IEnumerable<T> GetLogEntries(ulong logableId, Type logableType);
        IEnumerable<T> GetLogEntries(ulong logableId, Type logableType, string username);
    }
}