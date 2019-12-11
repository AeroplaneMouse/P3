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

        /// <summary>
        /// Attaches an ITagable and its fields to the controlled asset.
        /// </summary>
        /// <param name="tag">Tag ITagable that should be attached</param>
        public void AttachTags(ITagable tag)
        {
            List<ITagable> tagList = new List<ITagable>();
            tagList.Add(tag);
            AttachTags(tagList);
        }

        /// <summary>
        /// Attaches a list of ITagable and their fields to the controlled asset.
        /// </summary>
        /// <param name="tags">The list of ITagable that should be attached</param>
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

                        // Adds all the fields to the asset
                        foreach (Field tagField in currentTag.FieldList)
                            AddField(tagField);
                    }
                }
            }

            // Update the content of any field that has a placeholder text. (like "Current date")
            UpdateFieldContent();
        }

        /// <summary>
        /// Detaches an ITagable and its fields from the controlled asset.
        /// </summary>
        /// <param name="tag">The ITagable that should be detached</param>
        public void DetachTags(ITagable tag)
        {
            List<ITagable> tagList = new List<ITagable>();
            tagList.Add(tag);
            DetachTags(tagList);
        }

        /// <summary>
        /// Detaches a list of ITagable and their field from the controlled asset.
        /// </summary>
        /// <param name="tags">The list of ITagble that should be detached</param>
        public void DetachTags(List<ITagable> tags)
        {
            foreach (ITagable tag in tags)
            {
                // Check if the tag is in the list.
                if (CurrentlyAddedTags.Contains(tag))
                {
                    CurrentlyAddedTags.Remove(tag);

                    // Checks if the ITagable is a Tag, remove its fields.
                    if (tag is Tag currentTag)
                    {
                        // DeSerialize fields if there aren't any. It might be a parent tag.
                        if(currentTag.FieldList.Count == 0)
                            currentTag.DeSerializeFields();

                        // Remove relations to the tag from the fields and handle wether or not the field itself should be removed.
                        RemoveTagRelationsOnFields(currentTag);
                    }
                }
            }
        }

        /// <summary>
        /// Update any tag-inheritet old, new and removed fields.
        /// </summary>
        private void LoadTags()
        {
            // For every tag
            foreach (var tag in CurrentlyAddedTags)
            {
                if (tag is Tag currentTag)
                {
                    currentTag.DeSerializeFields();
                    foreach (var tagField in currentTag.FieldList)
                        AddField(tagField);
                }
            }

            // For every field that is not custom, check if its related tags still have the field associated.
            foreach(Field field in ControlledAsset.FieldList)
            {
                if (!field.IsCustom)
                {
                    // Find any tagIDs that should no longer be related to the field
                    List<ulong> idsToRemove = new List<ulong>();
                    foreach(ulong id in field.TagIDs)
                    {
                        ITagable tag = GetTagFromID(id);
                        if (!IsTagContainingField(tag, field))
                            idsToRemove.Add(id);
                    }

                    // Remove the tagIDs from the field.
                    foreach (ulong id in idsToRemove)
                        field.TagIDs.Remove(id);

                }
            }

            // Check the TagFieldRelations, remove any field that has no relations
            RemoveFieldsWithNoTagRelations();
        }

        /// <summary>
        /// Removes any field that have no relations to tags and any content. Fields with no relations,
        /// but with content, is made custom.
        /// </summary>
        private void RemoveFieldsWithNoTagRelations()
        {
            List<Field> fieldsToRemove = new List<Field>();
            foreach(Field field in ControlledAsset.FieldList)
            {
                if (!field.IsCustom && field.TagIDs.Count == 0)
                    fieldsToRemove.Add(field);
            }

            // Remove the marked fields
            foreach (Field field in fieldsToRemove)
                RemoveField(field);
        }

        /// <summary>
        /// Gets the full tag from its ID in the list currently add tags.
        /// </summary>
        /// <param name="id">The Id of the full tag, found in the CurrentlyAddTags list</param>
        /// <returns>The full tag object</returns>
        private ITagable GetTagFromID(ulong id)
        {
            return CurrentlyAddedTags.FirstOrDefault(t => t.TagId == id);
        }

        /// <summary>
        /// Checks wether or not a tag contains the given field.
        /// </summary>
        /// <param name="tag">The tag which should be check to have the given field</param>
        /// <param name="field">The field to be check if its contained</param>
        /// <returns>Returns wethere or not the given field was contained in the given tag</returns>
        private bool IsTagContainingField(ITagable tag, Field field)
        {
            if (tag is Tag currentTag)
            {
                Field containedField = currentTag.FieldList.FirstOrDefault(f => f.Hash == field.Hash);
                return containedField != null;
            }
            else
                return false;
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

        public void UpdateFieldContent()
        {
            foreach(Field field in ControlledAsset.FieldList)
            {
                // Date fields
                if (field.Type == Field.FieldType.Date && string.Equals(field.Content, "Current Date"))
                    field.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                // Checkbox fields
                if (field.Type == Field.FieldType.Checkbox && string.IsNullOrEmpty(field.Content))
                    field.Content = false.ToString();
            }
        }
    }
}