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

        private DepartmentRepository departmentRepository;
        private AssetRepository assetRepository;
        private Asset asset;
        private MySqlHandler mySqlHandler;

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
        public void Insert_ReceivesAWellDefinedAsset_ReturnsTrueAsAssetIsInserted()
        {
            //Arrange

            //Act
            bool result = assetRepository.Insert(asset);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Insert_ReceivesAssetWithoutFields_ReturnsTrueAsAssetIsInserted()
        {
            //Arrange
            asset.FieldsList = new List<Field>();

            //Act
            bool result = assetRepository.Insert(asset);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetById_ParameterIdExistsInDatabase_RetrievesAssetFromDatabase()
        {
            //Arrange
            assetRepository.Insert(asset);

            //Act
            Asset retrievedAsset = assetRepository.GetById(1);

            //Assert
            Assert.AreEqual(asset, retrievedAsset);
        }

        [TestMethod]
        public void Update_WellDefinedAssetFromDatabase_ReturnsTrueAsTheAssetIsUpdated()
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

        [TestMethod]
        public void Update_AssetWithoutFieldsInDatabase_ChecksIfAssetContainsField()
        {
            //Arrange
            Field expected = new Field(0, "Field", "Some content", 2, "Default");
            Asset assetWithoutFields = new Asset
            {
                Name = "Integrationtest",
                Description = "Desription",
                DepartmentID = 1,
                FieldsList = new List<Field>()
            };

            assetRepository.Insert(assetWithoutFields);

            //Act
            Asset assetToUpdate = this.assetRepository.GetById(1);
            assetToUpdate.AddField(expected);
            assetRepository.Update(assetToUpdate);

            Asset updatedAsset = this.assetRepository.GetById(1);

            //Assert
            Assert.AreEqual(expected, updatedAsset.FieldsList[0]);
        }

        [TestMethod]
        public void Delete_AssetInDataBase_GetByIdReturnsNullWithDeletedAssetId()
        {
            //Arrange
            bool isAdded = assetRepository.Insert(asset);
            if (!isAdded)
            {
                Assert.Fail();
            }

            //Act
            Asset retrievedAsset = assetRepository.GetById(1);
            bool isDeleted = assetRepository.Delete(retrievedAsset);
            Asset result = assetRepository.GetById(1);

            //Assert
            Assert.AreEqual(null, result);
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