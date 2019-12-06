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
        
        [TestMethod]
        public void AddTag_GivenValidTag_TagAddedToAppliedTags()
        {
            //Arrange
            ITagable tag = new Tag {Name = "TestTag"};
            
            //Act
            _tagHelper.AddTag(tag);
            var result = _tagHelper.GetAppliedTags();
            
            //Assert
            Assert.IsTrue(result.Contains(tag));
        }
        
        [TestMethod]
        public void RemoveTag_SingleTagInList_ListLengthIsZero()
        {
            //Arrange
            ITagable tag = new Tag {Name = "TestTag"};
            _tagHelper.SetAppliedTags(new ObservableCollection<ITagable>{tag});
            int expectedLength = 0;
            
            //Act
            _tagHelper.RemoveTag(tag);
            int actualLength = _tagHelper.GetAppliedTags().Count;
            
            //Assert
            Assert.AreEqual(expectedLength, actualLength);
        }
        
        [TestMethod]
        public void SetParent_GivenValidTag_SetsValueCorrectly()
        {
            //Arrange
            Tag tag = new Tag {Name = "TestTag"};
            
            //Act
            _tagHelper.SetParent(tag);
            Tag result = _tagHelper.GetParent();
            
            //Assert
            Assert.AreEqual(tag, result);
        }
        
    }
}