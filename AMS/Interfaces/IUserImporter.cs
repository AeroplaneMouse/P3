using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Interfaces
{
    public interface IUserImporter
    {
        List<User> ImportUsersFromFile();

        List<User> ImportUsersFromDatabase();

        List<UserWithStatus> CombineLists(List<User> imported, List<User> existing);

        bool IsInList(List<User> list, User user);
    }
}
