using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels
{
    public class TagsViewModel : ChangeableListPageViewModel<Tag>
    {
        public int ViewType => 2;

        public TagsViewModel(MainViewModel main, ITagService tagService) : base(main, tagService) 
        {
            
        }
    }
}