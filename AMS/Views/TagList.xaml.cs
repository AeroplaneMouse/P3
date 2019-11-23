using System;
using System.Windows;
using System.Windows.Controls;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
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

        protected void TreeView_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is Label)
            {
                TagListViewModel vm = this.DataContext as TagListViewModel;
                vm.EditCommand.Execute(null);
            }
        }

        private void TagAbleList_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TagListViewModel vm = this.DataContext as TagListViewModel;
            Tag tag = e.NewValue as Tag;
            vm.SelectedItem = tag;

        }
    }
}