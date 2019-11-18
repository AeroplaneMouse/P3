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
        #region Properties

        IUserService UserService { get; set; }

        IUserRepository UserRepository { get; set; }

        #endregion

        #region Methods

        List<User> ImportUsersFromFile(string filePath);

        List<User> ImportUsersFromDatabase();

        List<UserWithStatus> CombineLists(List<User> imported, List<User> existing);

        bool IsInList(List<User> list, User user);

        #endregion
    }
}
