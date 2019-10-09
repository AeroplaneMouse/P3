using Asset_Management_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.Commands
{
    internal class ShowHomePageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        readonly MainViewModel _main;

        public ShowHomePageCommand(MainViewModel main)
        {
            _main = main;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Console.WriteLine("Showing home page.");
            //_main.FrameMainContent.Content = new Views.HomeTest();
            _main.FrameMainContent.Navigate(new Views.HomeTest());
        }
    }
}
