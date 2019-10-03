using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Threading;
using Asset_Management_System.Pages;
using Asset_Management_System.Events;
using System.Threading.Tasks;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Page> pages = new List<Page>();
        TopNavigationPart2 topNavigationPage;
        LeftNavigation leftNavigationPage;

        public MainWindow()
        {
            InitializeComponent();

            //pages.Add(new Home(this));



            // Create pages
            // Hmm.. How should this shit work?!

            //home.ShowNotification += ShowNotification;

            SplashPage page = new SplashPage();
            FrameSplash.Content = page;
            page.SessionAuthenticated += SystemLoaded;

            //Assets.ChangeSourceRequest += ChangeSourceReguest;
            //FrameTopNavigationPart2.op
        }

        public async void ShowNotification(object sender, NotificationEventArgs e)
        {
            LbNotification.Content = e.Notification;
            CanvasNotificationBar.Background = e.Color;

            CanvasNotificationBar.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            HideNotification();
        }

        private void HideNotification()
        {
            CanvasNotificationBar.Visibility = Visibility.Hidden;

        }

        public void SystemLoaded(object sender, EventArgs e)
        {
            // Remove splash page
            FrameSplash.Visibility = Visibility.Hidden;
            FrameSplash.Source = null;

            // Set stuff
            topNavigationPage = new TopNavigationPart2(this);
            topNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            topNavigationPage.ExpandFrameRequest += ChangeFrameMode;
            FrameTopNavigationPart2.Content = topNavigationPage;

            leftNavigationPage = new LeftNavigation(this);
            leftNavigationPage.ChangeSourceRequest += ChangeSourceReguest;
            FrameLeftNavigation.Content = leftNavigationPage;

            ChangeSourceReguest(this, new ChangeSourceEventArgs(new Home(this)));
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
            Page newPage = null;
            foreach(Page page in pages)
            {
                if (page.GetType() == e.NewSource.GetType()){
                    Console.WriteLine("Found new page in pages.");
                    newPage = page;
                }
            }

            if (newPage == null)
            {
                Console.WriteLine("Unable to find new page in pages. Creating new page.");
                newPage = e.NewSource;
                pages.Add(newPage);
            }
            FrameMainContent.Content = newPage;
        }
    }
}
