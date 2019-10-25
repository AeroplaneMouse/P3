using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Database;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;

namespace IntegrationTests
{
    [TestClass]
    public class AssetRepositoryTests
    {

        private DepartmentRepository departmentRepository;
        private AssetRepository assetRepository;
        private Asset asset;
        private MySqlHandler mySqlHandler = new MySqlHandler();

        [TestInitialize]
        public void InitiateAssetAndRepository()
        {
            departmentRepository = new DepartmentRepository();
            assetRepository = new AssetRepository();
            asset = new Asset();

            departmentRepository.Insert(new Department { Name = "IntegrationTestDepartment" });

            asset.Name = "AssetRepositoryTests - Integrationtest";
            asset.Description = "Desription";
            asset.DepartmentID = 1;
            asset.AddField(new Field("Label of first field", "content of first field", 2, "Default value of first field"));
            asset.AddField(new Field("Label of second field", "content of second field", 4, "Default value of second field"));
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
        public void GetById_ParameterIdDoesNotExistInDatabase_ReturnsNull()
        {
            //Arrange

            //Act
            Asset retrievedAsset = assetRepository.GetById(1);

            //Assert
            Assert.IsNull(retrievedAsset);
        }

        [TestMethod]
        public void Update_WellDefinedAssetFromDatabase_ReturnsTrueAsTheAssetIsUpdated()
        {
            //Arrange
            assetRepository.Insert(asset);
            Asset assetToUpdate = this.assetRepository.GetById(1);
            assetToUpdate.AddField(new Field("Label of updated field", "content of updated field", 4, "Default value of second field"));

            //Act
            bool result = this.assetRepository.Update(assetToUpdate);

            //Assert
            Assert.IsTrue(condition: result);
        }

        [TestMethod]
        public void Update_AssetWithoutFieldsInDatabase_ChecksIfAssetContainsField()
        {
            //Arrange
            Field expected = new Field("Field", "Some content", 2, "Default");
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
            assetRepository.Insert(asset);

            //Act
            Asset retrievedAsset = assetRepository.GetById(1);
            bool isDeleted = assetRepository.Delete(retrievedAsset);
            Asset result = assetRepository.GetById(1);

            //Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void Delete_AssetNotInDatabase_ReturnsFalse()
        {
            //Arrange

            //Act
            bool isDeleted = assetRepository.Delete(asset);

            //Assert
            Assert.IsFalse(isDeleted);
        }

        [TestMethod]
        public void SearchByTags_SearchForAssetsWithTag1_ReturnsAssetWithTag1()
        {
            //Arrange
            Tag tag = new Tag();
            tag.Name = "IntegrationTests";
            (new TagRepository()).Insert(tag);

            assetRepository.Insert(asset);
            assetRepository.AttachTagsToAsset(asset, new List<Tag> { tag });
            
            Asset expected = asset;

            //Act
            List<Asset> assetList = (List <Asset>)assetRepository.SearchByTags(new List<int>() { 1 });

            //Assert
            Assert.IsTrue(assetList[0].Equals(expected));
        }

        [TestMethod]
        public void SearchByTags_SearchForAssetsWithNonExistingTag_ReturnsEmptyListOfAsset()
        {
            //Arrange
            Tag tag = new Tag();
            tag.Name = "IntegrationTests";
            (new TagRepository()).Insert(tag);
            int expected = 0;

            //Act
            List<Asset> assetList = (List<Asset>) assetRepository.SearchByTags(new List<int>() { 6 });
            int result = assetList.Count;

            //Assert
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void Search_SearchForAssetsStartingWithInt_ReturnsListWithOneAsset()
        {
            //Arrange
            assetRepository.Insert(asset);
            string keyWord = "Int%";
            Asset expected = asset;

            //Act
            ObservableCollection<Asset> assetList = assetRepository.Search(keyWord);

            //Assert
            Assert.IsTrue(assetList[0].Equals(expected));
        }

        [TestMethod]
        public void Search_SearchForAssetsEndingWithTest_ReturnsListWithOneAsset()
        {
            //Arrange
            assetRepository.Insert(asset);
            string keyWord = "%test";
            Asset expected = asset;

            //Act
            ObservableCollection<Asset> assetList = assetRepository.Search(keyWord);

            //Assert
            Assert.IsTrue(assetList[0].Equals(expected));
        }

        [TestMethod]
        public void Search_SearchForAssetsContainingGration_ReturnsListWithOneAsset()
        {
            //Arrange
            assetRepository.Insert(asset);
            string keyWord = "gration";
            Asset expected = asset;

            //Act
            ObservableCollection<Asset> assetList = assetRepository.Search(keyWord);

            //Assert
            Assert.IsTrue(assetList[0].Equals(expected));
        }

        [TestMethod]
        public void Search_SearchForAssetsContainingUnittest_ReturnsEmptyList()
        {
            //Arrange
            assetRepository.Insert(asset);
            string keyWord = "Unittest";

            //Act
            ObservableCollection<Asset> assetList = assetRepository.Search(keyWord);

            //Assert
            Assert.IsTrue(assetList.Count == 0);
        }

        [TestMethod]
        public void AttachTagsToAsset_InputTwoTagsAndCallsGetAssetTags_ReturnsTheTagWithId2()
        {
            //Arrange
            TagRepository tagRepository = new TagRepository();

            tagRepository.Insert(new Tag() { Name = "IntegrationTests" });
            tagRepository.Insert(new Tag() { Name = "Tag 1", ParentID = 1 });

            List<Tag> listOfTags = (List<Tag>) tagRepository.GetAll();

            assetRepository.Insert(asset);

            Tag expected = tagRepository.GetById(2);

            //Act
            assetRepository.AttachTagsToAsset(asset, listOfTags);

            List<Tag> returnedTags = assetRepository.GetAssetTags(asset);

            Tag result = returnedTags[0];

            //Assert
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void AttachTagsToAsset_InputTagNotInDatabase_Returns_______()
        {
            //Arrange
            TagRepository tagRepository = new TagRepository();

            tagRepository.Insert(new Tag() { Name = "IntegrationTests" });
            tagRepository.Insert(new Tag() { Name = "Tag 1", ParentID = 1 });

            List<Tag> listOfTags = (List<Tag>)tagRepository.GetAll();

            assetRepository.Insert(asset);

            Asset expected = asset;

            //Act
            assetRepository.AttachTagsToAsset(asset, listOfTags);

            List<Asset> assetList = (List<Asset>)assetRepository.SearchByTags(new List<int>() { 2 });

            //Assert
            Assert.IsTrue(assetList[0].Equals(expected));
        }

        [TestCleanup]
        public void CleanDatabase()
        {
            //Set foreign key check to 0
            mySqlHandler.RawQuery("SET FOREIGN_KEY_CHECKS = 0");

            //Clear assets
            mySqlHandler.RawQuery("TRUNCATE TABLE assets");

            //Clear departments
            mySqlHandler.RawQuery("TRUNCATE TABLE departments");

            //Clear tags
            mySqlHandler.RawQuery("TRUNCATE TABLE tags");

            //Clear asset_tags
            mySqlHandler.RawQuery("TRUNCATE TABLE asset_tags");

            //Reset foreign key check
            mySqlHandler.RawQuery("SET FOREIGN_KEY_CHECKS = 1");
        }
    }
}