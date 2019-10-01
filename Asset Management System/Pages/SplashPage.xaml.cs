using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Asset_Management_System.Authentication;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for SplashPage.xaml
    /// </summary>
    public partial class SplashPage : Page
    {
        public event EventHandler SessionAuthenticated;

        public SplashPage()
        {
            InitializeComponent();

            // Call the authenticate method when all child elements of the page has been loaded.
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => Authenticate()));
        }

        private void Authenticate()
        {
            Session currentSession = new Session();
            if (currentSession.Validate())
                SessionAuthenticated?.Invoke(this, null);
        }

        private void Btn_loadConfigs_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
