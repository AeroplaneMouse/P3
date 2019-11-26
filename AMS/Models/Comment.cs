using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models
{
    public class Comment : Model
    {
        private string _content;

        public ulong AssetID;
        public string Username { get; set; }
        public string Content {
            get {
                return this._content;
            }
            set {
                if (this.Content != null)
                {
                    this.Changes["Content"] = this.Content;
                }
                this._content = value;
            }
        }

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
