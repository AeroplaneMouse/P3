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
            Tag tag = new Tag("AssetRepositoryTests - IntegrationTests", "Black", 1);
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
            Tag tag = tag = new Tag("TagRepositoryTests - IntegrationTests", "Black", 1);
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
        public void AttachTagsToAsset_TwoTagsAndAssetInDbToBeAttachedCallsGetAssetTags_Returns2Tags()
        {
            //Arrange
            TagRepository tagRepository = new TagRepository();

            tagRepository.Insert(new Tag("Parent-TagRepositoryTests - IntegrationTests", "Black", 1));
            tagRepository.Insert(new Tag("TagRepositoryTests - IntegrationTests", "Blue", 1) { ParentID = 1 });

            List<Tag> listOfTags = (List<Tag>) tagRepository.GetAll();

            assetRepository.Insert(asset);

            int expected = 2;

            //Act
            Asset assetToAttachTagsTo = assetRepository.GetById(1);

            assetRepository.AttachTagsToAsset(assetToAttachTagsTo, listOfTags);

            List<Tag> returnedTags = assetRepository.GetAssetTags(asset);

            int result = returnedTags.Count;

            //Assert
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void AttachTagsToAsset_InputTagNotInDatabase_ReturnsFalse()
        {
            //Arrange
            TagRepository tagRepository = new TagRepository();
            tagRepository.Insert(new Tag("Parent-TagRepositoryTests - IntegrationTests", "Black", 1));
            tagRepository.Insert(new Tag("TagRepositoryTests - IntegrationTests", "Blue", 1) { ParentID = 1 });
            List<Tag> listOfTags = (List<Tag>)tagRepository.GetAll();

            listOfTags.Add(new Tag("TagRepositoryTestsNotInDb - IntegrationTests", "Blue", 1));

            assetRepository.Insert(asset);

            //Act
            bool result = assetRepository.AttachTagsToAsset(asset, listOfTags);

            //Assert
            Assert.IsFalse(result);
        }

        [TestCleanup]
        public void CleanDatabase()
        {
            //Clear database tables
            mySqlHandler.RawQuery("SET FOREIGN_KEY_CHECKS = 0;" + "TRUNCATE TABLE assets;" + "TRUNCATE TABLE departments;" + "TRUNCATE TABLE tags;" + "TRUNCATE TABLE asset_tags;" + "SET FOREIGN_KEY_CHECKS = 1;");
        }
    }
}