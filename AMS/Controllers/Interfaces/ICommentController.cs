using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface ICommentController
    {
        ObservableCollection<Comment> CommentList { get; set; }

        ulong AddNewComment(string contentInput, ulong assetId);

        void RemoveComment(Comment comment, ulong assetId);
    }
}
