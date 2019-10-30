using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IUserRepository : IMysqlRepository<User>
    {
        User GetByUsername(string username);

        Int32 GetCount();
    }
}