using System.Collections.Generic;
using System.Windows.Documents;
using AMS.Interfaces;
using AMS.Models;


namespace AMS.Controllers.Interfaces
{
    public interface IAssetController : IFieldController
    {
        Asset Asset { get; set; }
        List<ITagable> CurrentlyAddedTags { get; set; }
        
        bool AttachTag(ITagable tag);

        bool DetachTag(ITagable tag);

        bool Save();
        bool Update();

        bool Remove();

    }
}