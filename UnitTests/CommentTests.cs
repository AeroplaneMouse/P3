using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;

namespace UnitTests
{
    [TestClass]
    public class CommentTests
    {
        [TestMethod]
        public void Comment_ToString_ReturnsTheCommentAsAString()
        {
            //Arrange
            Comment comment = new Comment("This is a comment");

            //Act
            string result = comment.ToString();

            //Assert
            Assert.AreEqual("This is a comment", result);
        }
    }
}
