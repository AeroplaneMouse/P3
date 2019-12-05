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
        private Tag _tagOne;
        private Tag _tagTwo;
        private Tag _tagThree;

        [TestInitialize]
        public void InitiateAssets()
        {
            // Change this line to change the class that is tested
            _tagRepMock = new Mock<ITagRepository>();
            _tagRepMock.Setup(p => p.Delete(It.IsAny<Tag>())).Returns(true);
            // This creates a new instance of the class for each test

            _tagListController = new TagListController(_tagRepMock.Object, new PrintHelper());

            _tagOne = CreateTestTagWithId(1, "TagOne");
            _tagTwo = CreateTestTagWithId(2, "TagTwo");
            _tagThree = CreateTestTagWithId(3, "TagThree");
            _tagOne.TagColor = "#f2f2f2f2";
            _tagTwo.TagColor = "#f2f2f2f2";
            _tagThree.TagColor = "#f2f2f2f2";

        }
        [TestMethod]
        public void TagListRemove_ElementInList_ReturnContains()
        {
            //Arrange
            _tagListController.TagsList = new List<Tag>();
            _tagListController.TagsList.Add(_tagOne);
            _tagListController.TagsList.Add(_tagTwo);
            _tagListController.TagsList.Add(_tagThree);

            //Act
            _tagListController.Remove(_tagThree);

            //Assert
            Assert.IsFalse(_tagListController.TagsList.Contains(_tagThree));
        }


        private Tag CreateTestTagWithId(ulong rowId, string rowLabel)
        {
            return (Tag)Activator.CreateInstance(typeof(Tag),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { rowId, rowLabel, null, null, null, null, null, null, null }, null, null);
        }
    }
}
