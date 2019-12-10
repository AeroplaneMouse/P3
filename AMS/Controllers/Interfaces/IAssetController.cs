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

        string Name { get; set; }
        string Identifier { get; set; }
        string Description { get; set; }

        void AttachTags(List<ITagable> tags);
        void DetachTags(List<ITagable> tags);

        bool Save();
        bool Update();
        bool Remove();

        void LoadFields();
        void RevertChanges();
    }
}