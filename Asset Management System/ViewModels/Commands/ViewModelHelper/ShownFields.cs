using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.ViewModels.Commands.ViewModelHelper
{
    public class ShownField
    {
        public string Name { get; set; }

        public Field Field { get; set; }

        public ObservableCollection<Tag> FieldTags { get; set; }

        public ShownField()
        {
            FieldTags = new ObservableCollection<Tag>();
        }
    }
}