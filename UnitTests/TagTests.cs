using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;

namespace UnitTests
{
    [TestClass]
    public class TagTests
    {
        [TestMethod]
        public void Tag_ToString_ReturnsLabelOfTag()
        {
            //Arrange
            Tag tag = new Tag();
            tag.Name = "Tag_label";

            //Act
            string result = tag.ToString();

            //Assert
            Assert.AreEqual("Tag_label", result);
        }
    }
}
