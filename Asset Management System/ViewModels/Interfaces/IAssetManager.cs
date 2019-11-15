using System.Collections.Generic;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Interfaces
{
    public interface IAssetManager : IObjectManager, IFieldManager
    {
        bool AttachTag(Asset asset, ITagable tag);
        bool DetachTag(Asset asset, ITagable tag);

        List<ShownField> GetRelations(Asset asset, List<Tag> tags);
        bool RemoveRelations(Asset asset, Tag tag, List<ShownField> shownFields);

        Tag CreateTag(string name);
    }
}