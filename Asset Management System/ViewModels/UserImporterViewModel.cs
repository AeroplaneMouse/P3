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

            // Initialize the final list of users
            FinalUsersList = new List<User>();

            ConflictingUsersList = CheckForDuplicates();

            FinalUsersList = ExistingUsersList;


            RemovedUsersList = ExistingUsersList
                .Where(u => u.IsEnabled == true)
                .Where(e => ImportedUsersList.Where(n => e.Username.CompareTo(n.Username) == 0).Count() == 0)
                .ToList();

            AddedUsersList = ImportedUsersList
                .Where(i => ExistingUsersList.Where(e => i.Username.CompareTo(e.Username) == 0).Count() == 0)
                .ToList();

            FinalUsersList.AddRange(AddedUsersList);

            ShownUsersList = FinalUsersList.Select(u =>
            {
                UserWithStatus user = new UserWithStatus(u);
                user.Status = RemovedUsersList.Contains(u) == true ? "Removed" : AddedUsersList.Contains(u) == true ? "Added" : ConflictingUsersList.Contains(u) == true ? "Conflict" : "";
                return user;

            }).ToList();

            ShownUsersList.OrderBy(p => p.IsEnabled);
             
            //FinalUsersList = ExistingUsersList.Where(u => u.IsEnabled == true).ToList();

            //FinalUsersList.RemoveAll(p => RemovedUsersList.Where(r => p.Username.CompareTo(r.Username) == 0).Count() > 0);
            //FinalUsersList.AddRange(AddedUsersList);

            //_finalUsersList.AddRange(_existingUsersList != null ? _existingUsersList.Where(p => p.IsEnabled).ToList() : new List<User>());
            //_finalUsersList.AddRange(_newUsersList != null ? _newUsersList : new List<User>());

            // Initialize commands 
            CancelCommand = new Base.RelayCommand(() => _main.ChangeMainContent(new Views.Assets(_main)));
            ApplyCommand = new Base.RelayCommand(Apply);
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        private List<User> CheckForDuplicates()
        {
            var conflictingUsers = new List<User>();

            conflictingUsers = ExistingUsersList
                .Where(u => u.IsEnabled == false)
                .Where(e => ImportedUsersList.Where(n => e.Username.CompareTo(n.Username) == 0).Count() > 0)
                .ToList();

            return conflictingUsers;
        }

        private void Apply()
        {
            foreach (User user in FinalUsersList)
            {
                _rep.Insert(user, out ulong id);
            }
        }

        #endregion
    }
}
