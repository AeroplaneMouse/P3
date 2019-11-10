using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Asset_Management_System.Authentication;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.Resources.Users
{
    public class UserImporter
    {
        private string _filePath { get; set; }

        public UserImporter()
        {
            
        }

        public List<User> Import()
        {
            var n = new Microsoft.Win32.OpenFileDialog();

            var session = new Session();

            Nullable<bool> result = n.ShowDialog();

            string filePath = "";

            if (result == false)
            {
                return new List<User>();
            }

            filePath = n.FileName;

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
    }

    
}
