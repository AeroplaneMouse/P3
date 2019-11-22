using System.Collections.ObjectModel;

namespace AMS.Models
{
    public class FieldContainer : Model
    {
        public ObservableCollection<Field> FieldList = new ObservableCollection<Field>();

        public string SerializedFields;
    }
}