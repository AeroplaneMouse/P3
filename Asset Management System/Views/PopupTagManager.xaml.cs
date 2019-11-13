using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for PopupTagManager.xaml
    /// </summary>
    public partial class PopupTagManager : Window
    {
        public PopupTagManager(MainViewModel main, Tag inputTag)
        {
            InitializeComponent();
            DataContext = new PopupTagManagerViewModel(main, inputTag, this);
        }
    }
}
