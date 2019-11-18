using AMS.Authentication;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AMS.IO
{
    public class UserImporter : IUserImporter
    {
        #region Public Properties

        public IUserService UserService { get; set; }

        public IUserRepository UserRepository { get; set; }

        #endregion

        #region Constructor

        public UserImporter(IUserService service)
        {
            UserService = service;
        }

        #endregion

        #region Public Methods

        public List<UserWithStatus> CombineLists(List<User> imported, List<User> existing)
        {
            return new List<UserWithStatus>();
        }

        public List<User> ImportUsersFromDatabase()
        {
            return new List<User>();
        }

        public string GetUsersFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = dialog.ShowDialog();

            string filePath = String.Empty;

            if (result == false)
            {
                return filePath;
            }

            return dialog.FileName;
        }

        public List<User> ImportUsersFromFile(string filePath)
        {
            var session = new Session();

            if (!String.IsNullOrEmpty(filePath))
            {
                return GetEncoding(filePath).GetString(File.ReadAllBytes(filePath))
                .Split("\r\n")                  // Splits file into lines, by newlines
                .Select(p => p.Split('\t'))     // Splits lines into sections, by tabs
                .Where(p => p.Count() > 1)      // Only gets lines with something in them
                .Where(p => p[0].CompareTo("Name") != 0 &&
                            p[1].CompareTo("Type") != 0 &&
                            p[2].CompareTo("Description") != 0) // Don't use the first line of the file
                .Select(p =>
                {
                    User u = new User();

                    u.Username = p[0];
                    u.Domain = session.Domain;
                    u.IsEnabled = true;
                    u.IsAdmin = false;
                    u.Description = (p[2] != null) ? p[2] : String.Empty;

                    return u;
                })  // Converts each string[] into a user
                .ToList();
            }

            else
            {
                return null;
            }
        }

        public bool IsInList(List<User> list, User user)
        {
            return true;
        }


        #endregion

        #region Private Methods

        // Borrowed from https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding, answer 1
        // Checks the byte order mark of the file, and return the encoding that it should be read with. 
        // ANSI is set to code page 1252 by default here
        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;

            return Encoding.GetEncoding(1252);
        }

        #endregion
    }
}
