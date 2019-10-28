using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;
using System;

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
            //Converts the current DataContext to a TagManagerViewModel
            TagManagerViewModel viewModel = DataContext as TagManagerViewModel;

            //Checks if there was selected a parent tag prior to the selection
            if (args.RemovedItems.Count > 0)
            {
                //Checks if the old color correlates with the old parent tag
                Tag oldParentTag = (Tag)args.RemovedItems[0];
                if (viewModel.Color == oldParentTag.Color)
                {
                    //Changes the color to the color of the new parent tag
                    Tag newParentTag = viewModel.ParentTagsList[viewModel.SelectedParentIndex];
                    viewModel.Color = newParentTag.Color;
                }
            }
            else
            {
                //Changes the color to the color of the new parent tag
                Tag newParentTag = viewModel.ParentTagsList[viewModel.SelectedParentIndex];
                viewModel.Color = newParentTag.Color;
            }
        }
    }
}