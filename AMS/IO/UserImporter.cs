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
            return new List<UserWithStatus>();
        }

        public List<User> ImportUsersFromDatabase()
        {
            return new List<User>();
        }

        public List<User> ImportUsersFromFile(string filePath)
        {
            return new List<User>();
        }

        public bool IsInList(List<User> list, User user)
        {
            return true;
        }

       
        #endregion
    }
}
