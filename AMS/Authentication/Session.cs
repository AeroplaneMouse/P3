using System;
using System.Security.Principal;
using AMS.Models;
using AMS.Database.Repositories;
using AMS.Services.Interfaces;
using AMS.Database.Repositories.Interfaces;

namespace AMS.Authentication
{
    public class Session
    {
        public readonly User user;
        
        public string Username { get => GetIdentity().Split('\\')[1]; }
        public string Domain { get => GetIdentity().Split('\\')[0]; }

        public Session(IUserService service)
        {
            IUserRepository rep = (IUserRepository)service.GetRepository();
            user = rep.GetByIdentity(GetIdentity());
        }

        public bool Authenticated()
        {
            return user != null;
        }

        public bool IsAdmin()
        {
            return user.IsAdmin;
        }
        
        public static string GetIdentity()
        {
            return WindowsIdentity.GetCurrent().Name;
        }
    }
}
