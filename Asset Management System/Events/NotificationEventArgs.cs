using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;

namespace Asset_Management_System.Events
{
    public delegate void NotificationEventHandler(object sender, NotificationEventArgs e);
    public class NotificationEventArgs
    {
        public NotificationEventArgs(string notification)
            : this(notification, Brushes.Red) { }

        public NotificationEventArgs(string notification, Brush color)
        {
            Notification = notification;
            Color = color;
        }

        public string Notification { get; set; }
        public Brush Color { get; set; }
    }
}
