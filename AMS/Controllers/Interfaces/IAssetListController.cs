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
        // Main asset list
        ObservableCollection<Asset> AssetList { get; set; }

        // List of tags that can be searched
        List<Tag> TagsList { get; set; }

        void Search(string query, List<ulong> tags, List<ulong> users=null, bool strict=false);
        void ViewAsset(Asset asset);
        void Remove(Asset asset);
        void Export(List<Asset> assets);
    }
}
