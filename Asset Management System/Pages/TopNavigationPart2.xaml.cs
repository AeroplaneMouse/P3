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
            if (SpDepartments.Visibility == Visibility.Hidden)
            {
                SpDepartments.Visibility = Visibility.Visible;
                SpDepartments.Background = StackPanelUpper.Background;
                List<string> testElements = new List<string>();
                testElements.Add("IT");
                testElements.Add("HR");
                testElements.Add("Finance");

                
                foreach (string str in testElements)
                {
                    MenuItem item = new MenuItem();
                    item.Header = str;
                    MenuDepartments.Items.Add(item);
                }
            }
            else
            {
                SpDepartments.Visibility = Visibility.Hidden;
            }
        }
    }
}
