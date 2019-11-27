using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Controllers.Interfaces;
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
            {
                throw new NullReferenceException();
            }
            else
            {
                _fieldContainer = element;
            }
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
        /// <param name="field"></param>
        /// <param name="fieldContainer"></param>
        /// <returns></returns>
        public bool AddField(Field field, FieldContainer fieldContainer = null)
        {
            //Checks whether the field is present in HiddenFieldList, if not, checks NonHiddenFieldList.
            Field fieldInList = HiddenFieldList.FirstOrDefault(p => p.Equals(field)) ??
                                NonHiddenFieldList.FirstOrDefault(p => p.Equals(field));

            if (fieldInList == null)
            {
                if (fieldContainer != null)
                {
                    field.TagIDs.Add(fieldContainer.ID);
                }

                NonHiddenFieldList.Add(field);
            }
            else
            {
                if (fieldContainer != null && !fieldInList.TagIDs.Contains(fieldContainer.ID))
                {
                    fieldInList.TagIDs.Add(fieldContainer.ID);
                }
            }

            return _fieldContainer.FieldList.Contains(field);
        }

        /// <summary>
        /// Remove a field, or its relation to a tag, from the fields list.
        /// </summary>
        /// <param name="field">The field to update/remove</param>
        /// <param name="fieldContainer"></param>
        /// <returns>Rather the field was removed</returns>
        public bool RemoveField(Field field, FieldContainer fieldContainer = null)
        {
            if (field == null) return false;

            if (field.IsCustom)
            {
                NonHiddenFieldList.Remove(field);
                return true;
            }

            if (field.IsHidden)
            {
                if (!(_fieldContainer is Tag) && field.TagIDs.Count > 0)
                {
                    field.IsHidden = false;
                    NonHiddenFieldList.Add(field);
                }

                HiddenFieldList.Remove(field);
                return true;
            }

            if (!(_fieldContainer is Tag) && field.TagIDs.Count > 0)
            {
                field.IsHidden = true;
                HiddenFieldList.Add(field);
            }

            return NonHiddenFieldList.Remove(field);
        }


        public bool RemoveFieldRelations(ulong TagId)
        {
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