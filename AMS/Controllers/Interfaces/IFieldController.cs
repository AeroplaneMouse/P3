using AMS.Models;

namespace AMS.Controllers.Interfaces
{
    public interface IFieldController
    {
        bool SerializeFields();

        bool DeSerializeFields();

        bool AddField(Field field);

        bool RemoveFieldOrFieldRelations(Field inputField, Tag tag = null);
    }
}
