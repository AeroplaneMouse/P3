using System;
using System.Collections.Generic;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Logging;
using Org.BouncyCastle.Asn1.X509;

namespace Asset_Management_System.Models
{
    public class Comment : Model, ILoggable<Comment>
    {
        public string Username { get; set; }

        public string Content { get; set; }

        public ulong AssetID { get; set; }

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
            base.CreatedAt = createdAt;
            base.UpdatedAt = updatedAt;
        }

        /// <summary>
        /// Saves all properties to a dictionary with Property name and value
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetLoggableProperties()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.Add("ID", ID.ToString());
            props.Add("Username", Username);
            props.Add("Content", Content);
            props.Add("Asset ID", AssetID.ToString());
            props.Add("Created at", CreatedAt.ToString());
            return props;
        }

        /// <summary>
        /// Returns the Name that should be written in the log
        /// </summary>
        /// <returns></returns>
        public string GetLoggableName() => ID.ToString();

        /// <summary>
        /// Returns the ID
        /// </summary>
        /// <returns></returns>
        public ulong GetId() => ID;

        /// <summary>
        /// Returns a repository-instance for this class
        /// </summary>
        /// <returns></returns>
        public IRepository<Comment> GetRepository() => new CommentRepository();
    }
}
