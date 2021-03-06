﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using AMS.Models;

namespace UnitTests
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void ContentGetter_ReturnsContentAsString()
        {
            //Arrange
            string name = "Label", content = "Content", defaultValue = "Default Value";
            //int fieldType = 1;


            Field field = new Field(name, content, Field.FieldType.Textarea );
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


            Field field = new Field(name, content, Field.FieldType.Textarea);
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
            Field field = new Field("Field", "Some content", Field.FieldType.TextBox);
            string expected = "A372CC6EDC6253D4805CB9D58133217C".ToUpper();

            //Act
            string result = field.Hash;

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equals_ReceivesAnEqualField_ReturnsTrue()
        {
            //Arrange
            Field field = new Field("Field", "Some content", Field.FieldType.TextBox);
            Field otherField = new Field("Field", "Some content", Field.FieldType.TextBox);

            //Act
            bool result = field.Equals(otherField);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_ReceivesAnAsset_ReturnsFalse()
        {
            //Arrange
            Field field = new Field("Field", "Some content", Field.FieldType.TextBox);
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
            Field field = new Field("Field", "Some content", Field.FieldType.TextBox);
            //Sleep added as the hashID for fields, depend on the current time in milliseconds,
            //and it is impossible to create to fields within the same millisecond within the software.
            Thread.Sleep(1);
            Field otherField = new Field("Different field", "Some different content", Field.FieldType.TextBox);

            //Act
            bool result = field.Equals(otherField);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
