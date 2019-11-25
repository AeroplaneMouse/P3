using AMS.Models;
using System.Security.Principal;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;

namespace AMS.Authentication
{
    public class Session
    {
        public readonly User user;
        public string Username { get => GetIdentity().Split('\\')[1]; }
        public string Domain { get => GetIdentity().Split('\\')[0]; }

        public Session(IUserRepository repository) => user = repository.GetByIdentity(GetIdentity());

        public bool Authenticated() => user != null;
        public bool IsAdmin() => user.IsAdmin;
        public static string GetIdentity() => WindowsIdentity.GetCurrent().Name;
    }
}
