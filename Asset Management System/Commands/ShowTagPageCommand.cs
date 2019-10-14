using Asset_Management_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.Commands
{
    class ShowTagPageCommand : ICommand
    {
        private MainViewModel _main;

        public ShowTagPageCommand(MainViewModel main)
        {
            _main = main;
        }

        #region ICommand

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Console.WriteLine("Showing tag page.");
            _main.FrameMainContent.Navigate(new Views.TagTest());
        }

        #endregion
    }
}
