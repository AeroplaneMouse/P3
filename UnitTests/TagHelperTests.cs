using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using AMS.Database.Repositories.Interfaces;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class TagHelperTests
    {

        private Mock<ITagRepository> _tagRepMock;
        private Mock<IUserRepository> _userRepMock;
        private TagHelper _tagHelper;
        
        [TestInitialize]
        public void InitiateTagHelper()
        {
            _tagRepMock = new Mock<ITagRepository>();
            _userRepMock = new Mock<IUserRepository>();

            _tagHelper = new TagHelper(_tagRepMock.Object, _userRepMock.Object);
        }

        [TestMethod]
        public void Reload_Returns_CallsTagRepositoryGetAll()
        {
            //Arrange
            
            //Act
            _tagHelper.Reload();
            //Assert
            _tagRepMock.Verify(p => p.GetAll(), Times.AtLeastOnce);
        }
        
        [TestMethod]
        public void Reload_Returns_CallsUserRepositoryGetAll()
        {
            //Arrange

            //Act
            _tagHelper.Reload();
            //Assert
            _userRepMock.Verify(p => p.GetAll(false), Times.AtLeastOnce);
        }
        
        [TestMethod]
        public void SetCurrentTags_GivenListOfTags_SetsAppliedTags()
        {
            //Arrange
            var expectedTags = new ObservableCollection<ITagable>
            {
                new Tag {Name = "TestTag"},
                new User{Username = "TestUser"}
            };
            
            //Act
            _tagHelper.SetAppliedTags(expectedTags);
            var actualTags = _tagHelper.GetAppliedTags(true);
                
            //Assert
            Assert.AreEqual(expectedTags, actualTags);
        }
    }
}