using Asset_Management_System.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Helpers;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using Asset_Management_System.Resources.DataModels;

namespace Asset_Management_System.ViewModels
{
    public class LogsViewModel : ListPageViewModel<LogRepository, Entry>
    {
        #region Constructor

        public LogsViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType) { }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the selected log entry
        /// </summary>
        protected override void View()
        {
            Entry selected = GetSelectedItem();
            var dialog = new ShowEntry(selected);
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine("Displaying log entry saying : " + selected.Description);
            }
        }

        #endregion
    }
}
