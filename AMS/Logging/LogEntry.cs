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
        public DateTime CreatedAt { get; set; }
        public string Changes { get; set; }

        public LogEntry(ulong userId, string entryType, string description, string changes = "[]")
        {
            UserId = userId;
            EntryType = entryType;
            Description = description;
            Changes = changes;
        }

        /// <summary>
        /// Default constructor for initiating a new Logger object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="userId"></param>
        /// <param name="createdAt"></param>
        private LogEntry(ulong id, DateTime createdAt, ulong userId, string entryType, string description, string changes)
        {
            ID = id;
            CreatedAt = createdAt;
            UserId = userId;
            EntryType = entryType;
            Description = description;
            Changes = changes;
        }

        //Used for formatting the DateTimeOutput when showing the elements within a database.
        public string DateToStringConverter => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    }
}