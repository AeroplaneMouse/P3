using System;
using System.Windows;
using System.Windows.Controls;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for TopNavigation.xaml
    /// </summary>
    public partial class TopNavigation : Page
    {
        public event RoutedEventHandler ChangeSourceRequest;
        private readonly Session _currentSession;

        public TopNavigation(Session session)
        {
            if (session != null)
                _currentSession = session;
            else
                throw new ArgumentNullException();

            InitializeComponent();
            Lbl_currentUser.Content = $"{ (_currentSession.IsAdmin ? "Admin : " : "") }{ _currentSession.Username }";
        }
    }
}
