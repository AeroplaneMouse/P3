using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Controllers
{
    public class AssetListController : IAssetListController
    {
        private IExporter _exporter;
        private IAssetRepository _assetRepository;

        public List<Asset> AssetList { get; set; }
        public List<Tag> TagList { get; set; } 

        public AssetListController(IAssetRepository assetRepository, IExporter exporter)
        {
            AssetList = new List<Asset>();
            _assetRepository = assetRepository;
            _exporter = exporter;
            Search("");
        }
        
        /// <summary>
        /// Filters the list if Assets to only contain assets that match the searchQuery
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tags"></param>
        /// <param name="users"></param>
        /// <param name="strict"></param>
        /// <param name="searchInFields"></param>
        public void Search(string query, List<ulong> tags=null, List<ulong> users=null, bool strict=false, bool searchInFields=false)
        {
            List<Asset> searchResult = _assetRepository.Search(query, tags, users, strict, searchInFields);
            AssetList = searchResult;
        }

        /// <summary>
        /// Removes the given asset from the database and from the list
        /// </summary>
        /// <param name="asset"></param>
        public void Remove(Asset asset)
        {
            // Check if asset is in list
            if (!AssetList.Contains(asset))
                return;
            // Delete the asset
            if (_assetRepository.Delete(asset))
                AssetList.Remove(asset);
        }

        /// <summary>
        /// Exports the selected items in the listView to a .csv file
        /// Using an IExporter
        /// </summary>
        /// <param name="assets"></param>
        public void Export(List<Asset> assets)
        {
            _exporter.Print(assets);
        }

        /// <summary>
        /// Gets the tags currently attached to the input asset
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public List<ITagable> GetTags(Asset asset)
        {
            return _assetRepository.GetTags(asset).ToList();
        }
    }
}