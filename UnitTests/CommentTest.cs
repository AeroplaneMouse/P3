using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;

namespace UnitTests
{
    [TestClass]
    public class CommentTest
    {

        private readonly Comment _comment;

        [TestMethod]
        public void Comment_ToString_ReturnsTheCommentAsAString()
        {
            //Arrange
            Comment _comment = new Comment("This is a comment");

            //Act
            string result = _comment.ToString();

            //Assert
            Assert.AreEqual("This is a comment", result);
        }
    }
}
