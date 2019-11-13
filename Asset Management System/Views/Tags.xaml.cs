using System.Windows.Controls;
using Asset_Management_System.ViewModels;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Services.Interfaces;
using System.Windows;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.Commands;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : Page
    {
        MainViewModel mainView;
        private ITagService _service;

        public Tags(MainViewModel main, ITagService tagService)
        {
            InitializeComponent();
            DataContext = new TagsViewModel(main, tagService);
            mainView = main;
        }

        private void TreeView_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine(sender.ToString());
            System.Console.WriteLine(((Label)sender).Content);
            if (sender is Label)
            {
                TagsViewModel vm = this.DataContext as TagsViewModel;
                ulong treeViewParentTagID = ((Tag)TagAbleList.SelectedItem).ParentID;
                vm.GoToEdit(sender, e, treeViewParentTagID);
            }
        }
    }
}