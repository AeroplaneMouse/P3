using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.Commands;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public abstract class ObjectManagerController : FieldsController
    {
        protected abstract void LoadFields();

        public ObservableCollection<Tag> CurrentlyAddedTags { get; set; } = new ObservableCollection<Tag>();

        public ObservableCollection<ShownField> HiddenFields { get; set; } = new ObservableCollection<ShownField>();
        
        public static ICommand RemoveFieldCommand { get; set; }

        public ObjectManagerController()
        {
            RemoveFieldCommand = new RemoveFieldCommand(this);
        }
        

        protected void ConnectTags()
        {
            foreach (var tag in CurrentlyAddedTags)
            {
                ShowIfNewField(tag);
            }
        }


        private void ShowIfNewField(Tag tag)
        {
            FieldTagsPopulator(tag, FieldsList);
            FieldTagsPopulator(tag, HiddenFields);
        }

        private void FieldTagsPopulator(Tag tag,
            ObservableCollection<ShownField> listOffFields)
        {
            foreach (var currentTagField in tag.FieldsList)
            {
                bool alreadyExists = false;

                foreach (var shownField in listOffFields)
                {
                    if (shownField.ShownFieldToFieldComparator(currentTagField))
                    {
                        alreadyExists = true;
                        if (!shownField.Field.IsCustom && string.IsNullOrEmpty(shownField.Field.Content))
                        {
                            shownField.Field.Content = currentTagField.DefaultValue;
                        }

                        if (!shownField.FieldTags.Contains(tag))
                        {
                            shownField.FieldTags.Add(tag);
                        }
                    }

                    if (shownField.Field.HashId == currentTagField.HashId)
                    {
                        if (shownField.Field.Label != currentTagField.Label && !shownField.Field.IsCustom)
                        {
                            shownField.Field.Label = currentTagField.Label;
                        }
                    }
                }

                // If it already exists, jump to next iteration.
                if (alreadyExists) continue;
                if (currentTagField.IsHidden)
                {
                    HiddenFields.Add(new ShownField(currentTagField));
                }
                else
                {
                    if (HiddenFields.FirstOrDefault(p => Equals(p.Field, currentTagField)) != null) continue;
                    if (FieldsList.SingleOrDefault(field => field.Field.HashId == currentTagField.HashId) ==
                        null)
                    {
                        FieldsList.Add(new ShownField(currentTagField));
                    }
                }
            }
        }
    }
}