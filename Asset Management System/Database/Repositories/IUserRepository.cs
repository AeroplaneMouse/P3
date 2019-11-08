using System;
using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    public interface IUserRepository : IMysqlRepository<User>
    {
        User GetByUsername(string username);

        ulong GetCount();

        IEnumerable<User> GetAll();
    }
}