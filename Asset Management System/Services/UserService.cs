using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Services
{
    public class UserService : IUserService
    {

        private IUserRepository _rep;

        public UserService(IUserRepository rep)
        {
            _rep = rep;
        }

        public IRepository<User> GetRepository() => _rep;

        public string GetName(User user) => user.Name;
    }
}