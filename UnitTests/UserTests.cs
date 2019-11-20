using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using AMS.Database.Repositories;
using AMS.Models;
using AMS.Database.Repositories.Interfaces;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class UserTests
    {
        private IUserRepository _userRepository { get; set; }

        private IDepartmentRepository _departmentRepository { get; set; }

        private IUserImporter _userImporter { get; set; }

        private IUserListController _userListController { get; set; }

        private Mock<IUserRepository> _userRepMock { get; set; }

        [TestInitialize]
        public void InitializeUserTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _userRepMock = new Mock<IUserRepository>();
            _userRepMock.Setup(p => p.GetAll(true)).Returns(new List<User>());

            _userRepository = new UserRepository();
            _departmentRepository = new DepartmentRepository();

            _userImporter = new UserImporter(_userRepository);
            _userListController = new UserListController(_userImporter, _userRepository, _departmentRepository);

            
        }

        #region Helpers

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
            List<UserWithStatus> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2);

            // Cleanup
            DeleteFileAt(filePath);
        }

        [TestMethod]
        public void IUserImporter_ImportUsersFromFile_FileDoesNotExist_ReturnEmptyList()
        {
            // Arrange

            // Act
            List<UserWithStatus> users = _userImporter.ImportUsersFromFile(string.Empty);

            // Assert
            Assert.IsTrue(users.Count() == 0);
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
            List<UserWithStatus> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2 && users.Where(p => p.Username.ToLower().Contains('å')).Count() == 1);

            DeleteFileAt(filePath);
        }

        //[TestMethod]
        //public void IUserImporter_ImportUsersFromFile_EncodingIsUTF8_UsersReturnedInListWithSpecialCharacters()
        //{
        //    // Arrange
        //    string filePath = "userFileTest.txt";

        //    CreateFileAt(filePath, new UTF8Encoding(false));

        //    // Act
        //    List<User> users = _userImporter.ImportUsersFromFile(filePath);

        //    // Assert
        //    Assert.IsTrue(users.Count() == 2 && users.Where(p => p.Username.ToLower().Contains('å')).Count() == 1);

        //    DeleteFileAt(filePath);
        //}

        #endregion

        #region UserListController

        [TestMethod]
        public void UserListController_KeepUser_UserIsInConflictAndFromFile_()
        {
            // Arrange
            UserWithStatus existingUser = new UserWithStatus(new User() { IsEnabled = false, Username = "Hans Hansen" });
            UserWithStatus importedUser = new UserWithStatus(new User() { IsEnabled = true, Username = "Hans Hansen" });

            // Set up test lists
            List<UserWithStatus> existing = new List<UserWithStatus>();
            existing.Add(existingUser);

            List<UserWithStatus> imported = new List<UserWithStatus>();
            imported.Add(importedUser);

            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);

            IUserListController controller = new UserListController(importerMock.Object, new UserRepository(), new DepartmentRepository());


            controller.GetExistingUsers();
            controller.GetUsersFromFile();

            //controller.UsersList = controller.Importer.CombineLists(imported, existing);
            controller.UsersList = new List<UserWithStatus>();

            int noget = 1;
            // Act
            controller.KeepUser(importedUser);

            // Assert
        }

        #endregion
    }
}
