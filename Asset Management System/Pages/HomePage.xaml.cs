using System.Windows;
using System.Windows.Controls;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            AssetsPage newAssetsPage = new AssetsPage();
            this.NavigationService.Navigate(newAssetsPage);
        }
    }
}
