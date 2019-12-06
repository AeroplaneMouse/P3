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
                if (this.SerializedFields != null && TrackChanges)
                {
                    this.Changes["SerializedFields"] = this.SerializedFields;
                }
                this._serializedFields = value;
            }
        }
    }
}