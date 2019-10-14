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
            asset.SerializeFields();
        }

        [TestMethod]
        public void Insert_ReceivesAWellDefinedAsset_ReturnsTrueAsAssetIsInserted()
        {
            //Arrange

            //Act
            bool result = assetRepository.Insert(asset);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Update_ReceivesAnAssetExistingInTheDatabase_ReturnsTrueAsTheAssetIsUpdated()
        {
            //Arrange
            assetRepository.Insert(asset);
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
            //Set foreign key check to 0
            mySqlHandler.RawQuery("SET FOREIGN_KEY_CHECKS = 0");

            //Clear asset
            mySqlHandler.RawQuery("TRUNCATE TABLE assets");

            //Clear department
            mySqlHandler.RawQuery("TRUNCATE TABLE departments");

            //Reset foreign key check
            mySqlHandler.RawQuery("SET FOREIGN_KEY_CHECKS = 1");
        }
    }
}