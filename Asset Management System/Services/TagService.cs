using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Services
{
    public class TagService : ITagService
    {
        
        private ITagRepository _rep;

        public TagService(ITagRepository rep)
        {
            _rep = rep;
        }

        public ISearchableRepository<Tag> GetSearchableRepository() => _rep;

        public IRepository<Tag> GetRepository() => _rep;

        public Page GetManagerPage(MainViewModel main, Tag inputAsset = default, bool addMultiple = false)
        {
            return new Views.TagManager(main, this,  inputAsset);
        }

        public string GetName(Tag tag) => tag.Name;
    }
}