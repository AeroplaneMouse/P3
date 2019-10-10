using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Asset_Management_System.ViewModels
{
    internal class AssetsViewModel : Base.BaseViewModel
    {
        #region Private Properties

        #endregion

        #region Public Properties

        public int TitleHeight { get; set; }
        
        public GridLength TitleHeightGridLength { get => new GridLength(TitleHeight); }

        public List<Asset> Assets { get; set; }

        #endregion

        #region Commands


        #endregion

        #region Contructors

        public AssetsViewModel()
        {
            TitleHeight = 40;
            Assets = new List<Asset>();
            //Assets = new AssetRepository().Search("");

        }

        #endregion


    }
}