using System.Collections.Generic;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Controllers.Interfaces
{
    public interface IFieldListController
    {
        bool SerializeFields();

        bool AddField(Field field);
        bool RemoveField(Field inputField);
        
        bool RemoveTagRelationsOnFields(ITagable tag);
    }
}
