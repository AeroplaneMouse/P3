using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public class TagManagerViewModel : ObjectManagerController
    {
        // TODO: Edit style for fields to use dynamic binding, such fields on tags can be removed once added.
        private MainViewModel _main;
        private Tag _tag;
        private string _randomColor;

        public string Name { get; set; }
        public string Color { get; set; }
        public int SelectedParentIndex { get; set; }

        public string Title { get; set; }

        public ICommand CancelCommand { get; set; }

        public List<Tag> ParentTagsList
        {
            get
            {
                TagRepository tagRepository = new TagRepository();
                List<Tag> parentTagsList = new List<Tag>()
                {
                    new Tag()
                    {
                        Name = "[No Parent Tag]",
                        ParentID = 0,
                        Color = _randomColor
                    }
                };
                foreach (Tag parentTag in (List<Tag>) tagRepository.GetParentTags())
                {
                    parentTagsList.Add(parentTag);
                }

                return parentTagsList;
            }
        }

        public TagManagerViewModel(MainViewModel main, Tag inputTag)
        {
            _main = main;
            _randomColor = CreateRandomColor();

            FieldsList = new ObservableCollection<ShownField>();

            if (inputTag != null)
            {
                _tag = inputTag;
                _editing = true;
                LoadFields();
                Title = "Edit tag";
            }
            else
            {
                _tag = new Tag();
                _editing = false;
                Title = "Add tag";
            }

            // Initialize commands
            SaveTagCommand = new Commands.SaveTagCommand(this, _main, _tag, _editing);
            AddFieldCommand = new Commands.AddFieldCommand(_main, this);
            RemoveFieldCommand = new Commands.RemoveFieldCommand(this);

            CancelCommand = new Base.RelayCommand(() => _main.ChangeMainContent(new Views.Tags(_main)));
        }

        public ICommand SaveTagCommand { get; set; }

        private string CreateRandomColor()
        {
            //Creates an instance of the Random, to create pseudo random numbers
            Random random = new Random();

            //Creates a hex values from three random ints converted to bytes and then to string
            string hex = "#" + ((byte) random.Next(25, 230)).ToString("X2") +
                         ((byte) random.Next(25, 230)).ToString("X2") + ((byte) random.Next(25, 230)).ToString("X2");

            return hex;
        }

        public bool CanSaveTag()
        {
            //Todo Figure out the implementation of this one
            return true;
        }

        /// <summary>
        /// Runs through the saved fields within the tag, and adds these to the fieldList.
        /// </summary>
        /// <returns></returns>
        protected override void LoadFields()
        {
            foreach (Field field in _tag.FieldsList)
            {
                if (field.IsHidden)
                {
                    HiddenFields.Add(new ShownField(field));
                }
                else
                {
                    FieldsList.Add(new ShownField(field));
                }
            }

            ConnectTags();

            //Set Name to the name of the chosen tag
            Name = _tag.Name;

            //Set Color to the color of the chosen tag
            Color = _tag.Color;

            //Set the selected parent to the parent of the chosen tag
            int i = ParentTagsList.Count;
            while (i > 0 && ParentTagsList[i - 1].ID != _tag.ParentID)
            {
                i--;
            }

            if (i > 0)
            {
                SelectedParentIndex = i - 1;
            }

            // Notify view
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Color));
            OnPropertyChanged(nameof(SelectedParentIndex));
        }
    }
}