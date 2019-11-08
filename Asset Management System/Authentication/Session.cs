using System;
using System.Security.Principal;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.Authentication
{
    public class Session
    {
        public readonly User user;
        
        public string Username { get => GetIdentity().Split('\\')[1]; }
        public static string Domain { get => GetIdentity().Split('\\')[0]; }

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
