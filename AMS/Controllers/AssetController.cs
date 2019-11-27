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
using AMS.ViewModels;

namespace AMS.Controllers
{
    public class AssetController : FieldListController, IAssetController
    {
        public Asset Asset { get; set; }
        public List<ITagable> CurrentlyAddedTags { get; set; } = new List<ITagable>();

        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }

        private IAssetRepository _assetRepository;

        public AssetController(Asset asset, IAssetRepository assetRepository) : base(asset ?? new Asset())
        {
            if (asset == null)
            {
                Asset = new Asset();
            }
            else
            {
                Asset = asset;
            }

            Name = Asset.Name;
            Identifier = Asset.Identifier;
            Description = Asset.Description;
            NonHiddenFieldList = Asset.FieldList.Where(f => f.IsHidden == false).ToList();
            HiddenFieldList = Asset.FieldList.Where(f => f.IsHidden == true).ToList();
            _assetRepository = assetRepository;
            CurrentlyAddedTags = _assetRepository.GetTags(Asset).ToList();
            LoadTags();
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
                    AddField(tagField, currentTag);
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
            if (tag == null)
            {
                return false;
            }

            if (CurrentlyAddedTags.Contains(tag))
            {
                List<Field> removeFields = new List<Field>();
                CurrentlyAddedTags.Remove(tag);
                
                if (tag is Tag currentTag)
                {
                    RemoveFieldRelations(currentTag.ID);
                    RemoveFieldRelations(currentTag.ParentID);
                    
                    foreach (var field in currentTag.FieldList)
                    {
                        Field fieldInList = HiddenFieldList.FirstOrDefault(p => p.Equals(field)) ??
                                            NonHiddenFieldList.FirstOrDefault(p => p.Equals(field));
                        if (fieldInList != null)
                            removeFields.Add(fieldInList);
                    }

                    foreach (var field in removeFields)
                    {
                        RemoveField(field);
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
            if (Name != Asset.Name)
            {
                Asset.Name = Name;
            }

            if (Asset.Identifier != Identifier)
            {
                Asset.Identifier = Identifier;
            }

            if (Asset.Description != Description)
            {
                Asset.Description = Description;
            }

            Asset.DepartmentID = Features.GetCurrentSession().user.DefaultDepartment;

            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            Asset.FieldList = fieldList;
            SerializeFields();
            ulong id = 0;
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            bool success = _assetRepository.Insert(Asset, out id);
            return id != 0;
        }

        /// <summary>
        /// Updates the asset in the database.
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            if (Asset.Name != Name)
            {
                Asset.Name = Name;
            }

            if (Asset.Identifier != Identifier)
            {
                Asset.Identifier = Identifier;
            }

            if (Asset.Description != Description)
            {
                Asset.Description = Description;
            }

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

        private void LoadTags()
        {
            foreach (var tag in CurrentlyAddedTags)
            {
                if (tag is Tag currentTag)
                {
                    foreach (var tagField in currentTag.FieldList)
                    {
                        AddField(tagField, currentTag);
                    }
                }
            }
        }
    }
}