using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Database;
using System.Collections.Generic;
using System.Windows;

namespace UnitTests
{
    [TestClass]
    public class AssetRepositoryTests
    {

        public DepartmentRepository departmentRepository;
        public AssetRepository assetRepository;
        public Asset asset;
        public MySqlHandler mySqlHandler;

        [TestInitialize]
        public void InitiateAssetAndRepository()
        {
            departmentRepository = new DepartmentRepository();
            assetRepository = new AssetRepository();
            asset = new Asset();
            mySqlHandler = new MySqlHandler();

            departmentRepository.Insert(new Department("IntegrationTestDepartment"));

            asset.Name = "Integrationtest";
            asset.Description = "Desription";
            asset.DepartmentID = 1;
            asset.AddField(new Field(1, "Label of first field", "content of first field", 2, "Default value of first field"));
            asset.AddField(new Field(2, "Label of second field", "content of second field", 4, "Default value of second field"));
        }

        [TestMethod]
        public void AssetRepository_Insert_ReturnsTrueAsAssetIsInserted()
        {
            //Arrange

            //Act
            bool result = assetRepository.Insert(asset);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AssetRepository_Update_ReturnsTrueAsAssertIsUpdated()
        {
            //Arrange
            this.assetRepository.Insert(asset);
            Asset assetToUpdate = this.assetRepository.GetById(1);
            assetToUpdate.AddField(new Field(3, "Label of updated field", "content of updated field", 4, "Default value of second field"));

            //Act
            bool result = this.assetRepository.Update(assetToUpdate);

            //Assert
            Assert.IsTrue(condition: result);
        }

        [TestCleanup]
        public void CleanDatabase()
        {
            //Clear asset
            mySqlHandler.RawQuery("TRUNCATE TABLE assets");

            //Clear department
            mySqlHandler.RawQuery("TRUNCATE TABLE departments");
        }
    }
}