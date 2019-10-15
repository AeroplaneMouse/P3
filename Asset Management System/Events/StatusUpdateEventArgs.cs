using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.Events
{
    public delegate void StatusUpdateEventHandler (object sender, StatusUpdateEventArgs e);

    public class StatusUpdateEventArgs
    {
        public string Message;
        public StatusUpdateEventArgs(string message)
        {
            Message = message;
        }

    }
}
