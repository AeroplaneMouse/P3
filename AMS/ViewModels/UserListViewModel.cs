using AMS.Controllers.Interfaces;
using AMS.Events;
using AMS.Models;
using AMS.Views.Prompts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace AMS.ViewModels
{
    public class UserListViewModel : Base.BaseViewModel
    {
        #region Public Properties

        public string Title { get; set; }

        public ObservableCollection<UserWithStatus> ShownUsersList
        {
            get => new ObservableCollection<UserWithStatus>(_userListController.UserList);
        }

        public ObservableCollection<Department> DepartmentList
        {
            get => new ObservableCollection<Department>(_userListController.DepartmentList);
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

        #endregion

        #region Commands

        public ICommand CancelCommand { get; set; }

        public ICommand ApplyCommand { get; set; }

        public ICommand KeepUserCommand { get; set; }

        public ICommand ImportUsersCommand { get; set; }

        public ICommand ChangeStatusCommand { get; set; }

        #endregion

        #region Constructor

        public UserListViewModel(IUserListController userListController)
        {
            Title = "Users";

            _userListController = userListController;

            CancelCommand = new Base.RelayCommand(Cancel);
            ApplyCommand = new Base.RelayCommand(() => Features.DisplayPrompt(new Confirm("Are you sure you want to these changes?", Apply)));
            KeepUserCommand = new Base.RelayCommand<object>(KeepUser);
            ImportUsersCommand = new Base.RelayCommand(Import);

            ChangeStatusCommand = new Base.RelayCommand<object>(ChangeStatus);

            OnPropertyChanged(nameof(ShownUsersList));
        }

        #endregion

        #region Private Methods

        private void ChangeStatus(object user)
        {
            _userListController.ChangeStatusOfUser(user);
            OnPropertyChanged(nameof(ShownUsersList));
        }

        private void Import()
        {
            _userListController.GetUsersFromFile();
            OnPropertyChanged(nameof(ShownUsersList));
        }

        private void Cancel()
        {
            _userListController.CancelChanges();
            
            Features.AddNotification(new Notification("Changes cancelled", Notification.ERROR));

            OnPropertyChanged(nameof(ShownUsersList));
        }

        private void Apply(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                if (_userListController.ApplyChanges())
                    Features.AddNotification(new Notification("Changes applied", Notification.APPROVE));

                else
                    Features.AddNotification(new Notification("Not all conflicts solved", Notification.WARNING));
            }

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
