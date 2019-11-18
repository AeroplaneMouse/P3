using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.IO
{
    public class UserImporter : IUserImporter
    {
        #region Public Properties

        public IUserService UserService { get; set; }

        public IUserRepository UserRepository { get; set; }

        #endregion

        #region Constructor

        public UserImporter(IUserService service)
        {
            UserService = service;
        }

        #endregion

        #region Public Methods

        public List<UserWithStatus> CombineLists(List<User> imported, List<User> existing)
        {
            throw new NotImplementedException();
        }

        public List<User> ImportUsersFromDatabase()
        {
            throw new NotImplementedException();
        }

        public List<User> ImportUsersFromFile()
        {
            throw new NotImplementedException();
        }

        public bool IsInList(List<User> list, User user)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
