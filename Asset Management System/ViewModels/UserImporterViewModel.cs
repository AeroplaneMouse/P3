using Asset_Management_System.Authentication;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.Interfaces;
using Asset_Management_System.Resources.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    
    // Needs notifications

    public class UserImporterViewModel : Base.BaseViewModel, IListUpdate
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

        private int _selectedItemIndex { get; set; }

        #endregion

        #region Public Properties

        public string Title { get; set; } = "Import Users";

        public List<UserWithStatus> ShownUsersList
        {
            get
            {
                return (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(u => u.IsShown == true)
                    .OrderByDescending(p => p.IsEnabled)
                    .OrderBy(p => p.Username)
                    .OrderByDescending(p => p.Status.CompareTo("Removed") == 0)
                    .OrderByDescending(p => p.Status.CompareTo("Added") == 0)
                    .OrderByDescending(p => p.Status.CompareTo("Conflict") == 0)
                    .ToList();
            }

            set => _finalUsersList = value;
        }

        public int SelectedItemIndex
        {
            get => _selectedItemIndex;
            set => _selectedItemIndex = value;
        }

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

        public ICommand KeepUserCommand { get; set; }

        #endregion

        #region Constructor

        public UserImporterViewModel(MainViewModel main)
        {
            // Because page needs to be in the excluded pages, because of the GetAllUsers
            if (main == null)
            {
                return;
            }

            // Initialize checkboxes
            IsShowingAdded = true;
            IsShowingRemoved = true;
            IsShowingConflicting = true;
            IsShowingDisabled = false;

            _main = main;

            _rep = new UserRepository();

            _importer = new UserImporter();

            GetAllUsers();

            // Initialize commands 
            CancelCommand = new Base.RelayCommand(Cancel);
            ApplyCommand = new Base.RelayCommand(Apply);
            KeepUserCommand = new Base.RelayCommand(KeepUser);
        }

        #endregion

        #region Public Methods

        public void UpdateList()
        {
            OnPropertyChanged(nameof(ShownUsersList));
        }

        #endregion

        #region Private Methods

        private void GetAllUsers()
        {
            // Get the users already in the database
            _existingUsersList = (_rep.GetAll() ?? new List<User>())
                .Select(u => new UserWithStatus(u))
                .ToList();

            // Import the users from the user file
            _importedUsersList = (_importer.Import() ?? new List<User>())
                .Select(u => new UserWithStatus(u))
                .ToList();

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

            // Kept users. Users that are enabled, and are in both lists. Remove the copy coming from the imported file
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

        // TODO: Ville være virkelig fint hvis den her knap sad på hver bruger i listen, i stedet for én stor
        private void KeepUser()
        {
            // Get the user that is currently selected. This is the user that is kept
            UserWithStatus keptUser = GetSelectedItem();

            if (keptUser == null || keptUser.Status.CompareTo("Conflict") != 0)
            {
                return;
            }

            // Get the other conflicting user
            UserWithStatus otherUser = _finalUsersList
                .Where(p => p.Username.CompareTo(keptUser.Username) == 0 && p.Equals(keptUser) == false)
                .FirstOrDefault();

            // If the kept user i coming from the imported list: 
            // Set their status to "Added". 
            // Set the existing users status to empty, add the current date to their username, and set them to not show
            if (_importedUsersList.Contains(keptUser))
            {
                keptUser.Status = "Added";

                otherUser.Status = String.Empty;
                otherUser.Username = otherUser.Username + " (" + DateTime.Now.ToString() + ")";
                otherUser.IsShown = IsShowingDisabled;
            }

            // Hvis brugeren der beholdes er den eksisterende bruger, skal deres status være String.Empty, IsEnabled skal sættes til true og den importerede bruger bliver fjernet fra listen.
            else if (_existingUsersList.Contains(keptUser))
            {
                keptUser.Status = String.Empty;
                keptUser.IsEnabled = true;
                keptUser.IsShown = true;

                _finalUsersList.Remove(otherUser);
            }

            else
            {
                Console.WriteLine("Error. User not found in either list");
            }

            OnPropertyChanged(nameof(ShownUsersList));
        }

        // Goes back to the page that the user came from
        // TODO: Make this go back, instead of going to the assets page
        private void Cancel()
        {
            _main.ChangeMainContent(new Views.Home(_main));
        }

        // Applies the changes made to the users to the database
        private void Apply()
        {
            // Check if there are any conflicts left
            if (_finalUsersList.Where(p => p.Status.CompareTo("Conflict") == 0).Count() > 0)
            {
                Console.WriteLine("Not all conflicts are solved");
                return;
            }

            // Disable the removed users in the database
            _finalUsersList
                .Where(p => p.Status.CompareTo("Removed") == 0)
                .ToList()
                .ForEach(p =>
                {
                    p.IsEnabled = false;
                    _rep.Update(p);
                });

            // Insert added users to the database
            _finalUsersList
                .Where(p => p.Status.CompareTo("Added") == 0)
                .ToList()
                .ForEach(p => _rep.Insert(p, out ulong id));

            // Update the users that weren't removed, as they may have gotten new descriptions, etc.
            _finalUsersList
                .Where(p => p.Status.CompareTo(String.Empty) == 0)
                .ToList()
                .ForEach(p => _rep.Update(p));

            _main.ChangeMainContent(new Views.Home(_main));
        }

        #endregion


        private UserWithStatus GetSelectedItem()
        {
            if (ShownUsersList.Count == 0)
                return null;
            else
                return ShownUsersList.ElementAtOrDefault(SelectedItemIndex);
        }
    }
}
