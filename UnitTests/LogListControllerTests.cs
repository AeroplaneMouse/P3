using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class LogListControllerTests
    {
        private Mock<ILogRepository> _logRepMock;
        private Mock<IExporter> _exporterMock;
        private ILogListController _logListController;
        
      
        [TestInitialize]
        public void InitiateAssets()
        {
            // Create Mocks of dependencies
            _logRepMock = new Mock<ILogRepository>();
            _exporterMock = new Mock<IExporter>();
            
            // This creates a new instance of the class for each test
            _logListController = new LogListController(_logRepMock.Object, _exporterMock.Object);
        }
        
        [TestMethod]
        public void Search_CallsRepositorySearch_ReturnsTrue()
        {
            //Arrange
            _logRepMock.Setup(p => p.Search(It.IsAny<string>(), null, null, false)).Returns(new ObservableCollection<LogEntry>());

            //Act
            _logListController.Search("");

            //Assert
            _logRepMock.Verify((p => p.Search(It.IsAny<string>(), null, null, false)), Times.AtLeastOnce);
        }
        
        [TestMethod]
        public void Export_CallsExporterPrintMethod_ReturnsTrue()
        {
            //Arrange
            _exporterMock.Setup(p => p.Print(It.IsAny<List<LogEntry>>()));

            //Act
            _logListController.Export(new List<LogEntry>());

            //Assert
            _exporterMock.Verify((p => p.Print(It.IsAny<List<LogEntry>>())), Times.Once);
        }
        
    }
}