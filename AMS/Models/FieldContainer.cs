using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AMS.Models
{
    public class FieldContainer : Model
    {
        public List<Field> FieldList = new List<Field>();

        public string SerializedFields;
    }
}