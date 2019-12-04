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
        
        [TestInitialize]
        public void InitializeTagControllerTest()
        {
            _tagRepMock = new Mock<ITagRepository>();
            _depRepMock = new Mock<IDepartmentRepository>();
        }
        
        [TestMethod]
        public void Save_InjectedWithFakeRepository_TagSavedInFakeRepository()
        {
            //Arrange
            ulong id = 0;
            Tag tag = new Tag();
            tag.Name = "TestTag";
            
            _tagRepMock.Setup(repository => repository.Insert(tag, out id)).Returns(It.IsAny<Tag>());

            ITagController tagController = new TagController(tag, _tagRepMock.Object, _depRepMock.Object);
            tagController.ControlledTag = tag;


            //Act
            //tagController.tagRepository = mockRepository.Object;
            tagController.Save();

            //Assert
            _tagRepMock.Verify(p => p.Insert(It.IsAny<Tag>(),out id), Times.Once());
        }

        //TODO: lav flere tests
    }
}
