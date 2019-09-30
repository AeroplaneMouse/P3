using System;
using System.Collections.Generic;
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
        //List<Page> pages = new List<Page>();

        Top_navigation topNavigationPage;
        Left_navigation leftNavigationPage;

        public MainWindow()
        {
            InitializeComponent();
            
            // Create pages
            // Hmm.. How should this shit work?!
            
            SplashPage page = new SplashPage();
            Frame_splash.Content = page;
            page.SessionAuthenticated += SystemLoaded;

            Assets.ChangeSourceRequest += ChangeSourceReguest;
        }

        public void SystemLoaded(object sender, EventArgs e)
        {
            // Remove splash page
            Frame_splash.Visibility = Visibility.Hidden;
            Frame_splash.Source = null;

            // Set stuff
            topNavigationPage = new Top_navigation();
            topNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            Frame_topNavigation.Content = topNavigationPage;

            leftNavigationPage = new Left_navigation();
            leftNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            Frame_leftNavigation.Content = leftNavigationPage;

            Frame_mainContent.Source = new Uri("Pages/Home.xaml", UriKind.Relative);
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
            //if (showingSplashPage && e.Key == Key.Escape)
            //    SystemLoaded();
        }
    }
}
