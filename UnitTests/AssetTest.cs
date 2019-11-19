using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class AssetTest
    {
        
        [TestMethod]
        public void ToString_ReturnsNameOfAsset()
        {
            //Arrange
            Asset asset = new Asset();
            asset.Name = "AssetTests_Asset";
            string expected = "AssetTests_Asset";

            //Act
            string result = asset.ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}