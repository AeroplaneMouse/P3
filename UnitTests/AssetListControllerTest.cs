using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using Castle.Core.Internal;
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
        private Mock<IAssetRepository> assetRepMock;
        private Mock<IExporter> exporterMock;

        [TestInitialize]
        public void InitiateAssets()
        {
            // Create Mocks of dependencies
            assetRepMock = new Mock<IAssetRepository>();
            exporterMock = new Mock<IExporter>();
            // This creates a new instance of the class for each test
            _assetListController = new AssetListController(assetRepMock.Object, exporterMock.Object)
            {
                AssetList = new List<Asset>()
            };
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
            assetRepMock.Setup(p => p.Delete(It.IsAny<Asset>())).Returns(true);

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
            Asset asset1 = new Asset {Name = "asset1"};
            _assetListController.AssetList.Add(asset1);
            assetRepMock.Setup(p => p.Delete(It.IsAny<Asset>())).Returns(true);

            //Act
            _assetListController.Remove(asset1);

            bool result = _assetListController.AssetList.Any();

            //Assert
            Assert.IsFalse(result);
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
            Asset asset1 = CreateTestAssetWithId(1, "asset1");
            Asset asset2 = CreateTestAssetWithId(2, "asset2");
            _assetListController.AssetList.Add(asset1);
            _assetListController.AssetList.Add(asset2);
            Asset asset3 = CreateTestAssetWithId(3, "asset3");

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
            Asset asset1 = new Asset {Name = "asset1"};
            Asset asset2 = new Asset {Name = "asset2"};
            _assetListController.AssetList.Add(asset1);
            _assetListController.AssetList.Add(asset2);
            Asset asset3 = new Asset {Name = "asset3"};

            //Act
            _assetListController.Remove(asset3);

            bool result = _assetListController.AssetList.Contains(asset1)
                && _assetListController.AssetList.Contains(asset2)
                && _assetListController.AssetList.Count == 2;

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Search_CallsRepositorySearchMethod_ReturnsTrue()
        {
            //Arrange
            Asset asset1 = new Asset {Name = "asset1"};
            _assetListController.AssetList.Add(asset1);
            assetRepMock.Setup(p => p.Search(It.IsAny<string>(), null, null, false,false)).Returns(() => new List<Asset>());

            //Act
            _assetListController.Search("asset", null);

            //Assert
            // Verify that the method IAssetRepository.Delete(Asset) is called once
            assetRepMock.Verify((p => p.Search(It.IsAny<string>(), null, null, false,false)), Times.AtLeastOnce);
        }
        
        [TestMethod]
        public void Export_CallsExporterPrintMethod_ReturnsTrue()
        {
            //Arrange
            exporterMock.Setup(p => p.Print(It.IsAny<IEnumerable<object>>()));
            List<Asset> list = new List<Asset>();

            //Act
            _assetListController.Export(list);

            //Assert
            // Verify that the method IAssetRepository.Delete(Asset) is called once
            exporterMock.Verify((p => p.Print(It.IsAny<IEnumerable<object>>())), Times.Once);
        }

        // Create asset with id.
        private Asset CreateTestAssetWithId(ulong rowId, string rowName)
        {
            return (Asset)Activator.CreateInstance(typeof(Asset), BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowName, null, null, null, null, null, null }, null, null);
        }
    }
}
