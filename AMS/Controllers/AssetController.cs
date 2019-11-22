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
        public List<ITagable> CurrentlyAddedTags { get; set; } = new List<ITagable>();
        public List<Field> FieldList { get; set; } = new List<Field>();
        private ILogger logger;
        private IAssetRepository _assetRepository;
        

        public AssetController(Asset asset, IAssetRepository assetRepository) : base(asset)
        {
            Asset = asset;
            DeSerializeFields();
            FieldList = asset.FieldList.ToList<Field>();
            _assetRepository = assetRepository;
            logger = new Log(new LogRepository());
        }

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
            logger.LogCreate(this);
            return CurrentlyAddedTags.Contains(tag);
        }

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

        public bool Save()
        {
            Asset.FieldList = new ObservableCollection<Field>(FieldList);
            SerializeFields();
            ulong id = 0;
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            _assetRepository.Insert(Asset, out id);
            logger.LogCreate(this);
            return id != 0;
        }

        public bool Update()
        {
            Asset.FieldList = new ObservableCollection<Field>(FieldList);
            SerializeFields();
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            logger.LogCreate(this);
            return _assetRepository.Update(Asset);
        }

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
        /// Returns the name of the asset
        /// </summary>
        /// <returns>Name of the asset</returns>
        public string GetLoggableTypeName() => Asset.Name;
    }
}