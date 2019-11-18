using System.Collections.Generic;
using System.Linq;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using Castle.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ITagController = Asset_Management_System.ViewModels.Controllers.Interfaces.ITagController;

namespace UnitTests
{
    [TestClass]
    public class AssetListControllerTest
    {
        private IAssetListController _assetListController;
        private IExporter _exporter;
        private IAssetRepository _assetRepository;
        private Mock<IAssetRepository> assetRepMock;

        [TestInitialize]
        public void InitiateAssets()
        {
            // Change this line to change the class that is tested
            assetRepMock = new Mock<IAssetRepository>();
            assetRepMock.Setup(p => p.Delete(It.IsAny<Asset>())).Returns(true);
            // This creates a new instance of the class for each test
            _assetListController = new AssetListController(assetRepMock.Object, _exporter);
        }

        [TestMethod]
        public void AddNew_NewAssetAdded_ReturnsOne()
        {
            //Arrange
            int expected = 1;
            
            //Act
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
            int expected = 1;
            Asset asset1 = new Asset {Name = "asset1"};
            Asset asset2 = new Asset {Name = "asset2"};
            _assetListController.AssetList.Add(asset1);
            _assetListController.AssetList.Add(asset2);

            //Act
            _assetListController.Remove(asset1);

            int result = _assetListController.AssetList.Count;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Remove_RemoveOnlyAssetInList_ReturnsTrue()
        {
            //Arrange
            bool expected = true;
            Asset asset1 = new Asset {Name = "asset1"};
            _assetListController.AssetList.Add(asset1);

            //Act
            _assetListController.Remove(asset1);

            bool result = _assetListController.AssetList.IsNullOrEmpty();

            //Assert
            Assert.AreEqual(expected, result);
        }
        
        [TestMethod]
        public void Remove_RemoveCallsRepository_ReturnsTrue()
        {
            //Arrange
            Asset asset1 = new Asset {Name = "asset1"};
            _assetListController.AssetList.Add(asset1);

            //Act
            _assetListController.Remove(asset1);

            //Assert
            // Verify that the method IAssetRepository.Delete(Asset) is called once
            assetRepMock.Verify(p => p.Delete(It.IsAny<Asset>()), Times.Once());
        }
        
        [TestMethod]
        public void Remove_RemoveAssetNotInListDoesNotCallRepository_ReturnsTrue()
        {
            //Arrange
            Asset asset1 = new Asset {Name = "asset1"};
            Asset asset2 = new Asset {Name = "asset2"};
            _assetListController.AssetList.Add(asset1);
            _assetListController.AssetList.Add(asset2);
            Asset asset3 = new Asset {Name = "asset3"};

            //Act
            _assetListController.Remove(asset3);

            //Assert
            // Verify that the method IAssetRepository.Delete(Asset) is never called
            assetRepMock.Verify((p => p.Delete(It.IsAny<Asset>())), Times.Never);
        }
        
        [TestMethod]
        public void Remove_RemoveAssetNotInListDoesNotChangeList_ReturnsTrue()
        {
            //Arrange
            bool expected = true;
            Asset asset1 = new Asset {Name = "asset1"};
            Asset asset2 = new Asset {Name = "asset2"};
            _assetListController.AssetList.Add(asset1);
            _assetListController.AssetList.Add(asset2);
            List<Asset> list = _assetListController.AssetList;
            Asset asset3 = new Asset {Name = "asset3"};

            //Act
            _assetListController.Remove(asset3);

            bool result = _assetListController.AssetList.SequenceEqual(list);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
