using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using Microsoft.VisualBasic.CompilerServices;

namespace Asset_Management_System.Helpers
{
    public static class PrintHelper
    {
        public static void Print(IEnumerable<object> items)
        {
            Type objectType = items.FirstOrDefault().GetType();
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            string reportTitle = objectType.Name + "_report_";
            dlg.FileName = reportTitle + DateTime.Now.ToString("MM/dd/yy HH:mm:ss").Replace(":", "").Replace("/", "").Replace(" ", "-"); // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "CSV Files (*.csv)|*.csv"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string pathToFile = dlg.FileName;

                //Adds the ".csv" extension, if the name does not contain it
                if (!pathToFile.EndsWith(".csv"))
                {
                    pathToFile = pathToFile + ".csv";
                }

                //Writes the information to the file
                using (StreamWriter file = new StreamWriter(pathToFile, false))
                {
                    string fileHeader = "";
                    foreach (PropertyInfo prop in objectType.GetProperties())
                    {
                        // This condition applies only to assets, so the list of fields is not added to the file.
                        if (!prop.Name.Equals("FieldsList"))
                            if (prop.Name.Equals("DateToStringConverter"))
                                fileHeader += "Time,";
                            else
                                fileHeader += prop.Name + ",";
                    }
                    file.WriteLine(fileHeader);

                    foreach (var item in items)
                    {
                        PropertyInfo[] props = objectType.GetProperties();
                        string fileEntry = "";

                        foreach (PropertyInfo prop in props)
                        {
                            string key = prop.Name;
                            // Condition to exclude the property fieldslist, as it requires special formatting, and all the data is already contained in serializedFields
                            if (!prop.Name.Equals("FieldsList"))
                            {
                                fileEntry += objectType.GetProperty(key)?.GetValue(item, null)?.ToString().Split('.').Last().Replace(',', ' ') + ", ";
                            }
                        }
                        file.WriteLine(fileEntry);
                    }
                }
            }
        }
    }
}