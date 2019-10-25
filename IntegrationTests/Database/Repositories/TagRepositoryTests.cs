using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Database;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace IntegrationTests
{
    [TestClass]
    public class TagRepositoryTests
    {
        private DepartmentRepository departmentRepository;
        private TagRepository tagRepository;
        private Tag tag;
        private MySqlHandler mySqlHandler = new MySqlHandler();

        [ClassInitialize]
        public static void RemoveTableEntries(TestContext testContext)
        {
            MySqlHandler mySqlHandler = new MySqlHandler();

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

        [TestInitialize]
        public void SetUpTagAndRepository()
        {
            departmentRepository = new DepartmentRepository();
            departmentRepository.Insert(new Department { Name = "TagRepositoryTest"});

            tagRepository = new TagRepository();

            tag = new Tag("TagRepositoryTests - IntegrationTests", "Black", 1);
        }

        [TestMethod]
        public void Insert_ReceivesTag_ReturnsTrue()
        {
            //Act
            bool result = tagRepository.Insert(tag);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Insert_ReceivesTagAndReturnsTags_ReturnsOneTagWithSameName()
        {
            //Arrange
            tagRepository.Insert(tag);
            string expected = "TagRepositoryTests - IntegrationTests";

            //Act
            List<Tag> returnedTags = (List<Tag>)tagRepository.GetAll();

            string result = returnedTags[0].Name;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Update_()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Delete_()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void GetById_()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void GetParentTags_()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void GetChildTags_()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void Search_()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void GetAll_DatabaseHolds5Tags_ReturnsAll5Tags()
        {
            //Arrange
            List<Tag> tags = new List<Tag>();
            for (int i = 0; i < 5; i++)
            {
                tags.Add(new Tag("TagRepositoryTests " + i.ToString() + " - IntegrationTests", "Yellow", 1));
                tagRepository.Insert(tags[i]);
            }

            int expected = 5;

            //Act
            List<Tag> returnedTags = (List<Tag>)tagRepository.GetAll();
            int result = returnedTags.Count;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DBOToModelConvert_()
        {
            Assert.Fail();
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