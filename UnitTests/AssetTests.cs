using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS.Models;
using AMS.Controllers;
using AMS.Database.Repositories;

namespace UnitTests
{
    [TestClass]
    public class AssetTests
    {
        private AMS.Controllers.Interfaces.IAssetController _assetController;
        
        [TestInitialize]
        public void InitiateAsset()
        {
            _assetController = new AssetController(new Asset(), new AssetRepository());
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
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository());
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
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository());
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
            
        }
    }
}