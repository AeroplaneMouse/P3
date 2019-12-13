using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models
{
    public class Comment : Model
    {
        private string _content;

        public ulong AssetID { get; set; }
        public string Username { get; set; }
        public string AssetName { get; set; }

        public bool IsEditing { get; set; }

        public string Content 
        {
            get 
            {
                return _content;
            }
            set 
            {
                if (TrackChanges)
                {
                    Changes["Content"] = Content;
                }
                _content = value;
            }
        }

        public Comment()
        {

        }

        /* Constructor used by DB */
        private Comment(ulong id, string username, string assetName, string content, ulong assetId, DateTime createdAt, DateTime updatedAt)
        {
            ID = id;
            Username = username;
            Content = content;
            AssetID = assetId;
            AssetName = assetName;
            base.CreatedAt = createdAt;
            base.UpdatedAt = updatedAt;
            TrackChanges = true;
            IsEditing = false;
        }
    }
}
