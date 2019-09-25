using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Left_navigation.ChangeSourceRequest += ChangeSourceReguest;
            Assets.ChangeSourceRequest += ChangeSourceReguest;
        }

        public void ChangeSourceReguest(Object sender, EventArgs e)
        {
            Button b = (e as RoutedEventArgs).OriginalSource as Button;
            switch (b.Name)
            {
                case "Btn_homePage":
                    Frame_mainContent.Source = new Uri("Home.xaml", UriKind.Relative);
                    break;
                case "Btn_assetsPage":
                    Frame_mainContent.Source = new Uri("Assets.xaml", UriKind.Relative);
                    break;
                case "Btn_AddNewAsset":
                    Frame_mainContent.Source = new Uri("NewAsset.xaml", UriKind.Relative);
                    break;
            }
        }
    }
}
