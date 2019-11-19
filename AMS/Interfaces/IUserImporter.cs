using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Interfaces
{
    public interface IUserImporter
    {
        #region Methods

        List<UserWithStatus> ImportUsersFromFile(string filePath);

        string GetUsersFile();

        List<UserWithStatus> ImportUsersFromDatabase();

        List<UserWithStatus> CombineLists(List<UserWithStatus> imported, List<UserWithStatus> existing);

        bool UserIsInList(List<UserWithStatus> list, User user);

        #endregion
    }
}
