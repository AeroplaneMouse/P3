using AMS.Controllers.Interfaces;
using AMS.Models;
using System.Collections.ObjectModel;
using AMS.Authentication;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using System.Collections.Generic;
using System.Linq;
using AMS.Database.Repositories;
using AMS.ViewModels;

namespace AMS.Controllers 
{
    public class CommentListController : ICommentListController
    {
        public List<Comment> CommentList
        {
            get
            {
                if (_commentList == null)
                {
                    FetchComments();
                }

                return _commentList;
            }
            set => _commentList = value;
        }

        private List<Comment> _commentList { get; set; }
        private Session _session { get; set; }
        private Asset _asset { get; set; }
        private ICommentRepository _commentRep { get; set; }
        private Department _department { get; set; }

        private Dictionary<ulong, string> _oldCommentContent { get; set; }

        public CommentListController(Session session, ICommentRepository commentRepository, Department department, Asset asset)
        {
            _session = session;
            _commentRep = commentRepository;
            _department = department;
            _asset = asset;

            _oldCommentContent = new Dictionary<ulong, string>();

            FetchComments();
        }

        /// <summary>
        /// Adds a new comment to the database and updates the comment list with all 
        /// the comments for the chosen asset.
        /// </summary>
        /// <param name="contentInput"></param>
        /// <param name="assetId"></param>
        /// <returns> The id of the new comment </returns>
        public ulong AddNewComment(string contentInput)
        {
            // Checks that the string containing the content of the comment isn't empty
            if (!string.IsNullOrEmpty(contentInput))
            {
                // Creates a new comment based on the information available
                Comment newComment = new Comment 
                {
                    Username = _session.Username,
                    Content = contentInput,
                    AssetID = _asset.ID
                };

                // Adds that comment to the database and gets its id in return
                _commentRep.Insert(newComment, out ulong id);

                // Updates the CommentList, so it contains all the comments on the chosen asset
                CommentList = _commentRep.GetByAssetId(_asset.ID);

                return id;
            }
            
            return 0;
        }

        /// <summary>
        /// Removes a comment from an asset, and refreshes the list of comments
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="assetId"></param>
        public void RemoveComment(Comment comment)
        {
            if (comment != null)
            {
                _commentRep.Delete(comment);
            }

            FetchComments();
        }

        /// <summary>
        /// Changes the input comments editing status
        /// </summary>
        /// <param name="comment"></param>
        public void EditComment(Comment comment)
        {
            if (comment != null)
            {
                // If the comment is not being edited, save its content to the dict
                if (!comment.IsEditing)
                {
                    _oldCommentContent.Add(comment.ID, comment.Content);
                }

                // Else, fetch the content from the dict, and remove that comment from the dict
                else
                {
                    comment.Content = _oldCommentContent[comment.ID];
                    _oldCommentContent.Remove(comment.ID);

                    Features.AddNotification(new Notification("Changes cancelled", Notification.INFO));
                }

                // Change the comment's editing status to the opposite
                comment.IsEditing ^= true;
            }
        }

        /// <summary>
        /// Updates the comment's content in the database
        /// </summary>
        /// <param name="comment"></param>
        public void UpdateComment(Comment comment)
        {
            if (comment != null)
            {
                comment.IsEditing = false;
                _commentRep.Update(comment);

                _oldCommentContent.Remove(comment.ID);

                Features.AddNotification(new Notification("Changes saved", Notification.APPROVE));
            }
        }

        /// <summary>
        /// Fetches all comments from within the current department
        /// </summary>
        public void FetchComments()
        {
            _department = Features.GetCurrentDepartment();
            CommentList = (_asset != null) ? _commentRep.GetByAssetId(_asset.ID) : _commentRep.GetLatestComments(_department.ID);
        }
    }
}
