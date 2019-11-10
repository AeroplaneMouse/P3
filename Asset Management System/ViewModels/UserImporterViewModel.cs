using Asset_Management_System.Authentication;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class UserImporterViewModel : Base.BaseViewModel
    {
        #region Private Properties

        private MainViewModel _main { get; set; }

        private UserRepository _rep { get; set; }

        private UserImporter _importer { get; set; }


        // Checkboxes
        private bool _isShowingAdded { get; set; }

        private bool _isShowingRemoved { get; set; }

        private bool _isShowingConflicting { get; set; }

        private bool _isShowingDisabled { get; set; }

        // Lists
        private List<UserWithStatus> _importedUsersList { get; set; }

        private List<UserWithStatus> _existingUsersList { get; set; }

        private List<UserWithStatus> _finalUsersList { get; set; }

        #endregion

        #region Public Properties

        public string Title { get; set; } = "Import Users";

        public List<UserWithStatus> ShownUsersList
        {
            get
            {
                if (_finalUsersList == null)
                {
                    _finalUsersList = new List<UserWithStatus>();
                }

                return _finalUsersList
                    .Where(u => u.IsShown == true)
                    .OrderBy(p => p.IsEnabled)
                    .OrderBy(p => p.Username)
                    .OrderByDescending(p => p.Status.CompareTo("Added") == 0)
                    .OrderByDescending(p => p.Status.CompareTo("Removed") == 0)
                    .OrderByDescending(p => p.Status.CompareTo("Conflict") == 0)
                    .ToList();
            }

            set => _finalUsersList = value;
        }

        public UserWithStatus SelectedItemIndex { get; set; }

        // Checkboxes
        public bool IsShowingAdded
        {
            get
            {
                return _isShowingAdded;

            }

            set
            {
                _isShowingAdded = value;

                (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(p => p.Status.CompareTo("Added") == 0)
                    .ToList()
                    .ForEach(p => p.IsShown = value);

                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        public bool IsShowingRemoved
        {
            get
            {
                return _isShowingRemoved;
            }

            set
            {
                _isShowingRemoved = value;

                (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(p => p.Status.CompareTo("Removed") == 0)
                    .ToList()
                    .ForEach(p => p.IsShown = value);

                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        public bool IsShowingConflicting
        {
            get
            {
                return _isShowingConflicting;
            }

            set
            {
                _isShowingConflicting = value;

                (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(p => p.Status.CompareTo("Conflict") == 0)
                    .ToList()
                    .ForEach(p => p.IsShown = value);

                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        public bool IsShowingDisabled
        {
            get
            {
                return _isShowingDisabled;
            }

            set
            {
                _isShowingDisabled = value;

                (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(p => p.IsEnabled == false && p.Status.CompareTo("Conflict") != 0)
                    .ToList()
                    .ForEach(p => p.IsShown = value);

                OnPropertyChanged(nameof(ShownUsersList));
            }
        }

        #endregion

        #region Commands

        public ICommand CancelCommand { get; set; }

        public ICommand ApplyCommand { get; set; }

        #endregion

        #region Constructor

        public UserImporterViewModel(MainViewModel main)
        {
            // Initialize checkboxes
            IsShowingAdded = true;
            IsShowingRemoved = true;
            IsShowingConflicting = true;
            IsShowingDisabled = false;

            _main = main;

            GetAllUsers();

            

            // Initialize commands 
            CancelCommand = new Base.RelayCommand(Cancel);
            ApplyCommand = new Base.RelayCommand(Apply);   
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        private void GetAllUsers()
        {
            _rep = new UserRepository();

            _importer = new UserImporter();

            // Get the users already in the database
            _existingUsersList = _rep
                .GetAll()
                .Select(u => new UserWithStatus(u))
                .ToList();

            // Import the users from the user file
            _importedUsersList = _importer
                .Import()
                .Select(u => new UserWithStatus(u))
                .ToList();

            if (_importedUsersList == null)
            {
                return;
            }

            // List with all users
            _finalUsersList = new List<UserWithStatus>();
            _finalUsersList.AddRange(_existingUsersList);
            _finalUsersList.AddRange(_importedUsersList);

            // Conflicting users. Users that are not enabled, and are in both lists
            _finalUsersList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => IsInList(_existingUsersList.Where(p => p.IsEnabled == false).ToList(), u) && IsInList(_importedUsersList, u))
                .ToList()
                .ForEach(u =>
                {
                    u.Status = "Conflict";
                    u.IsShown = IsShowingConflicting;
                });

            // Added users. Users that are enabled, and only in the imported list
            _finalUsersList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => !IsInList(_existingUsersList.Where(p => p.IsEnabled == true).ToList(), u) && IsInList(_importedUsersList, u))
                .ToList()
                .ForEach(u =>
                {
                    u.Status = "Added";
                    u.IsShown = IsShowingAdded;
                });

            // Removed users. Users that are enabled, and are only in the existing list
            _finalUsersList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => IsInList(_existingUsersList.Where(p => p.IsEnabled == true).ToList(), u) && !IsInList(_importedUsersList, u))
                .ToList()
                .ForEach(u =>
                {
                    u.Status = "Removed";
                    u.IsShown = IsShowingRemoved;
                });

            // Kept users. Users that are enabled, and are in both lists
            _finalUsersList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => IsInList(_existingUsersList.Where(p => p.IsEnabled == true).ToList(), u) && IsInList(_importedUsersList, u))
                .Where(u => u.ID == 0)
                .ToList()
                .ForEach(u => _finalUsersList.Remove(u));

            // Set inactive users to hidden 
            _finalUsersList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => u.IsEnabled == false)
                .ToList()
                .ForEach(u => u.IsShown = IsShowingDisabled);
        }


        private bool IsInList(List<UserWithStatus> users, UserWithStatus user)
        {
            return users.Where(u => u.Username.CompareTo(user.Username) == 0).Count() > 0;
        }

        // Goes back to the page that the user came from
        // TODO: Make this go back, instead of going to the assets page
        private void Cancel()
        {
            _main.ChangeMainContent(new Views.Assets(_main));
        }

        // Applies the changes made to the users to the database
        private void Apply()
        {
            //foreach (User user in FinalUsersList)
            //{
            //    _rep.Insert(user, out ulong id);
            //}
        }

        #endregion
    }
}
