using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : Page
    {
        public Tags()
        {
            InitializeComponent();

            TagRepository rep = new TagRepository();
            
        }
    }
}
