using System;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IUserRepository : IMysqlRepository<User>
    {
        User GetByUsername(string username);

        ulong GetCount();
    }
}