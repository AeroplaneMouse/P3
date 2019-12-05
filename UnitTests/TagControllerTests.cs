using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS.Database.Repositories;
using Moq;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Controllers;
using System.Linq;
using AMS.Controllers.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class TagControllerTests
    {
        private Mock<ITagRepository> _tagRepMock;
        private Mock<IDepartmentRepository> _depRepMock;
        private ITagController _tagController;
        
        [TestInitialize]
        public void InitializeTagControllerTest()
        {
            _tagRepMock = new Mock<ITagRepository>();
            _depRepMock = new Mock<IDepartmentRepository>();
            
            Tag testTag = new Tag();
            _tagController = new TagController(testTag, _tagRepMock.Object, _depRepMock.Object);
        }
        
        [TestMethod]
        public void Save_InjectedWithFakeRepository_TagSavedInFakeRepository()
        {
            //Arrange
            ulong id = 0;
            Tag tag = new Tag();

            _tagRepMock.Setup(repository => repository.Insert(It.IsAny<Tag>(), out id)).Returns(It.IsAny<Tag>());

            ITagController tagController = new TagController(tag, _tagRepMock.Object, _depRepMock.Object);
            tagController.ControlledTag = tag;

            //Act
            //tagController.tagRepository = mockRepository.Object;
            tagController.Save();

            //Assert
            _tagRepMock.Verify(p => p.Insert(It.IsAny<Tag>(),out id), Times.Once());
        }
        
        [TestMethod]
        public void Save_Returns_RepositoryInsertUsed()
        {
            //Arrange
            ulong id = 0;
            _tagRepMock.Setup(p => p.Insert(It.IsAny<Tag>(), out id));
            //Act
            _tagController.Save();

            //Assert
            _tagRepMock.Verify(p => p.Insert(It.IsAny<Tag>(), out id), Times.Once());
        }

        [TestMethod]
        public void Update_Returns_RepositoryUpdateUsed()
        {
            //Arrange

            //Act
            _tagController.Update();

            //Assert
            _tagRepMock.Verify(p => p.Update(It.IsAny<Tag>()), Times.Once());
        }
        
        [TestMethod]
        public void Remove_Returns_RepositoryDeleteUsed()
        {
            //Arrange

            //Act
            _tagController.Remove();

            //Assert
            _tagRepMock.Verify(p => p.Delete(It.IsAny<Tag>()), Times.Once());
        }

        [TestMethod]
        public void RemoveChildren_Returns_RepositoryDeleteChildrenUsed()
        {
            //Arrange

            //Act
            _tagController.RemoveChildren();

            //Assert
            _tagRepMock.Verify(p => p.DeleteChildren(It.IsAny<ulong>()), Times.Once);
        }

        //TODO: lav flere tests
    }
}
