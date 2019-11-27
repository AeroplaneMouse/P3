using AMS.Controllers.Interfaces;
using AMS.Events;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using AMS.Controllers;
using AMS.Database.Repositories;
using AMS.Helpers;
using AMS.Views;
using AMS.Helpers;

namespace AMS.ViewModels
{
    class TagEditorViewModel : Base.BaseViewModel
    {
        #region Public Properties

        private bool _dropdownsEnabled = true;
        private bool _parentComboEnabled = true;
        private bool _departmentComboEnabled = true;

        public ObservableCollection<Field> NonHiddenFieldList =>
            new ObservableCollection<Field>(_controller.NonHiddenFieldList);

        public ObservableCollection<Field> HiddenFieldList =>
            new ObservableCollection<Field>(_controller.HiddenFieldList);

        public Tag _tag
        {
            get => _controller.Tag;
            set => _controller.Tag = value;
        }

        public string Name
        {
            get => _controller.Name;
            set => _controller.Name = value;
        }

        public string Color
        {
            get => _controller.Color;
            set => _controller.Color = value;
        }

        public ulong ParentID => _controller.ParentID;

        public ulong DepartmentID
        {
            get => _controller.DepartmentID;
            set => _controller.DepartmentID = value;
        }

        public string PageTitle { get; set; }

        public List<Tag> ParentTagList
        {
            get => _controller.ParentTagList;
        }

        public List<Department> DepartmentList { get => _controller.DepartmentList; }

        private int _selectedParentTagIndex;
        public int SelectedParentTagIndex
        {
            get => _selectedParentTagIndex;
            set
            {
                if (value == _selectedParentTagIndex) return;
                
                int oldValue = _selectedParentTagIndex;
                _selectedParentTagIndex = value;
                    
                _controller.ConnectTag(ParentTagList[_selectedParentTagIndex],ParentTagList[oldValue]);
                //_controller.RemoveFieldRelations(ParentTagList[oldValue].ID);
                   
                OnPropertyChanged(nameof(HiddenFieldList));
                OnPropertyChanged(nameof(NonHiddenFieldList));
            }
        }

        public int SelectedDepartmentIndex { get; set; }
        
        public bool ParentComboEnabled
        {
            get => _parentComboEnabled;
            set => _parentComboEnabled = value;
        }

        public bool DepartmentComboEnabled
        {
            get => _departmentComboEnabled;
            set => _departmentComboEnabled = value;
        }

        #endregion

        #region Private Methods

        private ITagController _controller;

        #endregion

        #region Commands

        public ICommand SaveTagCommand { get; set; }
        public ICommand AddFieldCommand { get; set; }
        public ICommand RemoveFieldCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Constructor

        public TagEditorViewModel(ITagController tagController)
        {
            _controller = tagController;

            if (_controller.Id == 1){
                _parentComboEnabled = false;
                _departmentComboEnabled = false;
            }

            if (_controller.ParentID > 0)
                _departmentComboEnabled = false;

            //Set the selected parent to the parent of the chosen tag
            int i = ParentTagList.Count;
            while (i > 0 && ParentTagList[i - 1].ID != _controller.Tag.ParentID)
                i--;

            if (i > 0)
                _selectedParentTagIndex = i - 1;

            OnPropertyChanged(nameof(SelectedParentTagIndex));

            Department currentDepartment;

            if (_controller.IsEditing)
            {
                PageTitle = "Edit tag";

                // Use the department of the tag
                currentDepartment = _controller.DepartmentList.Find(d => d.ID == _controller.Tag.DepartmentID);
            }
            else
            {
                PageTitle = "Add tag";
                currentDepartment = Features.Main.CurrentDepartment;
            }

            // Setting the selected department
            int index = 0;
            foreach (Department d in _controller.DepartmentList)
            {
                if (d.ID == currentDepartment.ID)
                    SelectedDepartmentIndex = index;
                index++;
            }



            // Initialize commands
            SaveTagCommand = new Base.RelayCommand(SaveTag);
            AddFieldCommand = new Base.RelayCommand(AddField);
            RemoveFieldCommand = new Base.RelayCommand<object>((parameter) => RemoveField(parameter));

            CancelCommand = new Base.RelayCommand(Cancel);
        }

        #endregion

        #region Methods

        private void SaveTag()
        {
            _controller.Tag.ParentID = ParentTagList[SelectedParentTagIndex].ID;
            _controller.Tag.DepartmentID = DepartmentList[SelectedDepartmentIndex].ID;
            if (_controller.IsEditing)
            {
                _controller.Update();
            }
            else
            {
                _controller.Save();
            }

            Features.Navigate.Back();

        }

        private void AddField()
        {
            Features.DisplayPrompt(new Views.Prompts.CustomField(null, AddNewFieldConfirmed));
        }

        private void AddNewFieldConfirmed(object sender, PromptEventArgs e)
        {
            if (e is FieldInputPromptEventArgs args)
            {
                _controller.AddField(args.Field);
                OnPropertyChanged(nameof(NonHiddenFieldList));
                OnPropertyChanged(nameof(HiddenFieldList));
            }
        }

        private void RemoveField(object field)
        {
            if (field is Field inputField)
            {
                inputField.TagIDs.Add(_controller.Tag.ID);
                _controller.RemoveField(inputField);
                OnPropertyChanged(nameof(NonHiddenFieldList));
                OnPropertyChanged(nameof(HiddenFieldList));
            }
        }

        private void Cancel()
        {
            if (Features.Navigate.Back())
                Console.WriteLine("Going back dude...");
            else
                Console.WriteLine("Naaa... it is not possible to go back at the point in time.");

        }

        #endregion
    }
}