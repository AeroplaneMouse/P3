using AMS.Database.Repositories;
using AMS.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AMS.Views.ValueConverters
{
    public class AssetToTagListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO: Ingen new repositories!
            return new AssetRepository().GetTags(value as Asset).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
