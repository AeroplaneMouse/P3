using System;
using System.Collections.Generic;
using System.Reflection;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class TagListControllerTest
    {
        private ITagListController _tagListController;
        private IExporter _exporter;
        private ITagRepository _tagRepository;
        private Mock<ITagRepository> _tagRepMock;

        [TestInitialize]
        public void InitiateAssets()
        {
            // Change this line to change the class that is tested
            _tagRepMock = new Mock<ITagRepository>();
            _tagRepMock.Setup(p => p.Delete(It.IsAny<Tag>())).Returns(true);
            // This creates a new instance of the class for each test

            _tagListController = new TagListController(_tagRepMock.Object);

        }
        [TestMethod]
        public void TagListRemove_ElementInList_ReturnContains()
        {
            //Arrange
            Tag tagOne = CreateTestTagWithId(1, "TagOne");
            Tag tagTwo = CreateTestTagWithId(2, "TagTwo");
            Tag tagThree = CreateTestTagWithId(3, "TagThree");
            tagOne.TagColor = "#f2f2f2f2";
            tagTwo.TagColor = "#f2f2f2f2";
            tagThree.TagColor = "#f2f2f2f2";
            _tagListController.TagsList = new List<Tag> {tagOne, tagTwo, tagThree};

            //Act
            _tagListController.Remove(tagThree);

            //Assert
            Assert.IsFalse(_tagListController.TagsList.Contains(tagThree));
        }

        //[TestMethod]
        //public void Search_CallsRepositorySearch_ReturnTrue()
        //{
        //    //Arrange
        //    _tagRepMock.Setup(p => p.Search(It.IsAny<string>(), null, null, false, false)).Returns(new List<Tag>());
        //    //Act
        //    _tagListController.Search("");

        //    //Assert
        //    _tagRepMock.Verify(p => p.Search(It.IsAny<string>(), null, null, false, false), Times.AtLeastOnce());
        //}

        [TestMethod]
        public void GetTag_CallsRepositoryGetById_ReturnTrue()
        {
            //Arrange

            //Act
            _tagListController.GetTag(0);

            //Assert
            _tagRepMock.Verify(p => p.GetById(It.IsAny<ulong>()), Times.Once);
        }

        [TestMethod]
        public void GetParentTags_CallsRepositoryGetParentTags_ReturnTrue()
        {
            //Arrange

            //Act
            _tagListController.GetParentTags();

            //Assert
            _tagRepMock.Verify(p => p.GetParentTags(), Times.Once);
        }

        [TestMethod]
        public void GetChildTags_CallsRepositoryGetChildTags_ReturnTrue()
        {
            //Arrange

            //Act
            _tagListController.GetChildTags(0);

            //Assert
            _tagRepMock.Verify(p => p.GetChildTags(It.IsAny<ulong>()), Times.Once);
        }

        [TestMethod]
        public void GetTreeviewData_CallsRepositoryGetTreeViewDataList_ReturnTrue()
        {
            //Arrange

            //Act
            _tagListController.GetTreeviewData();

            //Assert
            _tagRepMock.Verify(p => p.GetTreeViewDataList(It.IsAny<string>()), Times.Once);
        }


        private Tag CreateTestTagWithId(ulong rowId, string rowLabel)
        {
            return (Tag)Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowLabel, null, null, null, null, null, null, null }, null, null);
        }
    }
}
