﻿using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface IAssetListController
    {
        #region Properties

        // Main asset list
        ObservableCollection<Asset> AssetList { get; set; }

        // List of tags that can be searched
        List<Tag> TagList { get; set; }

        #endregion

        #region Methods

        void Search(string query);

        void Remove(Asset asset);

        void Export(List<Asset> assets);

        List<ITagable> GetTags(Asset asset);

        #endregion
    }
}
