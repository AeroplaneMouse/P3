using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for SplashPage.xaml
    /// </summary>
    public partial class SplashPage : Page
    {
        public event EventHandler SessionAuthenticated;
        private Session _currentSession;

        public SplashPage(Session session)
        {
            if (session != null)
                _currentSession = session;
            else
                throw new ArgumentNullException();

            InitializeComponent();

            // Call the authenticate method when all child elements of the page has been loaded.
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => Authenticate()));
        }

        /// <summary>
        /// Asks the current running session if the current user is authorised to use the program,
        /// and if so, tells the main window to display the main menu's.
        /// </summary>
        private void Authenticate()
        {
            if (_currentSession.Validate())
                SessionAuthenticated?.Invoke(this, new EventArgs());
        }

        private void Btn_loadConfigs_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
