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
        public ObservableCollection<ITagable> CurrentlyAddedTags { get; set; } = new ObservableCollection<ITagable>();

        public ObservableCollection<ShownField> HiddenFields { get; set; } = new ObservableCollection<ShownField>();

        public ObjectManagerController() => RemoveFieldCommand = new RemoveFieldCommand(this);


        protected void ConnectTags()
        {
            foreach (var tag in CurrentlyAddedTags)
                ShowIfNewField(tag);
        }


        private void ShowIfNewField(ITagable tag)
        {
            if (tag.GetType() == typeof(User)) return;
            FieldTagsPopulator((Tag)tag, FieldsList, false);
            FieldTagsPopulator((Tag)tag, HiddenFields, true);
        }

        private void FieldTagsPopulator(Tag tag,
            ObservableCollection<ShownField> listOffFields, bool hidden)
        {
            foreach (var currentTagField in tag.FieldsList)
            {
                bool alreadyExists = false;
                foreach (var shownField in listOffFields)
                {
                    if (shownField.Field.IsHidden == hidden)
                    {
                        if (shownField.ShownFieldToFieldComparator(currentTagField))
                        {
                            alreadyExists = true;

                            if (!shownField.Field.IsCustom && string.IsNullOrEmpty(shownField.Field.Content))
                                shownField.Field.Content = currentTagField.DefaultValue;

                            if (!shownField.FieldTags.Contains(tag))
                                shownField.FieldTags.Add(tag);
                        }

                        //Adds relation between tag and field.
                        if (tag.FieldsList.FirstOrDefault(field => field.Equals(currentTagField)) == null
                            && !shownField.FieldTags.Contains(tag))
                            shownField.FieldTags.Add(tag);

                        if (shownField.Field.HashId == currentTagField.HashId)
                            if (shownField.Field.Label != currentTagField.Label && !shownField.Field.IsCustom)
                                shownField.Field.Label = currentTagField.Label;
                    }
                }


                if (!alreadyExists)
                {
                    ShownField newField = new ShownField(currentTagField);
                    if (!newField.FieldTags.Contains(tag))
                        newField.FieldTags.Add(tag);

                    if (currentTagField.IsHidden &&
                        HiddenFields.FirstOrDefault(p => Equals(p.Field, currentTagField)) == null)
                        HiddenFields.Add(newField);
                    else if (HiddenFields.SingleOrDefault(field => Equals(field.Field, currentTagField)) ==
                             null)
                        if (FieldsList.SingleOrDefault(field => Equals(field.Field, currentTagField)) ==
                            null)
                            FieldsList.Add(newField);
                }
            }
        }
    }
}
