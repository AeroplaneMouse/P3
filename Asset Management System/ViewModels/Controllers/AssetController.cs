using System.Collections.ObjectModel;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Controllers
{
    public class AssetController : ObjectManagerController
    {
        public ObservableCollection<Tag> CurrentlyAddedTags { get; set; } = new ObservableCollection<Tag>();
        
        public Asset Asset { get; set; }
        
        protected override void LoadFields()
        {
            foreach (var field in Asset.FieldsList)
            {
                if (field.IsHidden)
                    HiddenFields.Add(new ShownField(field));
                else
                    FieldsList.Add(new ShownField(field));
            }
        }
    }
}