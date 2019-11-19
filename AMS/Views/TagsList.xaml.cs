using System;
using System.Windows;
using System.Windows.Controls;
using AMS.Database.Repositories;
using AMS.ViewModels;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;
using MainViewModel = Asset_Management_System.ViewModels.MainViewModel;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class TagsList : Page
    {
        MainViewModel mainView;
        private ITagService _service;

        public TagsList(MainViewModel main, TagRepository tagRepository)
        {
            InitializeComponent();
            DataContext = new TagListViewModel(tagRepository);
            mainView = main;
        }

        protected void TreeView_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is Label)
            {
                TagListViewModel vm = this.DataContext as TagListViewModel;
                ulong treeViewParentTagID = ((Tag)TagAbleList.SelectedItem).ParentID;
                vm.GetSelectedItem(sender, e, treeViewParentTagID);
            }
        }

        private void TagAbleList_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Console.WriteLine(e);
        }
    }
}