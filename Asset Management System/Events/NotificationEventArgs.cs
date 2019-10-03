using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.Events
{
    public delegate void NotificationEventHandler(object sender, string notification);
    public class NotificationEventArgs
    {
        public NotificationEventArgs(string notification)
        {

        }

    }
}
