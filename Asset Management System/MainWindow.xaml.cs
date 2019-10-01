using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Asset_Management_System.Pages;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List<Page> pages = new List<Page>();

        TopNavigation topNavigationPage;
        LeftNavigation leftNavigationPage;

        public MainWindow()
        {
            InitializeComponent();
            
            // Create pages
            // Hmm.. How should this shit work?!
            
            SplashPage page = new SplashPage();
            FrameSplash.Content = page;
            page.SessionAuthenticated += SystemLoaded;

            Assets.ChangeSourceRequest += ChangeSourceReguest;
        }

        public void SystemLoaded(object sender, EventArgs e)
        {
            // Remove splash page
            FrameSplash.Visibility = Visibility.Hidden;
            FrameSplash.Source = null;

            // Set stuff
            topNavigationPage = new TopNavigation();
            topNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            FrameTopNavigation.Content = topNavigationPage;

            leftNavigationPage = new LeftNavigation();
            leftNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            FrameLeftNavigation.Content = leftNavigationPage;

            FrameMainContent.Source = new Uri("Pages/Home.xaml", UriKind.Relative);
        }

        public void ChangeSourceReguest(Object sender, EventArgs e)
        {
            Button b = (e as RoutedEventArgs).OriginalSource as Button;
            switch (b.Name)
            {
                case "Btn_homePage":
                    FrameMainContent.Source = new Uri("Pages/Home.xaml", UriKind.Relative);
                    break;
                case "Btn_assetsPage":
                    FrameMainContent.Source = new Uri("Pages/Assets.xaml", UriKind.Relative);
                    break;
                case "Btn_templatesPage":
                    FrameMainContent.Source = new Uri("Pages/Templates.xaml", UriKind.Relative);
                    break;
                case "Btn_tagsPage":
                    FrameMainContent.Source = new Uri("Pages/Tags.xaml", UriKind.Relative);
                    break;
                case "Btn_settingsPage":
                    FrameMainContent.Source = new Uri("Pages/Settings.xaml", UriKind.Relative);
                    break;
                case "Btn_helpPage":
                    FrameMainContent.Source = new Uri("Pages/Help.xaml", UriKind.Relative);
                    break;
                case "Btn_AddNewAsset":
                    FrameMainContent.Source = new Uri("Pages/NewAsset.xaml", UriKind.Relative);
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
