using System;
using System.Collections.Generic;
using System.Text;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;

namespace AMS.Controllers
{
    public class AssetListController : IAssetListController
    {
        private IExporter _exporter;
        private IAssetRepository _assetRepository;
        
        public List<Asset> AssetList { get; set; }
        public List<Tag> TagsList { get; set; } 

        public AssetListController(IAssetRepository assetRepository, IExporter exporter)
        {
            AssetList = new List<Asset>();
            _assetRepository = assetRepository;
            _exporter = exporter;

        }
        public void Search(string query)
        {
            //TODO: Filter list based on search query, also search by tags
            throw new NotImplementedException();
        }

        public void AddNew()
        {
            //TODO: Redirect to blank editPage
            AssetList.Add(new Asset());
        }

        public void Edit(Asset asset)
        {
            //TODO: Redirect to EditPage
            throw new NotImplementedException();
        }
        
        public void ViewAsset(Asset asset)
        {
            //TODO: Redirect to ViewPage
            throw new NotImplementedException();
        }

        public void Remove(Asset asset)
        {
            // Delete the asset
            if (_assetRepository.Delete(asset))
                AssetList.Remove(asset);
        }

        public void Export(List<Asset> assets)
        {
            //TODO: Implement printHelper
            throw new NotImplementedException();
        }
    }
}
