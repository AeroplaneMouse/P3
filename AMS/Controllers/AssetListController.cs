using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        //TODO: Create tagging class
        //public Tagging _tags;

        public ObservableCollection<Asset> AssetList { get; set; }
        public List<Tag> TagsList { get; set; } 

        public AssetListController(IAssetRepository assetRepository, IExporter exporter)
        {
            AssetList = new ObservableCollection<Asset>();
            _assetRepository = assetRepository;
            _exporter = exporter;
            Search("");
        }
        
        /// <summary>
        /// Filters the list if Assets to only contain assets that match the searchQuery
        /// </summary>
        /// <param name="query"></param>
        public void Search(string query, List<ulong> tags=null, List<ulong> users=null, bool strict=false)
        {
            //TODO: Filter list based on search query, also search by tags
            ObservableCollection<Asset> searchResult = _assetRepository.Search(query, tags, users, strict);
            AssetList = searchResult;
            //TODO: Determine if search should be on assets or by tags
            /* Remove comment when tagging class is implementet
            SearchList = _rep.Search(SearchQueryText,
                Tags.AppliedTags.OfType<Tag>().Select(t => t.ID).ToList(),
                Tags.AppliedTags.OfType<User>().Select(u => u.ID).ToList(),
                IsStrict);
                */
        }

        /// <summary>
        /// Displays the editPage for a blank asset
        /// </summary>
        public void AddNew()
        {
            //TODO: Redirect to blank editPage
            AssetList.Add(new Asset());
        }

        /// <summary>
        /// Displays the editPage for the given page
        /// </summary>
        /// <param name="asset"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Edit(Asset asset)
        {
            // Handle if asset is null
            if (asset == null)
            {
                //TODO: Handle error and notify user
                return;
            }
            //TODO: Redirect to EditPage
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Displays the viewPage for the given asset
        /// </summary>
        /// <param name="asset"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ViewAsset(Asset asset)
        {
            //TODO: Redirect to ViewPage
            throw new NotImplementedException();
            
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
            //TODO: Implement printHelper
            _exporter.Print(assets);
        }
    }
}
