using System;
using System.Collections.Generic;
using AMS.Logging;

namespace AMS.Database.Repositories.Interfaces
{
    public interface ILogRepository
    {    
        bool Insert(LogEntry entity);
        
        IEnumerable<LogEntry> GetLogEntries(ulong logableId, Type logableType);
        IEnumerable<LogEntry> GetLogEntries(ulong logableId, Type logableType, string username);
        IEnumerable<LogEntry> Search(string keyword);
    }
}