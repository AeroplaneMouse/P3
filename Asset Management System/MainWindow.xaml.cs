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

            AssetsPage.ChangeSourceRequest += ChangeSourceReguest;
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

            Button b = new Button();
            b.Name = "Btn_homePage";
            ChangeSourceReguest(b, null);
            //FrameMainContent.Source = new Uri("Pages/Home.xaml", UriKind.Relative);
        }

        public void ChangeSourceReguest(Object sender, EventArgs e)
        {
            //(e as RoutedEventArgs).OriginalSource as Button
            Button b = sender as Button;
            switch (b.Name)
            {
                case "Btn_homePage":
                    FrameMainContent.Content = new HomePage();
                    break;
                case "Btn_assetsPage":
                    FrameMainContent.Content = new AssetsPage();
                    break;
                case "Btn_templatesPage":
                    FrameMainContent.Content = new TemplatesPage();
                    break;
                case "Btn_tagsPage":
                    FrameMainContent.Content = new TagsPage();
                    break;
                case "Btn_settingsPage":
                    FrameMainContent.Content = new SettingsPage();
                    break;
                case "Btn_helpPage":
                    FrameMainContent.Content = new HelpPage();
                    break;
                case "Btn_AddNewAsset":
                    FrameMainContent.Content = new NewAssetPage();
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
