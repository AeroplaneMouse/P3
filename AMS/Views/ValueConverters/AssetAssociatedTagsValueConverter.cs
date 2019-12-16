using AMS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace AMS.Views.ValueConverters
{
    class AssetAssociatedTagsValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Stopwatch stopwatch = new Stopwatch();

            List<Tag> tags = new List<Tag>();

            Asset asset = value as Asset;

            if (asset != null)
            {
                //stopwatch.Start();
                // Spilt string into tag:color elements
                string[] tagsWithColors = asset.AssociatedTags.Split(',');

                foreach(string tagStr in tagsWithColors)
                {
                    // Spilt color and label
                    string[] elements = tagStr.Split(':');
                    string label = elements[0];
                    string color = elements[1];

                    // Add tag to list
                    tags.Add(new Tag() { Color = color, FullTagLabel = label });
                    
                    // Stop at x tags
                    if (tags.Count > 4)
                        break;
                }
                //stopwatch.Stop();
            }

            //Console.WriteLine($"Time: { stopwatch.ElapsedTicks }");

            return tags;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
