using Asset_Management_System.Models;

namespace Asset_Management_System.Events
{
    public delegate void NotificationEventHandler(Notification n);

    public delegate void SqlConnectionEventHandler();
}
