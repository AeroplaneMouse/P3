using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class UserListControllerTests
    {
        [TestMethod]
        public void Interface_Method_ContextSituation_ExpectedReturn()
        {
            // Arrange

            // Act

            // Assert
        }

//        private Mock<IUserRepository> _userRepMock { get; set; }
//        
//        private Mock<IDepartmentRepository> _departmentRepMock { get; set; }

        [TestInitialize]
        public void InitializeUserTest()
        {
//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//
//            _userRepMock = new Mock<IUserRepository>();
//            _userRepMock.Setup(p => p.GetAll(true)).Returns(new List<User>());
//            
//            _departmentRepMock = new Mock<IDepartmentRepository>();
//
//            _userRepository = new UserRepository();
//            _departmentRepository = new DepartmentRepository();
//
//            _userImporter = new UserImporter(_userRepository);
//            _userListController = new UserListController(_userImporter, _userRepository, _departmentRepository);
        }

        [TestMethod]
        public void UserListController_KeepUser_KeptUserImported_KeptUserStatusSetToAdded()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = false,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };

            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };

            // Users in conflict
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = "Conflicting" };
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = "Conflicting" };

            // Set up test lists
            List<UserWithStatus> existing = new List<UserWithStatus>();
            existing.Add(existingUser);

            List<UserWithStatus> imported = new List<UserWithStatus>();
            imported.Add(importedUser);

            List<UserWithStatus> final = new List<UserWithStatus>();
            final.Add(existingUser);
            final.Add(importedUser);

            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.GetExistingUsers();
            controller.GetUsersFromFile();

            // Save a reference to the user, before changes are made
            var importedBefore = controller.UserList[1];
            
            // Act
            controller.KeepUser(importedBefore);

            // Assert
            Assert.IsTrue(importedBefore.Status.CompareTo("Added") == 0);
        }

        [TestMethod]
        public void UserListController_KeepUser_KeptUserIsFromFile_OtherUserSetToDisabled()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = false,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };

            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };

            // Users in conflict
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = "Conflicting" };
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = "Conflicting" };

            // Set up test lists
            List<UserWithStatus> existing = new List<UserWithStatus>();
            existing.Add(existingUser);

            List<UserWithStatus> imported = new List<UserWithStatus>();
            imported.Add(importedUser);

            List<UserWithStatus> final = new List<UserWithStatus>();
            final.Add(existingUser);
            final.Add(importedUser);

            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.GetExistingUsers();
            controller.GetUsersFromFile();

            // Save a reference to the user, before changes are made
            var existingBefore = controller.UserList[0];
            var importedBefore = controller.UserList[1];
            
            // Act
            controller.KeepUser(importedBefore);

            // Assert
            Assert.IsTrue(existingBefore.IsEnabled == false);
        }
        
        [TestMethod]
        public void UserListController_KeepUser_KeptUserIsFromFile_OtherUserUsernameAddedDate()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = false,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };

            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };

            // Users in conflict
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = "Conflicting" };
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = "Conflicting" };

            // Set up test lists
            List<UserWithStatus> existing = new List<UserWithStatus>();
            existing.Add(existingUser);

            List<UserWithStatus> imported = new List<UserWithStatus>();
            imported.Add(importedUser);

            List<UserWithStatus> final = new List<UserWithStatus>();
            final.Add(existingUser);
            final.Add(importedUser);

            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.GetExistingUsers();
            controller.GetUsersFromFile();

            // Save a reference to the user, before changes are made
            var existingBefore = controller.UserList[0];
            var importedBefore = controller.UserList[1];
            
            // Act
            controller.KeepUser(importedBefore);

            // Assert
            Assert.IsTrue(existingBefore.Username
                              .Trim(')')
                              .Split('(')
                              .ElementAt(1)
                              .CompareTo(DateTime.Now.ToLocalTime().ToString()) == 0);
        }
        
        [TestMethod]
        public void UserListController_KeepUser_KeptUserIsExisting_KeptUserStatusSetToEmpty()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = false,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };

            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };

            // Users in conflict
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = "Conflicting" };
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = "Conflicting" };

            // Set up test lists
            List<UserWithStatus> existing = new List<UserWithStatus>();
            existing.Add(existingUser);

            List<UserWithStatus> imported = new List<UserWithStatus>();
            imported.Add(importedUser);

            List<UserWithStatus> final = new List<UserWithStatus>();
            final.Add(existingUser);
            final.Add(importedUser);

            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.GetExistingUsers();
            controller.GetUsersFromFile();

            // Save a reference to the user, before changes are made
            var existingBefore = controller.UserList[0];
            var importedBefore = controller.UserList[1];
            
            // Act
            controller.KeepUser(existingBefore);

            // Assert
            Assert.IsTrue(existingBefore.Status.CompareTo(String.Empty) == 0);
        }
        
        [TestMethod]
        public void UserListController_KeepUser_KeptUserIsExisting_KeptUserSetToEnabled()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = false,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };

            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };

            // Users in conflict
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = "Conflicting" };
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = "Conflicting" };

            // Set up test lists
            List<UserWithStatus> existing = new List<UserWithStatus>();
            existing.Add(existingUser);

            List<UserWithStatus> imported = new List<UserWithStatus>();
            imported.Add(importedUser);

            List<UserWithStatus> final = new List<UserWithStatus>();
            final.Add(existingUser);
            final.Add(importedUser);

            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.GetExistingUsers();
            controller.GetUsersFromFile();

            // Save a reference to the user, before changes are made
            var existingBefore = controller.UserList[0];
            var importedBefore = controller.UserList[1];
            
            // Act
            controller.KeepUser(existingBefore);

            // Assert
            Assert.IsTrue(existingBefore.IsEnabled == true);
        }
        
        [TestMethod]
        public void UserListController_KeepUser_KeptUserIsExisting_OtherUserRemovedFromList()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = false,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };

            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };

            // Users in conflict
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = "Conflicting" };
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = "Conflicting" };

            // Set up test lists
            List<UserWithStatus> existing = new List<UserWithStatus>();
            existing.Add(existingUser);

            List<UserWithStatus> imported = new List<UserWithStatus>();
            imported.Add(importedUser);

            List<UserWithStatus> final = new List<UserWithStatus>();
            final.Add(existingUser);
            final.Add(importedUser);

            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.GetExistingUsers();
            controller.GetUsersFromFile();

            // Save a reference to the user, before changes are made
            var existingBefore = controller.UserList[0];
            var importedBefore = controller.UserList[1];
            
            // Act
            controller.KeepUser(existingBefore);
            
            // Assert
            Assert.IsFalse(controller.UserList.Contains(importedBefore));
        }
    }
}
