using AMS.Controllers.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AMS.ViewModels
{
    public class UserListViewModel : Base.BaseViewModel
    {
        #region Public Properties

        public string Title { get; set; }

        public List<UserWithStatus> UserList
        {
            get => _userListController.UsersList;
            set => _userListController.UsersList = value;
        }

        public List<Department> DepartmentsList
        {
            get => _userListController.DepartmentsList;
            set => _userListController.DepartmentsList = value;
        }

        // Checkboxes
        public bool IsShowingAdded
        {
            get => _userListController.IsShowingAdded;
            set
            {
                _userListController.IsShowingAdded = value;
                OnPropertyChanged(nameof(UserList));
            }
        }

        public bool IsShowingConflicting
        {
            get => _userListController.IsShowingConflicting;
            set
            {
                _userListController.IsShowingConflicting = value;
                OnPropertyChanged(nameof(UserList));
            }
        }

        public bool IsShowingRemoved
        {
            get => _userListController.IsShowingRemoved;
            set
            {
                _userListController.IsShowingRemoved = value;
                OnPropertyChanged(nameof(UserList));
            }
        }

        public bool IsShowingDisabled
        {
            get => _userListController.IsShowingDisabled;
            set
            {
                _userListController.IsShowingDisabled = value;
                OnPropertyChanged(nameof(UserList));
            }
        }

        #endregion

        #region Private Properties

        private IUserListController _userListController { get; set; }

        private MainViewModel _main { get; set; }

        #endregion

        #region Commands

        public ICommand CancelCommand { get; set; }

        public ICommand ApplyCommand { get; set; }

        public ICommand KeepUserCommand { get; set; }

        #endregion

        #region Constructor

        public UserListViewModel(MainViewModel main, IUserListController userListController)
        {
            if (main != null)
            {
                _main = main;
            }

            Title = "Users";

            _userListController = userListController;

            CancelCommand = new Base.RelayCommand(Cancel);
            ApplyCommand = new Base.RelayCommand(Apply);
            KeepUserCommand = new Base.RelayCommand<object>(KeepUser);

            OnPropertyChanged(nameof(UserList));
        }

        #endregion

        #region Private Methods

        private void Cancel()
        {
            _userListController.CancelChanges();
            _main.ContentFrame.GoBack();
        }

        private void Apply()
        {
            _userListController.ApplyChanges();
            _main.ContentFrame.GoBack();
        }

        private void KeepUser(object user)
        {
            _userListController.KeepUser(user);
        }

        #endregion
    }
}
