using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Asset_Management_System.Pages;
using Asset_Management_System.Events;

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
            //FrameTopNavigationPart2.op
        }

        public void SystemLoaded(object sender, EventArgs e)
        {
            // Remove splash page
            FrameSplash.Visibility = Visibility.Hidden;
            FrameSplash.Source = null;

            // Set stuff
            topNavigationPage = new TopNavigationPart2(FramePopup);
            topNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            topNavigationPage.ExpandFrameRequest += ChangeFrameMode;
            FrameTopNavigationPart2.Content = topNavigationPage;

            leftNavigationPage = new LeftNavigation();
            leftNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            FrameLeftNavigation.Content = leftNavigationPage;

            FrameMainContent.Content = new Home();
        }

        public void ChangeFrameMode(object sender, ChangeFrameModeEventArgs e)
        {
            if (sender is TopNavigationPart2 nav)
            {
                // If the frame is null, 
                if (e.Frame == null)
                    e.Frame = FrameTopNavigationPart2;

                ChangeFrameExpasion(e.Frame, e.Direction);
            }
        }

        public void ChangeFrameExpasion(Frame frame, string dir)
        {
            if (dir == ChangeFrameModeEventArgs.Right)
                Grid.SetColumnSpan(frame, 10);
            else if (dir == ChangeFrameModeEventArgs.Left)
                Grid.SetColumnSpan(frame, 1);
            else if (dir == ChangeFrameModeEventArgs.Down)
                Grid.SetRowSpan(frame, 10);
            else if (dir == ChangeFrameModeEventArgs.Up)
                Grid.SetRowSpan(frame, 1);
        }

        /// <summary>
        /// Changes the content of the main frame for content, to the new page object received through
        /// the changeSourceEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeSourceReguest(Object sender, ChangeSourceEventArgs e)
        {
            FrameMainContent.Content = e.NewSource;

            //if (e.OriginalSource is Button btn)
            //{
            //    FrameMainContent.Content = btn.Name switch
            //    {
            //        "Btn_homePage" => new Home(),
            //        "Btn_assetsPage" => new Assets(),
            //        "Btn_tagsPage" => new Tags(),
            //        "Btn_settingsPage" => new Settings(),
            //        "Btn_helpPage" => new Help(),
            //        "Btn_AddNewAsset" => new NewAsset(),
            //        _ => throw new ArgumentException($"Unknown routing. A change of source has been requested, but no "),
            //    };
            //}
        }
    }
}
