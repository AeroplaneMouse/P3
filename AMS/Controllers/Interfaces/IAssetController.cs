using System.Windows.Documents;
using AMS.Models;


namespace AMS.Controllers.Interfaces
{
    public interface IAssetController
    {
        Asset Asset { get; set; }
        bool AddField(Field field);
        bool RemoveField(Field inputField);

        bool Save();
        bool Update();
        
    }
}