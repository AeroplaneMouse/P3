using System;
using System.Collections.Generic;
using System.Linq;
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
        public int ViewType => 2;
        public List<ITagable> Tags { get; set; }
        public static ICommand TagSelectCommand { get; set; }

        private bool _isExpanded;
        private bool _isSelected;
        private object _selected;
        

        public object selectedElement
        {
            get { return _selected; }
            set
            {
                _selected = value;
                CalledStuff();
            }
        }

        private void CalledStuff()
        {
            Console.WriteLine("Element clicked!");
        }


        public TagsViewModel(MainViewModel main, ITagService tagService) : base(main, tagService)
        {
            TagSelectCommand = new TagSelectCommand(main);
            ITagRepository rep = (ITagRepository)tagService.GetRepository();
            Tags = new List<ITagable>();
            Tags.AddRange(rep.GetParentTags());

            foreach (var tag in Tags)
            {
                List<Tag> ofspring = rep.GetChildTags(tag.TagId).ToList();
                tag.Children = new List<ITagable>();
                tag.Children.AddRange(ofspring);
            }
            
            
        }
    }
}