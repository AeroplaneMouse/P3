using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Asset_Management_System.Database.Repositories
{
    interface ILogRepository<T>
    {    
        bool Insert(T entity);
        
        List<T> GetLogEntries(ulong logable_id, Type logable_type);
        List<T> GetLogEntries(ulong logable_id, Type logable_type, string username);

        List<T> Search(string keyword, int limit);
    }
}