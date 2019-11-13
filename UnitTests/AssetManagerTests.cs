using System;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services;
using Asset_Management_System.ViewModels;
using Asset_Management_System.ViewModels.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class AssetManagerTests
    {
        private IAssetManager _assetManager;
        private Tag _tag;
        private ITagable _tagable;
        private Asset _asset;

        [TestInitialize]
        public void InitiateAsset()
        {
            _assetManager = new AssetManagerViewModel(new MainViewModel(new Window()), new Asset(), new AssetService(new AssetRepository()), new TextBox());
            _tag = new Tag();
            _tag.FieldsList.Add(new Field("1","", Field.FieldType.Textbox,"",false, true));
            _tag.FieldsList.Add(new Field("2","", Field.FieldType.Textbox,"",false, true));
            _tagable = new Tag();
            _asset = new Asset();
        }

        [TestMethod]
        public void AttachTag_AreFieldsAdded_ReturnsTrue()
        {
            //Arrange
            bool expected = true;

            //Act
            bool result = _assetManager.AttachTag(_asset, _tag) && _asset.FieldsList.Equals(_tag.FieldsList);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}