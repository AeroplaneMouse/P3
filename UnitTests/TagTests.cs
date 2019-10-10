using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class TagTests
    {
        [TestMethod]
        public void Tag_ToString_ReturnsTagAsAString()
        {
            //Arrange
            Tag tag = new Tag("Tag_label", 1, 0);

            //Act
            string result = tag.ToString();

            //Assert
            Assert.AreEqual("Tag_label", result);
        }
    }
}
