using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Models;
using Newtonsoft.Json;

namespace AMS.Controllers
{
    public abstract class FieldController
    {
        private ContainsFields _containsFields;
        
        public FieldController(ContainsFields element)
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
            _containsFields.Fields = JsonConvert.DeserializeObject<ObservableCollection<Field>>(_containsFields.SerializedFields);
            return _containsFields.Fields.Count > 0;
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