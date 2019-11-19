using AMS.Controllers;
using AMS.Database.Repositories;
using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class FieldControllerTester
    {
        private Field _thirdField;
        private Field _fourthField;
        [TestInitialize]
        public void InitiateFieldsList()
        {
                        
            _thirdField = new Field("Label of third field", "content of third field",
                Field.FieldType.TextBox, "Default value of third field");
            _fourthField = new Field("Label of fourth field", "content of fourth field",
                Field.FieldType.Textarea, "Default value of fourth field");
            
        }
        
        [TestMethod]
        public void AddField_ChecksFieldsContains()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository());
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 1;
            //Act
            otherAsset.Asset.Fields.Add(_thirdField);
            otherAsset.Asset.Fields.Add(_fourthField);
            
            //Assert
            Assert.IsTrue(otherAsset.Asset.Fields.Contains(_thirdField) && otherAsset.Asset.Fields.Contains(_fourthField));
        }
        [TestMethod]
        public void RemoveField_CheckFieldDoesNotContain()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository());
            otherAsset.Asset.Name = "AssetTests_Asset";
            otherAsset.Asset.Description = "Desription";
            otherAsset.Asset.DepartmentID = 1;
            otherAsset.AddField(_thirdField);
            otherAsset.AddField(_fourthField);
            
            //Act
            otherAsset.Asset.Fields.Remove(_thirdField);
            otherAsset.Asset.Fields.Remove(_fourthField);
            
            //Assert
            Assert.IsFalse(otherAsset.Asset.Fields.Contains(_thirdField) && otherAsset.Asset.Fields.Contains(_fourthField));
        }
    }
}