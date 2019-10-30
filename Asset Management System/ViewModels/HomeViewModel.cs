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
        #region Private Properties

        private MainViewModel _main;

        #endregion

        #region Public Properties

        public string NumberOfAssets { get; set; }
        public string NumberOfTags { get; set; }
        public string NumberOfDepartments { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default contructor
        /// </summary>
        public HomeViewModel(MainViewModel main)
        {
            _main = main;

            // Get the number of stored assets, tags and departments
            int assets = new AssetRepository().GetCount();
            int tags = new TagRepository().GetCount();
            int departments = new DepartmentRepository().GetCount();

            // Generate strings
            NumberOfAssets = $"You have {assets} assets";
            NumberOfTags = $"You have {tags} tags";
            NumberOfDepartments = $"You have {departments} departments";

            // Notify view
            OnPropertyChanged(nameof(NumberOfAssets));
            OnPropertyChanged(nameof(NumberOfTags));
            OnPropertyChanged(nameof(NumberOfDepartments));   
        }

        #endregion

    }
}