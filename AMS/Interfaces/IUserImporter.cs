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

        List<User> ImportUsersFromFile(string filePath);

        string GetUsersFile();

        List<User> ImportUsersFromDatabase();

        List<UserWithStatus> CombineLists(List<User> imported, List<User> existing);

        bool IsInList(List<User> list, User user);

        #endregion
    }
}
