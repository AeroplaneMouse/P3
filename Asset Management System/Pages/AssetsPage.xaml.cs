using System.Windows;
using System.Windows.Controls;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for AssetsPage.xaml
    /// </summary>
    public partial class AssetsPage : Page
    {
        public static event RoutedEventHandler ChangeSourceRequest;

        public AssetsPage()
        {
            InitializeComponent();
        }

        private void Btn_AddNewAsset_Click(object sender, RoutedEventArgs e)
        {
            ChangeSourceRequest?.Invoke(this, e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load assets from database
        }
    }
}
