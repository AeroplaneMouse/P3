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
using Asset_Management_System.Database;

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
            DBConnection.Instance().SqlConnectionFailed += ShowNotification;
            ChangeSourceRequest(new SplashPage(this), FrameSplash);
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

        public void SystemLoaded()
        {
            // Remove splash page
            FrameSplash.Visibility = Visibility.Hidden;
            FrameSplash.Source = null;

            // Set stuff
            topNavigationPage = new TopNavigationPart2(this);
            leftNavigationPage = new LeftNavigation(this);
            FrameTopNavigationPart2.Content = topNavigationPage;
            FrameLeftNavigation.Content = leftNavigationPage;

            ChangeSourceRequest(new Home(this));
        }

        //public void ChangeFrameMode(object sender, ChangeFrameModeEventArgs e)
        //{
        //    if (sender is TopNavigationPart2 nav)
        //    {
        //        // If the frame is null, 
        //        if (e.Frame == null)
        //            e.Frame = FrameTopNavigationPart2;

        //        ChangeFrameExpasion(e.Frame, e.Direction);
        //    }
        //}

        public void ChangeFrameExpasion(Frame frame, string dir)
        {
            if (dir == FrameExpansionEventArgs.Right)
                Grid.SetColumnSpan(frame, 10);
            else if (dir == FrameExpansionEventArgs.Left)
                Grid.SetColumnSpan(frame, 1);
            else if (dir == FrameExpansionEventArgs.Down)
                Grid.SetRowSpan(frame, 10);
            else if (dir == FrameExpansionEventArgs.Up)
                Grid.SetRowSpan(frame, 1);
        }

        /// <summary>
        /// Changes the content for the main content frame to the new page. If the page exists in the
        /// list of loaded pages, that one would be used. One can also specify a different frame of 
        /// which content will be modified to contain the new page.
        /// </summary>
        public void ChangeSourceRequest(Page newPage) => ChangeSourceRequest(newPage, FrameMainContent);
        public void ChangeSourceRequest(Page newPage, Frame frame)
        {
            Page setPage = null;
            foreach(Page page in pages)
            {
                if (page.GetType() == newPage.GetType()){
                    Console.WriteLine("Found new page in pages.");
                    setPage = page;
                }
            }

            if (setPage == null)
            {
                Console.WriteLine("Unable to find new page in pages. Creating new page.");
                setPage = newPage;
                pages.Add(setPage);
            }
            frame.Content = setPage;
        }
    }
}
