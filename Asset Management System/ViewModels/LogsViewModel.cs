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
    public class LogsViewModel : ChangeableListPageViewModel<LogRepository, Entry>
    {
        public int ViewType => 3;

        public LogsViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType) 
        {
            Title = "Logs";
        }
    }
}
