using System.Collections.ObjectModel;
using System.Linq;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public abstract class ObjectManagerController : FieldsController
    {
        protected abstract void LoadFields();

        public ObservableCollection<Tag> CurrentlyAddedTags { get; set; } = new ObservableCollection<Tag>();

        public ObservableCollection<ShownField> HiddenFields { get; set; } = new ObservableCollection<ShownField>();

        protected void ConnectTags()
        {
            foreach (var tag in CurrentlyAddedTags)
            {
                ShowIfNewField(tag);
            }
        }

        
        private void ShowIfNewField(Tag tag)
        {
            foreach (var currentTagField in tag.FieldsList)
            {
                bool alreadyExists = false;

                foreach (var shownField in this.FieldsList)
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

                if (alreadyExists) continue;
                if (currentTagField.IsHidden)
                {
                    HiddenFields.Add(new ShownField(currentTagField));
                }
                else
                {
                    if (HiddenFields.FirstOrDefault(p => Equals(p.Field, currentTagField)) == null)
                    {
                        FieldsList.Add(new ShownField(currentTagField));
                    }
                }
            }
        }
    }
}