﻿using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class TagManagerViewModel : FieldsController
    {
        private MainViewModel _main;
        private Tag _tag;

        public string Name { get; set; }
        public string Color { get; set; }
        public int SelectedParentIndex { get; set; }

        public List<Tag> ParentTagsList
        {
            get
            {
                TagRepository tagRepository = new TagRepository();
                return (List<Tag>)tagRepository.GetParentTags();
            }
        }

        public TagManagerViewModel(MainViewModel main, Tag inputTag)
        {
            _main = main;
            _tag = inputTag;

            FieldsList = new ObservableCollection<Field>();

            if (inputTag != null)
            {
                _tag = inputTag;
                _editing = true;
                LoadFields();
            }
            else
            {
                _tag = new Tag();
                _editing = false;
            }

            // Initialize commands
            SaveTagCommand = new Commands.SaveTagCommand(this, _main, _tag, _editing);
            AddFieldCommand = new Commands.AddFieldCommand(this);
            RemoveFieldCommand = new Commands.RemoveFieldCommand(this);
        }

        public ICommand SaveTagCommand { get; set; }
        public static ICommand RemoveFieldCommand { get; set; }

        private string CreateRandomColor()
        {
            //Creates an instance of the Random, to create pseudo random numbers
            Random random = new Random();

            //Creates a hex values from three random ints converted to bytes and then to string
            string hex = "#" + ((byte)random.Next(25, 230)).ToString("X2") + ((byte)random.Next(25, 230)).ToString("X2") + ((byte)random.Next(25, 230)).ToString("X2");

            return hex;
        }

        public bool CanSaveTag()
        {
            // **** TODO ****
            // Only return true, if the entered values are valid.
            return true;
        }

        /// <summary>
        /// Runs through the saved fields within the tag, and adds these to the fieldList.
        /// </summary>
        /// <returns></returns>
        protected override void LoadFields()
        {
            _tag.DeserializeFields();
            foreach (Field field in _tag.FieldsList)
                FieldsList.Add(field);

            Name = _tag.Name;
            Color = _tag.Color;

            // Notify view
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Color));
        }
    }
}
