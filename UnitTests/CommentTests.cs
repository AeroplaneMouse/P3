using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMS.Models;
using System.Collections.ObjectModel;
using AMS.Controllers.Interfaces;
using AMS.Controllers;
using System.Linq;
using AMS.Authentication;
using Moq;
using AMS.Database.Repositories.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class CommentTests
    {
        ICommentController _controller;
        //ICommentRepository _commentRep;
        private Mock<ICommentRepository> _commentRepMock;
        private Mock<IUserRepository> _userRepMock;
        private Mock<Session> _sessionMock;


        [TestInitialize]
        public void InitializeCommentTest()
        {
            //_commentRep = new CommentRepository();
            _commentRepMock = new Mock<ICommentRepository>();
            _userRepMock = new Mock<IUserRepository>();
            _sessionMock = new Mock<Session>(_userRepMock.Object);
            _controller = new CommentController(_sessionMock.Object, _commentRepMock.Object);
        }

        [TestMethod]
        public void AddComment_CommentIsNotNull_CommentAddedToList()
        {
            // Arrange 
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
        public void AddComment_CallsRepositoryInsert_ReturnsTrue()
        {
            // Arrange 
            ulong id = 47;
            string content = "Test";
            ulong assetId = 1;
            _commentRepMock.Setup(p => p.Insert(It.IsAny<Comment>(), out id)).Returns(true);

            // Act
            ulong commentId = _controller.AddNewComment(content, assetId);

            // Assert
            _commentRepMock.Verify(p => p.Insert(It.IsAny<Comment>(), out id), Times.Once);
        }
        
        [TestMethod]
        public void AddComment_CallsRepositoryGetByAssetId_ReturnsTrue()
        {
            // Arrange 
            ulong id = 47;
            string content = "Test";
            ulong assetId = 1;
            _commentRepMock.Setup(p => p.GetByAssetId(It.IsAny<ulong>())).Returns(new ObservableCollection<Comment>());

            // Act
            ulong commentId = _controller.AddNewComment(content, assetId);

            // Assert
            _commentRepMock.Verify(p => p.GetByAssetId(It.IsAny<ulong>()), Times.Once);
        }

        [TestMethod]
        public void AddComment_CommentIsNull_CommentNotAddedToList()
        {
            // Arrange
            string content = null;
            ulong assetId = 2;
            
            ulong expected = 0;

            // Act
            ulong commentId = _controller.AddNewComment(content, assetId);

            // Assert
            Assert.AreEqual(expected, commentId);
        }

        [TestMethod]
        public void RemoveComment_CommentInList_CommentRemoved()
        {
            // Arrange
            string content = "Test";
            ulong assetId = 3;

            ulong id = _controller.AddNewComment(content, assetId);

            Comment comment = _controller.CommentList.SingleOrDefault(c => c.ID == id);

            // Act
            _controller.RemoveComment(comment, assetId);

            // Assert
            bool actual = _controller.CommentList.Where(c => c.ID == id).Count() == 0;

            Assert.IsTrue(actual);

        }
        
        [TestMethod]
        public void RemoveComment_CallsRepositoryDelete_ReturnsTrue()
        {
            // Arrange 
            ulong assetId = 1;
            Comment comment = new Comment();
            _commentRepMock.Setup(p => p.Delete(It.IsAny<Comment>())).Returns(true);

            // Act
            _controller.RemoveComment(comment, assetId);

            // Assert
            _commentRepMock.Verify(p => p.Delete(It.IsAny<Comment>()), Times.Once);
        }
        
        [TestMethod]
        public void RemoveComment_ListContainsRemovedAsset_ReturnsFalse()
        {
            // Arrange 
            ulong assetId = 1;
            Comment comment1 = new Comment {AssetID = assetId, Content = "Comment1"};
            Comment comment2 = new Comment {AssetID = assetId, Content = "Comment2"};
            Comment comment3 = new Comment {AssetID = assetId, Content = "Comment3"};
            _commentRepMock.Setup(p => p.Delete(It.IsAny<Comment>())).Returns(true);
            _commentRepMock.Setup(p => p.GetByAssetId(It.IsAny<ulong>())).Returns(new ObservableCollection<Comment> {comment1, comment2});
            _controller.CommentList = new ObservableCollection<Comment> {comment1, comment2, comment3};
            
            // Act
            _controller.RemoveComment(comment3, assetId);

            // Assert
            Assert.IsFalse(_controller.CommentList.Contains(comment3));
        }

        /*
        [TestMethod]
        public void RemoveComment_CommentNotInList_CommentNotRemoved()
        {
            // Arrange
            Comment comment = null;
            ulong _assetId = 3;


             _controller.CommentList = _commentRepMock.GetByAssetId(_assetId);
            int listCountBefore = _controller.CommentList.Count;
            // Act
            _controller.RemoveComment(comment, _assetId);

            // Assert
            bool actual = listCountBefore == _controller.CommentList.Count;

            Assert.IsFalse(actual);
        }
        */
    }
}
