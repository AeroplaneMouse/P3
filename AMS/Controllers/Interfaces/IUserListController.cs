using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface IUserListController
    {
        #region Properties

        List<UserWithStatus> UsersList { get; set; }

        IUserImporter Importer { get; set; }

        IUserService UserService { get; set; }

        IUserRepository UserRepository { get; set; }

        #endregion

        #region Methods

        void KeepUser(User user);

        void CancelChanges();

        void ApplyChanges();

        void SortUsers();

        #endregion
    }
}
