using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void Field_ConstructorReceivesFieldType20_ThrowsOutOfRangeException()
        {
            //Arrange
            string label = "Label", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 20;

            string expectedMessage = "fieldType is out of range. Must be an integer between 1-5 (both included) (Parameter 'fieldType')";

            //Act
            try
            {
                Field field = new Field(id, label, content, fieldType, defaultValue);
                //Assert
                Assert.Fail();
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                //Assert
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }

        [TestMethod]
        public void Field_ConstructorReceivesFieldType0_ThrowsOutOfRangeException()
        {
            //Arrange
            string label = "Label", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 0;

            string expectedMessage = "fieldType is out of range. Must be an integer between 1-5 (both included) (Parameter 'fieldType')";

            //Act
            try
            {
                Field field = new Field(id, label, content, fieldType, defaultValue);
                //Assert
                Assert.Fail();
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                //Assert
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }

        [TestMethod]
        public void Field_GetInformation_ReturnsDictionaryWithInformation()
        {
            //Arrange
            string label = "Label", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 1;


            Field field = new Field(id, label, content, fieldType, defaultValue);
            Dictionary<string, string> expected = new Dictionary<string, string>
            {
                { "Label", label },
                { "Description", content },
                { "Required", false.ToString() },
                { "FieldType", 1.ToString() },
                { "DefaultValue", defaultValue }
            };

            //Act
            Dictionary<string, string> result = field.GetInformation();

            //Assert
            Assert.AreEqual(ToAssertableString(expected), ToAssertableString(result));

            string ToAssertableString(IDictionary<string, string> dictionary)
            {
                var pairStrings = dictionary.OrderBy(p => p.Key)
                                            .Select(p => p.Key + ": " + string.Join(", ", p.Value));
                return string.Join("; ", pairStrings);
            }
        }

        [TestMethod]
        public void Field_ContentGetter_ReturnsContentAsString()
        {
            //Arrange
            string name = "Label", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 1;


            Field field = new Field(id, name, content, fieldType, defaultValue);
            string expected = "Content";

            //Act
            string result = field.Content;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Field_ContentSetterReceivesString_SetsContentToInputString()
        {
            //Arrange
            string name = "Label", content = "Content", defaultValue = "Default Value";
            int id = 1, fieldType = 1;


            Field field = new Field(id, name, content, fieldType, defaultValue);
            string expected = "New content";

            //Act
            field.Content = expected;
            string result = field.Content;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Field_Checksum_ReturnsCorrectChecksumForField()
        {
            //Arrange
            Field field = new Field(1, "Field", "Some content", 2, "Default");
            string expected = "EE42E2903EDB29CA88A78F4AA413B8D6";

            //Act
            string result = field.GetChecksum();

            //Assert
            Assert.AreEqual(expected, result);
        }

    }
}
