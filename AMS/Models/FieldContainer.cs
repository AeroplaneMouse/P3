using System.Collections.Generic;
using System.Collections.ObjectModel;

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
    }
}