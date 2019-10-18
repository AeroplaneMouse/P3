using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.Events
{
    public delegate void StatusUpdateEventHandler (StatusUpdateEventArgs e);

    public class StatusUpdateEventArgs
    {
        public string Title;
        public string Message;
        public string extraMessage;

        public StatusUpdateEventArgs(string titleMessage, string message)
            : this(titleMessage, message, null) { }
        
        public StatusUpdateEventArgs(string titleMessage, string message, string extra)
        {
            Title = titleMessage;
            Message = message;
            extraMessage = extra;
        }

    }
}
