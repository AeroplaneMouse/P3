using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
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
        
        void AddNew();

        void Edit(Tag tag);

        void ViewTag(Tag tag);

        void Remove(Tag tag);

        void Export(List<Tag> tags);
    }
}
