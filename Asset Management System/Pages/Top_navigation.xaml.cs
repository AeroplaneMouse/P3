using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Asset_Management_System;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for Top_navigation.xaml
    /// </summary>
    public partial class Top_navigation : Page
    {
        public Top_navigation()
        {
            InitializeComponent();
            Session session = new Session();
            Lbl_currentUser.Content = $"{ (session.IsAdmin ? "Admin : ": "") }{ session.Username }";
        }
    }
}
