using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Asset_Management_System.ViewModels
{
    public class PopupTagManagerViewModel : Base.BaseViewModel
    {
        public Frame Frame { get; set; }

        public PopupTagManagerViewModel(MainViewModel main, Tag inputTag, PopupTagManager popup)
        {
            Frame = new Frame();
            Frame.Content = new TagManager(main, inputTag, popup);
        }
    }
}
