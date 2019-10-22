using Asset_Management_System.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Helpers;
using Asset_Management_System.Models;
using Asset_Management_System.Views;

namespace Asset_Management_System.ViewModels
{
    public class LogsViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;

        public LogsViewModel(MainViewModel main)
        {
            // Saving reference to the main window
            _main = main;
            Search();
            
            // Initializing commands
            SearchCommand = new ViewModels.Base.RelayCommand(Search);
            PrintCommand = new Base.RelayCommand(Print);
            ViewCommand = new Base.RelayCommand(View);
        }
        
        #region Commands
        
        public ICommand SearchCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand ViewCommand { get; set; }

        #endregion

        #region Public Properties
        public string SearchQueryText { get; set; } = "";
        public int SelectedItemIndex { get; set; }
        
        private ObservableCollection<Entry> _list = new ObservableCollection<Entry>();

        public ObservableCollection<Entry> SearchList
        {
            get => _list;
            set
            {
                _list.Clear();
                foreach (Entry entry in value)
                    _list.Add(entry);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sends a search request to the database, and sets the list of entires to the result.
        /// </summary>
        private void Search()
        {
            Console.WriteLine();
            Console.WriteLine("Searching for: " + SearchQueryText);
            LogRepository rep = new LogRepository();
            ObservableCollection<Entry> entries = new ObservableCollection<Entry>(rep.Search(SearchQueryText));

            Console.WriteLine("Found: " + entries.Count.ToString() + " Log entries");

            SearchList = entries;
        }
        
        /// <summary>
        /// Creates a csv file containing all the log entries
        /// </summary>
        private void Print()
        {
            PrintHelper.Print(SearchList.ToList());
        }

        /// <summary>
        /// Displays the selected log entry
        /// </summary>
        private void View()
        {
            Entry selected = GetSelectedItem();
            var dialog = new ShowEntry(selected);
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine("Displaying log entry saying : " + selected.Description);
            }
        }

        private Entry GetSelectedItem()
        {
            if (SearchList.Count == 0)
                return null;
            else
                return SearchList.ElementAt(SelectedItemIndex);
        }

        #endregion
    }
}
