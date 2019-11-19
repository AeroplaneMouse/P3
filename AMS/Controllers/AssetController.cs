using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;

namespace AMS.Controllers
{
    public class AssetController : FieldController, IAssetController
    {

        public Asset Asset { get; set; }
        private IAssetRepository _assetRepository;
        
        public AssetController(Asset asset, IAssetRepository assetRepository) : base(asset)
        {
            Asset = asset;
            DeSerializeFields();
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
            SerializeFields();
            return _assetRepository.Update(Asset);
        }

        public bool Remove()
        {
            return _assetRepository.Delete(Asset);
        }
    }
}