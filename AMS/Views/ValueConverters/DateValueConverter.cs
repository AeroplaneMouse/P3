using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace AMS.Views.ValueConverters
{
    public class DateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Expected input format:
            // dd-mmm-yy h:mm:ss AM
            // dd-mmm-yy hh:mm:ss AM
            // 07-Nov-19 1:54:23 PM
            // 07-Nov-19 11:54:23 AM
            string input = value.ToString();
            string output;

            try
            {
                // Retrieving string date
                string date = input.Substring(0, 2);
                string month = input.Substring(3, 3);
                string year = "20" + input.Substring(7, 2);

                // Removing date
                input = input.Remove(0, 10);

                // Retrieving time
                string[] time = input.Split(':', ' ');

                int h = int.Parse(time[0]);
                int m = int.Parse(time[1]);
                //string s = time[2]; Ignore seconds
                bool isPM = time[3] == "PM";

                // Convert from AM/PM to the correct time format
                if (isPM)
                    h += 12;

                // Setting formated output
                output = $"{date} {month} {year} - "
                    + $"{ (h < 10 ? $"0{h}" : h.ToString()) }"
                    + ":"
                    + $"{ (m < 10 ? $"0{m}" : m.ToString()) }";
            }
            catch (Exception e) when(
                e is IndexOutOfRangeException ||
                e is FormatException ||
                e is ArgumentNullException ||
                e is ArgumentOutOfRangeException ||
                e is OverflowException)
            {
                output = "Format failed";
            }

            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
