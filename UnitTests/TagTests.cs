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
        public void Tag_ConstructorReceivedName_CheckTimeOfCreationIsCloserToTheTimeOfTheCheckThanHalfASecond()
        {
            //Arrange
            Tag tag = new Tag("TagName");
            DateTime expected = DateTime.Now;

            //Act
            DateTime result = tag.CreatedAt;

            //Assert
            Assert.IsTrue(Math.Abs((result - expected).TotalSeconds) < 0.5);
        }

        [TestMethod]
        public void Tag_RenameTagReceivedNewName_CheckOfTagIsNewName()
        {
            //Arrange
            Tag tag = new Tag("TagName");
            string expected = "NewName";

            //Act
            tag.RenameTag("NewName");
            string result = tag.Name;

            //Assert
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void Tag_RenameTagReceivedNull_NullReferenceExceptionWithMessageTagCannotBeRenamedToNull()
        {
            //Arrange
            Tag tag = new Tag("TagName");
            string expected = "Tag cannot be renamed to null";

            try
            {
                //Act
                tag.RenameTag(null);

                //Assert
                Assert.Fail();
            }
            catch (NullReferenceException e)
            {
                //Assert
                Assert.AreEqual(expected, e.Message);
            }
        }
    }
}
