using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Services;
using AMS.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    [TestClass]
    class UserTests
    {
        private IUserService _userService { get; set; }

        private IUserImporter _userImporter { get; set; }

        private IUserListController _userListController { get; set; }

        [TestInitialize]
        void InitializeUserTest()
        {
            _userService = new UserService();
            _userImporter = new UserImporter(_userService);

            _userListController = new UserListController(_userImporter, _userService);


            // Make test file

        }

        [TestMethod]
        void Noget()
        {

        }

        #region UserImporter
        


        #endregion

        #region UserListController



        #endregion
    }
}
