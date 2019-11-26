using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<Entry> EntryList { get; set; }

        public LogListController(ILogRepository logRepository, IExporter exporter, Asset asset = null)
        {
            _logRepository = logRepository;
            _exporter = exporter;
            if (asset == null)
                Search("");
            else
                //TODO Lav search for asset logs
                Search("");
        }
        
        /// <summary>
        /// Filters the list of entries based on given query
        /// </summary>
        /// <param name="query"></param>
        public void Search(string query)
        {
            EntryList = _logRepository.Search(query);
        }

        /// <summary>
        /// Exports the selected entries to a .csv file
        /// </summary>
        /// <param name="entries"></param>
        public void Export(List<Entry> entries)
        {
            _exporter.Print(entries);
        }
    }
}