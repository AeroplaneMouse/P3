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
        public string Title { get; set; }

        public ObservableCollection<UserWithStatus> ShownUsersList => new ObservableCollection<UserWithStatus>(_userListController.UserList);
        public ObservableCollection<Department> DepartmentList => new ObservableCollection<Department>(_userListController.DepartmentList);

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

        private IUserListController _userListController { get; set; }

        public ICommand CancelCommand { get; set; }
        public ICommand ApplyCommand { get; set; }
        public ICommand KeepUserCommand { get; set; }
        public ICommand ImportUsersCommand { get; set; }
        public ICommand ChangeStatusCommand { get; set; }

        public UserListViewModel(IUserListController userListController)
        {
            Title = "Users";

            _userListController = userListController;

            CancelCommand = new Base.RelayCommand(() => Features.DisplayPrompt(new Confirm("Are you sure you want to cancel these changes?", Cancel)));
            ApplyCommand = new Base.RelayCommand(() => Features.DisplayPrompt(new Confirm("Are you sure you want to apply these changes?", Apply)));
            KeepUserCommand = new Base.RelayCommand<object>(KeepUser);
            ImportUsersCommand = new Base.RelayCommand(Import);

            ChangeStatusCommand = new Base.RelayCommand<object>(ChangeStatus);

            OnPropertyChanged(nameof(ShownUsersList));
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(ShownUsersList));
            OnPropertyChanged(nameof(DepartmentList));
            OnPropertyChanged(nameof(IsShowingAdded));
            OnPropertyChanged(nameof(IsShowingConflicting));
            OnPropertyChanged(nameof(IsShowingDisabled));
            OnPropertyChanged(nameof(IsShowingRemoved));
        }

        /// <summary>
        /// Changes the status of the input user
        /// </summary>
        /// <param name="user"></param>
        private void ChangeStatus(object user)
        {
            _userListController.ChangeStatusOfUser(user);
            OnPropertyChanged(nameof(ShownUsersList));
        }

        /// <summary>
        /// Imports users from a file
        /// </summary>
        private void Import()
        {
            _userListController.GetUsersFromFile();
            OnPropertyChanged(nameof(ShownUsersList));
        }

        /// <summary>
        /// Cancels all changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                _userListController.CancelChanges();

                Features.AddNotification(new Notification("Changes cancelled", Notification.ERROR));

                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        /// <summary>
        /// Applies the changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                if (_userListController.ApplyChanges())
                    Features.AddNotification(new Notification("Changes applied", Notification.APPROVE));

                else
                    Features.AddNotification(new Notification("Not all conflicts solved", Notification.WARNING));

                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        /// <summary>
        /// Keeps the selected user in an import conflict
        /// </summary>
        /// <param name="user"></param>
        private void KeepUser(object user)
        {
            _userListController.KeepUser(user);
            OnPropertyChanged(nameof(ShownUsersList));
        }
    }
}
