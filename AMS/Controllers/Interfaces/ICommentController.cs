using AMS.Models;
using System.Collections.ObjectModel;

namespace AMS.Controllers.Interfaces
{
    public interface ICommentController
    {
        ObservableCollection<Comment> CommentList { get; set; }

        ulong AddNewComment(string contentInput, ulong assetId);

        void RemoveComment(Comment comment, ulong assetId);
    }
}
