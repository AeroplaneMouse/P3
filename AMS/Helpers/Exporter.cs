using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AMS.Interfaces;

namespace AMS.Helpers
{
    public class Exporter 
    {
        // Names of properties that should not be exported
        private readonly List<string> _excludedProperties = new List<string> {{"FieldsList"}, {"CreatedAtString"}, {"UpdatedAtString"}, {"Changes"}, {"DateToStringConverter"} };
        private readonly StreamWriter _streamWriter;
        public Exporter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        /// <summary>
        /// Writes the information about the given items to the stream
        /// </summary>
        /// <param name="items"></param>
        /// <param name="objectType"></param>
        public void WriteToFile(IEnumerable<object> items, Type objectType)
        {
            //Writes the information to the file
            using (_streamWriter)
            {
                // Write the column headers to the file
                _streamWriter.WriteLine(CreateFileHeader(objectType));

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
                                    + ",";
                            else
                                fileEntry += objectType.GetProperty(key)
                                             ?.GetValue(item, null)
                                             ?.ToString()
                                             .Split('.')
                                             .Last()
                                             .Replace(',', ' ') + ",";
                        
                    }
                    fileEntry = fileEntry.TrimEnd(',');
                    _streamWriter.WriteLine(fileEntry);
                }
            }
        }
        
        /// <summary>
        /// Creates a string containing the column headers consisting of the names of the properties
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
                    else
                        fileHeader += prop.Name + ",";
            }

            fileHeader = fileHeader.TrimEnd(',');
            return fileHeader;
        }
    }
}