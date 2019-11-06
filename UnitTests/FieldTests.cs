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
        public void Constructor_ReceivesFieldType20_ThrowsOutOfRangeException()
        {
            //Arrange
            string label = "Label", content = "Content", defaultValue = "Default Value";
            //int fieldType = 20;

            //TODO Delete or remove this test
            string expectedMessage = "fieldType is out of range. Must be an integer between 1-5 (both included) (Parameter 'fieldType')";

            //Act
            try
            {
                //Field field = new Field(label, content, fieldType, defaultValue);
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
        public void Constructor_ReceivesFieldType0_ThrowsOutOfRangeException()
        {
            //Arrange
            string label = "Label", content = "Content", defaultValue = "Default Value";
            //int fieldType = 0;
            //TODO Delete or remove this test
            string expectedMessage = "fieldType is out of range. Must be an integer between 1-5 (both included) (Parameter 'fieldType')";

            //Act
            try
            {
                //Field field = new Field(label, content, fieldType, defaultValue);
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
        public void GetInformation_ReturnsDictionaryWithInformation()
        {
            //Arrange
            string label = "Label", content = "Content", defaultValue = "Default Value";
            
            //TODO Edit or remove test

           Field field = new Field(label, content, Field.FieldType.Textarea, defaultValue);
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
        public void ContentGetter_ReturnsContentAsString()
        {
            //Arrange
            string name = "Label", content = "Content", defaultValue = "Default Value";
            //int fieldType = 1;


            Field field = new Field(name, content, Field.FieldType.Textarea, defaultValue);
            string expected = "Content";

            //Act
            string result = field.Content;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ContentSetter_ReceivesString_SetsContentToInputString()
        {
            //Arrange
            string name = "Label", content = "Content", defaultValue = "Default Value";
            int fieldType = 1;


            Field field = new Field(name, content, Field.FieldType.Textarea, defaultValue);
            string expected = "New content";

            //Act
            field.Content = expected;
            string result = field.Content;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetHashCode_ReturnsCorrectChecksumForField()
        {
            //Arrange
            Field field = new Field("Field", "Some content", Field.FieldType.Textbox, "Default");
            string expected = "ee42e2903edb29ca88a78f4aa413b8d6".ToUpper();

            //Act
            string result = field.Hash;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equals_ReceivesAnEqualField_ReturnsTrue()
        {
            //Arrange
            Field field = new Field("Field", "Some content", Field.FieldType.Textbox, "Default");
            Field otherField = new Field("Field", "Some content", Field.FieldType.Textbox, "Default");

            //Act
            bool result = field.Equals(otherField);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_ReceivesAnAsset_ReturnsFalse()
        {
            //Arrange
            Field field = new Field("Field", "Some content", Field.FieldType.Textbox, "Default");
            Asset asset = new Asset();

            //Act
            bool result = field.Equals(asset);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_ReceivesADifferentField_ReturnsFalse()
        {
            //Arrange
            Field field = new Field("Field", "Some content", Field.FieldType.Textbox, "Default");
            Field otherField = new Field("Different field", "Some different content", Field.FieldType.Textbox, "Different default");

            //Act
            bool result = field.Equals(otherField);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
