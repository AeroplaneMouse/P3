using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnitTests
{
    // TODO: UserIsInList, GetUsersFile, CombineLists

    [TestClass]
    public class UserImporterTests
    {
        private IUserImporter _userImporter { get; set; }

        Mock<IUserRepository> _userRep { get; set; }

        [TestInitialize]
        public void InitializeUserImporterTests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Mock<IUserRepository> rep = new Mock<IUserRepository>();

            rep.Setup(p => p.GetAll(true)).Returns(new List<User>());
            rep.Setup(p => p.GetByIdentity(It.IsAny<string>())).Returns(new User() { Username = "TestName", Domain = "TestDomain" });

            _userImporter = new UserImporter(rep.Object);
        }

        [TestMethod]
        public void ImportUsersFromFile_FileIsFormatted_UsersReturnedInList()
        {
            // Arrange
            string filePath = "userFileTest.txt";
            CreateFileAt(filePath, Encoding.GetEncoding(1252));

            // Act
            List<UserWithStatus> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2);

            // Cleanup
            DeleteFileAt(filePath);
        }

        [TestMethod]
        public void ImportUsersFromFile_FileIsNotFormattedNoTabs_ReturnEmptyList()
        {
            // Arrange
            string filePath = "userFileTest.txt";

            string fileContent = "Name;Type;Description\r\n" +
                                 "Hans Hansen;User;Han er bare for god\r\n" +
                                 "Åge Ågesen;User;Åge øser æsler\r\n";

            byte[] contentArray = Encoding.GetEncoding(1252).GetBytes(fileContent);

            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.Write(contentArray);
            }

            // Act
            List<UserWithStatus> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 0);

            // Cleanup
            DeleteFileAt(filePath);
        }

        [TestMethod]
        public void ImportUsersFromFile_FileDoesNotExist_ReturnEmptyList()
        {
            // Arrange

            // Act
            List<UserWithStatus> users = _userImporter.ImportUsersFromFile(string.Empty);

            // Assert
            Assert.IsTrue(users.Count() == 0);
        }

        [TestMethod]
        public void ImportUsersFromFile_EncodingIs1252_UsersReturnedInListWithSpecialCharacters()
        {
            // Arrange
            string filePath = "userFileTest.txt";
            CreateFileAt(filePath, Encoding.GetEncoding(1252));

            // Act
            List<UserWithStatus> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2 && users.Where(p => p.Username.ToLower().Contains('å')).Count() == 1);

            DeleteFileAt(filePath);
        }

        [TestMethod]
        public void ImportUsersFromFile_EncodingIsUTF8WithoutBOM_UsersReturnedInListWithSpecialCharacters()
        {
            // Arrange
            string filePath = "userFileTest.txt";
            CreateFileAt(filePath, new UTF8Encoding(false));

            // Act
            List<UserWithStatus> users = _userImporter.ImportUsersFromFile(filePath);

            // Assert
            Assert.IsTrue(users.Count() == 2 && users.Where(p => p.Username.ToLower().Contains('å')).Count() == 1);

            DeleteFileAt(filePath);
        }

        #region Helpers

        // Make test file
        void CreateFileAt(string filePath, Encoding encoding)
        {
            string fileContent = "Name\tType\tDescription\r\n" +
                                 "Hans Hansen\tUser\tHan er bare for god\r\n" +
                                 "Åge Ågesen\tUser\tÅge øser æsler\r\n";

            byte[] contentArray = encoding.GetBytes(fileContent);

            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.Write(contentArray);
            }
        }

        // Delete test file
        void DeleteFileAt(string filePath)
        {
            File.Delete(filePath);
        }

        #endregion
    }
}
