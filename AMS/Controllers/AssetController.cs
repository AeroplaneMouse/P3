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

        /// <summary>
        /// Attaches a tag and its fields to a asset.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool AttachTag(ITagable tag)
        {
            CurrentlyAddedTags.Add(tag);
            if (tag is Tag currentTag)
            {
                foreach (var tagField in currentTag.FieldList)
                {
                    if (Asset.FieldList.SingleOrDefault(assetField => assetField.Hash == tagField.Hash) == null)
                    {
                        Asset.FieldList.Add(tagField);
                    }
                    else
                    {
                        Asset.FieldList.Single(assetField => assetField.Hash == tagField.Hash).FieldPresentIn.Add(currentTag.ID);
                    }
                }
            }
            return CurrentlyAddedTags.Contains(tag);
        }

        /// <summary>
        /// Detaches tag from an asset.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool DetachTag(ITagable tag)
        {
            if (CurrentlyAddedTags.Contains(tag))
            {
                CurrentlyAddedTags.Remove(tag);
                if (tag is Tag currentTag)
                {
                    foreach (var field in currentTag.FieldList)
                    {
                        RemoveFieldOrFieldRelations(field, currentTag);
                    }
                }
            }

            return !CurrentlyAddedTags.Contains(tag);
        }

        /// <summary>
        /// Saves the asset to the database. As well as connects the tag in the tag repository.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            SerializeFields();
            ulong id = 0;
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            _assetRepository.Insert(Asset, out id);
            return id != 0;
        }

        /// <summary>
        /// Updates the asset in the database.
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            SerializeFields();
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            return _assetRepository.Update(Asset);
        }

        /// <summary>
        /// Removes the asset form the database.
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            return _assetRepository.Delete(Asset);
        }
    }
}