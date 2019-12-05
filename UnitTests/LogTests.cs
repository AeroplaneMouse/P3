using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using AMS.Controllers;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using AMS.Logging.Interfaces;
using AMS.Models;
using Asset_Management_System.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

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

        [TestMethod]
        public void LogCreate_AddEntryReceivesAsset_loggedItemTypeIsAsset()
        {
            //Arrange
            Asset entity = new Asset();
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.LoggedItemType == entity.GetType()))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesTag_loggedItemTypeIsTag()
        {
            //Arrange
            Tag entity = new Tag()
            {
                Name = "Tag name",
                Color = "#d3d3d3",
                DepartmentID = 1,
                ParentID = 0,
                NumOfChildren = 10
            };
            
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.LoggedItemType == entity.GetType()))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesDepartment_loggedItemTypeIsDepartment()
        {
            //Arrange
            Department entity = new Department()
            {
                Name = "Tag name"
            };
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.LoggedItemType == entity.GetType()))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesComment_loggedItemTypeIsComment()
        {
            //Arrange
            Comment entity = new Comment();
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.LoggedItemType == entity.GetType()))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesNewAsset_EntryTypeIsCreate()
        {
            //Arrange
            Asset entity = new Asset()
            {
                Name = "Asset name",
                Description = "Description",
                DepartmentID = 1,
                SerializedFields = "[]",
                Identifier = "Asset identifier"
            };

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.EntryType == "Create"))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesExistingAssetANdNewID_EntryTypeIsCreate()
        {
            //Arrange
            object[] assetConstructorParameters = new object[]
            {
                (ulong)10,
                "Asset name",
                "Asset description",
                "Asset identifier",
                (ulong)1,
                "[]",
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };

            ulong newId = 201;

            Asset entity = (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.NonPublic | BindingFlags.Instance, null, assetConstructorParameters, null, null);

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.EntryType == "Create"))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1, newId);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesAlteredAsset_EntryTypeIsUpdate()
        {
            //Arrange
            Asset entity = new Asset()
            {
                Name = "Asset name",
                Description = "Description",
                DepartmentID = 1,
                SerializedFields = "[]",
                Identifier = "Asset identifier"
            };
            entity.Name = "New asset name";

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.EntryType == "Update"))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesCleanAssetWithId10_EntryTypeIsDelete()
        {
            //Arrange
            object[] assetConstructorParameters = new object[]
            {
                (ulong)10,
                "Asset name",
                "Asset description",
                "Asset identifier",
                (ulong)1,
                "[]",
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };

            Asset entity = (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.NonPublic | BindingFlags.Instance, null, assetConstructorParameters, null, null);

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.EntryType == "Delete"))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesAlteredAsset_ChangesCorrespondsWithTheChangesMade()
        {
            //Arrange
            object[] assetConstructorParameters = new object[]
            {
                (ulong)10,
                "Asset name",
                "Asset description",
                "Asset identifier",
                (ulong)1,
                "[]",
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };

            Asset entity = (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.NonPublic | BindingFlags.Instance, null, assetConstructorParameters, null, null);
            entity.Name = "New asset name";
            entity.Description = "New asset description";

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => 
                le.Changes.Contains("New asset name")
                && le.Changes.Contains("New asset description")
                && !le.Changes.Contains("Asset identifier")
                && !le.Changes.Contains("DepartmentID"))))
                .Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesNewAsset_ChangesContainsAllValues()
        {
            //Arrange
            Asset entity = new Asset()
            {
                Name = "Asset name",
                Description = "Description",
                DepartmentID = 1,
                SerializedFields = "[]",
                Identifier = "Asset identifier"
            };

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                keyValuePairs.Add(property.Name, property.GetValue(entity)?.ToString());
            }

            string expectedChanges = JsonConvert.SerializeObject(keyValuePairs, Formatting.Indented);

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.EntryType == "Create" && le.Changes == expectedChanges))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1, 10);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesDeletedAsset_ChangesContainsAllValues()
        {
            //Arrange
            object[] assetConstructorParameters = new object[]
            {
                (ulong)10,
                "Asset name",
                "Asset description",
                "Asset identifier",
                (ulong)1,
                "[]",
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };

            Asset entity = (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.NonPublic | BindingFlags.Instance, null, assetConstructorParameters, null, null);

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                keyValuePairs.Add(property.Name, property.GetValue(entity)?.ToString());
            }

            string expectedChanges = JsonConvert.SerializeObject(keyValuePairs, Formatting.Indented);

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.EntryType == "Delete" && le.Changes == expectedChanges))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LogCreate_AddEntryReceivesException_DescriptionContainsErrorMessageAndStackTrace()
        {
            //Arrange
            Exception e = new Exception("Exception message!!!");
            string messageAndStackTrace = "\nError message: " + e.Message + "\nStack trace:" + e.StackTrace;
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.Description.EndsWith(messageAndStackTrace)))).Returns(true);

            //Act
            bool result = _Log.AddEntry("Exception", "Start of description", 1, "[]", e);

            //Assert
            Assert.IsTrue(result);
        }
    }
}