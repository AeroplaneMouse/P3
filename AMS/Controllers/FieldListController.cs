using System.Collections.ObjectModel;
using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Models;
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public abstract class FieldListController : IFieldListController
    {
        private ContainsFields _containsFields;

        protected FieldListController(ContainsFields element)
        {
            _containsFields = element;
        }

        public bool SerializeFields()
        {
            _containsFields.SerializedFields = JsonConvert.SerializeObject(_containsFields.Fields);
            return !string.IsNullOrEmpty(_containsFields.SerializedFields);
        }

        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(_containsFields.SerializedFields))
            {
                _containsFields.Fields =
                    JsonConvert.DeserializeObject<ObservableCollection<Field>>(_containsFields.SerializedFields);
                return true;
            }

            return false;
        }

        public bool AddField(Field field)
        {
            _containsFields.Fields.Add(field);
            return _containsFields.Fields.Contains(field);
        }

        public bool RemoveFieldOrFieldRelations(Field inputField, Tag tag = null)
        {
            Field currentField = _containsFields.Fields.SingleOrDefault(field => field.Equals(inputField));
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
                        _containsFields.Fields.Remove(inputField);
                    }
                }
                else
                {
                    _containsFields.Fields.Remove(inputField);
                }
            }

            return !_containsFields.Fields.Contains(inputField);
        }
    }
}