using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asset_Management_System.Models;

namespace UnitTests
{
    [TestClass]
    public class AssetTests
    {
        private Asset asset;

        [TestInitialize]
        public void InitiateAsset()
        {
            asset = new Asset();
            asset.Name = "AssetTests_Asset";
            asset.Description = "Desription";
            asset.DepartmentID = 1;
            asset.AddField(new Field(1, "Label of first field", "content of first field", 2, "Default value of first field"));
            asset.AddField(new Field(2, "Label of second field", "content of second field", 4, "Default value of second field"));
        }

        [TestMethod]
        public void ToString_ReturnsNameOfAsset()
        {
            //Arrange
            string expected = "AssetTests_Asset";

            //Act
            string result = asset.ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equals_ReceivesAnEqualAsset_ReturnsTrue()
        {
            //Arrange
            Asset otherAsset = new Asset();
            otherAsset.Name = "AssetTests_Asset";
            otherAsset.Description = "Desription";
            otherAsset.DepartmentID = 1;
            otherAsset.AddField(new Field(1, "Label of first field", "content of first field", 2, "Default value of first field"));
            otherAsset.AddField(new Field(2, "Label of second field", "content of second field", 4, "Default value of second field"));

            //Act
            bool result = asset.Equals(otherAsset);

            //Assert
            Assert.IsTrue(result);
        }

    }
}
