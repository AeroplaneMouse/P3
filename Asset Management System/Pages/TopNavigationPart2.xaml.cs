using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Asset_Management_System.Authentication;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for TopNavigation.xaml
    /// </summary>
    public partial class TopNavigationPart2 : Page
    {
        public event RoutedEventHandler ChangeSourceRequest;

        public TopNavigationPart2()
        {
            InitializeComponent();
            Session session = new Session();
            Lbl_currentUser.Content = session.Username;
        }

        private void BtnShowDepartments_Click(object sender, RoutedEventArgs e)
        {
            if (LbDepartments.Visibility == Visibility.Hidden)
            {
                LbDepartments.Visibility = Visibility.Visible;
                LbDepartments.Background = StackPanelUpper.Background;
                List<string> testElements = new List<string>();
                testElements.Add("IT");
                testElements.Add("HR");
                testElements.Add("Finance");

                LbDepartments.ItemsSource = testElements;

            }
            else
            {
                LbDepartments.Visibility = Visibility.Hidden;
            }
        }
    }
}
