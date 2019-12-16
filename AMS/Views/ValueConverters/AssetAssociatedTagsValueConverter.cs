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
            const int MaxTags = 5;
            List<Tag> tags = new List<Tag>();

            Asset asset = value as Asset;

            if (asset != null)
            {
                // Spilt string into tag:color elements
                string[] usersAndColor = asset.AssociatedUsers.Split(':');
                string color = usersAndColor[0];

                string[] users = usersAndColor[1].Split(',');

                foreach(string userLabel in users)
                {
                    // Add tag to list
                    tags.Add(new Tag() { Color = color, FullTagLabel = $"{ char.ConvertFromUtf32(0x1f464) } { userLabel }" });
                    
                    // Stop at x tags
                    if (tags.Count > MaxTags - 1)
                        break;
                }

                // Add other normal tags if there is space
                if (tags.Count < MaxTags)
                {

                    string[] tagsWithColors = asset.AssociatedTags.Remove(0,1).Split(',');

                    foreach (string tagStr in tagsWithColors)
                    {
                        string[] elements = tagStr.Split(':');
                        color = elements[0];
                        string label = elements[1].Replace("->", $"{ char.ConvertFromUtf32(0x202F) }{ char.ConvertFromUtf32(0x1f852) }{ char.ConvertFromUtf32(0x202F) }");

                        // Add tag to list
                        tags.Add(new Tag() { Color = color, FullTagLabel = label });

                         // Stop at x tags
                        if (tags.Count > MaxTags - 1)
                            break;

                    }
                }
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
