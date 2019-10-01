using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using System.Collections.Generic;

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
            Dictionary<string, string> expected = new Dictionary<string, string>();
            expected.Add("Name", name);
            expected.Add("Description", content);
            expected.Add("Required", false.ToString());
            expected.Add("FieldType", "TextBox");
            expected.Add("DefaultValue", defaultValue);

            //Act
            Dictionary<string, string> result = field.GetInformation();

            //Assert
            Assert.AreSame(expected, result);
        }
    }
}
