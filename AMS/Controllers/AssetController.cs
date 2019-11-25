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
    public class AssetController : FieldListController, IAssetController, ILoggableValues
    {
        public Asset Asset { get; set; }
        public List<ITagable> CurrentlyAddedTags { get; set; }
        private ILogger logger;
        private IAssetRepository _assetRepository;
        

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
                    if (Asset.FieldList.SingleOrDefault(assetField => assetField.Hash == tagField.Hash) == null)
                    {
                        Asset.FieldList.Add(tagField);
                    }
                    else
                    {
                        Asset.FieldList.Single(assetField => assetField.Hash == tagField.Hash).TagIDs.Add(currentTag.ID);
                    }
                }
            }
            logger.LogCreate(this);
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
            if(success)
                logger.LogCreate(this);
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
            logger.LogCreate(this);
            return _assetRepository.Update(Asset);
        }

        /// <summary>
        /// Removes the asset form the database.
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            logger.LogCreate(this);
            return _assetRepository.Delete(Asset);
        }

        /// <summary>
        /// Makes a loggable dictionary from the asset
        /// </summary>
        /// <returns>The asset formatted as a loggable dictionary</returns>
        public Dictionary<string, string> GetLoggableValues()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.Add("ID", Asset.ID.ToString());
            props.Add("Name", Asset.Name);
            props.Add("Description", Asset.Description);
            props.Add("Department ID", Asset.DepartmentID.ToString());
            SerializeFields();
            props.Add("Options", Asset.SerializedFields);
            props.Add("Created at", Asset.CreatedAt.ToString());
            props.Add("Last updated at", Asset.UpdatedAt.ToString());
            return props;

        }

        /// <summary>
        /// Returns the name of the type asset
        /// </summary>
        /// <returns>Name of the asset type</returns>
        public string GetLoggableTypeName() => "Asset";
    }
}