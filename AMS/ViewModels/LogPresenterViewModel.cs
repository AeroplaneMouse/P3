using AMS.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.ViewModels
{
    public class LogPresenterViewModel : Base.BaseViewModel
    {
        public LogEntry Entry { get; set; }


        public LogPresenterViewModel(LogEntry entry)
        {
            Entry = entry;
        }
    }
}
