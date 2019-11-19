using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS.Models;
using System.Collections.Generic;
using AMS.Controllers.Interfaces;
using AMS.Controllers;
using AMS.Database.Repositories;
using System.Linq;
using Moq;
using AMS.Database.Repositories.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class CommentTests
    {
        ICommentController _controller;
        CommentRepository _commentRep;
        private Mock<ICommentRepository> _commentRepMock;


        [TestInitialize]
        public void InitializeCommentTest()
        {
            _controller = new CommentController();
            _commentRep = new CommentRepository();
            _commentRepMock = new Mock<ICommentRepository>();
        }

        [TestMethod]
        public void AddComment_CommentIsNotNull_CommentAddedToList()
        {
            // Arrage 
            ulong id = 47;
            string content = "Test";
            ulong assetId = 1;
            _commentRepMock.Setup(p => p.Insert(It.IsAny<Comment>(), out id));
            _commentRepMock.Setup(p => p.GetByAssetId(It.IsAny<ulong>()));

            // Act
            ulong commentId = _controller.AddNewComment(content, assetId);

            // Assert
            _commentRepMock.Verify(p => p.Insert(It.IsAny<Comment>(), out id), Times.Once);
            _commentRepMock.Verify(p => p.GetByAssetId(It.IsAny<ulong>()), Times.Once);
            Assert.AreEqual(id, commentId);
        }

        [TestMethod]
        public void AddComment_CommentIsNull_CommentNotAddedToList()
        {
            // Arrange
            string content = null;
            ulong assetId = 2;

            // Act
            ulong commentId = _controller.AddNewComment(content, assetId);

            // Assert
            Assert.Equals(commentId, 0);
        }

        [TestMethod]
        public void RemoveComment_CommentInList_CommentRemoved()
        {
            // Arrange
            string content = "Test";
            ulong assetId = 3;

            ulong id = _controller.AddNewComment(content, assetId);

            Comment comment = _controller.CommentList.Where(c => c.ID == id).SingleOrDefault();

            // Act
            _controller.RemoveComment(comment, assetId);

            // Assert
            bool actual = _controller.CommentList.Where(c => c.ID == id).Count() == 0;

            Assert.IsTrue(actual);

        }

        [TestMethod]
        public void RemoveComment_CommentNotInList_CommentNotRemoved()
        {
            // Arrange
            Comment comment = null;
            ulong _assetId = 3;


             _controller.CommentList = _commentRep.GetByAssetId(_assetId);
            int listCountBefore = _controller.CommentList.Count;
            // Act
            _controller.RemoveComment(comment, _assetId);

            // Assert
            bool actual = listCountBefore == _controller.CommentList.Count;

            Assert.IsFalse(actual);
        }
    }
}
