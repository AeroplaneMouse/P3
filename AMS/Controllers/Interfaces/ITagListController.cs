using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AMS.Interfaces;

namespace AMS.Controllers.Interfaces
{
    public interface ITagListController
    {
        List<Tag> TagsList { get; set; }

        void Remove(Tag tag);

        Tag GetTag(ulong tagID);

        List<Tag> GetParentTags();

        List<Tag> GetChildTags(ulong id);

        void GetTreeviewData(string keyword = "");

    }
}
