using AMS.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AMS.Logging
{
    class CWRep : ILogRepository
    {
        public IEnumerable<LogEntry> GetLogEntries(ulong logableId, Type logableType)
        {
            return new List<LogEntry>();
        }

        public IEnumerable<LogEntry> GetLogEntries(ulong logableId, Type logableType, string username)
        {
            return new List<LogEntry>();
        }

        public bool Insert(LogEntry entity)
        {
            return true;
        }

        public ObservableCollection<LogEntry> Search(string keyword, List<ulong> tags = null, List<ulong> users = null, bool strict = false)
        {
            return new ObservableCollection<LogEntry>();
        }
    }
}
