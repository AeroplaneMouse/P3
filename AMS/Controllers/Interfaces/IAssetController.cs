using System.Collections.Generic;
using System.Windows.Documents;
using AMS.Interfaces;
using AMS.Models;


namespace AMS.Controllers.Interfaces
{
    public interface IAssetController : IFieldListController
    {
        Asset ControlledAsset { get; set; }
        List<ITagable> CurrentlyAddedTags { get; set; }

        void AttachTags(List<ITagable> tags);
        void AttachTags(ITagable tag);
        void DetachTags(List<ITagable> tags);
        void DetachTags(ITagable tag);

        bool Save();
        bool Update();
        bool Remove();

        void LoadFields();
        void UpdateFieldContent();

        void RevertChanges();
    }
}