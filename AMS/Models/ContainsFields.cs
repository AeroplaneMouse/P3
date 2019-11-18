using System.Collections.ObjectModel;

namespace AMS.Models
{
    public class ContainsFields : Model
    {
        public ObservableCollection<Field> Fields = new ObservableCollection<Field>();
        
        public string SerializedFields;
    }
}