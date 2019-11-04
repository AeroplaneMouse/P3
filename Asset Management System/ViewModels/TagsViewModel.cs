using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.DataModels;

namespace Asset_Management_System.ViewModels
{
    public class TagsViewModel : ChangeableListPageViewModel<TagRepository, Tag>
    {
        public int ViewType => 2;

        public TagsViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType) 
        {
            Title = "Tags";
        }
    }
}