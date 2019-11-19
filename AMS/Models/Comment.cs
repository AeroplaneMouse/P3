using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models
{
    public class Comment : Model
    {
        public object AssetID;
        public object Username { get; set; }
        public string Content { get; set; }

        public Comment()
        {

        }

        /* Constructor used by DB */
        private Comment(ulong id, string username, string content, ulong assetId, DateTime createdAt, DateTime updatedAt)
        {
            ID = id;
            Username = username;
            Content = content;
            AssetID = assetId;
            base.CreatedAt = createdAt;
            base.UpdatedAt = updatedAt;
        }
    }
}
