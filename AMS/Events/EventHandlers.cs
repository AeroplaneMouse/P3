using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Models;

namespace AMS.Events
{
    public delegate void NotificationEventHandler(Notification n);

    public delegate void SqlConnectionEventHandler();
}
