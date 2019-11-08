using System;
using Asset_Management_System.Models;

namespace Asset_Management_System.Logging
{
    public class Entry : Model
    {
        public Entry()
        {
        }

        /// <summary>
        /// Default constructor for initiating a new Log object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="logable_id"></param>
        /// <param name="logable_type"></param>
        /// <param name="description"></param>
        /// <param name="username"></param>
        /// <param name="options"></param>
        /// <param name="created_at"></param>
        private Entry(ulong id, ulong logable_id, Type logable_type, string description, string username, string options, DateTime created_at)
        {
            ID = id;
            LogableId = logable_id;
            LogableType = logable_type;
            Description = description;
            Username = username;
            Options = options;
            CreatedAt = created_at;
        }

        //Used for formatting the DateTimeOutput when showing the elements within a database.
        public string DateToStringConverter => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        
        //public ulong Id { get; protected set; }

        public ulong LogableId { get; set; }

        public Type LogableType { get; set; }
        public string Description { get; set; }

        public string Options { get; set; }

        //public DateTime CreatedAt { get; set; }

        public string Username { get; set; }
    }
}