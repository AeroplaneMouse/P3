using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface ITagController : IFieldListController
    {
        #region Properties

        Tag ControlledTag { get; set; }
        
        List<Field> ParentTagFields { get; set; }

        bool IsEditing { get; set; }

        List<Tag> ParentTagList { get; }
        List<Department> DepartmentList { get; }

        #endregion

        #region Methods

        void Save();

        bool Remove(bool removeChildren = false);

        //void RemoveChildren();

        void Update();

        string CreateRandomColor();

        void ConnectParentTag();

        Tag GetTagById(ulong id);

        void RevertChanges();

        #endregion
    }
}
