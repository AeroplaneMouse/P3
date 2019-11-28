using System;
using System.Windows;
using System.Windows.Controls;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;

using AMS.ViewModels;


namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for TagsList.xaml
    /// </summary>
    public partial class TagList : Page
    {
        public TagList(ITagListController controller)
        {
            InitializeComponent();
            DataContext = new TagListViewModel(controller);
        }

        private void TagAbleList_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TagListViewModel vm = this.DataContext as TagListViewModel;
            Tag tag = e.NewValue as Tag;
            vm.SelectedItem = tag;
        }
    }
}