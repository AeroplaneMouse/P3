using System.Collections.Generic;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Controllers.Interfaces
{
    public interface IFieldListController
    {
        bool SerializeFields();

        List<Field> NonHiddenFieldList { get; set; }
        List<Field> HiddenFieldList { get; set; }

        bool AddField(Field field, FieldContainer fieldContainer = null);

        bool RemoveField(Field inputField);
        
        bool HandleFieldsFromRemoveTag(Field inputField,Tag tag);

        bool RemoveTagRelationsOnFields(ulong TagId);
    }
}
