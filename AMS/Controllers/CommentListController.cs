using AMS.Controllers.Interfaces;
using AMS.Models;
using System.Collections.ObjectModel;
using AMS.Authentication;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AMS.Controllers 
{
    public class CommentListController : ICommentListController
    {
        public List<Comment> CommentList { get; set; }

        Session _session;

        ICommentRepository _commentRep;

        public CommentListController(Session session, ICommentRepository commentRepository)
        {
            _session = session;
            _commentRep = commentRepository;
            CommentList = new List<Comment>();


            var n = _commentRep.GetAll();
        }

        /// <summary>
        /// Adds a new comment to the database and updates the comment list with all 
        /// the comments for the chosen asset.
        /// </summary>
        /// <param name="contentInput"></param>
        /// <param name="assetId"></param>
        /// <returns> The id of the new comment </returns>
        public ulong AddNewComment(string contentInput, ulong assetId)
        {
            // Checks that the string containing the content of the comment isn't empty
            if (!string.IsNullOrEmpty(contentInput))
            {
                // Creates a new comment based on the information available
                Comment newComment = new Comment 
                {
                    Username = _session.Username,
                    Content = contentInput,
                    AssetID = assetId
                };

                // Adds that comment to the database and gets its id in return
                _commentRep.Insert(newComment, out ulong id);

                // Updates the CommentList, so it contains all the comments on the chosen asset
                CommentList = new ObservableCollection<Comment>(_commentRep.GetByAssetId(assetId));

                return id;
            }
            else return 0;
        }

        /// <summary>
        /// Removes a comment from an asset, and refreshes the list of comments
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="assetId"></param>
        public void RemoveComment(Comment comment, ulong assetId)
        {
            if (comment != null)
            {
                _commentRep.Delete(comment);
            }

            CommentList = new ObservableCollection<Comment>(_commentRep.GetByAssetId(assetId));
        }
    }
}
