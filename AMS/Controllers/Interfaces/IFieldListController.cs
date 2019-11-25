using System.Collections.Generic;
using AMS.Models;

namespace AMS.Controllers.Interfaces
{
    public interface IFieldListController
    {
        List<Field> NonHiddenFieldList { get; set; }
        List<Field> HiddenFieldList { get; set; }

        bool SerializeFields();

        bool DeSerializeFields();

        bool AddField(Field field);

        bool RemoveField(Field inputField);
    }
}
