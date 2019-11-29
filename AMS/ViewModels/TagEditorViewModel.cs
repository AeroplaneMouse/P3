using AMS.Controllers.Interfaces;
using AMS.Events;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Globalization;
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
            get => _controller.ControlledTag;
            set => _controller.ControlledTag = value;
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

        public List<Department> DepartmentList
        {
            get => _controller.DepartmentList;
        }

        private int _selectedParentTagIndex;

        public int SelectedParentTagIndex
        {
            get => _selectedParentTagIndex;
            set
            {
                if (value == _selectedParentTagIndex) return;

                int oldValue = _selectedParentTagIndex;
                _selectedParentTagIndex = value;

                _controller.ConnectTag(ParentTagList[_selectedParentTagIndex], ParentTagList[oldValue]);
                UpdateAll();
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

            if (_controller.Id == 1)
            {
                _parentComboEnabled = false;
                _departmentComboEnabled = false;
            }

            if (_controller.ParentID > 0)
                _departmentComboEnabled = false;

            //Set the selected parent to the parent of the chosen tag
            int i = ParentTagList.Count;
            while (i > 0 && ParentTagList[i - 1].ID != _controller.ControlledTag.ParentID)
                i--;

            if (i > 0)
                _selectedParentTagIndex = i - 1;

            UpdateAll();


            Department currentDepartment;

            if (_controller.IsEditing)
            {
                PageTitle = "Edit tag";

                // Use the department of the tag
                currentDepartment = _controller.DepartmentList.Find(d => d.ID == _controller.ControlledTag.DepartmentID);
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

        public override void UpdateOnFocus()
        {
            UpdateAll();

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Color));
            OnPropertyChanged(nameof(DepartmentID));
            OnPropertyChanged(nameof(ParentID));
            OnPropertyChanged(nameof(ParentTagList));
            OnPropertyChanged(nameof(DepartmentList));
        }

        /// <summary>
        /// Saves the tag.
        /// </summary>
        private void SaveTag()
        {
            _controller.ControlledTag.ParentID = ParentTagList[SelectedParentTagIndex].ID;
            _controller.ControlledTag.DepartmentID = DepartmentList[SelectedDepartmentIndex].ID;
            if (VerifyTagAndFields())
            {
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
                UpdateAll();
            }
        }

        private void RemoveField(object field)
        {
            if (field is Field inputField)
            {
                inputField.TagIDs.Add(_controller.ControlledTag.ID);
                _controller.RemoveField(inputField);
                UpdateAll();
            }
        }

        private void Cancel()
        {
            if (Features.Navigate.Back() == false)
            {
                Features.Navigate.To(Features.Create.TagList());
            }
        }

        private void UpdateAll()
        {
            OnPropertyChanged(nameof(NonHiddenFieldList));
            OnPropertyChanged(nameof(HiddenFieldList));
            OnPropertyChanged(nameof(SelectedParentTagIndex));
            UpdateTagRelations();
        }

        private void UpdateTagRelations()
        {
            Tag ParentTag = ParentTagList[_selectedParentTagIndex];
            foreach (var field in HiddenFieldList)
            {
                field.TagList = new List<Tag>();
                foreach (var id in field.TagIDs)
                {
                    if (field.TagIDs.Contains(id))
                    {
                        field.TagList.Add(ParentTag);
                    }
                }
            }

            foreach (var field in NonHiddenFieldList)
            {
                field.TagList = new List<Tag>();
                foreach (var id in field.TagIDs)
                {
                    if (field.TagIDs.Contains(id))
                    {
                        field.TagList.Add(ParentTag);
                    }
                }
            }
        }
        
        /// <summary>
        /// Verifies tag and its fields.
        /// </summary>
        /// <returns></returns>
        private bool VerifyTagAndFields()
        {
            //Verifies whether fields contains correct information, or the required information.
            List<Field> completeList = HiddenFieldList.ToList();
            completeList.AddRange(NonHiddenFieldList.ToList());

            //Checks whether the name is null
            if (string.IsNullOrEmpty(_controller.ControlledTag.Name))
            {
                Features.AddNotification(new Notification("Label is required and empty",
                    Notification.WARNING));
                return false;
            }
            
            foreach (var field in completeList)
            {
                if (field.Required && string.IsNullOrEmpty(field.Content))
                {
                    Features.AddNotification(new Notification("The field " + field.Label + " is required and empty",
                        Notification.WARNING));
                    return false;
                }

                if (field.Type == Field.FieldType.NumberField)
                {
                    bool check = field.Content.All(char.IsDigit);
                    if (check)
                    {
                        Features.AddNotification(
                            new Notification("The field " + field.Label + " cannot contain letters",
                                Notification.WARNING));
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}