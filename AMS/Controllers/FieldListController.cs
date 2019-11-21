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

        protected FieldListController(FieldContainer element)
        {
            _fieldContainer = element;
        }

        public bool SerializeFields()
        {
            _fieldContainer.SerializedFields = JsonConvert.SerializeObject(_fieldContainer.FieldList);
            return !string.IsNullOrEmpty(_fieldContainer.SerializedFields);
        }

        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(_fieldContainer.SerializedFields))
            {
                _fieldContainer.FieldList =
                    JsonConvert.DeserializeObject<ObservableCollection<Field>>(_fieldContainer.SerializedFields);
                return true;
            }

            return false;
        }

        public bool AddField(Field field)
        {
            _fieldContainer.FieldList.Add(field);
            return _fieldContainer.FieldList.Contains(field);
        }

        public bool RemoveFieldOrFieldRelations(Field inputField, Tag tag = null)
        {
            Field currentField = _fieldContainer.FieldList.SingleOrDefault(field => field.Equals(inputField));
            if (currentField != null)
            {
                if (inputField.IsCustom)
                {
                    if (tag != null)
                    {
                        currentField.FieldPresentIn.Remove(tag.ID);
                    }
                    if (inputField.FieldPresentIn.Count > 0)
                    {
                        currentField.IsHidden = !currentField.IsHidden;
                    }
                    else
                    {
                        _fieldContainer.FieldList.Remove(inputField);
                    }
                }
                else
                {
                    _fieldContainer.FieldList.Remove(inputField);
                }
            }

            return !_fieldContainer.FieldList.Contains(inputField);
        }
    }
}