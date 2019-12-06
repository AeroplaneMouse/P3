using AMS.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AMS.ViewModels
{
    public class LogPresenterViewModel : Base.BaseViewModel
    {
        public LogEntry Entry { get; set; }
        public ICommand CancelCommand { get; set; }

        public LogPresenterViewModel(LogEntry entry)
        {
            Entry = entry;

            CancelCommand = new Base.RelayCommand(() => Features.Navigate.Back());
        }

        public override void UpdateOnFocus() { }
    }
}
