using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AMS.Interfaces;
using Microsoft.Win32;

namespace AMS.Helpers
{
    public class PrintHelper : IExporter
    {
        // A list of the properties that should not be exported 
        private List<string> _excludedProperties = new List<string> {{"FieldsList"}, {"CreatedAtString"}, {"UpdatedAtString"}, {"Changes"}};
        public void Print(IEnumerable<object> items)
        {
            Type objectType = items.FirstOrDefault().GetType();
            SaveFileDialog dlg = CreateFileDialog(objectType);

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result != true) return;
            
            string pathToFile = dlg.FileName;

            //Adds the ".csv" extension, if the name does not contain it
            if (!pathToFile.EndsWith(".csv"))
                pathToFile += ".csv";
            
            // Creates the file, and writes information to it
            WriteToFile(pathToFile, items, objectType);
        }

        /// <summary>
        /// Creates a fileDialog with values based on given objectType and current time
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private SaveFileDialog CreateFileDialog(Type objectType)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            string reportTitle = objectType.Name + "_report_";
            dlg.FileName = reportTitle + DateTime.Now.ToString("u")
                               .TrimEnd('Z')
                               .Replace(":", "")
                               .Replace("/", "")
                               .Replace(" ", "-"); // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "CSV Files (*.csv)|*.csv"; // Filter files by extension

            return dlg;
        }

        /// <summary>
        /// Writes the information about the items to file with given path
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <param name="items"></param>
        /// <param name="objectType"></param>
        private void WriteToFile(string pathToFile, IEnumerable<object> items, Type objectType)
        {
            
            //Writes the information to the file
            using (StreamWriter file = new StreamWriter(pathToFile, false))
            {
                // Write the column headers to the file
                file.WriteLine(CreateFileHeader(objectType));

                // Foreach object to be exported
                foreach (var item in items)
                {
                    // Get the properties of the object
                    PropertyInfo[] props = objectType.GetProperties();
                    string fileEntry = "";

                    // Foreach property in the object
                    foreach (PropertyInfo prop in props)
                    {
                        string key = prop.Name;
                        // Condition to exclude the property fieldslist, as it requires special formatting, and all the data is already contained in serializedFields
                        if (!_excludedProperties.Contains(key))
                            if (key.Equals("CreatedAt") || key.Equals("UpdatedAt")) // Special formatting for CreatedAt & UpdatedAt
                                fileEntry +=
                                    (objectType.GetProperty(key)?.GetValue(item, null) is DateTime
                                        ? (DateTime) objectType.GetProperty(key)?.GetValue(item, null)
                                        : default)
                                    .ToString("u")
                                    .TrimEnd('Z')
                                    + ", ";
                            else
                                fileEntry += objectType.GetProperty(key)
                                             ?.GetValue(item, null)
                                             ?.ToString()
                                             .Split('.')
                                             .Last()
                                             .Replace(',', ' ') + ", ";
                        
                    }
                    file.WriteLine(fileEntry);
                }
            }
        }

        /// <summary>
        /// Creates a string containing the column headers
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns>String with column headers</returns>
        private string CreateFileHeader(Type objectType)
        {
            string fileHeader = "";
            foreach (PropertyInfo prop in objectType.GetProperties())
            {
                // This condition applies only to assets, so the list of fields is not added to the file.
                if (!_excludedProperties.Contains(prop.Name))
                    if (prop.Name.Equals("CreatedAt") || prop.Name.Equals("UpdatedAt"))
                        fileHeader += prop.Name.TrimEnd("At".ToCharArray()) + ",";
                    else if (prop.Name.Equals("DateToStringConverter"))
                        fileHeader += "Time,";
                    else
                        fileHeader += prop.Name + ",";
            }

            return fileHeader;
        }
    }
}