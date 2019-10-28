using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class TagManager : Page
    {

        /// <summary>
        /// TagManager is called when creating, or editing a tag.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="inputTag">Optional input, only used when editing a tag.</param>
        public TagManager(MainViewModel main, Tag inputTag = null)
        {
            InitializeComponent();
            DataContext = new TagManagerViewModel(main, inputTag);
        }

        private void ParentTag_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            TagManagerViewModel viewModel = DataContext as TagManagerViewModel;

            if(args.RemovedItems.Count > 0)
            {
                Tag oldParentTag = (Tag)args.RemovedItems[0];
                if (viewModel.Color == oldParentTag.Color)
                {
                    Tag newParentTag = viewModel.ParentTagsList[viewModel.SelectedParentIndex];

                    System.Console.WriteLine(newParentTag.Color);

                    viewModel.Color = newParentTag.Color;
                }
            }
        }

        private string ToHexStr(int i)
        {
            string hex = i.ToString("X");
            if (hex.Length == 1)
                hex = $"0{ hex }";
            return hex;
        }
    }
}