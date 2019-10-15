using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.Models
{
    public class Notification
    {
        private static int _id = 0;

        public readonly int ID;
        public string Message { get; set; }
        public SolidColorBrush Background { get; set; }
        public SolidColorBrush Foreground { get; set; }

        public Notification()
        {
            ID = _id++;

        }


    }
}
