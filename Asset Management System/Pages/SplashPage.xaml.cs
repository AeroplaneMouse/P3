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
        private MainWindow Main;
        public SplashPage(MainWindow main)
        {
            InitializeComponent();
            Main = main;

            // Call the authenticate method when all child elements of the page has been loaded.
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => Authenticate()));
        }

        private void Authenticate()
        {
            if (new Session().Validate())
                Main.SystemLoaded();
        }

        private void Btn_loadConfigs_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
