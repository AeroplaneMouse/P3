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

        private Session _session { get; set; }

        private string _domain { get; set; }

        #endregion

        #region Public Properties

        public List<User> ExistingUsersList { get; set; }

        public List<User> FinalUsersList { get; set; }

        public List<User> ConflictingUsersList { get; set; }

        public List<User> RemovedUsersList { get; set; }

        public List<User> AddedUsersList { get; set; }

        public List<User> ImportedUsersList { get; set; }

        public string Title { get; set; } = "Import Users";

        public List<UserWithStatus> ShownUsersList { get; set; }

        public List<User> KeptUsersList { get; set; }

        #endregion

        #region Commands

        public ICommand CancelCommand { get; set; }

        public ICommand ApplyCommand { get; set; }

        #endregion

        #region Constructor

        public UserImporterViewModel(MainViewModel main)
        {
            _main = main;

            _rep = new UserRepository();

            _importer = new UserImporter();

            _session = new Session();

            _domain = _session.Domain;


            // Get all existing users 
            ExistingUsersList = _rep.GetAll().ToList();

            // Get the new users from a .txt file
            ImportedUsersList = _importer.Import();

            if (ImportedUsersList == null)
            {
                return;
            }


            // TODO: Kombiner listerne ét sted, så det er nemt at splitte dem ad senere







            // Initialize the final list of users
            FinalUsersList = new List<User>();

            // Finds the users that are already in the database and disabled
            ConflictingUsersList = CheckForDuplicates();

            // Finds users that are not on the imported list, and are on the database. These need to be removed
            RemovedUsersList = ExistingUsersList
                .Where(u => u.IsEnabled == true)
                .Where(e => ImportedUsersList.Where(n => e.Username.CompareTo(n.Username) == 0).Count() == 0)
                .ToList();

            KeptUsersList = ExistingUsersList
                .Where(u => u.IsEnabled)
                .Where(e => !RemovedUsersList.Contains(e)).ToList();

            // Finds users who are on the imported list, and are not on the databse. These need to be added
            AddedUsersList = ImportedUsersList
                .Where(i => ExistingUsersList.Where(e => i.Username.CompareTo(e.Username) == 0).Count() == 0)
                .ToList();

            // Initialize final users list with the existing users in the database
            FinalUsersList.AddRange(ExistingUsersList);

            // Add the users that are being added to the database, to the final user list
            FinalUsersList.AddRange(AddedUsersList);

            // Add the users that are in conflict so that the Admin can manage the conflicts
            FinalUsersList.AddRange(ImportedUsersList.Where(p => ConflictingUsersList.Where(c => p.Username.CompareTo(c.Username) == 0).Count() > 0).ToList());

            // Convert the final user list, to a list that also shows the status of each user
            UpdateShownUsersList("All");

            // Initialize commands 
            CancelCommand = new Base.RelayCommand(() => _main.ChangeMainContent(new Views.Assets(_main)));
            ApplyCommand = new Base.RelayCommand(Apply);
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        private void UpdateShownUsersList(string predicate)
        {
            ShownUsersList = new List<UserWithStatus>();

            if (predicate.CompareTo("All") == 0)
            {
                ShownUsersList = FinalUsersList
                .Select(u =>
                {
                    UserWithStatus user = new UserWithStatus(u);
                    user.Status = RemovedUsersList.Contains(u) == true ? "Removed" :
                                  AddedUsersList.Contains(u) == true ? "Added" :
                                  ConflictingUsersList.Contains(u) == true ? "Conflict" : "";
                    return user;

                })
                // Sort the list in the descending order: Conflicts -> Removed -> Added -> IsEnabled == false
                .OrderBy(p => p.IsEnabled)
                .OrderBy(p => p.Username)
                .OrderByDescending(p => p.Status.CompareTo("Added") == 0)
                .OrderByDescending(p => p.Status.CompareTo("Removed") == 0)
                .OrderByDescending(p => p.Status.CompareTo("Conflict") == 0)
                .ToList();
            }
        }

        private List<User> CheckForDuplicates()
        {
            // Find existing users that are in conflict with the imported users
            var conflictingUsers = ExistingUsersList
                .Where(u => u.IsEnabled == false)
                .Where(e => ImportedUsersList.Where(n => e.Username.CompareTo(n.Username) == 0).Count() > 0)
                .ToList();

            // Find the imported users that are in conflict
            var importedConflictingUsers = ImportedUsersList
                .Where(p => conflictingUsers.Where(c => p.Username.CompareTo(c.Username) == 0).Count() > 0)
                .ToList();

            conflictingUsers.AddRange(importedConflictingUsers);

            return conflictingUsers;
        }

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
