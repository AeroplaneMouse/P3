using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Logging;
using Asset_Management_System.Models;
using Asset_Management_System.Views;

namespace Asset_Management_System.ViewModels
{
    public class AssetHistoryViewModel : Base.BaseViewModel
    {
        #region Constructor
        
        /// <summary>
        /// Default contructor
        /// </summary>
        public AssetHistoryViewModel(Window window, Model asset)
        {
            // Initialize commands
            _window = window;
            
            ViewCommand = new Base.RelayCommand(() => View());
            // Start the search over
            ExitCommand = new Base.RelayCommand(() => Exit());
            
            ILogRepository<Entry> rep = new LogRepository();
            History = (List<Entry>) rep.GetLogEntries(asset.ID, asset.GetType());

        }

        #endregion

        #region Private Properties

        private Window _window;

        #endregion

        #region Public Properties
        
        public int SelectedItemIndex { get; set; }
        
        public List<Entry> History { get; set; }

        #endregion
        
        #region Methods
        
        /// <summary>
        /// Displays the selected LogEntry
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

        /// <summary>
        /// Closes the history window
        /// </summary>
        private void Exit()
        {
            _window.DialogResult = true;
        }
        
        /// <summary>
        /// Finds and returns the currently selected item in the listview
        /// </summary>
        /// <returns>Selected Entry in listView</returns>
        private Entry GetSelectedItem()
        {
            if (History.Count == 0)
                return null;
            else
                return History.ElementAt(SelectedItemIndex);
        }
        
        #endregion

        #region Commands
        
        public ICommand ViewCommand { get; set; }

        public ICommand ExitCommand { get; set; }
        
        #endregion
    }
}