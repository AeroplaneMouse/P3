using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class DepartmentTests
    {
        [TestMethod]
        public void ToString_ReturnsDepartmentName()
        {
            //Arrange
            Department department = new Department {Name = "UnitTestDepartment"};
            string expected = "UnitTestDepartment";

            //Act
            string result = department.ToString();

            //Assert
            Assert.AreEqual(expected, result);

        }
    }
}
