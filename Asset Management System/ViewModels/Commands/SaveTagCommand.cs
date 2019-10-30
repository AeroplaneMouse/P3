using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Logging;

namespace Asset_Management_System.ViewModels.Commands
{
    class SaveTagCommand : ICommand
    {
        private TagManagerViewModel _viewModel;
        private MainViewModel _main;
        private Tag _tag;
        private bool _editing;
        public event EventHandler CanExecuteChanged;

        public SaveTagCommand(TagManagerViewModel viewModel, MainViewModel main, Tag tag, bool editing)
        {
            _viewModel = viewModel;
            _main = main;
            _tag = tag;
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

            // Add fields to tag
            _tag.FieldsList = new List<Field>();
            foreach (var shownFields in _viewModel.FieldsList)
            {
                if (shownFields.Field.Required && shownFields.Field.Content == string.Empty)
                {
                    _main.AddNotification(new Notification("ERROR! A required field wasn't filled.", Notification.ERROR));
                    return;
                }
                _tag.AddField(shownFields.Field);
            }

            Department department = _main.CurrentDepartment;
            if (department != null && department.ID != 0)
            {
                _tag.DepartmentID = department.ID;
                Tag parent = _viewModel.ParentTagsList[_viewModel.SelectedParentIndex];
                _tag.ParentID = parent.ID;

                // Save tag
                TagRepository rep = new TagRepository();
                if (_editing)
                {
                    Log<Tag>.CreateLog(_tag);
                    rep.Update(_tag);
                }
                    
                else
                {
                    rep.Insert(_tag, out ulong id);
                    Log<Tag>.CreateLog(_tag, id);
                }
                
                // Change view to tags
                _main.ChangeMainContent(new Tags(_main));
            }
            else
                _main.AddNotification(new Notification("ERROR! Department cannot be none or All Departments. Please select a department.", Notification.ERROR));
        }
    }
}
