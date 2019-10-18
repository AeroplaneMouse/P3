using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface ITagRepository : IMysqlRepository<Tag>
    {
        IEnumerable<Tag> GetAll();
        IEnumerable<Tag> GetParentTags();
        IEnumerable<Tag> GetChildTags(ulong parent_id);
        ObservableCollection<Tag> Search(string keyword);
    }
}
