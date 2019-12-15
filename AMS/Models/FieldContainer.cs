using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMS.Models
{
    public abstract class FieldContainer : Model
    {
        public List<Field> FieldList = new List<Field>();
        public List<Function> Functions = new List<Function>();

        private string _serializedFields;

        public string SerializedFields{
            get => this._serializedFields;
            set {
                string propertyName = "SerializedFields";
                if (TrackChanges && !Changes.ContainsKey(propertyName) && _serializedFields != value)
                    Changes[propertyName] = _serializedFields;
                else if (Changes.ContainsKey(propertyName) && (string)this.Changes[propertyName] == value)
                    this.Changes.Remove(propertyName);

                this._serializedFields = value;
            }
        }
        
        
        
        /// <summary>
        /// Loads the fields from the serialized fields property.
        /// </summary>
        /// <returns>Load successfull</returns>
        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(this.SerializedFields))
            {
                JArray jArray = JArray.Parse(SerializedFields);
                Functions = JsonConvert.DeserializeObject<List<Function>>(jArray[0].ToString());
                FieldList = JsonConvert.DeserializeObject<List<Field>>(jArray[1].ToString());
                return true;
            }

            return false;
        }
    }
}