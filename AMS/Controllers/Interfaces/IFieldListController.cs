using System.Collections.Generic;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Controllers.Interfaces
{
    public interface IFieldListController
    {
        bool SerializeFields();

        public List<Field> NonHiddenFieldList { get; set; }
        public List<Field> HiddenFieldList { get; set; }

        bool AddField(Field field);

        bool RemoveField(Field inputField);
        
        bool HandleFieldsFromRemoveTag(Field inputField,Tag tag);

        bool RemoveTagRelationsOnFields(ITagable tag);
    }
}
