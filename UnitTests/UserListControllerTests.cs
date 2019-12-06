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
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;

namespace UnitTests
{
    [TestClass]
    public class UserListControllerTests
    {
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
            List<UserWithStatus> existing = new List<UserWithStatus> {existingUser};

            List<UserWithStatus> imported = new List<UserWithStatus> {importedUser};

//            List<UserWithStatus> final = new List<UserWithStatus> {existingUser, importedUser};

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

        [TestMethod]
        public void UserListController_ApplyChanges_NotAllConflictsSolved_ReturnFalse()
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
            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};

            List<UserWithStatus> imported = new List<UserWithStatus>() {importedUser};

            List<UserWithStatus> final = new List<UserWithStatus>() {existingUser, importedUser};

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
            
            // Act
            bool result = controller.ApplyChanges();

            // Assert
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void UserListController_ApplyChanges_RemovingUsers_DisableUsersWithStatusRemoved()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };
            
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = "Removed" };

            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.IsShowingDisabled = true;
            
            // Act
            controller.ApplyChanges();
            
            // Assert
            Assert.IsTrue(controller.UserList[0].IsEnabled == false);
        }
        
        [TestMethod]
        public void UserListController_ApplyChanges_AddingUsers_UsersWithStatusAddedAddedToDatabase()
        {
            // Arrange
            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };
            
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = "Added" };

            List<UserWithStatus> imported = new List<UserWithStatus>() {importedUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(new List<UserWithStatus>());
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            controller.GetUsersFromFile();
            
            // Act
            controller.ApplyChanges();

            // Assert
            ulong id;
            
            userRepMock.Verify(p => p.Insert(It.IsAny<User>(), out id));
        }
        
        [TestMethod]
        public void UserListController_ApplyChanges_UpdatingUsers_UpdateCalled()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };
            
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = String.Empty };

            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            // Act
            controller.ApplyChanges();
            
            // Assert
            userRepMock.Verify(p => p.Update(It.IsAny<User>()), Times.Once);
        }

        [TestMethod]
        public void UserListController_CancelChanges_ImportedUserRemoved()
        {
            // Arrange
            object[] userConstructorParameters = new object[]
            {
                (ulong)10,
                "Hans Hansen",
                "Existing Hans PC",
                "Existing Hans",
                true,
                (ulong)1,
                true,
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };
            
            User existingHans = (User)Activator.CreateInstance(typeof(User), BindingFlags.NonPublic | BindingFlags.Instance, null, userConstructorParameters, null, null);

            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };
            
            User importedJens = new User()
            {
                IsEnabled = true,
                Username = "Jens Jensen",
                Description = "Imported Jens",
                DefaultDepartment = 0,
                Domain = "Imported Jens PC",
                IsAdmin = false
            };
            
            // Users in conflict
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = String.Empty };
            UserWithStatus importedUser1 = new UserWithStatus(importedHans) { Status = String.Empty };
            UserWithStatus importedUser2 = new UserWithStatus(importedJens) { Status = String.Empty };

            // Set up test lists
            List<UserWithStatus> existing = new List<UserWithStatus> {existingUser};

            List<UserWithStatus> imported = new List<UserWithStatus> {importedUser1, importedUser2};

//            List<UserWithStatus> final = new List<UserWithStatus> {existingUser, importedUser};

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

            // Act
            controller.CancelChanges();
            
            // Assert
            
            Assert.IsTrue(controller.UserList.SingleOrDefault(u => u.Description.Contains("Imported Jens")) == null);
        }

        [TestMethod]
        public void UserListController_ChangeStatusOfUser_UserIsNull_NoChangesMade()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };
            
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = String.Empty };

            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            // Act
            controller.ChangeStatusOfUser(null);
            
            // Assert
            Assert.IsTrue(controller.UserList[0].IsEnabled == true);
        }
        
        [TestMethod]
        public void UserListController_ChangeStatusOfUser_UserIsDisabled_UserSetToEnabled()
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
            
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = String.Empty };

            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.IsShowingDisabled = true;
            
            // Act
            controller.ChangeStatusOfUser(controller.UserList[0]);
            
            // Assert
            Assert.IsTrue(controller.UserList[0].IsEnabled == true);
        }
        
        [TestMethod]
        public void UserListController_ChangeStatusOfUser_UserIsDisabled_UserStatusSetToEmpty()
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
            
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = String.Empty };

            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            controller.IsShowingDisabled = true;
            
            // Act
            controller.ChangeStatusOfUser(controller.UserList[0]);
            
            // Assert
            Assert.IsTrue(controller.UserList[0].Status.CompareTo(String.Empty) == 0);
        }
        
        [TestMethod]
        public void UserListController_ChangeStatusOfUser_UserIsEnabled_UserSetToDisabled()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };
            
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = String.Empty };

            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            controller.IsShowingDisabled = true;
            
            // Act
            controller.ChangeStatusOfUser(controller.UserList[0]);
            
            // Assert
            Assert.IsTrue(controller.UserList[0].IsEnabled == false);
        }
        
        [TestMethod]
        public void UserListController_ChangeStatusOfUser_UserIsEnabled_UserStatusSetToDisabled()
        {
            // Arrange
            User existingHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Existing Hans",
                DefaultDepartment = 1,
                Domain = "Existing Hans PC",
                IsAdmin = true
            };
            
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = String.Empty };

            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            controller.IsShowingDisabled = true;
            
            // Act
            controller.ChangeStatusOfUser(controller.UserList[0]);
            
            // Assert
            Assert.IsTrue(controller.UserList[0].Status.CompareTo("Disabled") == 0);
        }

        [TestMethod]
        public void UserListController_GetExistingUsers_DisabledUsersStatusSetToDisabled()
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
            
            UserWithStatus existingUser = new UserWithStatus(existingHans) { Status = String.Empty };

            List<UserWithStatus> existing = new List<UserWithStatus>() {existingUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(existing);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

            controller.IsShowingDisabled = true;
            
            // Act
            controller.GetExistingUsers();
            
            // Assert
            Assert.IsTrue(controller.UserList[0].Status.CompareTo("Disabled") == 0);
        }

        [TestMethod]
        public void UserListController_GetUsersFromFile_UserImporterPromptsForFilePath()
        {
            // Arrange
            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };
            
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = String.Empty };
            
            List<UserWithStatus> imported = new List<UserWithStatus> {importedUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(new List<UserWithStatus>());
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            // Act
            controller.GetUsersFromFile();
            
            // Assert
            importerMock.Verify(p => p.GetUsersFilePath());
        }
        
        [TestMethod]
        public void UserListController_GetUsersFromFile_UserImporterImportsUsers()
        {
            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };
            
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = String.Empty };
            
            List<UserWithStatus> imported = new List<UserWithStatus> {importedUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(new List<UserWithStatus>());
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            // Act
            controller.GetUsersFromFile();
            
            // Assert
            importerMock.Verify(p => p.ImportUsersFromFile(It.IsAny<string>()));
        }
        
        [TestMethod]
        public void UserListController_GetUsersFromFile_UserImporterImportsUsers_UserListHasImportedUsers()
        {
            // Arrange
            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };
            
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = String.Empty };
            
            List<UserWithStatus> imported = new List<UserWithStatus> {importedUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(new List<UserWithStatus>());
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            // Act
            controller.GetUsersFromFile();
            
            // Assert
            Assert.IsTrue(controller.UserList.Contains(importedUser));
        }

        [TestMethod]
        public void UserListController_GetUsersFromFile_UserStatusUpdated()
        {
            // Arrange
            User importedHans = new User()
            {
                IsEnabled = true,
                Username = "Hans Hansen",
                Description = "Imported Hans",
                DefaultDepartment = 0,
                Domain = "Imported Hans PC",
                IsAdmin = false
            };
            
            UserWithStatus importedUser = new UserWithStatus(importedHans) { Status = String.Empty };
            
            List<UserWithStatus> imported = new List<UserWithStatus> {importedUser};
            
            // Set up mocks
            Mock<IUserImporter> importerMock = new Mock<IUserImporter>();

            importerMock.Setup(p => p.ImportUsersFromDatabase()).Returns(new List<UserWithStatus>());
            importerMock.Setup(p => p.ImportUsersFromFile(It.IsAny<string>())).Returns(imported);
            importerMock.Setup(p => p.GetUsersFilePath()).Returns("TestPath");
            
            Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
            
            Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

            IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);
            
            // Act
            controller.GetUsersFromFile();
            
            // Assert
            Assert.IsTrue(controller.UserList[0].Status.CompareTo("Added") == 0);
        }
    }
}
