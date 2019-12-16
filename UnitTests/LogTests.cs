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
        public void AddEntry_ReceivesComment_ReturnsTrue()
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
        public void AddEntry_ReceivesAsset_loggedItemIdIsSameAsAssetId()
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
        public void AddEntry_ReceivesAsset_loggedItemTypeIsAsset()
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
        public void AddEntry_ReceivesTag_loggedItemTypeIsTag()
        {
            //Arrange
            Tag entity = new Tag()
            {
                Name = "tag name",
                Color = "#d3d3d3",
                DepartmentID = 1,
                ParentId = 0,
                NumberOfChildren = 10
            };
            
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.LoggedItemType == entity.GetType()))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddEntry_ReceivesDepartment_loggedItemTypeIsDepartment()
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
        public void AddEntry_ReceivesComment_loggedItemTypeIsComment()
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
        public void AddEntry_ReceivesNewAsset_EntryTypeIsCreate()
        {
            //Arrange
            Asset entity = new Asset()
            {
                Name = "Asset name",
                Description = "Description",
                DepartmentdId = 1,
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
        public void AddEntry_ReceivesExistingAssetANdNewID_EntryTypeIsCreate()
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
        public void AddEntry_ReceivesAlteredAsset_EntryTypeIsUpdate()
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

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.EntryType == "Update"))).Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddEntry_ReceivesCleanAssetWithId10_EntryTypeIsDelete()
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
        public void AddEntry_ReceivesAlteredAsset_ChangesCorrespondsWithTheChangesMade()
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
                le.EntryType == "Update"
                && le.Changes.Contains("New asset name")
                && le.Changes.Contains("New asset description")
                && !le.Changes.Contains("Asset identifier")
                && !le.Changes.Contains("DepartmentdId"))))
                .Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddEntry_ReceivesCreatedAsset_ChangesContainsAllValues()
        {
            //Arrange
            Asset entity = new Asset()
            {
                Name = "Asset name",
                Description = "Description",
                DepartmentdId = 1,
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
        public void AddEntry_ReceivesDeletedAsset_ChangesContainsAllValues()
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
        public void AddEntry_ReceivesAlteredTag_ChangesCorrespondsWithTheChangesMade()
        {
            //Arrange
            object[] tagConstructorParameters = new object[]
            {
                (ulong)10,
                "tag name",
                (ulong)1,
                (ulong)0,
                "#d3d3d3",
                10,
                "[]",
                "tag name",
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };

            Tag entity = (Tag)Activator.CreateInstance(typeof(Tag), BindingFlags.NonPublic | BindingFlags.Instance, null, tagConstructorParameters, null, null);
            entity.Name = "new tag name";
            entity.Color = "#f9f9f9";

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le =>
                le.EntryType == "Update"
                && le.Changes.Contains("Name")
                && le.Changes.Contains("new tag name")
                && le.Changes.Contains("Color")
                && le.Changes.Contains("#f9f9f9")
                && !le.Changes.Contains("ParentId")
                && !le.Changes.Contains("DepartmentdId"))))
                .Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddEntry_ReceivesCreatedTag_ChangesContainsAllValues()
        {
            //Arrange
            Tag entity = new Tag()
            {
                Name = "tag name",
                Color = "#d3d3d3",
                DepartmentID = 1,
                ParentId = 0,
                NumberOfChildren = 10
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
        public void AddEntry_ReceivesDeletedTag_ChangesContainsAllValues()
        {
            //Arrange
            object[] tagConstructorParameters = new object[]
            {
                (ulong) 10,
                "tag name",
                (ulong) 1,
                (ulong) 0,
                "#d3d3d3",
                10,
                "[]",
                "tag name",
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };

            Tag entity = (Tag)Activator.CreateInstance(typeof(Tag), BindingFlags.NonPublic | BindingFlags.Instance, null, tagConstructorParameters, null, null);

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
        public void AddEntry_ReceivesAlteredDepartment_ChangesCorrespondsWithTheChangesMade()
        {
            //Arrange
            object[] departmentConstructorParameters = new object[]
            {
                (ulong)10,
                "Department name",
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };

            Department entity = (Department)Activator.CreateInstance(typeof(Department), BindingFlags.Instance | BindingFlags.NonPublic, null, departmentConstructorParameters, null, null);

            entity.Name = "New department name";

            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le =>
                le.EntryType == "Update"
                && le.Changes.Contains("Name")
                && le.Changes.Contains("New department name"))))
                .Returns(true);

            //Act
            bool result = _Log.AddEntry(entity, 1);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddEntry_ReceivesCreatedDepartment_ChangesContainsAllValues()
        {
            //Arrange
            Department entity = new Department()
            {
                Name = "Department name"
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
        public void AddEntry_ReceivesDeletedDepartment_ChangesContainsAllValues()
        {
            //Arrange
            object[] departmentConstructorParameters = new object[]
            {
                (ulong)10,
                "Department name",
                new DateTime(2019, 12, 4, 13, 57, 56),
                new DateTime(2019, 12, 4, 13, 57, 59)
            };

            Department entity = (Department)Activator.CreateInstance(typeof(Department), BindingFlags.NonPublic | BindingFlags.Instance, null, departmentConstructorParameters, null, null);

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
        public void AddEntry_ReceivesException_DescriptionContainsErrorMessageAndStackTrace()
        {
            //Arrange
            Exception e = new Exception("Exception message!!!");
            string messageAndStackTrace = "\nError message: " + e.Message + "\nStack trace:" + e.StackTrace;
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.Description.EndsWith(messageAndStackTrace)))).Returns(true);

            //Act
            bool result = _Log.AddEntry(e);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddEntry_ReceivesNullAsModel_NothingIsSentToRepositoryAndReturnsFalse()
        {
            //Arrange
            _logRepMock.Setup(lr => lr.Insert(It.IsAny<LogEntry>())).Returns(true);

            //Act
            bool result = _Log.AddEntry(null, 1);

            //Arrange
            Assert.IsTrue(!result);
        }

        [TestMethod]
        public void AddEntry_ReceivesEntryTypeAndDescription_RepositoryReceivesLogEntryWithTheGivenEntryTypeAndDescription()
        {
            //Arrange
            _logRepMock.Setup(lr => lr.Insert(It.Is<LogEntry>(le => le.EntryType == "Entry" && le.Description == "This is an entry"))).Returns(true);

            //Act
            bool result = _Log.AddEntry("Entry", "This is an entry");

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddEntry_ReceivesNullForEntryTypeAndDescription_NothingIsSentToRepositoryAndReturnsFalse()
        {
            //Arrange
            _logRepMock.Setup(lr => lr.Insert(It.IsAny<LogEntry>())).Returns(true);

            //Act
            bool result = _Log.AddEntry(null, null);

            //Assert
            Assert.IsTrue(!result);
        }
    }
}