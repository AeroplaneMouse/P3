using System;
using System.Collections.Generic;

namespace AMS.Database.Repositories.Interfaces
{
    public interface ILogRepository : ISearchableRepository<Entry>
    {    
        bool Insert(Entry entity);
        
        IEnumerable<Entry> GetLogEntries(ulong logableId, Type logableType);
        IEnumerable<Entry> GetLogEntries(ulong logableId, Type logableType, string username);
    }
}