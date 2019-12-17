using AMS.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AMS.Controllers.Interfaces
{
    public interface ICommentListController
    {
        List<Comment> CommentList { get; set; }

        ulong AddNewComment(string contentInput);

        void RemoveComment(Comment comment);

        void EditComment(Comment comment);

        void UpdateComment(Comment comment);

        void FetchComments();
    }
}
