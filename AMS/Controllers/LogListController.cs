using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using AMS.Models;

namespace AMS.Controllers
{
    public class LogListController : ILogListController
    {
        private readonly ILogRepository _logRepository;
        private readonly IExporter _exporter;
        private List<LogEntry> _entryList;

        public List<LogEntry> EntryList
        {
            get
            {
                if (_entryList == null)
                {
                    _entryList = _logRepository.Search("").ToList();
                }

                return _entryList.OrderByDescending(p => p.CreatedAt).ToList();
            }

            set => _entryList = value;
        }

        public LogListController(ILogRepository logRepository, IExporter exporter, Asset asset = null)
        {
            _logRepository = logRepository;
            _exporter = exporter;

            if (asset == null)
                Search("");
            else
                EntryList = _logRepository.GetLogEntries(asset.ID, typeof(Asset)).ToList();
        }
        
        /// <summary>
        /// Filters the list of entries based on given query
        /// </summary>
        /// <param name="query"></param>
        public void Search(string query)
        {
            EntryList = _logRepository.Search(query).ToList();
        }

        /// <summary>
        /// Exports the selected entries to a .csv file
        /// </summary>
        /// <param name="entries"></param>
        public void Export(List<LogEntry> entries)
        {
            _exporter.Print(entries);
        }

        public void UpdateEntries()
        {
            _entryList = (List<LogEntry>)_logRepository.Search("");
        }
    }
}