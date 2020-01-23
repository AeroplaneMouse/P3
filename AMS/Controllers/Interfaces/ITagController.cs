using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface ITagController : IFieldListController
    {
        bool IsEditing { get; set; }
        Tag ControlledTag { get; set; }
        List<Field> ParentTagFields { get; set; }
        List<Tag> ParentTagList { get; }
        List<Department> DepartmentList { get; }

        bool Save();
        bool Update();
        bool Remove(bool removeChildren = false);
        string CreateRandomColor();
        void ConnectParentTag();
        Tag GetTagById(ulong id);
        void RevertChanges();
    }
}
