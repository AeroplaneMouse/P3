using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class AssetListControllerTest
    {
        private IAssetListController _assetListController;
        private IExporter _exporter;
        private IAssetRepository _assetRepository;
        
        [TestInitialize]
        public void InitiateAssets()
        {
            //TODO: Create mocks for dependencies
            // Change this line to change the class that is tested
            //_assetListController = new AssetListController(_assetService, _exporter);
            //Asset asset1 = new Asset {Name = "asset1"};
            //Asset asset2 = new Asset {Name = "asset2"};
            //_assetListController.AssetList.Add(asset1);
            //_assetListController.AssetList.Add(asset2);
        }
        
        [TestMethod]
        public void AddNew_NewAssetAdded_ReturnsOne()
        {
            //Arrange
            int expected = 1;

            //Act
            _assetListController = new AssetListController(_assetRepository, _exporter);
            _assetListController.AddNew();
            int result = _assetListController.AssetList.Count;

            //Assert
            Assert.AreEqual(expected, result);
        }
        
        [TestMethod]
        public void AddNew_TwoNewAssetAdded_ReturnsTwo()
        {
            //Arrange
            int expected = 2;

            //Act
            _assetListController = new AssetListController(_assetRepository, _exporter);
            _assetListController.AddNew();
            _assetListController.AddNew();
            int result = _assetListController.AssetList.Count;

            //Assert
            Assert.AreEqual(expected, result);
        }
        
        [TestMethod]
        public void Remove_RemoveAsset_ReturnsOne()
        {
            //Arrange
            int expected = 2;

            //Act
            _assetListController = new AssetListController(_assetRepository, _exporter);
            Asset asset1 = new Asset {Name = "asset1"};
            Asset asset2 = new Asset {Name = "asset2"};
            _assetListController.AssetList.Add();
            _assetListController.AssetList.Add();
            
            int result = _assetListController.AssetList.Count;

            //Assert
            Assert.AreEqual(expected, result);
        }
        
        
    }
}