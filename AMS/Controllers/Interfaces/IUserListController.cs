﻿using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface IUserListController : ITagListController
    {
        #region Properties

        List<UserWithStatus> UsersList { get; set; }

        List<Department> DepartmentsList { get; set; }

        IUserImporter Importer { get; set; }

        bool IsShowingAdded { get; set; }

        bool IsShowingConflicting { get; set; }

        bool IsShowingDisabled { get; set; }

        bool IsShowingRemoved { get; set; }

        #endregion

        #region Methods

        void KeepUser(object user);

        void CancelChanges();

        void ApplyChanges();

        void SortUsers();

        #endregion

        

        
    }
}
