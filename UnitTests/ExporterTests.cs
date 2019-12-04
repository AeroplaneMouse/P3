using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using AMS.Controllers;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class ExporterTests
    {
        private Exporter _exporter;
        private Mock<StreamWriter> _streamMock;
        private MemoryStream _memoryStream;
        [TestInitialize]
        public void InitiateExporter()
        {
            _memoryStream = new MemoryStream();
            _streamMock = new Mock<StreamWriter>(_memoryStream);
            _streamMock.Setup(p => p.Write(It.IsAny<string>()));
            _exporter = new Exporter(_streamMock.Object);
        }
        
        [TestCleanup]
        public void CleanUpExporter()
        {
            _memoryStream.Dispose();
        }
        
        [TestMethod]
        public void WriteToFile_GivenEmptyList_WritesToStream()
        {
            //Arrange
            List<Asset> testList = new List<Asset>();
            Type testType = new Asset().GetType();
            
            //Act
            _exporter.WriteToFile(testList, testType);
            
            //Assert
            _streamMock.Verify(p => p.WriteLine(It.IsAny<string>()), Times.Once);
        }
        
        [TestMethod]
        public void WriteToFile_GivenListWithAssets_WritesToStream()
        {
            //Arrange
            List<Asset> testList = new List<Asset>{new Asset{Name = "TestAsset1"}, new Asset{Name = "TestAsset2"}};
            Type testType = new Asset().GetType();
            
            //Act
            _exporter.WriteToFile(testList, testType);
            
            //Assert
            _streamMock.Verify(p => p.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
        }
        
    }
}