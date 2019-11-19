using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AMS.Controllers
{
    public class UserListController : IUserListController
    {
        #region Public Properties

        public List<UserWithStatus> UsersList { get; set; }

        public IUserImporter Importer { get; set; }

        public IUserService UserService { get; set; }

        public IUserRepository UserRepository { get; set; }
        public List<Tag> TagsList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        

        #endregion

        #region Constructor

        public UserListController(IUserImporter importer, IUserService service)
        {
            Importer = importer;
            UserService = service;

            UserRepository = service.GetRepository() as UserRepository;
        }

        #endregion

        #region Public Methods

        public void ApplyChanges()
        {
            throw new NotImplementedException();
        }

        public void CancelChanges()
        {
            throw new NotImplementedException();
        }

        public void KeepUser(User user)
        {
            throw new NotImplementedException();
        }

        public void SortUsers()
        {
            throw new NotImplementedException();
        }

        public void Search()
        {
            throw new NotImplementedException();
        }
        
        public void Search(string query)
        {
            throw new NotImplementedException();
        }

        public void AddNew()
        {
            throw new NotImplementedException();
        }

        public void Edit(Tag tag)
        {
            throw new NotImplementedException();
        }

        public void ViewTag(Tag tag)
        {
            throw new NotImplementedException();
        }

        public void Remove(Tag tag)
        {
            throw new NotImplementedException();
        }

        public void Export(List<Tag> tags)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
