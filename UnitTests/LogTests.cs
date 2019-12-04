using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Controllers;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using AMS.Logging.Interfaces;
using AMS.Models;
using Asset_Management_System.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class LogTests
    {
        private Mock<ILogRepository> _logRepMock;
        private Ilogger _Log;
        private Dictionary<string, string> _testDict;
        private Dictionary<string, string> _prevValuesTestDict;

        [TestInitialize]
        public void InitiateTest()
        {
            // Create Mocks of dependencies
            _logRepMock = new Mock<ILogRepository>();
            
            // This creates a new instance of the class for each test
            _Log = new Logger(_logRepMock.Object);
        }
        
        [TestMethod]
        public void LogCreate_AddEntryReceivesComment_ReturnsTrue()
        {
            //Arrange
            _logRepMock.Setup(p => p.Insert(It.IsAny<LogEntry>())).Returns(true);
            Comment comment = new Comment();
            
            //Act
            bool result = _Log.AddEntry(comment, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesAsset_loggedItemIdIsSameAsAssetId()
        {
            //Arrange
            Asset asset = new Asset();
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.LoggedItemId == asset.ID))).Returns(true);

            //Act
            bool result = _Log.AddEntry(asset, 1);

            //Assert
            Assert.IsTrue(result);
        }
    }
}