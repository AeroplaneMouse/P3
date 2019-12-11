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
        private FieldContainer _fieldContainer;

        public List<Field> NonHiddenFieldList { get; set; } = new List<Field>();
        public List<Field> HiddenFieldList { get; set; } = new List<Field>();

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

            // If it does exist, update label and required
            if (field != null)
            {
                field.Label = inputField.Label;
                field.Required = inputField.Required;
                return false;
            }


            // Check if a field with same label or of same type is on the fieldContainer.
            field = _fieldContainer.FieldList.FirstOrDefault(f => f.Equals(inputField));

            // If so, combine them and set content if content is empty
            if (field != null && field.Content != String.Empty)
            {
                field.Content = inputField.Content;
                // Add the ID of the inputfields originating tag.
                field.TagIDs.Add(inputField.TagIDs.First());
                return false;
            }


            // If the fieldContainer is a tag, add the tags ID to the field
            if (_fieldContainer is Tag)
                inputField.TagIDs.Add(_fieldContainer.ID);

            // Add the field to the fieldContainer
            _fieldContainer.FieldList.Add(new Field(inputField.Label, inputField.Content, inputField.Type,
                inputField.HashId, inputField.Required, inputField.IsCustom, inputField.IsHidden,
                inputField.TagIDs));

            return true;
        }

        /// <summary>
        /// Remove a field, or its relation to a tag, from the fields list.
        /// </summary>
        /// <param name="field">The field to update/remove</param>
        /// <param name="fieldContainer"></param>
        /// <returns>Rather the field was removed</returns>
        public bool RemoveField(Field field)
        {
            //If no input field is given, return.
            //if (field == null) 
            //    return false;

            // Remove custom asset field or field when editing tag
            if (field.IsCustom || _fieldContainer is Tag)
                _fieldContainer.FieldList.Remove(field);
            else
                // Toggle the hidden state of the field.
                field.IsHidden = !field.IsHidden;


            //Checks if the field is custom (Ie made on a asset)
            //if (field.IsCustom && !field.IsHidden)
            //{
            //    NonHiddenFieldList.Remove(field);
            //    return true;
            //}

            //if (field.IsHidden)
            //{
            //    field.IsHidden = false;
            //    if (!NonHiddenFieldList.Contains(field))
            //    {
            //        NonHiddenFieldList.Add(field);
            //    }

            //    HiddenFieldList.Remove(field);
            //}
            //else
            //{
            //    if (!(_fieldContainer is Tag))
            //    {
            //        field.IsHidden = !field.IsHidden;
            //        if (!HiddenFieldList.Contains(field))
            //        {
            //            HiddenFieldList.Add(field);
            //        }
            //    }

            //    //NonHiddenFieldList.Remove(field);
            //    _fieldContainer.FieldList.Remove(field);
            //}

            return true;
        }

        public bool HandleFieldsFromRemoveTag(Field inputField, Tag tagable)
        {
            if (!inputField.IsCustom && inputField.Content ==
                tagable.FieldList.SingleOrDefault(p => p.Hash == inputField.Hash)?.Content)
            {
                if (inputField.IsHidden)
                    HiddenFieldList.Remove(inputField);
                else
                    NonHiddenFieldList.Remove(inputField);

                return true;
            }
            else
            {
                inputField.IsCustom = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the relation between a tagID and any of the fields.
        /// </summary>
        /// <param name="TagId"></param>
        /// <returns></returns>
        public bool RemoveTagRelationsOnFields(ulong TagId)
        {
            foreach(Field field in _fieldContainer.FieldList)
            {
                if (field.TagIDs.Contains(TagId))
                {
                    field.TagIDs.Remove(TagId);
                }
            }

            // Checks the hiddenlist and checks whether a field contains the tagID, if it does, remove it.
            //foreach (var field in HiddenFieldList)
            //{
            //    if (field.TagIDs.Contains(TagId))
            //    {
            //        field.TagIDs.Remove(TagId);
            //    }
            //}

            //foreach (var field in NonHiddenFieldList)
            //{
            //    if (field.TagIDs.Contains(TagId))
            //    {
            //        field.TagIDs.Remove(TagId);
            //    }
            //}

            return true;
        }
    }
}