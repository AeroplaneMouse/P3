using AMS.Controllers.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AMS.ViewModels
{
    public class UserListViewModel : Base.BaseViewModel
    {
        #region Public Properties

        public string Title { get; set; }

        public ObservableCollection<UserWithStatus> ShownUsersList
        {
            get => new ObservableCollection<UserWithStatus>(_userListController.UsersList);
            set => _userListController.UsersList = value.ToList();
        }

        //public List<UserWithStatus> ShownUsersList { get; set; }

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
                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        public bool IsShowingConflicting
        {
            get => _userListController.IsShowingConflicting;
            set
            {
                _userListController.IsShowingConflicting = value;
                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        public bool IsShowingRemoved
        {
            get => _userListController.IsShowingRemoved;
            set
            {
                _userListController.IsShowingRemoved = value;
                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        public bool IsShowingDisabled
        {
            get => _userListController.IsShowingDisabled;
            set
            {
                _userListController.IsShowingDisabled = value;
                OnPropertyChanged(nameof(ShownUsersList));
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

        public ICommand ImportUsersCommand { get; set; }

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
            ImportUsersCommand = new Base.RelayCommand(Import);

            OnPropertyChanged(nameof(ShownUsersList));
        }

        #endregion

        #region Private Methods

        private void Import()
        {
            _userListController.GetUsersFromFile();
            OnPropertyChanged(nameof(ShownUsersList));
        }

        private void Cancel()
        {
            _userListController.CancelChanges();
            
            _main.AddNotification(new Notification("Changes cancelled", Notification.ERROR));

            OnPropertyChanged(nameof(ShownUsersList));
        }

        private void Apply()
        {
            if (_userListController.ApplyChanges())
                _main.AddNotification(new Notification("Changes applied", Notification.APPROVE));

            else
                _main.AddNotification(new Notification("Not all conflicts solved", Notification.WARNING));

            OnPropertyChanged(nameof(ShownUsersList));
        }

        private void KeepUser(object user)
        {
            _userListController.KeepUser(user);
            OnPropertyChanged(nameof(ShownUsersList));
        }

        #endregion
    }
}
