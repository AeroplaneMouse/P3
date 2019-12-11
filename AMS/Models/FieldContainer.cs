using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AMS.Models
{
    public abstract class FieldContainer : Model
    {
        public List<Field> FieldList = new List<Field>();

        private string _serializedFields;

        public string SerializedFields{
            get => _serializedFields;
            set 
            {
                if (SerializedFields != null && TrackChanges)
                {
                    Changes["SerializedFields"] = SerializedFields;
                }
                _serializedFields = value;
            }
        }
    }
}