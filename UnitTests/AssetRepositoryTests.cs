using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using System.Collections.Generic;
using System.Windows;

namespace UnitTests
{
    [TestClass]
    public class AssetRepositoryTests
    {
        public AssetRepository assetRepository;
        public Asset asset;

        [TestInitialize]
        public void InitiateAssetAndRepository()
        {
            this.assetRepository = new AssetRepository();
            this.asset = new Asset();
            this.asset.Name = "Integrationtest";
            this.asset.Description = "Desription";
            this.asset.DepartmentID = 1;
            this.asset.AddField(new Field(1, "Label of first field", "content of first field", 2, "Default value of first field"));
            this.asset.AddField(new Field(2, "Label of second field", "content of second field", 4, "Default value of second field"));
        }

        [TestMethod]
        public void AssetRepository_Insert_ReturnsTrueAsAssetIsInserted()
        {
            //Arrange
            

            //Act
            bool result = this.assetRepository.Insert(asset);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AssetRepository_Update_ReturnsTrueAsAssertIsUpdated()
        {
            //Arrange
            this.assetRepository.Insert(asset);
            ulong assetID = 1;
            Asset assetToUpdate = this.assetRepository.GetById(assetID);
            assetToUpdate.AddField(new Field(3, "Label of updated field", "content of updated field", 4, "Default value of second field"));

            //Act
            bool result = this.assetRepository.Update(assetToUpdate);

            //Assert
            Assert.IsTrue(result);
        }

        [TestCleanup]
        public void DeleteAssetsFromRepository()
        {
            this.assetRepository.Delete(asset);
        }
    }
}
