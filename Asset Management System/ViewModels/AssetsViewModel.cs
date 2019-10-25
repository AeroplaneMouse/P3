﻿using Asset_Management_System.Database.Repositories;
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

        public AssetsViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType)
        {
        }

        #endregion

        #region Public Properties

        public int ViewType => 1;

        #endregion

        #region Methods

        protected override void View()
        {
            Console.WriteLine("Asset view");

            Asset selected = GetSelectedItem();

            var dialog = new AssetHistory(selected);
        }

        #endregion
    }
}