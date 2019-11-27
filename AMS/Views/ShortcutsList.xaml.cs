using AMS.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for ShortcutsList.xaml
    /// </summary>
    public partial class ShortcutsList : Page
    {
        public ShortcutsList()
        {
            InitializeComponent();
            DataContext = new ShortcutsListViewModel();
        }
    }
}
