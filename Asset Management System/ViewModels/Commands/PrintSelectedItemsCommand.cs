using Asset_Management_System.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Helpers;
using System.Windows.Controls;

namespace Asset_Management_System.ViewModels.Commands
{
    class PrintSelectedItemsCommand : ICommand
    {
        private MainViewModel _main;
        public event EventHandler CanExecuteChanged;

        public PrintSelectedItemsCommand(MainViewModel main)
        {
            _main = main;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
                PrintHelper.Print(parameter as IEnumerable<object>);
            else
                _main.AddNotification(new Notification("Error! Cannot export nothing...", Notification.ERROR), 3000);
        }
    }
}
