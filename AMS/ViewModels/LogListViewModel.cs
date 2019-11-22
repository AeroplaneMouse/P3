using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Logging;
using AMS.ViewModels.Base;

namespace AMS.ViewModels
{
    public class LogListViewModel
    {
        private readonly MainViewModel _main;
        private ILogListController _logListController;

        public ObservableCollection<Entry> SearchList { get; set; }
        public string SearchQuery { get; set; }

        public ICommand ViewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        

        public LogListViewModel(MainViewModel main, ILogListController logListController)
        {
            _main = main;
            _logListController = logListController;
            SearchList = _logListController.EntryList;

            ViewCommand = new RelayCommand(View);
            SearchCommand = new RelayCommand(Search);
            PrintCommand = new RelayCommand(Export);
        }

        private void View()
        {
            //TODO: Create Entry view page
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update list of entries with elements matching the searchQuery
        /// </summary>
        private void Search()
        {
            if (SearchQuery == null)
                return;
            
            _logListController.Search(SearchQuery);
            SearchList = _logListController.EntryList;
        }

        /// <summary>
        /// Exports the selected Entries to a .csv file
        /// </summary>
        private void Export()
        {
            //TODO: figure out how to get selected items
            throw new NotImplementedException();
        }
    }
}