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
        /// Loads the fields from the serialized fields property.
        /// </summary>
        /// <returns>Load successfull</returns>
        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(_fieldContainer.SerializedFields))
            {
                _fieldContainer.FieldList =
                    JsonConvert.DeserializeObject<List<Field>>(_fieldContainer.SerializedFields);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a field to the field list.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool AddField(Field field)
        {
            NonHiddenFieldList.Add(field);
            return _fieldContainer.FieldList.Contains(field);
        }

        /// <summary>
        /// Remove a field, or its relation to a tag, from the fields list.
        /// </summary>
        /// <param name="inputField">The field to update/remove</param>
        /// <returns>Rather the field was removed</returns>
        public bool RemoveField(Field field)
        {
            if (field != null)
            {
                if (field.IsCustom)
                {
                    NonHiddenFieldList.Remove(field);
                    return true;
                }
                else
                {
                    if (field.IsHidden)
                    {
                        field.IsHidden = false;
                        NonHiddenFieldList.Add(field);
                        HiddenFieldList.Remove(field);
                        return true;
                    }
                    else
                    {
                        field.IsHidden = true;
                        if (!(_fieldContainer is Tag && field.TagIDs.Count == 1 && field.TagIDs.Contains(_fieldContainer.ID)))
                        {
                            HiddenFieldList.Add(field);    
                        }
                        NonHiddenFieldList.Remove(field);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}