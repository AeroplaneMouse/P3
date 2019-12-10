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
        /// Add a field to the field list.
        /// </summary>
        /// <param name="inputField"></param>
        /// <param name="fieldContainer">Fieldcontainer is used when wanting to add a relation to a field when adding the field</param>
        /// <returns></returns>
        public bool AddField(Field inputField, FieldContainer fieldContainer = null)
        {
            // Does the field already exist in shown or hidden fields?
            Field fieldInListByHashId = HiddenFieldList.FirstOrDefault(p => p.HashId == inputField.HashId) ??
                                        NonHiddenFieldList.FirstOrDefault(p => p.HashId == inputField.HashId);
            
            // If field already exist check got updates
            if (fieldInListByHashId != null)
            {
                // Checks if a field label has been updated
                if (fieldInListByHashId.Label != inputField.Label)
                    fieldInListByHashId.Label = inputField.Label;

                // Checks if a fields required property has been updated
                if (fieldInListByHashId.Required != inputField.Required)
                    fieldInListByHashId.Required = inputField.Required;

                // Adds a reference to the field container if its added.
                if (fieldContainer != null && !fieldInListByHashId.TagIDs.Contains(fieldContainer.ID))
                    fieldInListByHashId.TagIDs.Add(fieldContainer.ID);

                fieldInListByHashId.IsCustom = false;
            }
            
            // Checks whether the field is present in HiddenFieldList, if not, checks NonHiddenFieldList.
            Field fieldInList = HiddenFieldList.FirstOrDefault(p => p.Equals(inputField)) ??
                                NonHiddenFieldList.FirstOrDefault(p => p.Equals(inputField));

            //If the field is not in the list, add the field (With or without a relation to the fieldContainer)
            if (fieldInList == null && fieldInListByHashId == null)
            {
                if (fieldContainer != null)
                    inputField.TagIDs.Add(fieldContainer.ID);

                NonHiddenFieldList.Add(new Field(inputField.Label, inputField.Content, inputField.Type,
                    inputField.HashId, inputField.Required, inputField.IsCustom, inputField.IsHidden,
                    inputField.TagIDs));
            }
            
            if (fieldInList != null)
                fieldInList.IsCustom = false;

            return _fieldContainer.FieldList.Contains(inputField);
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
            if (field == null) return false;

            //Checks if the field is custom (Ie made on a asset)
            if (field.IsCustom && !field.IsHidden)
            {
                NonHiddenFieldList.Remove(field);
                return true;
            }

            if (field.IsHidden)
            {
                field.IsHidden = false;
                if (!NonHiddenFieldList.Contains(field))
                {
                    NonHiddenFieldList.Add(field);
                }

                HiddenFieldList.Remove(field);
            }
            else
            {
                if (!(_fieldContainer is Tag))
                {
                    field.IsHidden = !field.IsHidden;
                    if (!HiddenFieldList.Contains(field))
                    {
                        HiddenFieldList.Add(field);
                    }
                }

                NonHiddenFieldList.Remove(field);
            }

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
            // Checks the hiddenlist and checks whether a field contains the tagID, if it does, remove it.
            foreach (var field in HiddenFieldList)
            {
                if (field.TagIDs.Contains(TagId))
                {
                    field.TagIDs.Remove(TagId);
                }
            }

            foreach (var field in NonHiddenFieldList)
            {
                if (field.TagIDs.Contains(TagId))
                {
                    field.TagIDs.Remove(TagId);
                }
            }

            return true;
        }
    }
}