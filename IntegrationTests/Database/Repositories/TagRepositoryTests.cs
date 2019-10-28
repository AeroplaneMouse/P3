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
        public void Update_ChangeNameOfTagInDbAndUpdateInDb_GetByIdReturnsTagWithNewName()
        {
            //Arrange
            tagRepository.Insert(tag);
            Tag fetchedOriginalTag = tagRepository.GetById(1);
            string expected = "This is the new name of the tag";

            //Act
            fetchedOriginalTag.Name = expected;
            tagRepository.Update(fetchedOriginalTag);
            Tag fetchedUpdatedTag = tagRepository.GetById(1);
            string result = fetchedUpdatedTag.Name;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Delete_()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void GetById_FetchTagWithId1FromDb_ReturnsTagWithCorrectNameColorAndParentId()
        {
            //Arrange
            tagRepository.Insert(tag);
            string expected = tag.Name + tag.Color + tag.ParentID.ToString();

            //Act
            Tag fetchedTag = tagRepository.GetById(1);
            string result = fetchedTag.Name + fetchedTag.Color + fetchedTag.ParentID.ToString();

            //Assert
            Assert.AreEqual(expected, result);
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
        public void RemoveTableEntriesAfterEachTest()
        {
            //Clear database tables
            mySqlHandler.RawQuery("SET FOREIGN_KEY_CHECKS = 0;" + "TRUNCATE TABLE assets;" + "TRUNCATE TABLE departments;" + "TRUNCATE TABLE tags;" + "TRUNCATE TABLE asset_tags;" + "SET FOREIGN_KEY_CHECKS = 1;");
        }
    }
}