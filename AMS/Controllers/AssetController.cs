using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public class AssetController : FieldController, IAssetController
    {

        public Asset Asset { get; set; }
        private IAssetRepository _assetRepository;
        
        public AssetController(Asset asset, IAssetRepository assetRepository) : base(asset)
        {
            Asset = asset;
            _assetRepository = assetRepository;
        }
        
        public bool Save()
        {
            SerializeFields();
            ulong id = 0;
            _assetRepository.Insert(Asset,out id);
            return id != 0;
        }

        public bool Update()
        {
            throw new System.NotImplementedException();
        }
        
    }
}