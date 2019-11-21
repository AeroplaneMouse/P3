using System.Collections.Generic;
using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Controllers
{
    public class AssetController : FieldListController, IAssetController
    {
        public Asset Asset { get; set; }
        public List<ITagable> CurrentlyAddedTags { get; } = new List<ITagable>();
        
        private IAssetRepository _assetRepository;
        

        public AssetController(Asset asset, IAssetRepository assetRepository) : base(asset)
        {
            Asset = asset;
            DeSerializeFields();
            _assetRepository = assetRepository;
        }

        public bool AttachTag(ITagable tag)
        {
            CurrentlyAddedTags.Add(tag);
            if (tag is Tag currentTag)
            {
                foreach (var tagField in currentTag.Fields)
                {
                    if (Asset.Fields.SingleOrDefault(assetField => assetField.Hash == tagField.Hash) == null)
                    {
                        Asset.Fields.Add(tagField);
                    }
                    else
                    {
                        Asset.Fields.Single(assetField => assetField.Hash == tagField.Hash).FieldPresentIn.Add(currentTag.ID);
                    }
                }
            }
            return CurrentlyAddedTags.Contains(tag);
        }

        public bool DetachTag(ITagable tag)
        {
            if (CurrentlyAddedTags.Contains(tag))
            {
                CurrentlyAddedTags.Remove(tag);
                if (tag is Tag currentTag)
                {
                    foreach (var field in currentTag.Fields)
                    {
                        RemoveFieldOrFieldRelations(field, currentTag);
                    }
                }
            }

            return !CurrentlyAddedTags.Contains(tag);
        }

        public bool Save()
        {
            SerializeFields();
            ulong id = 0;
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            _assetRepository.Insert(Asset, out id);
            return id != 0;
        }

        public bool Update()
        {
            SerializeFields();
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            return _assetRepository.Update(Asset);
        }

        public bool Remove()
        {
            return _assetRepository.Delete(Asset);
        }
    }
}