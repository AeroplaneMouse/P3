using System.Windows.Controls;
using AMS.ViewModels;
using AMS.Views;

namespace AMS.Helpers.Features
{
    // Navigation
    public class Navigate
    {
        private MainViewModel _main;

        public Navigate(MainViewModel main)
        {
            _main = main;
        }
        
        public bool To(Page page)
        {
            if (_main.ContentFrame.Navigate(page))
            {
                _main.History.Push(page);
                return true;
            }
            else
                return false;
        }

        public bool Back()
        {
            if (_main.History.Count > 0)
            {
                _main.History.Pop();
                _main.ContentFrame.Navigate(_main.History.Pop());
                return true;
            }
            else
                return false;
        }
    }
}