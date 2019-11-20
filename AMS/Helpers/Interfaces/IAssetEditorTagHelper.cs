using System.Collections.Generic;
using System.Windows.Controls;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;

namespace AMS.Helpers.Interfaces
{
    public interface IAssetEditorTagHelper
    {
        // The string that the user is searching with
        string _searchString { get; set; }

        // The id of the parent currently being used
        ulong _parentID { get; set; }

        // List of all available tags, based on the search
        List<Tag> _tagList { get; set; }

        // Index that the user has tabbed to in the search results
        int _tabIndex { get; set; }

        // The name of the parent currently displayed
        string _parentString { get; set; }

        // A tag repository, for communication with the database
        ITagRepository _tagRep { get; set; }

        // TODO: Kom uden om mig
        TextBox _box { get; set; }
    }
}