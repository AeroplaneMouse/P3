using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.ViewModels.Commands.ViewModelHelper
{
    public class ShownField
    {
        public ShownField(Field field)
        {
            FieldTags = new ObservableCollection<Tag>();

            Name = field.GetHashCode().ToString();
            this.Field = field;
        }

        public string Name { get; set; }

        public Field Field { get; set; }

        public ObservableCollection<Tag> FieldTags { get; set; }
    }
}