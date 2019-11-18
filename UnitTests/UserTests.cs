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
using System.Drawing;
using System.IO;
using System.Linq;
using AMS.Database.Repositories;
using AMS.Models;

namespace UnitTests
{
    [TestClass]
    class UserTests
    {
        private IUserService _userService { get; set; }

        private IUserImporter _userImporter { get; set; }

        private IUserController _userListController { get; set; }

        #region Helpers

        [TestInitialize]
        public void InitializeUserTest()
        {
            _userService = new UserService();
            _userImporter = new UserImporter(_userService);

            _userListController = new UserListController(_userImporter, _userService);


            

        }

        [TestCleanup]
        public void CleanUpUserTest()
        {

        }

        void CreateFileAt(string filePath)
        {
            // Make test file
            Encoding encoding = Encoding.GetEncoding(1252);

            string fileContent = "Name\tType\tDescription\r\n" +
                                 "Hans Hansen\tUser\tHan er bare for god\r\n" +
                                 "Åge Ågesen\tUser\tÅge øser æsler";

            byte[] contentArray = encoding.GetBytes(fileContent);

            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.Write(contentArray);
            }
        }

        void DestroyFileAt(string filePath)
        {

        }

        #endregion




        [TestMethod]
        public void Method_ContextSituation_ExpectedReturn()
        {
            // Arrange

            // Act

            // Assert
        }

        #region UserImporter

        [TestMethod]
        public void IUserImporter_ImportUsersFromFile_FileIsFormatted()
        {
            // Arrange
            string filePath = "userFileTest";
            CreateFileAt(filePath);

            // Act
            List<User> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2);

            DestroyFileAt(filePath);
        }

        #endregion

        #region UserListController



        #endregion
    }
}
