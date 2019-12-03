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
using AMS.ViewModels.Base;
using AMS.Views.Prompts;

namespace AMS.ViewModels
{
    class TagEditorViewModel : Base.BaseViewModel
    {
        private ITagController _controller;
        private bool _dropdownsEnabled = true;
        private int _selectedParentTagIndex;


        #region Public Properties

        public ObservableCollection<Field> NonHiddenFieldList =>
            new ObservableCollection<Field>(_controller.NonHiddenFieldList);
        public ObservableCollection<Field> HiddenFieldList =>
            new ObservableCollection<Field>(_controller.HiddenFieldList);

        public string Name { get => _controller.Name;
                             set => _controller.Name = value; }

        public string Color { get => _controller.Color;
                              set => _controller.Color = value; }

        public ulong ParentID => _controller.ParentID;

        public ulong DepartmentID { get => _controller.DepartmentID;
                                    set => _controller.DepartmentID = value; }

        public string PageTitle { get; set; }
        public List<Tag> ParentTagList { get => _controller.ParentTagList; }
        public List<Department> DepartmentList { get => _controller.DepartmentList; }

        public int SelectedParentTagIndex
        {
            get => _selectedParentTagIndex;
            set
            {

                if (value == _selectedParentTagIndex) return;

                int oldValue = _selectedParentTagIndex;
                _selectedParentTagIndex = value;

                if (_controller.Color == ParentTagList[oldValue].Color || oldValue == 0)
                {
                    _controller.Color = ParentTagList[value].Color;
                    OnPropertyChanged(nameof(Color));
                }

                _controller.ParentID = ParentTagList[_selectedParentTagIndex].ID;

                if (value > 0)
                {
                    DepartmentSelectionEnabled = false;
                    UpdateDepartmentSelectionToParentDepartment();
                }
                else
                {
                    DepartmentSelectionEnabled = true;
                }

                _controller.ConnectTag(ParentTagList[_selectedParentTagIndex], ParentTagList[oldValue]);
                UpdateAll();
            }
        }

        public int SelectedDepartmentIndex { get; set; }
        public bool ParentSelectionEnabled { get; set; } = true;
        public bool DepartmentSelectionEnabled { get; set; } = true;

        #endregion

        #region Commands

        public ICommand SaveTagCommand { get; set; }
        public ICommand AddFieldCommand { get; set; }
        public ICommand RemoveFieldCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ShowFieldEditPromptCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        #endregion

        #region Constructor

        public TagEditorViewModel(ITagController tagController)
        {
            _controller = tagController;

            // Disabling the ability to change parent and department for the user tag
            if (_controller.Id == 1)
            {
                ParentSelectionEnabled = false;
                DepartmentSelectionEnabled = false;
            }

            if(_controller.ControlledTag.ChildrenCount > 0)
            {
                ParentSelectionEnabled = false;
            }

            // Enabling department selection for parent tags
            DepartmentSelectionEnabled = _controller.ParentID == 0;

            //Set the selected parent to the parent of the chosen tag
            int i = ParentTagList.Count - 1;
            while (i > 0 && ParentTagList[i].ID != _controller.ParentID)
                i--;

            if (i > 0)
                _selectedParentTagIndex = i;

            OnPropertyChanged(nameof(Color));

            // Identifying the department to be the currently selected department.
            Department currentDepartment = _controller.IsEditing
                ? _controller.DepartmentList.Find(d => d.ID == _controller.DepartmentID)
                : Features.Main.CurrentDepartment;

            // Setting the title of the page
            PageTitle = _controller.IsEditing
                ? "Edit tag"
                : "Add tag";

            UpdateAll();

            // Setting the selected department
            int index = 0;
            foreach (Department d in _controller.DepartmentList)
            {
                if (d.ID == currentDepartment.ID)
                    SelectedDepartmentIndex = index;
                index++;
            }

            // Initialize commands
            SaveTagCommand = new RelayCommand(SaveTag);
            AddFieldCommand = new RelayCommand(AddField);
            RemoveFieldCommand = new Base.RelayCommand<object>((parameter) => RemoveField(parameter));

            RemoveCommand = new RelayCommand(RemoveTag);
            CancelCommand = new RelayCommand(Cancel);

            ShowFieldEditPromptCommand = new RelayCommand<object>((parameter) =>
            {
                if (parameter is Field field)
                    Features.DisplayPrompt(new Views.Prompts.CustomField(null, EditFieldConfirmed, false, field));
                else
                    //TODO Handle not field event
                    return;
            });
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
            _controller.ParentID = ParentTagList[SelectedParentTagIndex].ID;
            _controller.DepartmentID = DepartmentList[SelectedDepartmentIndex].ID;
            if (VerifyTagAndFields())
            {
                if (_controller.IsEditing)
                    _controller.Update();
                else
                    _controller.Save();

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
        
        /// <summary>
        /// Returns to the tag list without saving anything
        /// </summary>
        private void Cancel()
        {
            if (!Features.Navigate.Back())
                Features.Navigate.To(Features.Create.TagList());
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
                field.TagList = new List<Tag>(); // <=== How does this work?! TODO: Question
                foreach (var id in field.TagIDs)
                {
                    if (field.TagIDs.Contains(id))
                        field.TagList.Add(ParentTag);
                }
            }

            foreach (var field in NonHiddenFieldList)
            {
                field.TagList = new List<Tag>(); // <=== How does this work?!
                foreach (var id in field.TagIDs)
                {
                    if (field.TagIDs.Contains(id))
                        field.TagList.Add(ParentTag);
                }
            }
        }

        /// <summary>
        /// Verifies tag and its fields.
        /// </summary>
        /// <returns></returns>
        private bool VerifyTagAndFields()
        {
            // Verifies whether fields contains correct information, or the required information.
            List<Field> completeList = HiddenFieldList.ToList();
            completeList.AddRange(NonHiddenFieldList.ToList());

            // Checks whether the name is null
            if (string.IsNullOrEmpty(_controller.Name))
            {
                Features.AddNotification(new Notification("Label is required and empty", background: Notification.WARNING));
                return false;
            }

            foreach (var field in completeList)
            {
                if (field.Type == Field.FieldType.NumberField)
                {
                    if (!string.IsNullOrEmpty(field.Content))
                    {
                        bool check = field.Content.All(char.IsDigit);
                        if (!check)
                        {
                            Features.AddNotification(
                                new Notification("The field " + field.Label + " cannot contain letters",
                                    Notification.WARNING));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void EditFieldConfirmed(object sender, PromptEventArgs e)
        {
            if (e is FieldEditPromptEventArgs args)
            {
                args.OldField.Label = args.NewField.Label;
                args.OldField.Required = args.NewField.Required;
                args.OldField.Type = args.NewField.Type;
                args.OldField.Content = args.NewField.Content;
                UpdateAll();
            }
        }

        private void RemoveTag()
        {
            string message = String.Empty;

            // Check if parent
            if (_controller.ParentID == 0 && _controller.ControlledTag.ChildrenCount > 0)
            {
                message = "You are about to remove a parent tag!\n"
                        + $"There are { _controller.ControlledTag.ChildrenCount } children attached to this parent.";
              
                List<string> buttons = new List<string>();
                buttons.Add("Remove parent and all children?");
                buttons.Add("Remove parent and convert children to parents?");

                Features.DisplayPrompt(new Views.Prompts.ExpandedConfirm(message, buttons, (sender, e) =>
                {
                    if (e is ExpandedPromptEventArgs args)
                    {
                        string extraMessage = String.Empty;
                        if (args.ButtonNumber == 0)
                        {
                            _controller.RemoveChildren();
                            _controller.Remove();
                            extraMessage = $" aswell as { _controller.ControlledTag.ChildrenCount } children";
                        }
                        else
                            _controller.Remove();
                        Features.Navigate.Back();
                        Features.AddNotification(new Notification($"{ _controller.Name } has been removed{ extraMessage }.", background: Notification.APPROVE), displayTime: 4000);
                    }
                }));
            }
            else
            {
                Features.DisplayPrompt(new Views.Prompts.Confirm("You are about to remove a tag which cannot be UNDONE!\nAre you sure?", (sender, e) =>
                {
                    if (e.Result)
                    {
                        _controller.Remove();
                        Features.Navigate.Back();
                        Features.AddNotification(new Notification($"{ _controller.Name } has been remove.", background: Notification.APPROVE));
                    }
                }));
            }
        }

        private void UpdateDepartmentSelectionToParentDepartment()
        {
            int i = DepartmentList.Count - 1;
            while (i > 0 && DepartmentList[i].ID != ParentTagList[SelectedParentTagIndex].DepartmentID)
                i--;
            SelectedDepartmentIndex = i;
        }

        #endregion
    }
}