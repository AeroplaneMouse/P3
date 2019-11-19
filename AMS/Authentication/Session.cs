using AMS.Models;
using System.Security.Principal;
using AMS.Database.Repositories;

namespace AMS.Authentication
{
    public class Session
    {
        public readonly User user;
        public string Username { get => GetIdentity().Split('\\')[1]; }
        public string Domain { get => GetIdentity().Split('\\')[0]; }

        public Session(IUserRepository repository)
        {
            user = repository.GetByIdentity(GetIdentity());
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
