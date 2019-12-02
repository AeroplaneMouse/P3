using AMS.Models;
using System.Security.Principal;
using AMS.ConfigurationHandler;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.ViewModels;
using System;

namespace AMS.Authentication
{
    public class Session
    {
        public readonly User user;
        public string Username { get => GetIdentity().Split('\\')[1]; }
        public string Domain { get => GetIdentity().Split('\\')[0]; }

        private static string _dbKey = "";

        public Session(IUserRepository repository) => user = repository.GetByIdentity(GetIdentity());

        public bool Authenticated() => user != null;
        
        public bool IsAdmin() => user.IsAdmin;
        
        public static string GetIdentity() => WindowsIdentity.GetCurrent().Name;
        
        public static string GetDBKey()
        {
            if (string.IsNullOrEmpty(_dbKey))
            {
                FileConfigurationHandler fileConfigurationHandler = new FileConfigurationHandler(Features.GetCurrentSession());
                _dbKey = fileConfigurationHandler.GetConfigValue();
            }
            return _dbKey;
        }

        public static void ClearDBKey()
        {
            _dbKey = String.Empty;
        }
    }
}
