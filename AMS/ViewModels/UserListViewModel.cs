using AMS.Controllers.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.ViewModels
{
    public class UserListViewModel : Base.BaseViewModel
    {
        #region Public Properties

        public string Title { get; set; }

        public List<UserWithStatus> UserList { get; set; }

        #endregion

        #region Private Properties

        private IUserListController _userController { get; set; }

        #endregion

        #region Constructor

        public UserListViewModel(IUserListController userController)
        {
            Title = "Users";

            _userController = userController;
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion
    }
}
