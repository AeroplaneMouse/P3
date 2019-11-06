using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Helpers;
using Asset_Management_System.Logging;
using Asset_Management_System.Resources.DataModels;
using System.Windows;

namespace Asset_Management_System.ViewModels
{
    public class AssetsViewModel : ChangeableListPageViewModel<AssetRepository, Asset>
    {

        private string _currentGroup = String.Empty;
        private string _searchQueryText = String.Empty;
        private bool IsTagMode = false;

        public int ViewType => 1;
        public Visibility IsCurrentGroupVisible { get; set; } = Visibility.Hidden;
        public ICommand DeleteCommand { get; set; }

        public string CurrentGroup
        {
            get => _currentGroup;
            set
            {
                _currentGroup = value;
                if (value == String.Empty)
                    IsCurrentGroupVisible = Visibility.Hidden;
                else
                    IsCurrentGroupVisible = Visibility.Visible;
            }
        }

        public new string SearchQueryText
        {
            get => _searchQueryText;
            set
            {
                _searchQueryText = value;
                if (value == "#" && !IsTagMode)
                {
                    _searchQueryText = "";
                    CurrentGroup = "#";
                    IsTagMode = true;
                }
            }
        }

        private MainViewModel _main;

        public AssetsViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType)
        {
            DeleteCommand = new Base.RelayCommand(Delete);
        }

        private void Delete()
        {
            if (CurrentGroup != String.Empty)
            {
                if (CurrentGroup == "#")
                {
                    CurrentGroup = String.Empty;
                    IsTagMode = false;
                }
                else
                    CurrentGroup = "#";
            }
        }

        protected override void Search()
        {
            base.Search();
        }

        #region Public Properties

        public int ViewType => 1;

        #endregion
    }
}