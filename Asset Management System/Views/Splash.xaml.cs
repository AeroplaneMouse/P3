using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Asset_Management_System.Authentication;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for SplashPage.xaml
    /// </summary>
    public partial class Splash : Page
    {
        public Splash(MainViewModel main)
        {
            InitializeComponent();
            DataContext = new ViewModels.SplashViewModel(main);
        }
    }
}
