using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AMS.Authentication;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class FieldControllerTester
    {
        private Field _thirdField;
        private Field _fourthField;
        private Mock<IAssetRepository> _mockedReppository;
        private Mock<Session> _sessionMock;
        private Mock<IUserRepository> _userRepMock;

        [TestInitialize]
        public void InitiateFieldsList()
        {
            _thirdField = new Field("Label of third field", "content of third field",
                Field.FieldType.TextBox, true, false);
            _thirdField.TagIDs.Add(5);

            _fourthField = new Field("Label of fourth field", "content of fourth field",
                Field.FieldType.Textarea, true, true);

            _mockedReppository = new Mock<IAssetRepository>();
            _mockedReppository.Setup(p => p.GetTags(It.IsAny<Asset>())).Returns(new List<ITagable>());
            _userRepMock = new Mock<IUserRepository>();
            _userRepMock.Setup(p => p.GetByIdentity(It.IsAny<string>()))
                .Returns(new User {Username = "TestUser", DefaultDepartment = 1});
            _sessionMock = new Mock<Session>(_userRepMock.Object);
        }

        [TestMethod]
        public void AddField_ChecksFieldsContains()
        {
            //Arrange
            AssetController otherAssetController =
                new AssetController(new Asset(), _mockedReppository.Object, _sessionMock.Object)
                {
                    ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentdId = 1}
                };
            //Act
            otherAssetController.AddField(_thirdField);
            otherAssetController.AddField(_fourthField);

            //Assert
            Assert.IsTrue(otherAssetController.ControlledAsset.FieldList.Contains(_thirdField) &&
                          otherAssetController.ControlledAsset.FieldList.Contains(_fourthField));
        }

        [TestMethod]
        public void RemoveField_CheckFieldDoesNotContain()
        {
            //Arrange
            AssetController otherAsset =
                new AssetController(new Asset(), _mockedReppository.Object, _sessionMock.Object)
                {
                    ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentdId = 1}
                };
            otherAsset.AddField(_thirdField);
            otherAsset.AddField(_fourthField);

            //Act
            otherAsset.RemoveField(_fourthField);

            //Assert
            Assert.IsFalse(otherAsset.ControlledAsset.FieldList.Contains(_fourthField) ||
                           otherAsset.ControlledAsset.FieldList.Contains(_fourthField));
        }

        [TestMethod]
        public void RemoveField_UsingInheritedField_CheckFieldIsHidden()
        {
            //Arrange
            AssetController otherAsset =
                new AssetController(new Asset(), _mockedReppository.Object, _sessionMock.Object)
                {
                    ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentdId = 1}
                };
            otherAsset.AddField(_thirdField);
            otherAsset.AddField(_thirdField);

            //Act
            otherAsset.RemoveField(_thirdField);

            //Assert
            Assert.IsTrue(otherAsset.ControlledAsset.FieldList.Contains(_thirdField) && _thirdField.IsHidden);
        }

        [TestMethod]
        public void AddField_UsingInheritedField_CheckFieldsContains()
        {
            //Arrange
            AssetController otherAssetController =
                new AssetController(new Asset(), _mockedReppository.Object, _sessionMock.Object)
                {
                    ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentdId = 1}
                };
            otherAssetController.AddField(_fourthField);

            //Act
            otherAssetController.AddField(_thirdField);

            //Assert
            Assert.IsTrue(otherAssetController.ControlledAsset.FieldList.Contains(_thirdField));
        }

        [TestMethod]
        public void RemoveFieldRelations_Returns_IdInField_FieldList()
        {
            //Arrange
            AssetController otherAssetController =
                new AssetController(new Asset(), _mockedReppository.Object, _sessionMock.Object)
                {
                    ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentdId = 1}
                };
            otherAssetController.AddField(_fourthField);
            otherAssetController.AddField(_thirdField);
            //Act
            otherAssetController.RemoveTagRelationsOnFields(CreateTestTagWithId(5));

            //Assert
            Assert.IsTrue(otherAssetController.ControlledAsset.FieldList.SingleOrDefault(p => p.TagIDs.Contains(5)) ==
                          null);
        }


        private Tag CreateTestTagWithId(ulong rowId)
        {
            return (Tag) Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {rowId, "TagTest", null, null, null, null, null, null, null, null}, null,
                null);
        }
    }
}