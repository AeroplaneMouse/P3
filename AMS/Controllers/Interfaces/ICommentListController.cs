using AMS.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AMS.Controllers.Interfaces
{
    public interface ICommentListController
    {
        List<Comment> CommentList { get; set; }

        ulong AddNewComment(string contentInput, ulong assetId);

        void RemoveComment(Comment comment, ulong assetId);
    }
}
