using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Logging;

namespace AMS.Controllers.Interfaces
{
    public interface ILogListController
    {
        List<LogEntry> EntryList { get; set; }

        void Search(string query);

        void Export(List<LogEntry> entries);
    }
}