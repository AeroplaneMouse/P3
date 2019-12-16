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
        private List<string> _excludedProperties = new List<string> {{"FieldsList"}, {"CreatedAtString"}, {"UpdatedAtString"}, {"Changes"}, {"DateToStringConverter"} };

        /// <summary>
        /// Creates a fileDialog, and then writes the given items to the resulting file using Exporter.
        /// </summary>
        /// <param name="items"></param>
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
            Exporter exporter = new Exporter(new StreamWriter(pathToFile, false));
            exporter.WriteToFile(items, objectType);
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
    }
}