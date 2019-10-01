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

        TopNavigationPart2 topNavigationPage;
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
            topNavigationPage = new TopNavigationPart2();
            topNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            FrameTopNavigationPart2.Content = topNavigationPage;

            leftNavigationPage = new LeftNavigation();
            leftNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            FrameLeftNavigation.Content = leftNavigationPage;

            FrameMainContent.Content = new Home();
        }

        public void ChangeSourceReguest(Object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button btn)
            {
                switch (btn.Name)
                {
                    case "Btn_homePage":
                        FrameMainContent.Content = new Home();
                        break;
                    case "Btn_assetsPage":
                        FrameMainContent.Content = new Assets();
                        break;
                    case "Btn_tagsPage":
                        FrameMainContent.Content = new Tags();
                        break;
                    case "Btn_settingsPage":
                        FrameMainContent.Content = new Settings();
                        break;
                    case "Btn_helpPage":
                        FrameMainContent.Content = new Help();
                        break;
                    case "Btn_AddNewAsset":
                        FrameMainContent.Content = new NewAsset();
                        break;
                }
            }
        }
    }
}
