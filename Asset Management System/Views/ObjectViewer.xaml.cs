using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    public partial class ObjectViewer : Page
    {
        public ObjectViewer(MainViewModel main, ICommentService commentService, DoesContainFields inputObject)
        {
            InitializeComponent();
            DataContext = new ObjectViewerViewModel(main, commentService, inputObject);
        }
    }
}