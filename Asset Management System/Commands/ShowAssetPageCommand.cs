using System;
using System.Windows.Input;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Commands
{
    internal class ShowAssetPageCommand : ICommand
    {
        private MainViewModel _main;

        public ShowAssetPageCommand(MainViewModel main)
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
            Console.WriteLine("Showing asset page.");
            _main.FrameMainContent.Navigate(new Views.Assets(_main));
        }

        #endregion

    }
}