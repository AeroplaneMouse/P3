using System;
using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IUserRepository : IMysqlRepository<User>
    {
        User GetByIdentity(string identity);
        
        IEnumerable<User> GetAll(bool includeDisabled=false);

        IEnumerable<User> GetUsersForAsset(ulong id);

        ulong GetCount();
    }
}