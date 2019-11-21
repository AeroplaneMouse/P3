using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AMS.Controllers
{
    public class UserListController : IUserListController
    {
        #region Public Properties

        public List<UserWithStatus> UserList
        {
            get
            {
                return (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(u => u.IsShown == true)
                    .OrderByDescending(p => p.IsEnabled)
                    .OrderBy(p => p.Username)
                    .OrderByDescending(p => p.Status.CompareTo("Removed") == 0)
                    .OrderByDescending(p => p.Status.CompareTo("Added") == 0)
                    .OrderByDescending(p => p.Status.CompareTo("Conflicting") == 0)
                    .ToList();
            }

            set => _finalUsersList = value;
        }

        public List<Department> DepartmentList
        {
            get
            {
                if (_departmentList == null)
                {
                    return _departmentRep.GetAll().ToList();
                }

                return _departmentList;
            }

            set => _departmentList = value;
        }

        // Checkboxes
        public bool IsShowingAdded
        {
            get => _isShowingAdded;
        
            set
            {
                _isShowingAdded = value;

                (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(p => p.Status.CompareTo("Added") == 0)
                    .ToList()
                    .ForEach(p => p.IsShown = value);
            }
        }

        public bool IsShowingRemoved
        {
            get => _isShowingRemoved;

            set
            {
                _isShowingRemoved = value;

                (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(p => p.Status.CompareTo("Removed") == 0)
                    .ToList()
                    .ForEach(p => p.IsShown = value);
            }
        }

        public bool IsShowingConflicting
        {
            get => _isShowingConflicting; 

            set
            {
                _isShowingConflicting = value;

                (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(p => p.Status.CompareTo("Conflicting") == 0)
                    .ToList()
                    .ForEach(p => p.IsShown = value);
            }
        }

        public bool IsShowingDisabled
        {
            get => _isShowingDisabled;

            set
            {
                _isShowingDisabled = value;

                (_finalUsersList ?? new List<UserWithStatus>())
                    .Where(p => p.IsEnabled == false && p.Status.CompareTo("Conflicting") != 0)
                    .ToList()
                    .ForEach(p => p.IsShown = value);
            }
        }

        #endregion

        #region Private Properties

        private IUserRepository _userRep { get; set; }

        private IDepartmentRepository _departmentRep { get; set; }

        private IUserImporter _importer { get; set; }

        // Lists
        private List<UserWithStatus> _importedUsersList { get; set; }

        private List<UserWithStatus> _existingUsersList { get; set; }

        private List<UserWithStatus> _finalUsersList { get; set; }

        private List<Department> _departmentList { get; set; }


        // Checkboxes
        private bool _isShowingAdded { get; set; }

        private bool _isShowingRemoved { get; set; }

        private bool _isShowingConflicting { get; set; }

        private bool _isShowingDisabled { get; set; }

        #endregion

        #region Constructor

        public UserListController(IUserImporter importer, IUserRepository userRep, IDepartmentRepository departmentRep)
        {
            _importer = importer;

            _userRep = userRep;
            _departmentRep = departmentRep;

            _isShowingAdded = true;
            _isShowingConflicting = true;
            _isShowingDisabled = false;
            _isShowingRemoved = true;

            GetExistingUsers();

            _finalUsersList = _existingUsersList;

            UpdateShownUsers(_finalUsersList);
        }

        #endregion

        #region Public Methods

        public bool ApplyChanges()
        {
            // Check if there are any conflicts left
            if (_finalUsersList.Where(p => p.Status.CompareTo("Conflicting") == 0).Count() > 0)
            {
                return false;
            }

            // Disable the removed users in the database
            _finalUsersList
                .Where(p => p.Status.CompareTo("Removed") == 0)
                .ToList()
                .ForEach(p =>
                {
                    p.IsEnabled = false;
                    _userRep.Update(p);
                });

            // Insert added users to the database
            _finalUsersList
                .Where(p => p.Status.CompareTo("Added") == 0)
                .ToList()
                .ForEach(p => _userRep.Insert(p, out ulong id));

            // Update the users that weren't removed, as they may have gotten new descriptions, etc.
            _finalUsersList
                .Where(p => p.Status.CompareTo(String.Empty) == 0)
                .ToList()
                .ForEach(p => _userRep.Update(p));

            GetExistingUsers();
            _finalUsersList = _existingUsersList;
            UpdateShownUsers(_finalUsersList);

            return true;
        }

        public void CancelChanges()
        {
            GetExistingUsers();
            _finalUsersList = _existingUsersList;
            UpdateShownUsers(_finalUsersList);
        }

        public void KeepUser(object user)
        {
            // Get the kept user
            UserWithStatus keptUser = user as UserWithStatus;

            // If there weren't any selected users, or the selected user is not in conflict
            if (keptUser == null || keptUser.Status.CompareTo("Conflicting") != 0)
            {
                return;
            }

            // Get the other conflicting user
            UserWithStatus otherUser = _finalUsersList
                .Where(p => p.Username.CompareTo(keptUser.Username) == 0 && p.Equals(keptUser) == false)
                .FirstOrDefault();

            // If the kept user is coming from the imported list:
            // Set their status to "Added".
            // Set the existing users status to empty, add the current date to their username, and set them to not show.
            if (_importedUsersList.Contains(keptUser))
            {
                keptUser.Status = "Added";

                otherUser.Status = String.Empty;
                otherUser.Username = otherUser.Username + " (" + DateTime.Now.ToString() + ")";
                otherUser.IsShown = IsShowingDisabled;
            }

            // If the kept user is coming from the database:
            // Set their status to String.Empty and IsEnabled to true.
            // Remove the imported user from the final list.
            else if (_existingUsersList.Contains(keptUser))
            {
                keptUser.Status = String.Empty;
                keptUser.IsEnabled = true;
                keptUser.IsShown = true;

                _finalUsersList.Remove(otherUser);
            }
        }

        public void ChangeStatusOfUser(object user)
        {
            if (user == null)
                return;

            UserWithStatus selectedUser = user as UserWithStatus;

            if (selectedUser.Status.CompareTo(String.Empty) == 0)
            {
                selectedUser.IsEnabled = !selectedUser.IsEnabled;
            }

            UpdateShownUsers(_finalUsersList);
        }
       
        public void GetExistingUsers()
        {
            _existingUsersList = _importer.ImportUsersFromDatabase().Select(u => new UserWithStatus(u)).ToList();
        }

        public void GetUsersFromFile()
        {
            string filePath = _importer.GetUsersFile();

            if (!string.IsNullOrEmpty(filePath))
            {
                _importedUsersList = _importer.ImportUsersFromFile(filePath).Select(u => new UserWithStatus(u)).ToList();
                _finalUsersList = _importer.CombineLists(_importedUsersList, _existingUsersList);
                UpdateShownUsers(_finalUsersList);
            }
        }

        #endregion

        #region Private Methods

        private void UpdateShownUsers(List<UserWithStatus> list)
        {
            list.ForEach(u =>
                {
                    if (u.Status.CompareTo("Added") == 0)
                        u.IsShown = IsShowingAdded;

                    else if (u.Status.CompareTo("Removed") == 0)
                        u.IsShown = IsShowingRemoved;

                    else if (u.Status.CompareTo("Conflicting") == 0)
                        u.IsShown = IsShowingConflicting;

                    else if (u.Status.CompareTo(String.Empty) == 0 && u.IsEnabled == false)
                        u.IsShown = IsShowingDisabled;
                });
        }

        #endregion
    }
}
