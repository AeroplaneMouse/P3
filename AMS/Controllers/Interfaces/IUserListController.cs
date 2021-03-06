using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface IUserListController
    {
        #region Properties

        List<UserWithStatus> UserList { get; set; }

        List<Department> DepartmentList { get; set; }

        bool IsShowingAdded { get; set; }

        bool IsShowingConflicting { get; set; }

        bool IsShowingDisabled { get; set; }

        bool IsShowingRemoved { get; set; }

        #endregion

        #region Methods

        void GetExistingUsers();

        void GetUsersFromFile();

        void KeepUser(object user);

        void ChangeStatusOfUser(object user);

        void CancelChanges();

        bool ApplyChanges();

        #endregion

        

        
    }
}
