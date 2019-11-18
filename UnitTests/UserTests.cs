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
using AMS.Authentication;
using System.Security.Permissions;

namespace UnitTests
{
    [TestClass]
    public class UserTests
    {
        private IUserService _userService { get; set; }

        private IUserImporter _userImporter { get; set; }

        private IUserListController _userListController { get; set; }

        #region Helpers

        [TestInitialize]
        public void InitializeUserTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _userService = new UserService();
            _userImporter = new UserImporter(_userService);
            _userListController = new UserListController(_userImporter, _userService);
        }

        [TestCleanup]
        public void CleanUpUserTest()
        {

        }

        // Make test file
        void CreateFileAt(string filePath, Encoding encoding)
        {
            string fileContent = "Name\tType\tDescription\r\n" +
                                 "Hans Hansen\tUser\tHan er bare for god\r\n" +
                                 "Åge Ågesen\tUser\tÅge øser æsler\r\n";

            byte[] contentArray = encoding.GetBytes(fileContent);

            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.Write(contentArray);
            }
        }

        // Delete test file
        void DeleteFileAt(string filePath)
        {
            File.Delete(filePath);
        }

        #endregion

        [TestMethod]
        public void Interface_Method_ContextSituation_ExpectedReturn()
        {
            // Arrange

            // Act

            // Assert
        }

        #region UserImporter

        [TestMethod]
        public void IUserImporter_ImportUsersFromFile_FileIsFormatted_UsersReturnedInList()
        {
            // Arrange
            string filePath = "userFileTest.txt";
            CreateFileAt(filePath, Encoding.GetEncoding(1252));

            // Act
            List<User> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2);

            // Cleanup
            DeleteFileAt(filePath);
        }

        [TestMethod]
        public void IUserImporter_ImportUsersFromFile_FileDoesNotExist_ReturnNull()
        {
            // Arrange

            // Act
            List<User> users = _userImporter.ImportUsersFromFile(string.Empty);

            // Assert
            Assert.IsTrue(users == null);
        }

        [TestMethod]
        public void IUserImporter_ImportUsersFromFile_FileNotFormattedCorrectly_FileNotFormattedCorrectlyExceptionThrown()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void IUserImporter_ImportUsersFromFile_EncodingIs1252_UsersReturnedInListWithSpecialCharacters()
        {
            // Arrange
            string filePath = "userFileTest.txt";
            CreateFileAt(filePath, Encoding.GetEncoding(1252));

            // Act
            List<User> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2 && users.Where(p => p.Username.ToLower().Contains('å')).Count() == 1);

            //DeleteFileAt(filePath);
        }

        [TestMethod]
        public void IUserImporter_ImportUsersFromFile_EncodingIsUTF8_UsersReturnedInListWithSpecialCharacters()
        {
            // Arrange
            string filePath = "userFileTest.txt";
            CreateFileAt(filePath, Encoding.UTF8);

            // Act
            List<User> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2 && users.Where(p => p.Username.ToLower().Contains('å')).Count() == 1);

            //DeleteFileAt(filePath);
        }

        #endregion

        #region UserListController



        #endregion
    }
}
