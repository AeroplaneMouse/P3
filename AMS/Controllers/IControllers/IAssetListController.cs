﻿using AMS.Interfaces;
using AMS.Models;
using AMS.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.IControllers
{
    public interface IAssetListController
    {
        #region Properties

        // Main asset list
        List<Asset> AssetList { get; set; }

        // List of tags that can be searched
        List<Tag> TagsList { get; set; }

        IExporter Exporter { get; set; }

        IAssetService AssetService { get; set; }

        #endregion

        #region Methods

        void Search(string query);

        void AddNew();

        void Edit(Asset asset);

        void ViewAsset(Asset asset);

        #endregion





    }
}
