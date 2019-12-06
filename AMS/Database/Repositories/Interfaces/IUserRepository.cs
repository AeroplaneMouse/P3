using System;
using System.Collections.Generic;
using AMS.Models;

namespace AMS.Database.Repositories.Interfaces
{
    public interface IUserRepository : IMysqlRepository<User>
    {
        User GetByIdentity(string identity);
        
        IEnumerable<User> GetAll(bool includeDisabled=false);

        IEnumerable<User> GetUsersForAsset(ulong id);

        ulong GetCount(bool? onlyEnabledUsers);
    }
}