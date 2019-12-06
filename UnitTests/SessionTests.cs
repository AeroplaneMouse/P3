using System;
using AMS.Authentication;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class SessionTests
    {
        private Session _session;
        private Mock<IUserRepository> _userRepMock;
        private User _testUser;
        private ulong _id;
        
        [TestInitialize]
        public void InitiateAssets()
        {
            _testUser = new User{Username = "TestUser", IsEnabled = true};
            
            _userRepMock = new Mock<IUserRepository>();
            _userRepMock.Setup(p => p.Insert(It.IsAny<User>(), out _id)).Returns(_testUser);
            
        }
        
        [TestMethod]
        public void Authenticated_AuthenticatedUser_ReturnsTrue()
        {
            //Arrange
            _userRepMock.Setup(p => p.GetByIdentity(It.IsAny<string>())).Returns(_testUser);
            _session = new Session(_userRepMock.Object);
            //Act
            bool result = _session.Authenticated();

            //Assert
            Assert.IsTrue(result);
        }
        /*
        [TestMethod]
        public void Authenticated_UserIsNull_ReturnsTrue()
        {
            //Arrange
            _userRepMock.Setup(p => p.GetByIdentity(It.IsAny<string>())).Returns((User)null);
            _userRepMock.Setup(p => p.GetCount(It.IsAny<Nullable<bool>>())).Returns(0);
            _session = new Session(_userRepMock.Object);
            //Act
            bool result = _session.Authenticated();

            //Assert
            Assert.IsTrue(result);
        }
        /**/
    }
}