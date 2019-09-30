using System;
using System.Windows.Controls;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for TopNavigation.xaml
    /// </summary>
    public partial class TopNavigation : Page
    {
        public event EventHandler ChangeSourceRequest;

        public TopNavigation()
        {
            InitializeComponent();
            Session session = new Session();
            Lbl_currentUser.Content = $"{ (session.IsAdmin ? "Admin : " : "") }{ session.Username }";
        }
    }
}
