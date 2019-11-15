using System.Collections.Generic;
using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Services
{
    public class AssetService : IAssetService
    {
        
        private IAssetRepository _rep;

        public AssetService(IAssetRepository rep)
        {
            _rep = rep;
        }

        public ISearchableRepository<Asset> GetSearchableRepository() => _rep;

        public IRepository<Asset> GetRepository() => _rep;

        public Page GetManagerPage(MainViewModel main, Asset inputAsset = default, bool addMultiple = false)
        {
            //TODO: Inject the page
            return new Views.AssetManager(main, this,  inputAsset, addMultiple);
        }

        public string GetName(Asset asset) => asset.Name;

        public List<Tag> LoadTags(ITagRepository rep)
        {
            throw new System.NotImplementedException();
        }

        public List<Field> LoadFields()
        {
            throw new System.NotImplementedException();
        }

        public List<Comment> LoadComments(ICommentRepository rep)
        {
            throw new System.NotImplementedException();
        }
        
    }
}