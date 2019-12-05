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
        /// <summary>
        /// Class used for testing PrintHelper & Exporter
        /// </summary>
        public class TestObject
        {
            public string TestProp1 { get; set; }
            public string TestProp2 { get; set; }
            
            public TestObject() : this(string.Empty, string.Empty) {}
            public TestObject(string prop1, string prop2)
            {
                TestProp1 = prop1;
                TestProp2 = prop2;
            }
        }
        
        private Exporter _exporter;
        private Mock<StreamWriter> _streamMock;
        private MemoryStream _memoryStream;
        [TestInitialize]
        public void InitiateExporter()
        {
            _memoryStream = new MemoryStream();
            _streamMock = new Mock<StreamWriter>(_memoryStream);
            _streamMock.Setup(p => p.Write(It.IsAny<string>())).Verifiable();
            _exporter = new Exporter(_streamMock.Object);
        }
        
        [TestCleanup]
        public void CleanUpExporter()
        {
            _memoryStream.Dispose();
        }
        
        [TestMethod]
        public void WriteToFile_GivenEmptyList_WritesToStreamOnlyOnce()
        {
            //Arrange
            List<Asset> testList = new List<Asset>();
            Type testType = new Asset().GetType();
            
            //Act
            _exporter.WriteToFile(testList, testType);
            
            //Assert
            _streamMock.Verify(p => p.WriteLine(It.IsAny<string>()), Times.Exactly(1));
        }
        
        [TestMethod]
        public void WriteToFile_GivenListWithAssets_WritesCorrectNumberOfLinesToStream()
        {
            //Arrange
            List<Asset> testList = new List<Asset>{new Asset{Name = "TestAsset1"}, new Asset{Name = "TestAsset2"}};
            Type testType = new Asset().GetType();
            //There should be one line for each item and one for the header
            int linesToWrite = testList.Count + 1;

            //Act
            _exporter.WriteToFile(testList, testType);
            
            //Assert
            _streamMock.Verify(p => p.WriteLine(It.IsAny<string>()), Times.Exactly(linesToWrite));
        }

        [TestMethod]
        public void WriteToFile_GivenEmptyListOfObjects_WritesCorrectHeader()
        {
            //Arrange
            List<TestObject> testList = new List<TestObject>();
            Type testType = new TestObject().GetType();

            string expectedHeader = "TestProp1,TestProp2";

            //Act
            _exporter.WriteToFile(testList, testType);
            
            //Assert
            _streamMock.Verify(p => p.WriteLine(It.Is<string>(param => param.Equals(expectedHeader))), Times.Once);
        }
        
        [TestMethod]
        public void WriteToFile_GivenListWithAssets_WritesCorrectLineForTestObject()
        {
            //Arrange
            List<TestObject> testList = new List<TestObject>{new TestObject{TestProp1 = "one", TestProp2 = "two"}};
            Type testType = new TestObject().GetType();

            string expectedLine = "one,two";

            //Act
            _exporter.WriteToFile(testList, testType);
            
            //Assert
            _streamMock.Verify(p => p.WriteLine(It.Is<string>(param => param.Equals(expectedLine))), Times.Once);
        }
    }
}