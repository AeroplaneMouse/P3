using System.Collections.Generic;
using AMS.Models;

namespace AMS.Controllers.Interfaces
{
    public interface IFieldListController
    {
        List<Field> NonHiddenFieldList { get; set; }
        List<Field> HiddenFieldList { get; set; }

        bool SerializeFields();

        bool AddField(Field field,FieldContainer fieldContainer = null);

        bool RemoveField(Field inputField, FieldContainer fieldContainer = null);

        bool RemoveFieldRelations(ulong TagId);
    }
}
