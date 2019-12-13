using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public abstract class FieldListController : IFieldListController
    {
        protected FieldContainer _fieldContainer;

        protected FieldListController(FieldContainer element)
        {
            if (element == null)
                throw new ArgumentNullException();
            else
                _fieldContainer = element;
        }

        /// <summary>
        /// Saves the fields to the Serialized fields property.
        /// </summary>
        /// <returns>Serialization completed</returns>
        public bool SerializeFields()
        {
            _fieldContainer.SerializedFields = JsonConvert.SerializeObject(_fieldContainer.FieldList);
            return !string.IsNullOrEmpty(_fieldContainer.SerializedFields);
        }

        /// <summary>
        /// Add a field to the fieldContainers list. If it's already there, update label and required.
        /// If a field with same label and of same type, combine the two.
        /// </summary>
        /// <param name="inputField">The new field to be added</param>
        /// <returns>Wether or not the input field was added to the list</returns>
        public bool AddField(Field inputField)
        {
            // Check if field already exists on the fieldContainer
            Field field = _fieldContainer.FieldList.FirstOrDefault(f => f.HashId == inputField.HashId);

            // If it exist, update label and required
            if (field != null)
            {
                field.Label = inputField.Label;
                field.Required = inputField.Required;
                return false;
            }


            // Check if a field with same label or of same type is on the fieldContainer.
            field = _fieldContainer.FieldList.FirstOrDefault(f => f.Hash == inputField.Hash);

            // If so, combine them and set content if content is empty or equal
            if (field != null && (field.Content == String.Empty || field.Content == inputField.Content))
            {
                field.Content = inputField.Content;
                // Add the ID of the inputfields originating tag, if it aren't already.
                if (inputField.TagIDs.Count > 0 && !field.TagIDs.Contains(inputField.TagIDs.First()))
                    field.TagIDs.Add(inputField.TagIDs.First());
                return false;
            }


            // If the fieldContainer is a tag, add the tags ID to the field
            if (_fieldContainer is Tag && _fieldContainer.ID != 0)
                inputField.TagIDs.Add(_fieldContainer.ID);

            // Add the field to the fieldContainer
            _fieldContainer.FieldList.Add(new Field(inputField.Label, inputField.Content, inputField.Type,
                inputField.HashId, inputField.Required, inputField.IsCustom, inputField.IsHidden,
                inputField.TagIDs));

            return true;
        }

        /// <summary>
        /// Removes custom fields and empty fields with no tag relations. Non empty field with no tag relations is made custom.
        /// Fields with tag relations would have its IsHidden property toggled.
        /// </summary>
        /// <param name="field">The field to update/remove</param>
        /// <returns>Always returns true</returns>
        public bool RemoveField(Field field)
        {
            // Remove custom asset field or field when editing tag
            if (field.IsCustom || _fieldContainer is Tag)
                _fieldContainer.FieldList.Remove(field);
            else
            {
                // Remove field if it has not relations to tags and do not contain anything
                if (field.TagIDs.Count == 0 && field.Content == String.Empty)
                    _fieldContainer.FieldList.Remove(field);

                // Make custom, if there is no relations to tag and content is not empty
                else if (field.TagIDs.Count == 0)
                {
                    field.IsCustom = true;
                    field.IsHidden = false;
                    // Update hashID, since it is now a new field, and shouldn't have any relation to the tag it originated from.
                    field.UpdateHashID();
                }
                
                // Toggle the hidden state of the field.
                else
                    field.IsHidden = !field.IsHidden;
            }

            return true;
        }

        /// <summary>
        /// Removes any relation to the given tag. If there are no relations left on the field,
        /// remove the field, if it's empty, otherwise make it custom.
        /// </summary>
        /// <param name="tag">The tag for which all the fields relation to, should be removed.</param>
        /// <returns>Always return true</returns>
        public bool RemoveTagRelationsOnFields(ITagable tag)
        {
            List<Field> fieldsToRemove = new List<Field>();

            // Update all the fields
            foreach(Field field in _fieldContainer.FieldList)
            {
                // Only act on non-custom fields
                if (!field.IsCustom)
                {
                    //if (field.TagIDs.Count == 1 && tag is Tag currentTag)
                    //{
                    //    Field tagField = currentTag.FieldList.FirstOrDefault(f => f.HashId == field.HashId);
                    //    if (tagField != null && tagField.Content == field.Content)
                    //        field.Content = "";
                    //}

                    if (field.TagIDs.Contains(tag.TagId))
                    {
                        field.TagIDs.Remove(tag.TagId);
                        field.TagList.Remove(tag);
                    }

                    // Check if the field has any relations to tags left on it
                    if (field.TagIDs.Count == 0)
                        fieldsToRemove.Add(field);
                }
            }

            // Remove the fields, that was marked
            foreach (Field field in fieldsToRemove)
                RemoveField(field);

            return true;
        }
    }
}