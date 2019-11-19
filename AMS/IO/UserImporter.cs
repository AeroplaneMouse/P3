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
using System.Security.Permissions;

namespace AMS.IO
{
    public class UserImporter : IUserImporter
    {
        #region Public Properties

        private IUserRepository _userRep { get; set; }

        #endregion

        #region Constructor

        public UserImporter(IUserRepository repository)
        {
            _userRep = repository;
        }

        #endregion

        #region Public Methods

        public List<UserWithStatus> CombineLists(List<UserWithStatus> imported, List<UserWithStatus> existing)
        {
            List<UserWithStatus> importedList = imported;
            List<UserWithStatus> existingList = existing;

            List<UserWithStatus> finalList = new List<UserWithStatus>();
            finalList.AddRange(existingList);
            finalList.AddRange(importedList);

            // Conflicting users. Existing users that are not enabled, whose username occures in both lists
            finalList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => UserIsInList(existingList.Where(p => p.IsEnabled == false).ToList(), u) && UserIsInList(importedList, u))
                .ToList()
                .ForEach(u =>
                {
                    u.Status = "Conflicting";
                    //u.IsShown = IsShowningConflicting;
                });

            // Added users. Users who are in the imported list, and not in the existing list
            finalList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => !UserIsInList(existingList.Where(p => p.IsEnabled == true).ToList(), u) && UserIsInList(importedList, u))
                .ToList()
                .ForEach(u =>
                {
                    u.Status = "Added";
                    //u.IsShown = IsShowingAdded;
                });

            // Removed users. Users that are enabled, and are only in the existing list
            finalList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => UserIsInList(existingList.Where(p => p.IsEnabled == true).ToList(), u) && !UserIsInList(importedList, u))
                .ToList()
                .ForEach(u =>
                {
                    u.Status = "Removed";
                    //u.IsShown = IsShowingRemoved;
                });

            // Kept users. Users that are enabled, and are in both lists. Remove the copy coming from the imported file
            finalList
                .Where(u => u.Status.CompareTo(String.Empty) == 0)
                .Where(u => UserIsInList(existingList.Where(p => p.IsEnabled == true).ToList(), u) && UserIsInList(importedList, u))
                .Where(u => u.ID == 0)
                .ToList()
                .ForEach(u => finalList.Remove(u));

            return finalList;
        }

        public List<User> ImportUsersFromDatabase()
        {
            return (_userRep.GetAll(true) ?? new List<User>()).ToList();
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
            var session = new Session(_userRep);

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
                return new List<User>();
            }
        }

        public bool UserIsInList(List<UserWithStatus> list, User user)
        {
            return list.Where(u => u.Username.CompareTo(user.Username) == 0).Count() > 0;
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
            //if (bom[0] == 0x4e && bom[1] == 0x61 && bom[2] == 0x6d && bom[3] == 0x65)
            //    return Encoding.UTF8;
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
