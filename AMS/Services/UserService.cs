using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Services
{
    public class UserService : IUserService
    {
        public string GetName(User obj)
        {
            throw new NotImplementedException();
        }

        public IRepository<User> GetRepository()
        {
            throw new NotImplementedException();
        }
    }
}
