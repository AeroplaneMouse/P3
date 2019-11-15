using System;
using System.Collections.Generic;
using AMS.Logging;

namespace AMS.Database.Repositories
{
    public interface ILogRepository : ISearchableRepository<Entry>
    {    
        bool Insert(Entry entity);
        
        IEnumerable<Entry> GetLogEntries(ulong logableId, Type logableType);
        IEnumerable<Entry> GetLogEntries(ulong logableId, Type logableType, string username);
    }
}