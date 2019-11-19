﻿using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Models;
using Asset_Management_System.Authentication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AMS.Controllers 
{
    public class CommentController : ICommentController
    {
        public ObservableCollection<Comment> CommentList { get; set; }

        Session _session;

        CommentRepository _commentRep;


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
                Comment newComment = new Comment()
                {
                    Username = _session.Username,
                    Content = contentInput,
                    AssetID = assetId
                };

                // Adds that comment to the database and gets its id in return
                _commentRep.Insert(newComment, out ulong id);

                // Updates the CommentList, so it contains all the comments on the chosen asset
                CommentList = _commentRep.GetByAssetId(assetId);

                return id;
            }
            else return 0;
        }

        public void RemoveComment(Comment comment, ulong assetId)
        {
            if (comment != null)
            {
                _commentRep.Delete(comment);
            }

            CommentList = _commentRep.GetByAssetId(assetId);
        }
    }
}
