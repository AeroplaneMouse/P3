using AMS.Controllers.Interfaces;
using AMS.Events;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;

namespace AMS.ViewModels
{
    class TagEditorViewModel :Base.BaseViewModel
    {
        #region Properties
        ITagController _controller;
        public Tag _tag
        {
            get => _controller.Tag;
            set => _controller.Tag = value;
        }

        public string Name
        {
            get => _controller.Tag.Name;
            set => _controller.Tag.Name = value;
        }
        public string Color
        {
            get => _controller.Tag.TagColor;
            set => _controller.Tag.TagColor = value;
        }
        public ulong ParentID
        {
            get => _controller.Tag.ParentID;
            set => _controller.Tag.ParentID = value;
        }
        public ulong DepartmentID
        {
            get => _controller.Tag.DepartmentID;
            set => _controller.Tag.DepartmentID = value;
        }
        public string PageTitle { get; set; }
        public List<Tag> ParentTagList
        {
            get => _controller.ParentTagList;
        }
        public int SelectedParentIndex { get; set; }
        public ObservableCollection<Field> FieldList
        {
            get => new ObservableCollection<Field>(_controller.Tag.FieldList);
            set => _controller.Tag.FieldList = value.ToList();
        }
        #endregion

        #region Commands
        public ICommand SaveTagCommand { get; set; }
        public ICommand AddFieldCommand { get; set; }
        public ICommand RemoveFieldCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        #endregion

        public TagEditorViewModel(ITagController tagController)
        {
            _controller = tagController;

            PageTitle = _controller.PageTitle;
            // Initialize commands
            SaveTagCommand = new Base.RelayCommand(SaveTag);
            AddFieldCommand = new Base.RelayCommand(AddField);
            RemoveFieldCommand = new Base.RelayCommand<object>(RemoveField);

            CancelCommand = new Base.RelayCommand(Cancel);
        }

        #region Methods
        private void SaveTag()
        {
            _controller.Save();
            Features.NavigateBack();
        }

        private void AddField()
        {
            //TODO: Fix det her!
            Features.DisplayPrompt(new Views.Prompts.CustomField(null, AddNewFieldConfirmed));
        }

        private void AddNewFieldConfirmed(object sender, PromptEventArgs e)
        {
            if (e is FieldInputPromptEventArgs && e.Result)
            {
                Field newField = ((FieldInputPromptEventArgs)e).Field;
                if (newField is Field )
                {
                    _controller.AddField(newField);
                }
                else
                    Features.AddNotification(
                        new Notification("Adding field failed. Received object is not a field.",
                            Notification.ERROR), 5000);
            }
        }

        private void RemoveField(object field)
        {
            _controller.RemoveField(field as Field);
        }

        private void Cancel()
        {
            Features.NavigateBack();
        }
        #endregion
    }
}
