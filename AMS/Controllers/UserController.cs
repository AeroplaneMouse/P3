using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers
{
    public class UserController : IUserController
    {
        public List<UserWithStatus> UsersList { get; set; }

        public IUserImporter Importer { get; set; }
        
        public IUserService UserService { get; set; }

        public UserController(IUserImporter importer, IUserService service)
        {
            Importer = importer;
            UserService = service;
        }

        public void ApplyChanges()
        {
            throw new NotImplementedException();
        }

        public void CancelChanges()
        {
            throw new NotImplementedException();
        }

        public void KeepUser()
        {
            throw new NotImplementedException();
        }

        public void SortUsers()
        {
            throw new NotImplementedException();
        }
    }
}
