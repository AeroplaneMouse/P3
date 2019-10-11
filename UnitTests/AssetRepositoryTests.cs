using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class AssetRepositoryTests
    {
        [TestMethod]
        public void AssetRepository_Insert_ReturnsTrueAsAssertIsInserted()
        {
            //Arrange
            Asset asset = new Asset();
            asset.Name = "Name";
            asset.Description = "Desription";
            asset.DepartmentID = 1;
            asset.FieldsList = new List<Field>();
            asset.FieldsList.Add(new Field(1, "label", "content", 2, "Default"));
            AssetRepository assetRepository = new AssetRepository();

            //Act
            bool result = assetRepository.Insert(asset);

            //Assert
            Assert.IsTrue(result);

        }
    }
}
