using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Threading;
using Asset_Management_System.Views;
using Asset_Management_System.Events;
using System.Threading.Tasks;
using Asset_Management_System.Database;
using System.ComponentModel;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Page> pages = new List<Page>();
        public TopNavigationPart2 topNavigationPage;
        public LeftNavigation leftNavigationPage;

        public MainWindow()
        {
            InitializeComponent();
            ChangeMainContent(new SplashPage(this), FrameSplash);

            // Starting the new UI window.
            new Main().Show();
        }

        /// <summary>
        /// Displays a notification with a given background color and text.
        /// Then calls a remove method after a given amount of time, to 
        /// remove it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void ShowNotification(object sender, NotificationEventArgs e)
        {
            // TODO: If another notification is to be displayed before the last has disappeared. Make them stack.
            LbNotification.Content = e.Notification;
            CanvasNotificationBar.Background = e.Color;

            CanvasNotificationBar.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            HideNotification();
        }

        /// <summary>
        /// Removes a notification.
        /// </summary>
        private void HideNotification()
        {
            CanvasNotificationBar.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Used when the application has connected to the database and other external services,
        /// to remove the splash page and show the navigation menu's and homepage.
        /// </summary>
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

            //ChangeMainContent(new Home(this));
        }

        /// <summary>
        /// Changes how many rows or columns a specific frame spans over. 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="dir"></param>
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
        public void ChangeMainContent(Page newPage) => ChangeFrameContent(newPage, FrameMainContent);

        public void ChangeFrameContent(Page newPage, Frame frame)
        {
            Page setPage = null;
            // Search the loaded page list, for the given page to check if it has allready been loaded.
            foreach(Page page in pages)
            {
                if (page.GetType() == newPage.GetType()){
                    Console.WriteLine("Found new page in pages.");
                    setPage = page;
                }
            }

            // If the new page wasn't found in the list, the given newPage object is used and added to the list of pages.
            if (setPage == null)
            {
                Console.WriteLine("Unable to find new page in pages. Creating new page.");
                setPage = newPage;
                pages.Add(setPage);
            }

            // Setting the content of the given frame, to the newPage object to display the requested page.
            frame.Content = setPage;
        }
    }
}