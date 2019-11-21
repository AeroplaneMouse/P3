using System.Collections.Generic;
using AMS.Models;

namespace AMS.Controllers.Interfaces
{
    public interface IFieldListController
    {
        bool SerializeFields();

        bool DeSerializeFields();

        bool AddField(Field field);

        bool RemoveFieldOrFieldRelations(Field inputField, Tag tag = null);
    }
}
