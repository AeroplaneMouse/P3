using Asset_Management_System.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.Base;

namespace Asset_Management_System.ViewModels
{
    class HomeViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;

        public int NumberOfUsers { get; set; }
        public int NumberOfAssets { get; set; }
        public int NumberOfTags { get; set; }
        public int NumberOfDepartments { get; set; }

        /// <summary>
        /// Default contructor
        /// </summary>
        public HomeViewModel(MainViewModel main)
        {
            _main = main;

            // Get the number of stored assets, tags and departments
            NumberOfUsers = new UserRepository().GetCount();
            NumberOfAssets = new AssetRepository().GetCount();
            NumberOfTags = new TagRepository().GetCount();
            NumberOfDepartments = new DepartmentRepository().GetCount();

            // Notify view
            OnPropertyChanged(nameof(NumberOfUsers));
            OnPropertyChanged(nameof(NumberOfAssets));
            OnPropertyChanged(nameof(NumberOfTags));
            OnPropertyChanged(nameof(NumberOfDepartments));
        }
    }
}