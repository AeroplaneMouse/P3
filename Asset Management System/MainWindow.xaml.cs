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
        private readonly Session currentSession = new Session();

        TopNavigation topNavigationPage;
        LeftNavigation leftNavigationPage;

        public MainWindow()
        {
            InitializeComponent();
            
            // Create pages
            // Hmm.. How should this shit work?!
            
            SplashPage page = new SplashPage(currentSession);
            page.SessionAuthenticated += SystemLoaded;
            FrameSplash.Content = page;

            AssetsPage.ChangeSourceRequest += ChangeSourceReguest;
        }

        /// <summary>
        /// Unloades the splash page, and sets the menu windows up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SystemLoaded(object sender, EventArgs e)
        {
            // Removing the splash page
            FrameSplash.Content = null;

            // Setting up top navigation menu
            topNavigationPage = new TopNavigation(currentSession);
            topNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            FrameTopNavigation.Content = topNavigationPage;

            // Setting up left navigation menu
            leftNavigationPage = new LeftNavigation(currentSession);
            leftNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            FrameLeftNavigation.Content = leftNavigationPage;

            // Requesting the home page to be viewed in the main content frame
            Button tempButton = new Button() { Name = "Btn_homePage" };
            ChangeSourceReguest(null, new RoutedEventArgs() { Source = tempButton });
            //FrameMainContent.Source = new Uri("Pages/Home.xaml", UriKind.Relative);
        }

        /// <summary>
        /// Changes the main content frame's content depending on the name of source button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeSourceReguest(object sender, RoutedEventArgs e)
        {
            //(e as RoutedEventArgs).OriginalSource as Button
            if(e.Source is Button btn)
            {
                switch (btn.Name)
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
            else
            {
                Console.WriteLine("An unknown button has been pressed.");
            }
        }
        
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (showingSplashPage && e.Key == Key.Escape)
            //    SystemLoaded();
        }
    }
}
