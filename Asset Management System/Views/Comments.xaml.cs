using System.Windows.Controls;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Comments.xaml
    /// </summary>
    public partial class Comments : Page
    {
        public Comments(MainViewModel main)
        {
            InitializeComponent();
            this.DataContext = new CommentsViewModel(main);
        }
    }
}
