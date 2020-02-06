using System.Collections.Generic;
using System.Windows.Documents;
using AMS.Interfaces;
using AMS.Models;


namespace AMS.Controllers.Interfaces
{
    public interface IAssetController : IFieldListController
    {
        bool IsEditing { get; set; }
        Asset ControlledAsset { get; set; }
        List<ITagable> CurrentlyAddedTags { get; set; }

        void AttachTags(List<ITagable> tags);
        void AttachTags(ITagable tag);
        void DetachTags(List<ITagable> tags);

        bool Save();
        bool Update();
        bool Remove();

        void UpdateFieldContent();

        void RevertChanges();
    }
}