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
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            /*IEnumerable<object> objects = (IEnumerable<object>)parameter;

            foreach (object item in objects)
            {
                Console.WriteLine(item.ToString());
            }*/
            PrintHelper.Print(parameter as IEnumerable<object>);
        }
    }
}
