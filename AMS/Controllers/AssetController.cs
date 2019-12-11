using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        private List<ITagable> _tags;
        private IAssetRepository _assetRepository;
        private Session _session;

        public Asset ControlledAsset { get; set; }
        public List<ITagable> CurrentlyAddedTags
        {
            get
            {
                if (_tags == null)
                    _tags = _assetRepository.GetTags(ControlledAsset).ToList();

                return _tags;
            }
            set => _tags = value;
        }

        public AssetController(Asset asset, IAssetRepository assetRepository, Session session)
            : base(asset)
        {
            ControlledAsset = asset;

            _assetRepository = assetRepository;
            _session = session;

            ControlledAsset.DeSerializeFields();
            LoadTags();
            LoadFields();
        }

        /// <summary>
        /// Saves the asset to the database. As well as connects the tag in the tag repository.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            // Save the current department onto the asset.
            ControlledAsset.DepartmentdId = _session.user.DefaultDepartment;
            SerializeFields();

            // Database saving
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
            SerializeFields();
            _assetRepository.AttachTags(ControlledAsset, CurrentlyAddedTags);
            return _assetRepository.Update(ControlledAsset);
        }

        /// <summary>
        /// Removes the asset from the database
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            return _assetRepository.Delete(ControlledAsset);
        }

        /// <summary>
        /// Resets the attributes of the controller to correspond with the attributes on the asset
        /// </summary>
        public void RevertChanges()
        {
            foreach (PropertyInfo property in ControlledAsset.GetType().GetProperties())
            {
                if (ControlledAsset.Changes.ContainsKey(property.Name))
                    property.SetValue(ControlledAsset, ControlledAsset.Changes[property.Name].ToString());
            }

            ControlledAsset.Changes = new Dictionary<string, object>();
        }

        public void AttachTags(ITagable tag)
        {
            List<ITagable> tagList = new List<ITagable>();
            tagList.Add(tag);
            AttachTags(tagList);
        }

        /// <summary>
        /// Attaches a tag and its fields to a asset.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public void AttachTags(List<ITagable> tags)
        {
            foreach(ITagable tag in tags)
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
                            //AddField(tagField, currentTag);
                            AddField(tagField);
                        }
                    }
                }
            }

            LoadFields();
        }

        public void DetachTags(ITagable tag)
        {
            List<ITagable> tagList = new List<ITagable>();
            tagList.Add(tag);
            DetachTags(tagList);
        }

        /// <summary>
        /// Detaches tag from an asset.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public void DetachTags(List<ITagable> tags)
        {
            foreach (ITagable tag in tags)
            {
                // Check if the tag is in the list.
                if (CurrentlyAddedTags.Contains(tag))
                {
                    List<Field> removeFields = new List<Field>();
                    CurrentlyAddedTags.Remove(tag);

                    //Checks if the ITagable is a Tag.
                    if (tag is Tag currentTag)
                    {
                        if(currentTag.FieldList.Count == 0)
                            currentTag.DeSerializeFields();
                        
                        List<Field> removelist = currentTag.FieldList;
                        //Remove relations to the field.
                        RemoveTagRelationsOnFields(currentTag.ID);

                        //Remove a fields relation to the parent tag, if no other tag with the same parent tag exists in CurrentlyAddedTags.
                        //if (!CurrentlyAddedTags.Any(p => p.ParentId == currentTag.ParentId && p.TagId != currentTag.ID))
                        //{
                        //    RemoveTagRelationsOnFields(currentTag.ParentId);
                        //    Tag parentTag = (Tag)CurrentlyAddedTags.SingleOrDefault(p => p.TagId == currentTag.ParentId);
                        //    if (parentTag != null) removelist.AddRange(parentTag.FieldList);
                        //    CurrentlyAddedTags.RemoveAll(p => p.TagId == currentTag.ParentId);
                        //}

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
                            HandleFieldsFromRemoveTag(field, currentTag);
                    }
                }
            }

            LoadFields();
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
                        //AddField(tagField, currentTag);
                        AddField(tagField);
                }
            }
        }

        /// <summary>
        /// Runs on startup, loads fields, and updates fields that are dependent on values.
        /// </summary>
        public void LoadFields()
        {


            //foreach (var field in HiddenFieldList)
            //{
            //    if (field.Type == Field.FieldType.Date && string.Equals(field.Content, "Current Date"))
            //        field.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            //    if (field.Type == Field.FieldType.Checkbox && string.IsNullOrEmpty(field.Content))
            //        field.Content = false.ToString();
            //}

            //foreach (var field in NonHiddenFieldList)
            //{
            //    if (field.Type == Field.FieldType.Date && string.Equals(field.Content, "Current Date"))
            //        field.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            //    if (field.Type == Field.FieldType.Checkbox && string.IsNullOrEmpty(field.Content))
            //        field.Content = false.ToString();
            //}
        }
    }
}