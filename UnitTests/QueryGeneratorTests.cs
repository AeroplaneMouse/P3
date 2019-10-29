using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Models;
using Asset_Management_System.Database;

namespace IntegrationTests
{
    [TestClass]
    public class QueryGeneratorTests
    {
        [TestMethod]
        public void PrepareUpdate_SetNameOfAsset3ToNewName_BuildsQueryString()
        {
            //Arrange
            QueryGenerator query = new QueryGenerator();
            query.AddTable("assets");
            query.Columns.AddRange(new[] { "name" });
            query.Values.AddRange(new[] { "NewName" });
            query.Where("name", "Asset #3");
            string expected = "UPDATE assets SET name = 'NewName' WHERE name = 'Asset #3'";

            //Act
            string result = query.PrepareUpdate();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void PrepareUpdate_SetLabelAndColorOfTag_BuildsQueryString()
        {
            //Arrange
            QueryGenerator query = new QueryGenerator();
            query.AddTable("tags");
            query.Columns.AddRange(new[] { "label", "color" });
            query.Values.AddRange(new[] { "NewLabel", "#666666" });
            query.Where("label", "QueryGeneratorTestsTag");
            string expected = "UPDATE tags SET label = 'NewLabel', color = '#666666' WHERE label = 'QueryGeneratorTestsTag'";

            //Act
            string result = query.PrepareUpdate();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void PrepareInsert_InsertNewTagIntoDatabase_BuildsQueryString()
        {
            //Arrange
            QueryGenerator query = new QueryGenerator();
            query.AddTable("tags");
            query.Columns.AddRange(new[] { "label", "color", "parent_id", "department_id", "options" });
            query.Values.AddRange(new[] { "QueryGeneratorTestsTag", "#666666", "1", "20", "[]" });
            string expected = "INSERT INTO tags ( label, color, parent_id, department_id, options ) VALUES ( 'QueryGeneratorTestsTag', '#666666', 1, 20, '[]' )";

            //Act
            string result = query.PrepareInsert();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void PrepareSelect_SelectLabelAndColorFromTagTable_BuildsQueryString()
        {
            //Arrange
            QueryGenerator query = new QueryGenerator();
            query.AddTable("tags");
            query.Columns.AddRange(new[] { "label", "color" });
            query.Where("id", "1");
            string expected = "SELECT label, color FROM tags WHERE id = 1";

            //Act
            string result = query.PrepareSelect();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void PrepareDelete_DeleteTagFromDatabase_BuildsQueryString()
        {
            //Arrange
            QueryGenerator query = new QueryGenerator();
            query.AddTable("tags");
            query.Where("id", "1");
            string expected = "DELETE FROM tags WHERE id = 1";

            //Act
            string result = query.PrepareDelete();

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
