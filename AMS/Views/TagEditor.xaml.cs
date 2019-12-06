using System.Windows.Controls;
using AMS.Controllers.Interfaces;
using AMS.Models;
using AMS.ViewModels;

namespace AMS.Views
{
    public partial class TagEditor : Page
    {
        public TagEditor(ITagController controller)
        {
            InitializeComponent();
            DataContext = new TagEditorViewModel(controller);
        }
    }
}