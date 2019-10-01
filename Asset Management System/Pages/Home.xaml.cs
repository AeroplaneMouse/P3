using System.Windows;
using System.Windows.Controls;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            Assets newAssetsPage = new Assets();
            this.NavigationService.Navigate(newAssetsPage);
        }

        private void BtnShowDepartments_Click(object sender, RoutedEventArgs e)
        {



            //LbDepartments.ItemsSource = 
        }
    }
}
