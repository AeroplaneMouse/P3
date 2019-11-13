using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels.Commands;

namespace Asset_Management_System.ViewModels
{
    public class TagsViewModel : ChangeableListPageViewModel<Tag>
    {
        private MainViewModel mainViewModel;
        public int ViewType => 2;
        public List<ITagable> Tags { get; set; }
        
        public TagsViewModel(MainViewModel main, ITagService tagService) : base(main, tagService)
        {
            ITagRepository rep = (ITagRepository)tagService.GetRepository();
            Tags = new List<ITagable>();
            Tags.AddRange(rep.GetParentTags());
            mainViewModel = main;

            foreach (var tag in Tags)
            {
                List<Tag> ofspring = rep.GetChildTags(tag.TagId).ToList();
                tag.Children = new List<ITagable>();
                tag.Children.AddRange(ofspring);
            }
        }

        public void GoToEdit(object sender, RoutedEventArgs e, ulong treeViewParentTagID)
        {
            Label pressedItem = (Label)sender;
            if (pressedItem != null)
            {
                string pressedItemLabel = pressedItem.Content.ToString();
                if (treeViewParentTagID != 0)
                {
                    Tag tag = (Tag)Tags
                        .SingleOrDefault(tag => tag.TagLabel == pressedItemLabel || tag.TagId == treeViewParentTagID);
                    if (tag != null)
                    {
                        if(tag.TagId() == treeViewParentTagID)
                        {
                            tag = (Tag)tag.Children.SingleOrDefault(tag => tag.TagLabel == pressedItemLabel);
                        }
                        mainViewModel.ChangeMainContent(_service.GetManagerPage(mainViewModel, tag));
                    }
                }
                else
                {
                    Tag tag = (Tag)Tags.SingleOrDefault(tag => tag.TagLabel == pressedItemLabel);
                    if (tag != null)
                    {
                        mainViewModel.ChangeMainContent(_service.GetManagerPage(mainViewModel, tag));
                    }
                }
            }
        }
    }
}