using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Logging;
using AMS.Models;
using AMS.ViewModels.Base;

namespace AMS.ViewModels
{
    public class LogListViewModel : BaseViewModel
    {
        private string _searchQuery = "";
        private bool _searchCreates = false;
        private bool _searchUpdates = false;
        private bool _searchDeletes = false;
        private bool _searchErrors = false;
        private bool _searchTagAttaches = false;

        private ILogListController _logListController { get; set; }

        public ObservableCollection<LogEntry> Entries => new ObservableCollection<LogEntry>(_logListController.EntryList);

        public bool SearchCreates {
            get => _searchCreates;
            set {
                _searchCreates = value;
                Search();
            }
        }

        public bool SearchUpdates {
            get => _searchUpdates;
            set {
                _searchUpdates = value;
                Search();
            }
        }

        public bool SearchDeletes {
            get => _searchDeletes;
            set {
                _searchDeletes = value;
                Search();
            }
        }

        public bool SearchErrors {
            get => _searchErrors;
            set {
                _searchErrors = value;
                Search();
            }
        }
        public bool SearchTagAttaches {
            get => _searchTagAttaches;
            set {
                _searchTagAttaches = value;
                Search();
            }
        }

        public bool CheckAll { get; set; }
        public string SearchQuery {
            get => _searchQuery;
            set {
                _searchQuery = value;
                Search();
            }
        }

        public List<LogEntry> SelectedItems { get; set; } = new List<LogEntry>();
        public Visibility SingleSelected { get; set; } = Visibility.Collapsed;
        public Visibility MultipleSelected { get; set; } = Visibility.Collapsed;
        public ICommand ViewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand CheckAllChangedCommand { get; set; }

        public LogListViewModel(ILogListController logListController)
        {
            _logListController = logListController;

            ViewCommand = new RelayCommand(View);
            SearchCommand = new RelayCommand(Search);
            PrintCommand = new RelayCommand(Export);
            CheckAllChangedCommand = new RelayCommand<object>((parameter) => CheckAllChanged(parameter as ListView));
        }

        /// <summary>
        /// Selects or unselects all elements in the log list
        /// </summary>
        /// <param name="list"></param>
        private void CheckAllChanged(ListView list)
        {
            if (SelectedItems.Count == 0)
            {
                // None selected. Select all.
                CheckAll = true;
                list.SelectAll();
            }
            else if (SelectedItems.Count <= Entries.Count)
            {
                // Some selected or all selected. Unselect all
                CheckAll = false;
                list.UnselectAll();
            }
        }

        public override void UpdateOnFocus()
        {
            _logListController.UpdateEntries();
            OnPropertyChanged(nameof(Entries));
        }

        /// <summary>
        /// View log
        /// </summary>
        private void View()
        {
            if (SelectedItems.Count == 1)
                Features.Navigate.To(Features.Create.LogPresenter(SelectedItems.First()));
        }

        /// <summary>
        /// Update list of entries with elements matching the searchQuery
        /// </summary>
        private void Search()
        {
            if (_searchQuery == null)
                return;

            List<string> types = new List<string>();

            if (SearchCreates)
                types.Add("Create");
            if (SearchUpdates)
                types.Add("Update");
            if (SearchDeletes)
                types.Add("Delete");
            if (SearchTagAttaches)
                types.Add("Tag attached");
            if (SearchErrors)
                types.Add("Error");


            _logListController.Search(_searchQuery, types);
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