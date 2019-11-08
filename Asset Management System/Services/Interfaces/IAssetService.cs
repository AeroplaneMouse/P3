using System.Collections.Generic;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.Services.Interfaces
{
    public interface IAssetService : IDisplayableService<Asset>
    {
        List<Tag> LoadTags(ITagRepository rep);

        List<Field> LoadFields();
        
        List<Comment> LoadComments(ICommentRepository rep);
    }
}