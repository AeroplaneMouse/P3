using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS.Models;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class AssetControllerTests
    {
        private IAssetController _assetController;

        private Mock<IAssetRepository> _assetRepMock;
        private TestingTag _tagOne;
        private TestingTag _tagTwo;
        private Field _fieldOne;
        private Field _fieldTwo;

        public class TestingTag : Tag
        {
            public new ulong ID;
        }

        [TestInitialize]
        public void InitiateAsset()
        {
            ulong id;
            // Mock setup
            _assetRepMock = new Mock<IAssetRepository>();
            _assetRepMock.Setup(p => p.Delete(It.IsAny<Asset>())).Returns(true);
            _assetRepMock.Setup(p => p.Insert(It.IsAny<Asset>(), out id)).Returns(true);
            _assetRepMock.Setup(p => p.Update(It.IsAny<Asset>())).Returns(true);
            _assetRepMock.Setup(p => p.AttachTags(It.IsAny<Asset>(), It.IsAny<List<ITagable>>())).Returns(true);

            //Field setup
            _fieldOne = new Field("Label of first field", "content of first field",
                Field.FieldType.TextBox);
            _fieldTwo = new Field("Label of second field", "content of second field",
                Field.FieldType.Checkbox, false,true);


            //Asset controller setup
            _assetController = new AssetController(new Asset(), _assetRepMock.Object);
            _assetController.Asset.Name = "AssetTests_Asset";
            _assetController.Asset.Description = "Desription";
            _assetController.Asset.DepartmentID = 1;
            _assetController.AddField(_fieldOne);
            _assetController.AddField(_fieldTwo);

            //Tags setup
            _tagOne = new TestingTag();
            _tagOne.Name = "First tag";
            _tagOne.ID = 1;
            _tagTwo = new TestingTag();
            _tagTwo.Name = "Second tag";
            _tagTwo.ID = 2;
            
        }

        [TestMethod]
        public void Equals_ReceivesAnEqualAsset_ReturnsTrue()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 1;

            otherAsset.AddField(new Field("Label of first field", "content of first field", Field.FieldType.TextBox));
            otherAsset.AddField(new Field("Label of second field", "content of second field", Field.FieldType.Checkbox));

            //Act
            bool result = _assetController.Asset.Equals(otherAsset.Asset);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_ReceivesDifferentAsset_ReturnsFalse()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 4;

            otherAsset.AddField(new Field("Label of first field", "content of first field", Field.FieldType.TextBox));

            //Act
            bool result = _assetController.Asset.Equals(otherAsset.Asset);

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
        public void TagTag_Returns_MOCKRepositoryFunctionUsed()
        {
            //Arrange 
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 4;

            //Act
            otherAsset.AttachTag(_tagOne);

            //Assert
            Assert.IsTrue(otherAsset.CurrentlyAddedTags.Contains(_tagOne));
        }

        [TestMethod]
        public void DetachTag_Returns_TagInCurrentlyAddedTagsList()
        {
            //Arrange 
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 4;
            otherAsset.AttachTag(_tagOne);
            otherAsset.AttachTag(_tagTwo);


            //Act
            otherAsset.DetachTag(_tagTwo);

            //Assert
            Assert.IsFalse(otherAsset.CurrentlyAddedTags.Contains(_tagTwo));
        }

        [TestMethod]
        public void Attatch_WithField_TagInCurrentlyAddedTagsList()
        {
            //Arrange 
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 4;
            Field localField = new Field("Label of first field", "content of first field", Field.FieldType.TextBox);
            TestingTag localTag = new TestingTag();
            localTag.Name = "First tag";
            localTag.ID = 1;
            localTag.FieldList.Add(localField);


            otherAsset.AttachTag(_tagOne);
            otherAsset.AttachTag(_tagTwo);

            //Act
            otherAsset.AttachTag(localTag);

            //Assert
            Assert.IsTrue(otherAsset.CurrentlyAddedTags.Contains(localTag) &&
                          _assetController.Asset.FieldList.Contains(localField));
        }

        [TestMethod]
        public void Attach_WithFieldAlreadyOnAsset_TagInCurrentlyAddedTagsList()
        {
            //Arrange 
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 4;
            Field localField = new Field("Label of first field", "content of first field", Field.FieldType.TextBox);
            otherAsset.AddField(localField);
            TestingTag localTag = new TestingTag();
            localTag.Name = "First tag";
            localTag.ID = 3;
            localTag.FieldList.Add(localField);


            otherAsset.AttachTag(_tagOne);
            otherAsset.AttachTag(_tagTwo);

            //Act
            otherAsset.AttachTag(localTag);

            //Assert
            Assert.IsTrue(otherAsset.CurrentlyAddedTags.Contains(localTag) &&
                          _assetController.Asset.FieldList.Contains(localField));
        }

        [TestMethod]
        public void DetachTag_WithField_TagInCurrentlyAddedTagsList()
        {
            //Arrange 
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 4;
            Field localField = new Field("Label of first field", "content of first field", Field.FieldType.TextBox,true,true);
            TestingTag localTag = new TestingTag();
            localTag.Name = "First tag";
            localTag.ID = 1;
            localTag.FieldList.Add(localField);


            otherAsset.AttachTag(_tagOne);
            otherAsset.AttachTag(_tagTwo);
            otherAsset.AttachTag(localTag);


            //Act
            otherAsset.DetachTag(localTag);

            //Assert
            Assert.IsFalse(otherAsset.CurrentlyAddedTags.Contains(localTag) &&
                           _assetController.Asset.FieldList.Contains(localField));
        }

        [TestMethod]
        public void Attach_WithFieldAlreadyOnAsset_Returns_FieldPresentInListElementAdded()
        {
            //Arrange 
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 4;

            //Local field moq
            Field localField = new Field("Label of the first field", "content of the first field",
                Field.FieldType.TextBox);
            otherAsset.AddField(localField);

            TestingTag localTag = new TestingTag {Name = "First tag"};
            localTag.FieldList.Add(localField);


            otherAsset.AttachTag(_tagOne);
            otherAsset.AttachTag(_tagTwo);

            //Act
            int count = localField.TagIDs.Count;
            otherAsset.AttachTag(localTag);

            //Assert
            Assert.IsTrue(localField.TagIDs.Count == count + 1);
        }
    }
}