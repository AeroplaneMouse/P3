using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Asset_Management_System;
using Asset_Management_System.Authentication;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for TopNavigation.xaml
    /// </summary>
    public partial class TopNavigationPart2 : Page
    {
        public event RoutedEventHandler ChangeSourceRequest;
        public event ChangeFrameModeEventHandler ExpandFrameRequest;

        public TopNavigationPart2()
        {
            InitializeComponent();
            Session session = new Session();
            LblCurrentUser.Content = session.Username;
        }

        private void BtnShowDepartments_Click(object sender, RoutedEventArgs e)
        {
            if (LbDepartments.Visibility == Visibility.Hidden)
            {
                // Make the suggestion list visible
                LbDepartments.Visibility = Visibility.Visible;
                BtnShowDepartments.Background = Brushes.White;
                BtnShowDepartments.Foreground = Brushes.Black;

                // Expand the navigation frame
                ChangeFrameModeEventArgs args = new ChangeFrameModeEventArgs(ChangeFrameModeEventArgs.Extend, ChangeFrameModeEventArgs.Down);
                if (ExpandFrameRequest != null)
                    ExpandFrameRequest?.Invoke(this, args);

                // Fill suggestion list
                List<string> testElements = new List<string>();
                testElements.Add("IT");
                testElements.Add("HR");
                testElements.Add("Finance");

                LbDepartments.ItemsSource = testElements;
            }
            else
            {
                // Hide the suggestion list
                LbDepartments.Visibility = Visibility.Hidden;
                BtnShowDepartments.Background = Brushes.Transparent;
                BtnShowDepartments.Foreground = Brushes.White;

                // Collapse the navigation frame
                ChangeFrameModeEventArgs args = new ChangeFrameModeEventArgs(ChangeFrameModeEventArgs.Collapse, ChangeFrameModeEventArgs.Up);
                if (ExpandFrameRequest != null)
                    ExpandFrameRequest?.Invoke(this, args);
            }
        }
    }
}
