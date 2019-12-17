using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using AMS.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS.Models;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using Moq;
using System.Reflection;

namespace UnitTests
{
    [TestClass]
    public class AssetControllerTests
    {
        private IAssetController _assetController;

        private Mock<IAssetRepository> _assetRepMock;
        private Mock<IUserRepository> _userRepMock;
        private Mock<Session> _sessionMock;
        private Tag _tagOne;
        private Tag _tagTwo;
        private Field _fieldOne;
        private Field _fieldTwo;

        [TestInitialize]
        public void InitiateAsset()
        {
            ulong id;
            // Mock setup
            _assetRepMock = new Mock<IAssetRepository>();
            _assetRepMock.Setup(p => p.Delete(It.IsAny<Asset>())).Returns(true);
            _assetRepMock.Setup(p => p.Insert(It.IsAny<Asset>(), out id)).Returns(It.IsAny<Asset>());
            _assetRepMock.Setup(p => p.Update(It.IsAny<Asset>())).Returns(true);
            _assetRepMock.Setup(p => p.AttachTags(It.IsAny<Asset>(), It.IsAny<List<ITagable>>())).Returns(true);

            _userRepMock = new Mock<IUserRepository>();
            _userRepMock.Setup(p => p.GetByIdentity(It.IsAny<string>()))
                        .Returns(new User {Username = "TestUser", DefaultDepartment = 1});

            _sessionMock = new Mock<Session>(_userRepMock.Object);

            //Field setup
            _fieldOne = new Field("Label of first field", "content of first field", Field.FieldType.TextBox);
            _fieldTwo = new Field("Label of second field", "content of second field", Field.FieldType.Checkbox, false, true);

            //Asset controller setup
            _assetController = new AssetController(CreateTestAssetWithId((ulong) 1), _assetRepMock.Object,
                _sessionMock.Object);
            _assetController.ControlledAsset.Name = "AssetTests_Asset";
            _assetController.ControlledAsset.Description = "Desription";
            _assetController.ControlledAsset.DepartmentdId = 1;
            _assetController.AddField(_fieldOne);
            _assetController.AddField(_fieldTwo);

            //Tags setup
            _tagOne = new Tag();
            _tagOne.Name = "First tag";
            _tagTwo = new Tag();
            _tagTwo.Name = "Second tag";
        }

        [TestMethod]
        public void Equals_ReceivesAnEqualAsset_ReturnsTrue()
        {
            //Arrange
            AssetController otherAsset =
                new AssetController(CreateTestAssetWithId(1), _assetRepMock.Object, _sessionMock.Object);
            otherAsset.ControlledAsset.Name = "AssetTests_Asset";
            otherAsset.ControlledAsset.Description = "Desription";
            otherAsset.ControlledAsset.DepartmentdId = 1;

            otherAsset.AddField(new Field("Label of first field", "content of first field", Field.FieldType.TextBox));
            otherAsset.AddField(new Field("Label of second field", "content of second field", Field.FieldType.Checkbox));

            //Act
            bool result = _assetController.ControlledAsset.Equals(otherAsset.ControlledAsset);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_ReceivesDifferentAsset_ReturnsFalse()
        {
            //Arrange
            AssetController otherAsset =
                new AssetController(CreateTestAssetWithId(2), _assetRepMock.Object, _sessionMock.Object);
            otherAsset.ControlledAsset.Name = "AssetTests_Asset";
            otherAsset.ControlledAsset.Description = "Desription";
            otherAsset.ControlledAsset.DepartmentdId = 4;

            otherAsset.AddField(new Field("Label of first field", "content of first field", Field.FieldType.TextBox));

            //Act
            bool result = _assetController.ControlledAsset.Equals(otherAsset.ControlledAsset);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SaveAsset_Returns_RepositoryFunctionUsed()
        {
            //Arrange
            ulong id = 0;

            //Act
            _assetController.Save();

            //Assert
            _assetRepMock.Verify(p => p.Insert(It.IsAny<Asset>(), out id), Times.Once());
        }

        [TestMethod]
        public void SaveAsset_RepositoryInsertSetsIdValid_ReturnsTrue()
        {
            //Arrange
            ulong idValue = 1;
            _assetRepMock.Setup(p => p.Insert(It.IsAny<Asset>(), out idValue));
            _assetController.ControlledAsset = new Asset();
            //Act
            bool result = _assetController.Save();

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SaveAsset_RepositoryInsertSetsIdInvalid_ReturnsFalse()
        {
            //Arrange
            ulong idValue = 0;
            _assetRepMock.Setup(p => p.Insert(It.IsAny<Asset>(), out idValue));
            _assetController.ControlledAsset = new Asset();

            //Act
            bool result = _assetController.Save();

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DeleteAsset_Returns_RepositoryFunctionUsed()
        {
            //Arrange
            //Nothin


            //Act
            _assetController.Remove();

            //Assert
            _assetRepMock.Verify(p => p.Delete(It.IsAny<Asset>()), Times.Once());
        }

        [TestMethod]
        public void DeleteAsset_RepositoryDeleteReturnsTrue_ReturnsTrue()
        {
            //Arrange
            _assetRepMock.Setup(p => p.Delete(It.IsAny<Asset>())).Returns(true);
            _assetController.ControlledAsset = new Asset();

            //Act
            bool result = _assetController.Remove();

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DeleteAsset_RepositoryDeleteReturnsFalse_ReturnsFalse()
        {
            //Arrange
            _assetRepMock.Setup(p => p.Delete(It.IsAny<Asset>())).Returns(false);
            _assetController.ControlledAsset = new Asset();

            //Act
            bool result = _assetController.Remove();

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UpdateAsset_Returns_RepositoryFunctionUsed()
        {
            //Arrange
            //Nothin


            //Act
            _assetController.Update();

            //Assert
            _assetRepMock.Verify(p => p.Update(It.IsAny<Asset>()), Times.Once());
        }

        [TestMethod]
        public void UpdateAsset_RepositoryUpdateReturnsTrue_ReturnsTrue()
        {
            //Arrange
            _assetRepMock.Setup(p => p.Update(It.IsAny<Asset>())).Returns(true);
            _assetController.ControlledAsset = new Asset();

            //Act
            bool result = _assetController.Update();

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateAsset_RepositoryUpdateReturnsFalse_ReturnsFalse()
        {
            //Arrange
            _assetRepMock.Setup(p => p.Update(It.IsAny<Asset>())).Returns(false);
            _assetController.ControlledAsset = new Asset();

            //Act
            bool result = _assetController.Update();

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TagTag_Returns_MOCKRepositoryFunctionUsed()
        {
            //Arrange 
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object, _sessionMock.Object);
            otherAsset.ControlledAsset.Name = "AssetTests_Asset";
            otherAsset.ControlledAsset.Description = "Desription";
            otherAsset.ControlledAsset.DepartmentdId = 4;

            //Act
            otherAsset.AttachTags(_tagOne);

            //Assert
            Assert.IsTrue(otherAsset.CurrentlyAddedTags.Contains(_tagOne));
        }

        [TestMethod]
        public void DetachTag_Returns_TagInCurrentlyAddedTagsList()
        {
            //Arrange 
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object, _sessionMock.Object);
            otherAsset.ControlledAsset.Name = "AssetTests_Asset";
            otherAsset.ControlledAsset.Description = "Desription";

            otherAsset.ControlledAsset.DepartmentdId = 4;
            otherAsset.AttachTags(_tagOne);
            otherAsset.AttachTags(_tagTwo);

            //Act
            List<ITagable> tagsToRemove = new List<ITagable>();
            tagsToRemove.Add(_tagTwo);
            otherAsset.DetachTags(tagsToRemove);

            //Assert
            Assert.IsFalse(otherAsset.CurrentlyAddedTags.Contains(_tagTwo));
        }

        [TestMethod]
        public void AttatchTag_WithField_Returns_FieldInFieldsList()
        {
            //Arrange 
            AssetController otherAssetController =
                new AssetController(new Asset(), _assetRepMock.Object, _sessionMock.Object);
            otherAssetController.ControlledAsset.Name = "AssetTests_Asset";
            otherAssetController.ControlledAsset.Description = "Desription";
            otherAssetController.ControlledAsset.DepartmentdId = 4;
            Field localField = new Field("Label of first field", "content of first field", Field.FieldType.TextBox);

            Tag localTag = new Tag();
            localTag.Name = "First tag";
            localTag.FieldList.Add(localField);

            otherAssetController.AttachTags(_tagOne);
            otherAssetController.AttachTags(_tagTwo);

            //Act
            otherAssetController.AttachTags(localTag);

            //Assert
            Assert.IsTrue(
                _assetController.ControlledAsset.FieldList.SingleOrDefault(field => field.Equals(localField)) != null);
        }

        [TestMethod]
        public void AttachTag_WithFieldAlreadyOnAsset_TagInCurrentlyAddedTagsList()
        {
            //Arrange 
            AssetController otherAssetController =
                new AssetController(new Asset(), _assetRepMock.Object, _sessionMock.Object);
            otherAssetController.ControlledAsset.Name = "AssetTests_Asset";
            otherAssetController.ControlledAsset.Description = "Desription";
            otherAssetController.ControlledAsset.DepartmentdId = 4;
            Field localField = new Field("Label of first field", "content of first field", Field.FieldType.TextBox);
            otherAssetController.AddField(localField);
            Tag localTag = new Tag();
            localTag.FieldList.Add(localField);


            otherAssetController.AttachTags(_tagOne);
            otherAssetController.AttachTags(_tagTwo);

            //Act
            otherAssetController.AttachTags(localTag);

            //Assert
            Assert.IsTrue(otherAssetController.CurrentlyAddedTags.Contains(localTag));
        }

        [TestMethod]
        public void DetachTag_WithField_TagInCurrentlyAddedTagsList()
        {
            //Arrange 
            AssetController otherAssetController =
                new AssetController(new Asset(), _assetRepMock.Object, _sessionMock.Object);
            otherAssetController.ControlledAsset.Name = "AssetTests_Asset";
            otherAssetController.ControlledAsset.Description = "Desription";
            otherAssetController.ControlledAsset.DepartmentdId = 4;
            Field localField = new Field("Label of first field", "content of first field", Field.FieldType.TextBox,
                true, true);
            Tag localTag = new Tag();
            localTag.Name = "First tag";
            localTag.FieldList.Add(localField);


            otherAssetController.AttachTags(_tagOne);
            otherAssetController.AttachTags(_tagTwo);
            otherAssetController.AttachTags(localTag);


            //Act
            List<ITagable> tagsToRemove = new List<ITagable>();
            tagsToRemove.Add(localTag);
            otherAssetController.DetachTags(tagsToRemove);

            //Assert
            Assert.IsFalse(otherAssetController.CurrentlyAddedTags.Contains(localTag) &&
                           _assetController.ControlledAsset.FieldList.Contains(localField));
        }

        [TestMethod]
        public void AttachTag_WithFieldAlreadyOnAsset_Returns_FieldPresentInListElementAdded()
        {
            //Arrange 
            AssetController otherAssetController =
                new AssetController(new Asset(), _assetRepMock.Object, _sessionMock.Object);
            otherAssetController.ControlledAsset.Name = "AssetTests_Asset";
            otherAssetController.ControlledAsset.Description = "Desription";
            otherAssetController.ControlledAsset.DepartmentdId = 4;

            //Local field moq
            Field localField = new Field("Label of the first field", "content of the first field",
                Field.FieldType.TextBox);
            otherAssetController.AddField(localField);

            Tag localTag = CreateTestTagWithId(4);
            localTag.FieldList.Add(localField);


            otherAssetController.AttachTags(_tagOne);
            otherAssetController.AttachTags(_tagTwo);

            //Act
            int count = localField.TagIDs.Count;
            otherAssetController.AttachTags(localTag);

            //Assert
            Assert.IsTrue(localField.TagIDs.Count == count + 1);
        }

        // Create asset with id.
        private Asset CreateTestAssetWithId(ulong rowId)
        {
            return (Asset) Activator.CreateInstance(typeof(Asset), BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {rowId, null, null, null, null, null, null, null}, null, null);
        }

        // Create tag with id.
        private Tag CreateTestTagWithId(ulong rowId)
        {
            return (Tag) Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] {rowId, "TagTest", null, null, null, null, null, null, null, null}, null,
                null);
        }
    }
}