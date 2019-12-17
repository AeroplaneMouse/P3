using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class TagTests
    {
        [TestMethod]
        public void ToString_ReturnsLabelOfTag()
        {
            //Arrange
            Tag tag = new Tag();
            tag.Name = "Tag_label";

            //Act
            string result = tag.ToString();

            //Assert
            Assert.AreEqual("tag_label", result);
        }
    }
}
