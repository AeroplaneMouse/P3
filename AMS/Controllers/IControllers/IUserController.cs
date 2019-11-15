using AMS.Interfaces;
using AMS.IO;
using AMS.Models;
using AMS.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.IControllers
{
    public interface IUserController
    {
        #region Properties

        IUserImporter Importer { get; set; }

        List<UserWithStatus> UsersList { get; set; }

        IUserService UserService { get; set; }

        #endregion

        #region Methods

        void KeepUser();

        void CancelChanges();

        void ApplyChanges();

        void SortUsers();

        #endregion
    }
}
