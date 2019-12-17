using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using AMS.Models;

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
