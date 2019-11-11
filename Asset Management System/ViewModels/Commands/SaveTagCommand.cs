using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Logging;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels.Commands
{
    class SaveTagCommand : ICommand
    {
        private TagManagerViewModel _viewModel;
        private MainViewModel _main;
        private Tag _tag;
        private ITagService _service;
        private bool _editing;
        private ITagRepository _rep;
        public event EventHandler CanExecuteChanged;

        public SaveTagCommand(TagManagerViewModel viewModel, MainViewModel main, Tag tag, ITagService service, bool editing)
        {
            _viewModel = viewModel;
            _main = main;
            _tag = tag;
            _service = service;
            _rep = _service.GetSearchableRepository() as ITagRepository;
            _editing = editing;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModel.CanSaveTag();
        }

        public void Execute(object parameter)
        {
            _tag.Name = _viewModel.Name;
            _tag.Color = _viewModel.Color;

            // Check if Name and Color is not empty
            if (string.IsNullOrEmpty(_tag.Name) || string.IsNullOrEmpty(_tag.Color))
            {
                _main.AddNotification(new Notification("ERROR! A required field wasn't filled.", Notification.ERROR));
                return;
            }


            // Add fields to tag
            _tag.FieldsList = new List<Field>();
            foreach (var shownFields in _viewModel.FieldsList)
            {
                shownFields.Field.DefaultValue = shownFields.Field.Content;
                _tag.AddField(shownFields.Field);
            }
            
            foreach (var shownField in _viewModel.HiddenFields)
            {
                _tag.AddField(shownField.Field);
            }

            Department department = _main.CurrentDepartment;
            if (department != null && department.ID != 0)
            {
                _tag.DepartmentID = department.ID;
                Tag parent = _viewModel.ParentTagsList[_viewModel.SelectedParentIndex];
                _tag.ParentID = parent.ID;

                // Save tag
                if (_editing)
                {
                    Log<Tag>.CreateLog(_tag);
                    _rep.Update(_tag);
                }

                else
                {
                    _rep.Insert(_tag, out ulong id);
                    Log<Tag>.CreateLog(_tag, id);
                }

                // Change view to tags
                _main.ChangeMainContent(new Tags(_main, _service));
            }
            else
                _main.AddNotification(new Notification(
                    "ERROR! Department cannot be none or All Departments. Please select a department.",
                    Notification.ERROR));
        }
    }
}