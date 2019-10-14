using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for AssetTest.xaml
    /// </summary>
    public partial class AssetTest : Page
    {
        public AssetTest()
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetsViewModel();
        }
    }
}
