using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.ViewModels.Commands.ViewModelHelper
{
    public class ShowFields
    {
        public string Name { get; set; }

        public Field Field { get; set; }

        public ObservableCollection<Tag> FieldTags { get; set; }

        public ShowFields()
        {
            FieldTags = new ObservableCollection<Tag>();
        }
    }
}