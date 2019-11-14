using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels.Controllers;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public class TagManagerViewModel : ObjectViewModelHelper
    {
        // TODO: Edit style for fields to use dynamic binding, such fields on tags can be removed once added.
        private MainViewModel _main;
        private Tag _tag;
        private string _randomColor;
        private ITagService _service;
        private readonly bool Editing;
        public List<Tag> ParentTagsList;
        public int SelectedParentIndex;


        public string Name { get; set; }
        public string Color { get; set; }

        public string Title { get; set; }

        public ICommand CancelCommand { get; set; }

        public TagManagerViewModel(MainViewModel main, ITagService service, Tag inputTag, bool editing = false)
        {
            _main = main;
            _service = service;
            Editing = editing;

            if (inputTag != null)
            {
                _tag = inputTag;
                //Set Name to the name of the chosen tag
                Name = _tag.Name;

                //Set Color to the color of the chosen tag
                Color = _tag.Color;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Color));
                Title = "Edit tag";
            }
            else
            {
                Title = "Add tag";
            }

            // Initialize commands
            SaveTagCommand = new Commands.SaveTagCommand(this, _main, _tag, _service, Editing);
            AddFieldCommand = new Commands.AddFieldCommand(_main, this);
            RemoveFieldCommand = new Commands.RemoveFieldCommand(this);

            CancelCommand = new Base.RelayCommand(() => _main.ReturnToPreviousPage());
        }

        public ICommand SaveTagCommand { get; set; }
        

        public bool CanSaveTag()
        {
            //Todo Figure out the implementation of this one
            return true;
        }
    }
}