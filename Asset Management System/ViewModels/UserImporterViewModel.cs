using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private List<Department> _departments { get; set; }

        #endregion

        #region Public Properties

        public List<User> ExistingUsersList
        {
            get
            {
                //if (_existingUsersList == null)
                //{
                //    _existingUsersList = _rep.GetAll().ToList();
                //}

                return _existingUsersList;
            }
            set => _existingUsersList = value;
        }

        public List<User> NewUsersList
        {
            get
            {
                //if (_newUsersList == null)
                //{
                //    _newUsersList = _importer.Import();
                //}

                return _newUsersList;
            }

            set => _newUsersList = value;
        }

        public List<User> FinalUsersList
        {
            get
            {
                //if (_finalUsersList == null)
                //{
                //    _finalUsersList = new List<User>();
                //}

                return _finalUsersList;
            }

            set => _finalUsersList = value;
        }

        public List<Department> DepartmentList { get; set; }

        #endregion

        #region Constructor

        public UserImporterViewModel()
        {
            _rep = new UserRepository();

            _importer = new UserImporter();

            _existingUsersList = _rep.GetAll().ToList();

            _newUsersList = _importer.Import();

            _finalUsersList = new List<User>();

            _finalUsersList.AddRange(_existingUsersList);
            _finalUsersList.AddRange(_newUsersList);

            DepartmentList = new DepartmentRepository().GetAll().ToList();
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods

        #endregion
    }
}
