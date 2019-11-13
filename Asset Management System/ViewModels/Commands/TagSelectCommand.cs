using System;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    public class TagSelectCommand : ICommand
    {
        private MainViewModel _main;
        public event EventHandler CanExecuteChanged;

        public TagSelectCommand(MainViewModel main)
        {
            _main = main;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
               
            }
            catch (Exception e)
            {
                
            }
        }
    }
}