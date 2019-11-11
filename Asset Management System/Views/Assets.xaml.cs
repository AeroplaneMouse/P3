using System.Windows.Controls;
using Asset_Management_System.ViewModels;
using Asset_Management_System.Resources.DataModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Assets.xaml
    /// </summary>
    public partial class Assets : Page
    {
        public Assets(MainViewModel main)
        {
            InitializeComponent();

            DataContext = new ViewModels.AssetsViewModel(main, ListPageType.Asset);
        }
    }
}