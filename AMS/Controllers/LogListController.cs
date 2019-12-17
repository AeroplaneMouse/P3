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
        private ILogRepository _logRepository { get; set; }
        private IExporter _exporter { get; set; }
        private List<LogEntry> _entryList;
        private Asset _asset;
        
        public List<string> types;

        public List<LogEntry> EntryList
        {
            get
            {
                if (_entryList == null)
                {
                    _entryList = _logRepository.Search("", types).ToList();
                }

                return _entryList.OrderByDescending(p => p.CreatedAt).ToList();
            }

            set => _entryList = value;
        }

        public LogListController(ILogRepository logRepository, IExporter exporter, Asset asset = null)
        {
            _logRepository = logRepository;
            _exporter = exporter;
            _asset = asset;

            if (_asset == null)
                Search("", types);

            else
                EntryList = _logRepository.GetLogEntries(_asset.ID, typeof(Asset)).ToList();
        }
        
        /// <summary>
        /// Filters the list of entries based on given query
        /// </summary>
        /// <param name="query"></param>
        public void Search(string query, List<string> updateTypes)
        {
            types = updateTypes;
            EntryList = _logRepository.Search(query, types).ToList();
        }

        /// <summary>
        /// Exports the selected entries to a .csv file
        /// </summary>
        /// <param name="entries"></param>
        public void Export(List<LogEntry> entries)
        {
            _exporter.Print(entries);
        }

        /// <summary>
        /// Searches for log entries
        /// </summary>
        public void UpdateEntries()
        {
            if (_asset == null)
                Search("", types);
            else
                EntryList = _logRepository.GetLogEntries(_asset.ID, typeof(Asset)).ToList();
        }
    }
}