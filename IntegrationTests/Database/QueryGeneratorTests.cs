using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Models;
using Asset_Management_System.Database;

namespace IntegrationTests
{
    [TestClass]
    public class QueryGeneratorTests
    {
        private MySqlHandler mySqlHandler;
        private DepartmentRepository departmentRepository;
        private Department department;
        private AssetRepository assetRepository;
        private Asset asset1;
        private Asset asset2;
        private Asset asset3;
        private TagRepository tagRepository;
        private Tag tag;

        [TestInitialize]
        public void InitializeTests()
        {
            asset1 = new Asset()
            {
                Name = "Asset #1",
                FieldsList = new List<Field>()
                {
                    new Field("Field 1", "Content 1", 1, "None"),
                    new Field("Field 2", "Content 2", 2, "None")
                }
            };
            asset2 = new Asset()
            {
                Name = "Asset #2",
                FieldsList = new List<Field>()
                {
                    new Field("Field 1", "Content 1", 1, "None"),
                    new Field("Field 2", "Content 2", 2, "None")
                }
            };
            asset3 = new Asset()
            {
                Name = "Asset #3",
                FieldsList = new List<Field>()
                {
                    new Field("Field 1", "Content 1", 1, "None"),
                    new Field("Field 2", "Content 2", 2, "None")
                }
            };

            assetRepository = new AssetRepository();

            assetRepository.Insert(asset1);
            assetRepository.Insert(asset2);
            assetRepository.Insert(asset3);

            department = new Department()
            {
                Name = "QueryGeneratorTestsDepartment"
            };

            departmentRepository = new DepartmentRepository();

            departmentRepository.Insert(department);

            tag = new Tag("QueryGeneratorTestsTag", "#FAFAFA", 1);

            tagRepository = new TagRepository();

            tagRepository.Insert(tag);
    }

        [TestMethod]
        public void PrepareUpdate_SetNameOfAsset3ToNewName_BuildsQueryString()
        {
            //Arrange
            QueryGenerator query = new QueryGenerator();
            query.AddTable("assets");
            query.Columns.AddRange(new[] { "name" });
            query.Values.AddRange(new[] { "NewName" });
            query.Where("name", "Asset #3");
            string expected = "UPDATE assets SET name = 'NewName' WHERE name = 'Asset #3'";

            //Act
            string result = query.PrepareUpdate();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void PrepareUpdate_SetLabelAndColorOfTag_BuildsQueryString()
        {
            //Arrange
            QueryGenerator query = new QueryGenerator();
            query.AddTable("tags");
            query.Columns.AddRange(new[] { "label", "color" });
            query.Values.AddRange(new[] { "NewLabel", "#666666" });
            query.Where("label", "QueryGeneratorTestsTag");
            string expected = "UPDATE tags SET label = 'NewLabel', color = '#666666' WHERE label = 'QueryGeneratorTestsTag'";

            //Act
            string result = query.PrepareUpdate();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void PrepareInsert_PrepareInsert_BuildsQueryString()
        {
            //Arrange
            QueryGenerator query = new QueryGenerator();
            query.AddTable("tags");
            query.Columns.AddRange(new[] { "label", "color", "parent_id", "department_id", "options" });
            query.Values.AddRange(new[] { "QueryGeneratorTestsTag", "#666666", "1", "20", "[]" });
            string expected = "INSERT INTO tags ( label, color, parent_id, department_id, options ) VALUES ( 'QueryGeneratorTestsTag', '#666666', 1, 20, '[]' )";

            //Act
            string result = query.PrepareInsert();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestCleanup]
        public void CleanupAfterTests()
        {
            mySqlHandler = new MySqlHandler();

            //Clear database tables
            mySqlHandler.RawQuery("SET FOREIGN_KEY_CHECKS = 0;" + "TRUNCATE TABLE assets;" + "TRUNCATE TABLE departments;" + "TRUNCATE TABLE tags;" + "TRUNCATE TABLE asset_tags;" + "SET FOREIGN_KEY_CHECKS = 1;");
        }
    }
}
