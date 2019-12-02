using AMS.Controllers.Interfaces;
using AMS.Models;
using System.Collections.ObjectModel;
using AMS.Authentication;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using System.Collections.Generic;
using System.Linq;
using AMS.Database.Repositories;

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
                    _commentList = (_asset != null) ? _commentRep.GetByAssetId(_asset.ID) : _commentRep.GetAll();
                }

                return _commentList.OrderByDescending(p => p.CreatedAt).ToList();
            }

            set => _commentList = value;
        }

        private List<Comment> _commentList { get; set; }
        private Session _session { get; set; }
        private Asset _asset { get; set; }
        private ICommentRepository _commentRep { get; set; }

        public CommentListController(Session session, ICommentRepository commentRepository, Asset asset = null)
        {
            _session = session;
            _commentRep = commentRepository;
            // Create new asset if optional parameter not given
            _asset = asset;
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

            CommentList = (_asset != null) ? _commentRep.GetByAssetId(_asset.ID) : _commentRep.GetAll();
        }
    }
}
