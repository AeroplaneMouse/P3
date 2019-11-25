using AMS.Database.Repositories.Interfaces;
using AMS.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Interfaces
{
    public interface IUserImporter
    {
        #region Methods

        string GetUsersFile();
        List<UserWithStatus> ImportUsersFromFile(string filePath);
        List<UserWithStatus> ImportUsersFromDatabase();

        #endregion
    }
}
