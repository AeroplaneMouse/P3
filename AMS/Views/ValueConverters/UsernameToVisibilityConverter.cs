using AMS.Authentication;
using AMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace AMS.Views.ValueConverters
{
    public class UsernameToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string user = value as string;

            if (Session.GetUsername().CompareTo(user) == 0 || Features.GetCurrentSession().IsAdmin())
            {
                return Visibility.Visible;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
