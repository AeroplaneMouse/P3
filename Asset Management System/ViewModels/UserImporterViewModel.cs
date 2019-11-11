using System;
using System.Linq;
using System.Collections.Generic;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.Users;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.ViewModels
{
    public class UserImporterViewModel : Base.BaseViewModel
    {
        #region Private Properties

        private UserRepository _rep { get; set; }

        private UserImporter _importer { get; set; }

        private List<User> _existingUsersList { get; set; }

        private List<User> _newUsersList { get; set; }

        private List<User> _finalUsersList { get; set; }

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

        #endregion

        #region Constructor

        public UserImporterViewModel()
        {
            _rep = new UserRepository();

            _importer = new UserImporter();

            _existingUsersList = _rep.GetAll().ToList();

            _newUsersList = _importer.Import();

            _finalUsersList = new List<User>();

            _finalUsersList.AddRange(_existingUsersList != null ? _existingUsersList : new List<User>());
            _finalUsersList.AddRange(_newUsersList != null ? _newUsersList : new List<User>());
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        #endregion
    }
}
