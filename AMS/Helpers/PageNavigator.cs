using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Logging;
using AMS.Models;
using AMS.ViewModels;
using AMS.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AMS.Helpers
{
    public class PageNavigator
    {
        private Page _currentPage;

        public bool To(Page page)
        {
            if (Features.Main.ContentFrame.Navigate(page))
            {
                if (_currentPage == null)
                {
                    _currentPage = page;

                    return true;
                }

                if (CurrentPageIsPrimary())
                {
                    Features.Main.History.Clear();
                }

                (page.DataContext as IPageUpdateOnFocus).UpdateOnFocus();
                Features.Main.History.Push(_currentPage);
                _currentPage = page;

                return true;
            }

            return false;
        }

        public bool Back()
        {
            if (Features.Main.History.Count > 0)
            {
                _currentPage = Features.Main.History.Pop();

                if (CurrentPageIsPrimary())
                {
                    Features.Main.History.Clear();
                }

                (_currentPage.DataContext as IPageUpdateOnFocus).UpdateOnFocus();
                Features.Main.ContentFrame.Navigate(_currentPage);
                return true;
            }

            return false;
        }

        private bool CurrentPageIsPrimary()
        {
            return _currentPage.GetType() == typeof(Home) ||
                   _currentPage.GetType() == typeof(AssetList) ||
                   _currentPage.GetType() == typeof(TagList) ||
                   _currentPage.GetType() == typeof(UserList) ||
                   _currentPage.GetType() == typeof(LogList);
        }
    }
}
