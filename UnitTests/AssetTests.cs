using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS.Models;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class AssetTests
    {
        private IAssetController _assetController;
        
        private Mock<IAssetRepository> _assetRepMock;
        
        [TestInitialize]
        public void InitiateAsset()
        {
            ulong id;
            // Mock setup
            _assetRepMock = new Mock<IAssetRepository>();
            _assetRepMock.Setup(p => p.Delete(It.IsAny<Asset>())).Returns(true);
            _assetRepMock.Setup(p => p.Insert(It.IsAny<Asset>(),out id)).Returns(true);
            _assetRepMock.Setup(p => p.Update(It.IsAny<Asset>())).Returns(true);
            
            //Asset controller setup
            _assetController = new AssetController(new Asset(), _assetRepMock.Object);
            _assetController.Asset.Name = "AssetTests_Asset";
            _assetController.Asset.Description = "Desription";
            _assetController.Asset.DepartmentID = 1;
            
            
            _assetController.AddField(new Field("Label of first field", "content of first field",
                Field.FieldType.TextBox, "Default value of first field"));
            _assetController.AddField(new Field("Label of second field", "content of second field",
                Field.FieldType.Checkbox, "Default value of second field"));
        }


        [TestMethod]
        public void ToString_ReturnsNameOfAsset()
        {
            //Arrange
            string expected = "AssetTests_Asset";

            //Act
            string result = _assetController.Asset.ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }

 

        [TestMethod]
        public void Equals_ReceivesAnEqualAsset_ReturnsTrue()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), _assetRepMock.Object);
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 1;
            
            otherAsset.AddField(new Field("Label of first field", "content of first field", Field.FieldType.TextBox,
                "Default value of first field"));
            otherAsset.AddField(new Field("Label of second field", "content of second field", Field.FieldType.Checkbox,
                "Default value of second field"));

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
            
            otherAsset.AddField(new Field("Label of first field", "content of first field", Field.FieldType.TextBox,
                "Default value of first field"));

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
            _assetRepMock.Verify(p => p.Insert(It.IsAny<Asset>(),out id), Times.Once());
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
    }
}