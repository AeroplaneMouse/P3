using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface ITagListController
    {
        List<Tag> TagsList { get; set; }

        void Search(string query);

        void Remove(Tag tag);

        void Export(List<Tag> tags);

        List<Tag> GetParentTags();

        List<Tag> GetChildTags(ulong id);

        void GetTreeviewData(string keyword = "");

    }
}
