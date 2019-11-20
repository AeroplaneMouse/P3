using System.Collections.Generic;
using System.Windows.Controls;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Helpers.Interfaces;
using AMS.Models;

namespace AMS.Helpers
{
    public class AssetEditorTagHelper : IAssetEditorTagHelper
    {
        public string _searchString { get; set; }
        
        public ulong _parentID { get; set; }
        
        public List<Tag> _tagList { get; set; }
        
        public int _tabIndex { get; set; }
        
        public string _parentString { get; set; }
        
        public ITagRepository _tagRep { get; set; }
        
        public TextBox _box { get; set; }

        public AssetEditorTagHelper(ITagListController tagRepository)
        {
            
        }
    }
}