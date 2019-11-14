using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.Commands;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public class ObjectManagerController : Base.BaseViewModel
    {
        public ICommand AddFieldCommand;
        public ICommand RemoveFieldCommand;

        private ObjectManagerController _objectManagerController;


        public void ConnectTags(ObservableCollection<ITagable> CurrentlyAddedTags,ObservableCollection<ShownField> shownFields,ObservableCollection<ShownField> hiddenList)
        {
            foreach (var tag in CurrentlyAddedTags)
            {
                ShowIfNewField(tag,shownFields,false);
                ShowIfNewField(tag,hiddenList,true);
            }
        }


        private void ShowIfNewField(ITagable tag, ObservableCollection<ShownField> _fieldslist, bool isHidden = false)
        {
            if (tag.GetType() == typeof(User)) return;
            FieldTagsPopulator((Tag) tag, _fieldslist, isHidden);
        }

        public void FieldTagsPopulator(Tag tag,
            ObservableCollection<ShownField> listOffFields, bool hidden)
        {
            foreach (var currentTagField in tag.FieldsList)
            {
                bool alreadyExists = false;
                foreach (var shownField in listOfFields)
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

                    if (listOffFields.FirstOrDefault(p => Equals(p.Field, currentTagField)) == null)
                        listOffFields.Add(newField);

                }
            }
        }

        public ObservableCollection<ShownField> ShownFieldsInitializer(List<Field> fields, bool hidden)
        {
            ObservableCollection<ShownField> output = new ObservableCollection<ShownField>();
            foreach (var field in fields.Where(field => field.IsHidden == hidden))
            {
                output.Add(new ShownField(field));
            }

            return output;
        }
    }
}
