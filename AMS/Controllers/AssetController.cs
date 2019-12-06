using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AMS.Authentication;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Events;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Controllers
{
    public class AssetController : FieldListController, IAssetController
    {
        public Asset ControlledAsset { get; set; }

        private List<ITagable> _tags;

        public List<ITagable> CurrentlyAddedTags
        {
            get
            {
                if (_tags == null)
                {
                    _tags = _assetRepository.GetTags(ControlledAsset).ToList();
                }

                return _tags;
            }

            set => _tags = value;
        }

        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }

        private IAssetRepository _assetRepository;
        private Session _session;

        public AssetController(Asset asset, IAssetRepository assetRepository, Session session) 
            : base(asset)
        {
            ControlledAsset = asset;

            _assetRepository = assetRepository;
            _session = session;

            ControlledAsset.DeSerializeFields();

            Name = ControlledAsset.Name;
            Identifier = ControlledAsset.Identifier;
            Description = ControlledAsset.Description;
            NonHiddenFieldList = ControlledAsset.FieldList.Where(f => f.IsHidden == false).ToList();
            HiddenFieldList = ControlledAsset.FieldList.Where(f => f.IsHidden == true).ToList();
            LoadTags();
            LoadFields();
        }
        
        /// <summary>
        /// Saves the asset to the database. As well as connects the tag in the tag repository.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            //Updates the fields on the asset
            if (Name != ControlledAsset.Name)
                ControlledAsset.Name = Name;

            if (ControlledAsset.Identifier != Identifier)
                ControlledAsset.Identifier = Identifier;

            if (ControlledAsset.Description != Description)
                ControlledAsset.Description = Description;

            ControlledAsset.DepartmentID = _session.user.DefaultDepartment;

            //Combines the two lists
            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            ControlledAsset.FieldList = fieldList;
            SerializeFields();

            //Database saving
            ulong id = 0;
            Asset insertedAsset = _assetRepository.Insert(ControlledAsset, out id);
            _assetRepository.AttachTags(insertedAsset, CurrentlyAddedTags);

            //Asset saved verification
            return id != 0;
        }

        /// <summary>
        /// Updates the asset in the database.
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            if (ControlledAsset.Name != Name)
                ControlledAsset.Name = Name;

            if (ControlledAsset.Identifier != Identifier)
                ControlledAsset.Identifier = Identifier;

            if (ControlledAsset.Description != Description)
                ControlledAsset.Description = Description;

            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            ControlledAsset.FieldList = fieldList;
            SerializeFields();
            _assetRepository.AttachTags(ControlledAsset, CurrentlyAddedTags);
            return _assetRepository.Update(ControlledAsset);
        }

        /// <summary>
        /// Removes the asset form the database.
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            //Deletes the asset from the database.
            return _assetRepository.Delete(ControlledAsset);
        }

        /// <summary>
        /// Resets the attributes of the controller to correspond with the attributes on the asset
        /// </summary>
        public void RevertChanges()
        {
            Name = ControlledAsset.Name;
            Identifier = ControlledAsset.Identifier;
            Description = ControlledAsset.Description;
            _tags = _assetRepository.GetTags(ControlledAsset).ToList();
        }

        /// <summary>
        /// Attaches a tag and its fields to a asset.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool AttachTag(ITagable tag)
        {
            if (!CurrentlyAddedTags.Contains(tag))
            {
                CurrentlyAddedTags.Add(tag);
                if (tag is Tag currentTag)
                {
                    //DeSerialize the fields, so the fieldList is instantiated
                    currentTag.DeSerializeFields();
                    foreach (var tagField in currentTag.FieldList)
                    {
                        AddField(tagField, currentTag);
                    }
                }
            }

            LoadFields();
            return CurrentlyAddedTags.Contains(tag);
        }

        /// <summary>
        /// Detaches tag from an asset.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool DetachTag(ITagable tag)
        {
            
            //If no tag was given, return.
            if (tag == null)
                return false;

            //Check if the tag is in the list.
            if (CurrentlyAddedTags.Contains(tag))
            {
                List<Field> removeFields = new List<Field>();
                CurrentlyAddedTags.Remove(tag);

                //Checks if the ITagable is a Tag.
                if (tag is Tag currentTag)
                {
                    List<Field> removelist = currentTag.FieldList;
                    //Remove relations to the field.
                    RemoveTagRelationsOnFields(currentTag.ID);

                    //Remove a fields relation to the parent tag, if no other tag with the same parent tag exists in CurrentlyAddedTags.
                    if (!CurrentlyAddedTags.Any(p => p.ParentId == currentTag.ParentId && p.TagId != currentTag.ID))
                    {
                        RemoveTagRelationsOnFields(currentTag.ParentId);
                        Tag parentTag = (Tag)CurrentlyAddedTags.SingleOrDefault(p => p.TagId == currentTag.ParentId);
                        if (parentTag != null) removelist.AddRange(parentTag.FieldList);
                        CurrentlyAddedTags.RemoveAll(p=>p.TagId == currentTag.ParentId);
                        
                    }

                    //Checks if the field is in the fieldList on the asset, and the tag, if so, remove it.
                    foreach (var field in removelist)
                    {
                        Field fieldInList = HiddenFieldList.FirstOrDefault(p => p.Equals(field)) ??
                                            NonHiddenFieldList.FirstOrDefault(p => p.Equals(field));
                        if (fieldInList != null)
                            removeFields.Add(fieldInList);
                    }

                    //Remove the fields.
                    foreach (var field in removeFields)
                        HandleFieldsFromRemoveTag(field,currentTag);
                }
            }
            LoadFields();
            return !CurrentlyAddedTags.Contains(tag);
        }

        /// <summary>
        /// Loads the tags when opening the page, and adds any fields added to the tag since the editor was last opened.
        /// </summary>
        private void LoadTags()
        {
            foreach (var tag in CurrentlyAddedTags)
            {
                if (tag is Tag currentTag)
                {
                    currentTag.DeSerializeFields();
                    foreach (var tagField in currentTag.FieldList)
                        AddField(tagField, currentTag);
                }
            }
        }

        /// <summary>
        /// Runs on startup, loads fields, and updates fields that are dependent on values.
        /// </summary>
        private void LoadFields()
        {
            foreach (var field in HiddenFieldList)
            {
                if (field.Type == Field.FieldType.Date && string.Equals(field.Content, "Current Date"))
                {
                    field.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                }
            }

            foreach (var field in NonHiddenFieldList)
            {
                if (field.Type == Field.FieldType.Date)
                {
                    Console.WriteLine(field.Content);
                }

                if (field.Type == Field.FieldType.Date && string.Equals(field.Content, "Current Date"))
                {
                    field.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    }
}