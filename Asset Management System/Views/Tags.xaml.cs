using System.Windows.Controls;
using Asset_Management_System.ViewModels;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : Page
    {
        public Tags(MainViewModel main, ITagService tagService)
        {
            InitializeComponent();
            DataContext = new ViewModels.TagsViewModel(main, tagService);
        }
    }
}