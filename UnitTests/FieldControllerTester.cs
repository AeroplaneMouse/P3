using AMS.Controllers;
using AMS.Controllers.Interfaces;
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
                Field.FieldType.TextBox,true,true);
            _thirdField.TagIDs.Add(5);
            
            _fourthField = new Field("Label of fourth field", "content of fourth field",
                Field.FieldType.Textarea);
            
        }
        
        [TestMethod]
        public void AddField_ChecksFieldsContains()
        {
            //Arrange
            AssetController otherAssetController = new AssetController(new Asset(), new AssetRepository())
            {
                Asset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            //Act
            otherAssetController.AddField(_thirdField);
            otherAssetController.AddField(_fourthField);
            
            //Assert
            Assert.IsTrue(otherAssetController.NonHiddenFieldList.Contains(_thirdField) && otherAssetController.NonHiddenFieldList.Contains(_fourthField));
        }
        
        public void RemoveField_CheckFieldDoesNotContain()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository())
            {
                Asset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAsset.AddField(_thirdField);
            otherAsset.AddField(_fourthField);
            
            //Act
            otherAsset.RemoveField(_fourthField);

            //Assert
            Assert.IsFalse(otherAsset.Asset.FieldList.Contains(_fourthField));
        }
        
        public void RemoveField_UsingInheritedField_CheckFieldIsHidden()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository())
            {
                Asset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAsset.AddField(_thirdField);
            otherAsset.AddField(_fourthField);
            
            //Act
            otherAsset.RemoveField(_thirdField);

            //Assert
            Assert.IsTrue(otherAsset.Asset.FieldList.Contains(_thirdField) && _thirdField.IsHidden);
        }
        
        [TestMethod]
        public void AddField_UsingInheritedField_CheckFieldsContains()
        {
            //Arrange
            AssetController otherAssetController = new AssetController(new Asset(), new AssetRepository())
            {
                Asset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAssetController.AddField(_fourthField);
            
            //Act
            otherAssetController.AddField(_thirdField);
            
            //Assert
            Assert.IsTrue(otherAssetController.NonHiddenFieldList.Contains(_thirdField));
        }
    }
}