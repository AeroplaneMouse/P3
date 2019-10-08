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
        public void Tag_TryingToGetCreatedAtAfterUsingConstructor_ReturnsTimeOfCreation()
        {
            //Arrange
            Tag tag = new Tag("TagName");

            //Act
            DateTime expected = new DateTime().AddSeconds(-0.01);
            DateTime result = tag.CreatedAt;

            //Assert
            Assert.IsTrue((result - expected).TotalSeconds < 1);
        }
    }
}
