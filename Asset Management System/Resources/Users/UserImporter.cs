using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.Resources.Users
{
    public class UserImporter
    {
        private string _filePath { get; set; }

        public UserImporter(string filePath)
        {
            _filePath = filePath;
        }

        public List<User> Import()
        {
            return Encoding.Default.GetString(File.ReadAllBytes(_filePath))
                .Split('\n')                // Splits file into lines, by newlines
                .ToList()
                .Select(p => p.Split('\t')) // Splits lines into sections, by tabs
                .Where(p => p.Count() > 1)  // Only gets lines with something in them
                .Select(p =>
                {
                    User u = new User();

                    u.Name = p[0];
                    u.Username = p[0];
                    u.IsEnabled = true;
                    u.IsAdmin = false;
                    u.Description = (p[2] != null) ? p[2] : String.Empty;

                    return u;
                })                          // Converts each string[] into a user
                .ToList();
        }
    }
}
