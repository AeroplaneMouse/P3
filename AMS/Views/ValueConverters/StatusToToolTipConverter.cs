using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace AMS.Views
{
    public class StatusToToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;

            if (status.CompareTo(String.Empty) == 0)
                return "Active user. Click to disable";

            else if (status.CompareTo("Disabled") == 0)
                return "Inactive user. Click to enable";

            else if (status.CompareTo("Conflicting") == 0)
                return "Another user has the same username. Click to keep this user";

            else if (status.CompareTo("Added") == 0)
                return "New user";

            else if (status.CompareTo("Removed") == 0)
                return "Removed user";

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
