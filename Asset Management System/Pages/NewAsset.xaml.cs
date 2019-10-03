using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for NewAsset.xaml
    /// </summary>
    public partial class NewAsset : Page
    {
        private MainWindow Main;
        public NewAsset(MainWindow main)
        {
            InitializeComponent();
            Main = main;
        }

        private void BtnSaveNewAsset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string name = TbName.Text;
            string description = TbDescription.Text;

            Asset asset = new Asset();
            asset.Label = name;
            asset.Description = description;

            AssetRepository rep = new AssetRepository();
            rep.Insert(asset);
            Main.ChangeSourceRequest(new Assets(Main));
        }
    }
}
