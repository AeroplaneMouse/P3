using System;

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
        public string Username { get; set; }
        public string UserDomain { get; set; }

        /// <summary>
        /// Constructor to construct a new LogEntry
        /// </summary>
        /// <param name="userId">The id of the related user</param>
        /// <param name="entryType">The entry type of the LogEntry</param>
        /// <param name="description">A description of the entry</param>
        /// <param name="loggedItemId">The id of the item to be logged</param>
        /// <param name="loggedItemType">The type of the item to be logged</param>
        /// <param name="changes">The changes made</param>
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
        /// Default constructor for initiating a new Logger object from the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entryType"></param>
        /// <param name="description"></param>
        /// <param name="loggedItemId"></param>
        /// <param name="loggedItemType"></param>
        /// <param name="changes"></param>
        /// <param name="createdAt"></param>
        /// <param name="userDomain"></param>
        /// <param name="userName"></param>
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
            Username = userName;
        }

        /// <summary>
        /// Used for formatting the DateTimeOutput when showing the elements within a database.
        /// </summary>
        public string DateToStringConverter => CreatedAt.ToString("u").TrimEnd('Z');
    }
}