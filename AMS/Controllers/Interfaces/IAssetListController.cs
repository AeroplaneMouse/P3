using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface IAssetListController
    {
        #region Properties

        // Main asset list
        List<Asset> AssetList { get; set; }

        // List of tags that can be searched
        List<Tag> TagsList { get; set; }

        #endregion

        #region Methods

        void Search(string query);

        void ViewAsset(Asset asset);

        void Remove(Asset asset);

        void Export(List<Asset> assets);
        

        #endregion
    }
}
