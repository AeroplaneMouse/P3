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

            //ShownUsersList = new List<UserWithStatus>()
            //{
            //    new UserWithStatus(new User() {Username = "Hans"}) {Status = "Added"},
            //    new UserWithStatus(new User() {Username = "Grethe"}) {Status = String.Empty},
            //};

            OnPropertyChanged(nameof(ShownUsersList));
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
            OnPropertyChanged(nameof(ShownUsersList));
        }

        #endregion
    }
}
