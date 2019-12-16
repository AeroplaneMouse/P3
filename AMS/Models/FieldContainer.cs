using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMS.Models
{
    public abstract class FieldContainer : Model
    {
        public List<Field> FieldList = new List<Field>();

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
            if (!string.IsNullOrEmpty(this.SerializedFields) && _serializedFields != "[]")
            {
                if (this is Tag tag)
                {
                    tag.SerializationOrder = JsonConvert.DeserializeObject<Tag.SerializationOrderStruct>(_serializedFields);
                    tag.FieldList = tag.SerializationOrder.Fields;
                    tag.Functions = tag.SerializationOrder.Functions;
                }

                if (this is Asset)
                {
                    
                }
                return true;
            }

            return false;
        }
    }
}