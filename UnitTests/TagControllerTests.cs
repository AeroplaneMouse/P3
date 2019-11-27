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
        [TestMethod]
        public void Save_InjectedWithFakeRepository_TagSavedInFakeRepository()
        {
            //Arrange
            ulong id = 0;
            Tag tag = new Tag();

            var mockRepository = new Mock<ITagRepository>();
            mockRepository.Setup(repository => repository.Insert(tag, out id)).Returns(true);

            ITagController tagController = new TagController(tag, mockRepository.Object, new DepartmentRepository());
            tagController.Tag = tag;


            //Act
            //tagController.tagRepository = mockRepository.Object;
            tagController.Save();

            //Assert
            mockRepository.Verify(p => p.Insert(It.IsAny<Tag>(),out id), Times.Once());
        }

        //TODO: lav flere tests
    }
}
