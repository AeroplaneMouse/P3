using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.Events
{
    public delegate void StatusUpdateEventHandler (StatusUpdateEventArgs e);

    public class StatusUpdateEventArgs
    {
        public string Message;
        public string extraMessage;

        public StatusUpdateEventArgs()
            : this("") {}

        public StatusUpdateEventArgs(string message)
            : this(message, null) { }
        
        public StatusUpdateEventArgs(string message, string extra)
        {
            Message = message;
            extraMessage = extra;
        }

    }
}
