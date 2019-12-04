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
        public void UserListController_KeepUser_KeptUserIsFromFile_KeptUserSetToAddedOtherUserIsDisabled()
        {
            try
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
                importerMock.Setup(p => p.GetUsersFilePath()).Returns("test");
                
                Mock<IDepartmentRepository> departmentRepMock = new Mock<IDepartmentRepository>();
                
                Mock<IUserRepository> userRepMock = new Mock<IUserRepository>();

                IUserListController controller = new UserListController(importerMock.Object, userRepMock.Object, departmentRepMock.Object);

                controller.GetExistingUsers();
                controller.GetUsersFromFile();

                // Forskellige:
                // Status color
                // Status

                // CombineLists er mocked, den bliver derfor ikke kaldt. Existing bliver sat til disabled ved GetExistingUsers, så de har forskellige statusser. Øv øv

                // Act
                controller.KeepUser(existingUser);

                // Assert
            }

            catch (Exception e)
            {

            }


        }
    }
}
