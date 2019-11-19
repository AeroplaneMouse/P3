using System.Collections.ObjectModel;
using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Models;
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public abstract class FieldController : IFieldController
    {
        private ContainsFields _containsFields;

        protected FieldController(ContainsFields element)
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
                _containsFields.Fields = JsonConvert.DeserializeObject<ObservableCollection<Field>>(_containsFields.SerializedFields);
                return true;
            }

            return false;
        }
        
        public bool AddField(Field field)
        {
            _containsFields.Fields.Add(field);
            return _containsFields.Fields.Contains(field);
        }

        public bool RemoveField(Field inputField)
        {
            if (_containsFields.Fields.SingleOrDefault(field => field.Equals(inputField))!= null)
            {
                _containsFields.Fields.Remove(inputField);
            }

            return !_containsFields.Fields.Contains(inputField);
        }
    }
}