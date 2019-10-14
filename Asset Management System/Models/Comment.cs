using System;

namespace Asset_Management_System.Models
{
    public class Comment : Model
    {
        /*Constructor used by DB*/
        private Comment(ulong id, string content, string username, ulong assetId)
        {
            ID = id;
            Content = content;
            Username = username;
            AssetID = assetId;
            SavePrevValues();
        }

        public string Content { get; set; }
        public string Username { get; }

        public ulong AssetID { get; }
    }
}
