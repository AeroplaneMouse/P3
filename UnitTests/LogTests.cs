using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Controllers;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using AMS.Logging.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class LogTests
    {
        private Mock<ILogRepository> _logRepMock;
        private Mock<ILoggableValues> _newItemMock;
        private Mock<ILoggableValues> _prevItemMock;
        private Ilogger _Log;
        private Dictionary<string, string> _testDict;
        private Dictionary<string, string> _prevValuesTestDict;

        [TestInitialize]
        public void InitiateAssets()
        {
            // Create Mocks of dependencies
            _logRepMock = new Mock<ILogRepository>();
            _newItemMock = new Mock<ILoggableValues>();
            _prevItemMock = new Mock<ILoggableValues>();
            
            _testDict = new Dictionary<string, string> {{"ID", "1"}, {"Name", "NewValue"}};
            _prevValuesTestDict = new Dictionary<string, string>{{"ID", "1"}, {"Name", "PrevValue"}};
            
            // This creates a new instance of the class for each test
            _Log = new Log(_logRepMock.Object);
        }
        
        [TestMethod]
        public void LogCreate_CallsRepositoryInsert_ReturnsTrue()
        {
            //Arrange
            _logRepMock.Setup(p => p.Insert(It.IsAny<Entry>())).Returns(true);
            _newItemMock.Setup(p => p.GetLoggableValues()).Returns(_testDict);
            
            //Act
            _Log.LogCreate(_newItemMock.Object);

            //Assert
            _logRepMock.Verify((p => p.Insert(It.IsAny<Entry>())), Times.Once);
        }
        
        [TestMethod]
        public void LogDelete_CallsRepositoryInsert_ReturnsTrue()
        {
            //Arrange
            _logRepMock.Setup(p => p.Insert(It.IsAny<Entry>())).Returns(true);
            _newItemMock.Setup(p => p.GetLoggableValues()).Returns(_testDict);
            
            //Act
            _Log.LogDelete(_newItemMock.Object);

            //Assert
            _logRepMock.Verify((p => p.Insert(It.IsAny<Entry>())), Times.Once);
        }
        
        [TestMethod]
        public void LogUpdate_CallsRepositoryInsert_ReturnsTrue()
        {
            //Arrange
            _logRepMock.Setup(p => p.Insert(It.IsAny<Entry>())).Returns(true);
            _newItemMock.Setup(p => p.GetLoggableValues()).Returns(_testDict);
            _prevItemMock.Setup(p => p.GetLoggableValues()).Returns(_prevValuesTestDict);
            
            //TODO: Find out if it is possible to avoid using this method
            _Log.SavePreviousValues(_prevItemMock.Object);
            
            //Act
            _Log.LogUpdate(_newItemMock.Object);

            //Assert
            _logRepMock.Verify((p => p.Insert(It.IsAny<Entry>())), Times.Once);
        }
        
        [TestMethod]
        public void LogUpdate_DoesNotCallRepositoryInsertWhenPrevValuesAreNull_ReturnsTrue()
        {
            //Arrange
            _logRepMock.Setup(p => p.Insert(It.IsAny<Entry>())).Returns(true);
            _newItemMock.Setup(p => p.GetLoggableValues()).Returns(_testDict);

            //Act
            _Log.LogUpdate(_newItemMock.Object);

            //Assert
            _logRepMock.Verify((p => p.Insert(It.IsAny<Entry>())), Times.Never);
        }
        
        [TestMethod]
        public void LogUpdate_PrevValuesAreNull_ReturnsFalse()
        {
            //Arrange
            _logRepMock.Setup(p => p.Insert(It.IsAny<Entry>())).Returns(true);
            _newItemMock.Setup(p => p.GetLoggableValues()).Returns(_testDict);

            //Act
            bool result = _Log.LogUpdate(_newItemMock.Object);

            //Assert
            Assert.IsFalse(result);
        }
    }
}