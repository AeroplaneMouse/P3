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

        private List<User> _existingUsersList { get; set; }

        private List<User> _newUsersList { get; set; }

        private List<User> _finalUsersList { get; set; }

        private List<User> _conflictingUsersList { get; set; }
        #endregion

        #region Public Properties

        public List<User> ExistingUsersList
        {
            get => _existingUsersList;
            set => _existingUsersList = value;
        }

        public List<User> NewUsersList
        {
            get => _newUsersList;
            set => _newUsersList = value;
        }

        public List<User> FinalUsersList
        {
            get => _finalUsersList;
            set => _finalUsersList = value;
        }

        public List<User> ConflictingUsersList
        {
            get => _conflictingUsersList;
            set => _conflictingUsersList = value;
        }

        public string Title { get; set; } = "Import Users";

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
            _existingUsersList = _rep.GetAll().ToList();

            // Get the new users from a .txt file
            _newUsersList = _importer.Import();

            // Initialize the final list of users
            _finalUsersList = new List<User>();

            _conflictingUsersList = CheckForDuplicates();






            _finalUsersList.AddRange(_existingUsersList != null ? _existingUsersList.Where(p => p.IsEnabled).ToList() : new List<User>());
            _finalUsersList.AddRange(_newUsersList != null ? _newUsersList : new List<User>());

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

            conflictingUsers = _existingUsersList
                .Where(u => u.IsEnabled == false)
                .Where(e => _newUsersList.Where(n => e.Username.CompareTo(n.Username) == 0).Count() > 0)
                .ToList();

            return conflictingUsers;
        }

        private void Apply()
        {
            foreach (User user in _finalUsersList)
            {
                _rep.Insert(user, out ulong id);
            }
        }

        #endregion
    }
}
