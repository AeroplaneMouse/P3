using System;
using AMS.Events;
using AMS.Models;
using System.Linq;
using AMS.ViewModels.Base;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AMS.ViewModels
{
    class TagEditorViewModel : Base.BaseViewModel
    {
        private ITagController _controller;
        private bool _dropdownsEnabled = true;
        private int _selectedParentTagIndex;


        #region Public Properties

        public ObservableCollection<Field> NonHiddenFieldList => new ObservableCollection<Field>(_controller.ControlledTag.FieldList);
        public ObservableCollection<Field> ParentTagFields => new ObservableCollection<Field>(_controller.ParentTagFields);
        
        public ObservableCollection<Function> Functions => new ObservableCollection<Function>(_controller.ControlledTag.Functions);
        public ObservableCollection<Function> ParentFunctions => new ObservableCollection<Function>(_controller.ControlledTag.Functions);

        public string Name { get => _controller.ControlledTag.Name; set => _controller.ControlledTag.Name = value; }
        public string Color { get => _controller.ControlledTag.Color; set => _controller.ControlledTag.Color = value; }
        public ulong ParentID => _controller.ControlledTag.ParentId;
        public ulong DepartmentID { get => _controller.ControlledTag.DepartmentID; set => _controller.ControlledTag.DepartmentID = value; }
        public string PageTitle { get; set; }
        public List<Tag> ParentTagList { get => _controller.ParentTagList; }
        public List<Department> DepartmentList { get => _controller.DepartmentList; }

        public int SelectedParentTagIndex
        {
            get => _selectedParentTagIndex;
            set
            {
                if (value == _selectedParentTagIndex) 
                    return;

                int oldValue = _selectedParentTagIndex;
                _selectedParentTagIndex = value;

                if (_controller.ControlledTag.Color == ParentTagList[oldValue].Color || oldValue == 0)
                {
                    _controller.ControlledTag.Color = ParentTagList[value].Color;
                    OnPropertyChanged(nameof(Color));
                }

                _controller.ControlledTag.ParentId = ParentTagList[_selectedParentTagIndex].ID;

                if (value > 0)
                {
                    DepartmentSelectionEnabled = false;
                    UpdateDepartmentSelectionToParentDepartment();
                }
                else
                    DepartmentSelectionEnabled = true;

                _controller.ConnectParentTag();
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
        
        public ICommand AddFunctionCommand { get; set; }

        #endregion

        #region Constructor

        public TagEditorViewModel(ITagController tagController)
        {
            _controller = tagController;

            // Disabling the ability to change parent and department for the user tag
            if (_controller.ControlledTag.TagId == 1)
            {
                ParentSelectionEnabled = false;
                DepartmentSelectionEnabled = false;
            }
            else
            {
                // Enabling department selection for parent tags
                DepartmentSelectionEnabled = _controller.ControlledTag.ParentId == 0;
            }

            if(_controller.ControlledTag.NumberOfChildren > 0)
                ParentSelectionEnabled = false;
            
            //Set the selected parent to the parent of the chosen tag
            int i = ParentTagList.Count - 1;
            while (i > 0 && ParentTagList[i].ID != _controller.ControlledTag.ParentId)
                i--;

            if (i > 0)
                _selectedParentTagIndex = i;

            OnPropertyChanged(nameof(Color));

            // Identifying the department to be the currently selected department.
            Department currentDepartment = _controller.IsEditing
                ? _controller.DepartmentList.Find(d => d.ID == _controller.ControlledTag.DepartmentID)
                : Features.Main.CurrentDepartment;

            // Setting the title of the page
            PageTitle = _controller.IsEditing
                ? "Edit tag"
                : "Add tag";

            if (_controller.IsEditing && _selectedParentTagIndex != 0)
                _controller.ConnectParentTag();

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
            
            AddFunctionCommand = new RelayCommand(AddFunction);
            RemoveFieldCommand = new Base.RelayCommand<object>((parameter) => RemoveField(parameter));

            RemoveCommand = new RelayCommand(RemoveTag);
            CancelCommand = new RelayCommand(Cancel);

            ShowFieldEditPromptCommand = new RelayCommand<object>((parameter) =>
            {
                if (parameter is Field field)
                    Features.DisplayPrompt(new Views.Prompts.CustomField(null, EditFieldConfirmed, false, field));
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
            _controller.ControlledTag.ParentId = ParentTagList[SelectedParentTagIndex].ID;
            _controller.ControlledTag.DepartmentID = DepartmentList[SelectedDepartmentIndex].ID;
            if (VerifyTagAndFields())
            {
                string message = $"'{ _controller.ControlledTag.Name }' has been ";
                if (_controller.IsEditing)
                {
                    _controller.Update();
                    message += "updated";
                }
                else
                {
                    _controller.Save();
                    message += "added";
                }

                Features.AddNotification(new Notification(message, background: Notification.APPROVE), displayTime: 3500);
                Features.Navigate.Back();
            }
        }

        private void AddField()
        {
            Features.DisplayPrompt(new Views.Prompts.CustomField(null, AddNewFieldConfirmed));
        }
        
        private void AddFunction()
        {
            Features.DisplayPrompt(new Views.Prompts.CustomFunction(null, AddNewCommandConfirmed));
        }

        private void AddNewFieldConfirmed(object sender, PromptEventArgs e)
        {
            if (e is FieldInputPromptEventArgs args)
            {
                _controller.AddField(args.Field);
                UpdateAll();
            }
        }
        
        private void AddNewCommandConfirmed(object sender, PromptEventArgs e)
        {
            if (e is FunctionInputPromptEventArgs args)
            {
                _controller.AddFunction(args.Function);
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
            {
                _controller.RevertChanges();
                Features.Navigate.To(Features.Create.TagList());
            }
        }

        private void UpdateAll()
        {
            OnPropertyChanged(nameof(NonHiddenFieldList));
            //OnPropertyChanged(nameof(HiddenFieldList));
            OnPropertyChanged(nameof(SelectedParentTagIndex));
            OnPropertyChanged(nameof(ParentTagFields));
            OnPropertyChanged(nameof(Functions));
        }

        /// <summary>
        /// Verifies tag and its fields.
        /// </summary>
        /// <returns></returns>
        private bool VerifyTagAndFields()
        {
            // Verifies whether fields contains correct information, or the required information.
            //List<Field> completeList = HiddenFieldList.ToList();
            //completeList.AddRange(NonHiddenFieldList.ToList());

            List<Field> completeList = new List<Field>(NonHiddenFieldList);

            // Checks whether the name is null
            if (string.IsNullOrEmpty(_controller.ControlledTag.Name))
            {
                Features.AddNotification(new Notification("Label is required and empty", background: Notification.WARNING));
                return false;
            }

            // Verify number fields
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
                                new Notification("The field " + field.Label + " cannot contain letters", background: Notification.WARNING));
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
            if (_controller.ControlledTag.ParentId == 0 && _controller.ControlledTag.NumberOfChildren > 0)
            {
                message = "You are about to remove a parent tag!\n"
                        + $"There are { _controller.ControlledTag.NumberOfChildren } children attached to this parent.\n\n"
                        + "- Remove parent and all children\n"
                        + "- Remove parent and convert children to parents";

                List<string> buttons = new List<string>();
                buttons.Add("Remove all");
                buttons.Add("Remove parent");

                Features.DisplayPrompt(new Views.Prompts.ExpandedConfirm(message, buttons, (sender, e) =>
                {
                    if (e is ExpandedPromptEventArgs args)
                    {
                        string extraMessage = $"{ _controller.ControlledTag.Name } has been removed";
                        bool actionSuccess;
                        if (args.ButtonNumber == 0)
                        {
                            actionSuccess = _controller.Remove(removeChildren: true);
                            extraMessage += $" aswell as { _controller.ControlledTag.NumberOfChildren } children";
                        }
                        else
                            actionSuccess = _controller.Remove();

                        if (!actionSuccess)
                            extraMessage = "Error! Unable to remove tag(s).";

                        Features.Navigate.Back();
                        Features.AddNotification(new Notification(extraMessage, background: actionSuccess ? Notification.APPROVE : Notification.ERROR), displayTime: 4000);
                    }
                }));
            }
            else
            {
                Features.DisplayPrompt(new Views.Prompts.Confirm(
                    "You are about to remove a tag which cannot be UNDONE!\n"
                    + "Are you sure?\n"
                    + $"Tag: { _controller.ControlledTag.Name }", (sender, e) =>
                    {
                        if (e.Result)
                        {
                            _controller.Remove();
                            UpdateOnFocus();
                            Features.AddNotification(new Notification($"{ _controller.ControlledTag.Name } has been removed.", background: Notification.APPROVE));
                            Features.Navigate.Back();
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