using System.Linq;
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
                Field.FieldType.TextBox, true, false);
            _thirdField.TagIDs.Add(5);

            _fourthField = new Field("Label of fourth field", "content of fourth field",
                Field.FieldType.Textarea, true, true);
        }

        [TestMethod]
        public void AddField_ChecksFieldsContains()
        {
            //Arrange
            AssetController otherAssetController = new AssetController(new Asset(), new AssetRepository())
            {
                ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            //Act
            otherAssetController.AddField(_thirdField);
            otherAssetController.AddField(_fourthField);

            //Assert
            Assert.IsTrue(otherAssetController.NonHiddenFieldList.Contains(_thirdField) &&
                          otherAssetController.NonHiddenFieldList.Contains(_fourthField));
        }

        [TestMethod]
        public void RemoveField_CheckFieldDoesNotContain()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository())
            {
                ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAsset.AddField(_thirdField);
            otherAsset.AddField(_fourthField);

            //Act
            otherAsset.RemoveField(_fourthField);

            //Assert
            Assert.IsFalse(otherAsset.HiddenFieldList.Contains(_fourthField) ||
                           otherAsset.NonHiddenFieldList.Contains(_fourthField));
        }

        [TestMethod]
        public void RemoveField_UsingInheritedField_CheckFieldIsHidden()
        {
            //Arrange
            AssetController otherAsset = new AssetController(new Asset(), new AssetRepository())
            {
                ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAsset.AddField(_thirdField);
            otherAsset.AddField(_thirdField);

            //Act
            otherAsset.RemoveField(_thirdField);

            //Assert
            Assert.IsTrue(otherAsset.HiddenFieldList.Contains(_thirdField) && _thirdField.IsHidden);
        }

        [TestMethod]
        public void AddField_UsingInheritedField_CheckFieldsContains()
        {
            //Arrange
            AssetController otherAssetController = new AssetController(new Asset(), new AssetRepository())
            {
                ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAssetController.AddField(_fourthField);

            //Act
            otherAssetController.AddField(_thirdField);

            //Assert
            Assert.IsTrue(otherAssetController.NonHiddenFieldList.Contains(_thirdField));
        }

        [TestMethod]
        public void RemoveFieldRelations_Returns_IdInField_NonHiddenFieldList()
        {
            //Arrange
            AssetController otherAssetController = new AssetController(new Asset(), new AssetRepository())
            {
                ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAssetController.AddField(_fourthField);
            otherAssetController.AddField(_thirdField);
            //Act
            otherAssetController.RemoveFieldRelations(5);

            //Assert
            Assert.IsFalse(otherAssetController.NonHiddenFieldList.SingleOrDefault(p => p.TagIDs.Contains(5)) != null);
        }
        
        [TestMethod]
        public void RemoveFieldRelations_Returns_IdInField_HiddenFieldList()
        {
            //Arrange
            AssetController otherAssetController = new AssetController(new Asset(), new AssetRepository())
            {
                ControlledAsset = {Name = "AssetTests_Asset", Description = "Description", DepartmentID = 1}
            };
            otherAssetController.AddField(_fourthField);
            otherAssetController.AddField(_thirdField);

            otherAssetController.RemoveField(_thirdField);
            //Act
            otherAssetController.RemoveFieldRelations(5);

            //Assert
            Assert.IsFalse(otherAssetController.HiddenFieldList.SingleOrDefault(p => p.TagIDs.Contains(5)) != null);
        }
    }
}