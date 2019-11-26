using AMS.Authentication;
using AMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AMS.Logging
{
    public class LogEntry
    {
        public ulong ID { get; set; }
        public ulong UserId { get; set; }
        public string EntryType { get; set; }
        public string Description { get; set; }
        public ulong LoggedItemId { get; set; }
        public Type LoggedItemType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Changes { get; set; }
        public string UserName { get; set; }
        public string UserDomain { get; set; }

        public LogEntry(ulong userId, string entryType, string description, ulong loggedItemId = 0, Type loggedItemType = null, string changes = "[]")
        {
            UserId = userId;
            EntryType = entryType;
            Description = description;
            Changes = changes;
            LoggedItemId = loggedItemId;
            LoggedItemType = loggedItemType;
        }

        /// <summary>
        /// Default constructor for initiating a new Logger object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="userId"></param>
        /// <param name="createdAt"></param>
        private LogEntry(ulong id, string entryType, string description, ulong loggedItemId, Type loggedItemType, string changes, DateTime createdAt, string userDomain, string userName)
        {
            ID = id;
            CreatedAt = createdAt;
            EntryType = entryType;
            Description = description;
            Changes = changes;
            LoggedItemId = loggedItemId;
            LoggedItemType = loggedItemType;
            UserDomain = userDomain;
            UserName = userName;
        }

        //Used for formatting the DateTimeOutput when showing the elements within a database.
        public string DateToStringConverter => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    }
}