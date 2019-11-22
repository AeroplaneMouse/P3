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
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository())
            {
                Asset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            //Act
            otherAsset.AddField(_thirdField);
            otherAsset.AddField(_fourthField);
            
            //Assert
            Assert.IsTrue(otherAsset.Asset.FieldList.Contains(_thirdField) && otherAsset.Asset.FieldList.Contains(_fourthField));
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
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository())
            {
                Asset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAsset.AddField(_fourthField);
            
            //Act
            otherAsset.AddField(_thirdField);
            
            //Assert
            Assert.IsTrue(otherAsset.Asset.FieldList.Contains(_thirdField));
        }
        
        [TestMethod]
        public void DeserializeField_WithNullField()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository())
            {
                Asset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAsset.SerializeFields();
            
            //Act
            //Done in assert.
            
            
            //Assert
            Assert.IsTrue(otherAsset.DeSerializeFields());
        }
    }
}