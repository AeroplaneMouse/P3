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
    class QueryGeneratorTests
    {
        private MySqlHandler mySqlHandler;
        private DepartmentRepository departmentRepository;
        private Department department;
        private AssetRepository assetRepository;
        private Asset asset1;
        private Asset asset2;
        private Asset asset3;

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
            asset2 = new Asset()
            {
                Name = "Asset #3",
                FieldsList = new List<Field>()
                {
                    new Field("Field 1", "Content 1", 1, "None"),
                    new Field("Field 2", "Content 2", 2, "None")
                }
            };

            assetRepository.Insert(asset1);
            assetRepository.Insert(asset2);
            assetRepository.Insert(asset3);

            department = new Department()
            {
                Name = "QueryGeneratorTestsDepartment"
            };

            departmentRepository = new DepartmentRepository();
            departmentRepository.Insert(department);

    }

        [TestMethod]
        public void PrepareUpdate_Receives3Columns3ValuesAndATable_BuildsExpectedQueryString()
        {

        }

        [TestCleanup]
        public void CleanupAfterTests()
        {
            //Clear database tables
            mySqlHandler.RawQuery("SET FOREIGN_KEY_CHECKS = 0;" + "TRUNCATE TABLE assets;" + "TRUNCATE TABLE departments;" + "TRUNCATE TABLE tags;" + "TRUNCATE TABLE asset_tags;" + "SET FOREIGN_KEY_CHECKS = 1;");
        }
    }
}
