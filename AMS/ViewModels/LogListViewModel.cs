using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Logging;
using AMS.Models;
using AMS.ViewModels.Base;

namespace AMS.ViewModels
{
    public class LogListViewModel : Base.BaseViewModel
    {
        private ILogListController _logListController;

        public ObservableCollection<LogEntry> Entries
        {
            get => new ObservableCollection<LogEntry>(_logListController.EntryList);
        }
        public string SearchQuery { get; set; }
        public List<LogEntry> SelectedItems { get; set; } = new List<LogEntry>();
        public Visibility SingleSelected { get; set; } = Visibility.Collapsed;
        public Visibility MultipleSelected { get; set; } = Visibility.Collapsed;

        public ICommand ViewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        

        public LogListViewModel(ILogListController logListController)
        {
            _logListController = logListController;

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
            OnPropertyChanged(nameof(Entries));
        }

        /// <summary>
        /// Exports the selected Entries to a .csv file
        /// </summary>
        private void Export()
        {
            if (SelectedItems.Count > 0)
                // Export selected items
                _logListController.Export(SelectedItems);
            else
                // Export all items found by search
                _logListController.Export(Entries.ToList());
        }
    }
}