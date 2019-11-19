using System.Windows;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            DataContext = new ViewModels.MainViewModel(this);
        }
    }
}
