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

namespace Asset_Management_System.ViewModels
{
    public class AssetsViewModel : ChangeableListPageViewModel<AssetRepository, Asset>
    {
        #region Constructors

        private MainViewModel _main;
        public AssetsViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType)
        {
            _main = main;
            Title = "Assets";
        }

        #endregion

        #region Public Properties

        public int ViewType => 1;


        #endregion
    }
}