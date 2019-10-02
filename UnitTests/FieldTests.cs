using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using System.Collections.Generic;
using System;

namespace UnitTests
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void Field_GetInformation_ReturnsDictionaryWithInformation()
        {
            //Arrange
            string name = "Name", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 1;


            Field field = new Field(id, name, content, fieldType, defaultValue);
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { "Name", name },
                { "Description", content },
                { "Required", false.ToString() },
                { "FieldType", "TextBox" },
                { "DefaultValue", defaultValue }
            };

            //Act
            Dictionary<string, string> result = field.GetInformation();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Field_GetContent_ReturnsContentAsString()
        {
            //Arrange
            string name = "Name", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 1;


            Field field = new Field(id, name, content, fieldType, defaultValue);
            string expected = "Content";

            //Act
            string result = field.GetContent();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Field_UpdateContentReceivesString_UpdatesContentToInputString()
        {
            //Arrange
            string name = "Name", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 1;


            Field field = new Field(id, name, content, fieldType, defaultValue);
            string expected = "New content";

            //Act
            field.UpdateContent(expected);
            string result = field.GetContent();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Field_ConstructorReceivesFieldTypeOutOfRange_ThrowsOutOfRangeException()
        {
            //Arrange
            string name = "Name", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 20;

            string expectedMessage = "fieldType is out of range. Must be an integer between 1-5 (both included) (Parameter 'fieldType')";

            //Act
            try
            {
                Field field = new Field(id, name, content, fieldType, defaultValue);
                //Assert
                Assert.Fail();
            }
            catch(System.ArgumentOutOfRangeException e)
            {
                //Assert
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }
    }
}
