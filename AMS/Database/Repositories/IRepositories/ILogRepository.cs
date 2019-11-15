using System;
using System.Collections.Generic;
using Asset_Management_System.Logging;

namespace Asset_Management_System.Database.Repositories
{
    public interface ILogRepository : ISearchableRepository<Entry>
    {    
        bool Insert(Entry entity);
        
        IEnumerable<Entry> GetLogEntries(ulong logableId, Type logableType);
        IEnumerable<Entry> GetLogEntries(ulong logableId, Type logableType, string username);
    }
}