using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AMS.Controllers;
using AMS.Database.Repositories.Interfaces;
using AMS.Helpers;
using AMS.IO;
using AMS.Models;

namespace AMS.ViewModels
{
    public class TagListViewModel
    {
        public List<Tag> Tags { get; set; }
        private TagListController _tagListController;

        public TagListViewModel(ITagRepository tagRepository)
        {
            ITagRepository rep = tagRepository;
            Tags = _tagListController.TagsList= new List<Tag>();
            Tags.AddRange(rep.GetParentTags());
            //Todo Evt inkluder en exporter i stedet for at skrive new
            _tagListController = new TagListController(tagRepository, new PrintHelper());

            foreach (var tag in Tags)
            {
                List<Tag> offspring = rep.GetChildTags(tag.TagId).ToList();
                tag.Children = new List<ITagable>();
                tag.Children.AddRange(offspring);
            }
        }
        
        

        public Tag GetSelectedItem(object sender, RoutedEventArgs e, ulong treeViewParentTagID)
        {
            Label pressedItem = (Label) sender;
            if (pressedItem != null)
            {
                string pressedItemLabel = pressedItem.Content.ToString();
                if (treeViewParentTagID != 0)
                {
                    Tag tag = (Tag) Tags
                    .SingleOrDefault(tag => tag.TagLabel == pressedItemLabel || tag.TagId == treeViewParentTagID);
                    if (tag != null)
                    {
                        if (tag.TagId == treeViewParentTagID)
                        {
                            tag = (Tag) tag.Children.SingleOrDefault(tag => tag.TagLabel == pressedItemLabel);
                        }

                        return tag;
                    }
                }
                else
                {
                    Tag tag = (Tag) Tags.SingleOrDefault(tag => tag.TagLabel == pressedItemLabel);
                    if (tag != null)
                    {
                        return tag;
                    }
                }
            }

            return null;
        }      
    }
}