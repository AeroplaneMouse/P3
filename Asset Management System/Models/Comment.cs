using System;

namespace Asset_Management_System.Models
{
    public class Comment : Model
    {
        public string Username { get; set; }

        public string Content { get; set; }

        public ulong AssetID { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Comment()
        {

        }

        /*Constructor used by DB*/
        private Comment(ulong id, string username, string content, ulong assetId, DateTime createdAt, DateTime updatedAt)
        {
            ID = id;
            Username = username;
            Content = content;
            AssetID = assetId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            SavePrevValues();
        }
    }
}
