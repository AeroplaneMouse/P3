using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS.Database.Repositories;
using Moq;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Controllers;
using System.Linq;

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
            TagController tagController = new TagController();
            tagController.tag = tag;

            var mockRepository = new Mock<ITagRepository>();
            mockRepository.Setup(repository => repository.Insert(tag, out id)).Returns<Tag>(t => t.Equals(tag));

            //Act
            tagController.tagRepository = (ITagRepository)mockRepository;
            tagController.Save();

            //Assert

        }
    }
}
