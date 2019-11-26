using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using AMS.Logging.Interfaces;
using AMS.Models;

namespace AMS.Controllers
{
    public class AssetController : FieldListController, IAssetController
    {
        public Asset Asset { get; set; }
        public List<ITagable> CurrentlyAddedTags { get; set; } = new List<ITagable>();
        private ILogger logger;
        private IAssetRepository _assetRepository;

        public string name;
        public string identifier;
        public string description;

        public AssetController(Asset asset, IAssetRepository assetRepository) : base(asset)
        {
            if(asset == null)
            {
                Asset = new Asset();
            }
            else
            {
                Asset = asset;
            }
            NonHiddenFieldList = Asset.FieldList.Where(f => f.IsHidden == false).ToList();
            HiddenFieldList = Asset.FieldList.Where(f => f.IsHidden == true).ToList();
            _assetRepository = assetRepository;
            CurrentlyAddedTags = _assetRepository.GetTags(Asset).ToList();
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
                    AddField(tagField,currentTag);
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
            if(tag == null)
            {
                return false;
            }
            if (CurrentlyAddedTags.Contains(tag))
            {
                CurrentlyAddedTags.Remove(tag);
                if (tag is Tag currentTag)
                {
                    foreach (var field in currentTag.FieldList)
                    {
                        if(field.TagIDs.Count == 1 && field.TagIDs.Contains(currentTag.ID))
                        {
                            RemoveField(field);
                        }
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
            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            Asset.FieldList = fieldList;
            SerializeFields();
            ulong id = 0;
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            // Log creation of an asset if repository insert was successful
            bool success = _assetRepository.Insert(Asset, out id);
            return id != 0;
        }

        /// <summary>
        /// Updates the asset in the database.
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            Asset.FieldList = fieldList;
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