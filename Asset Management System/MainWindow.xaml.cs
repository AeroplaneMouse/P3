using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _showingSplashPage = true;

        public MainWindow()
        {
            InitializeComponent();
            Left_navigation.ChangeSourceRequest += ChangeSourceReguest;
            Assets.ChangeSourceRequest += ChangeSourceReguest;
        }

        public void SystemLoaded()
        {
            // Remove splash page
            Frame_splash.Visibility = Visibility.Hidden;
            Frame_splash.Source = null;
            _showingSplashPage = false;
        }

        public void ChangeSourceReguest(Object sender, EventArgs e)
        {
            Button b = (e as RoutedEventArgs).OriginalSource as Button;
            switch (b.Name)
            {
                case "Btn_homePage":
                    Frame_mainContent.Source = new Uri("Pages/Home.xaml", UriKind.Relative);
                    break;
                case "Btn_assetsPage":
                    Frame_mainContent.Source = new Uri("Pages/Assets.xaml", UriKind.Relative);
                    break;
                case "Btn_templatesPage":
                    Frame_mainContent.Source = new Uri("Pages/Templates.xaml", UriKind.Relative);
                    break;
                case "Btn_tagsPage":
                    Frame_mainContent.Source = new Uri("Pages/Tags.xaml", UriKind.Relative);
                    break;
                case "Btn_settingsPage":
                    Frame_mainContent.Source = new Uri("Pages/Settings.xaml", UriKind.Relative);
                    break;
                case "Btn_helpPage":
                    Frame_mainContent.Source = new Uri("Pages/Help.xaml", UriKind.Relative);
                    break;
                case "Btn_AddNewAsset":
                    Frame_mainContent.Source = new Uri("Pages/NewAsset.xaml", UriKind.Relative);
                    break;
            }
        }
        
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_showingSplashPage && e.Key == Key.Escape)
                SystemLoaded();
        }

        private void Frame_splash_LoadCompleted(object sender, NavigationEventArgs e)
        {
            SystemLoaded();
        }
    }
}
