using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Asset_Management_System.Authentication;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Newtonsoft.Json;

namespace Asset_Management_System.Logging
{
    public static class Log<T>
    {
        /// <summary>
        /// Logs the changes made to a subject from an old entry
        /// </summary>
        /// <param name="newEntry"></param>
        /// <param name="id"></param>
        /// <param name="delete"></param>
        public static void CreateLog(ILoggable<T> newEntry, ulong id = 0, bool delete = false)
        {
            // Generate description
            string description = GenerateDescription(newEntry, id, delete);
            
            // Fetch object with same ID from database to determine changes
            IRepository<T> rep = newEntry.GetRepository();
            ILoggable<T> oldEntry = rep.GetById(newEntry.GetId()) as ILoggable<T>;
            
            // Determine the changes and serialize to json
            Dictionary<string, Change> changes = GetChanges(oldEntry, newEntry);
            string serializedChanges = changes.Count == 0 ? "[]" : JsonConvert.SerializeObject(changes, Formatting.Indented);
            
            // Create the log entry
            Write(newEntry, description, serializedChanges, id);
            Console.WriteLine("Creating log entry: " + description);
        }
        /// <summary>
        /// Overload method that does not need id
        /// </summary>
        /// <param name="newEntry"></param>
        /// <param name="delete"></param>
        public static void CreateLog(ILoggable<T> newEntry, bool delete)
        {
            CreateLog(newEntry, 0, delete);
        }

        /// <summary>
        /// Creates an entry and inserts it into the database
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="description"></param>
        /// <param name="options"></param>
        /// <param name="id"></param>
        public static void Write(ILoggable<T> subject, string description, string options="{}", ulong id = 0)
        {
            Entry entry = new Entry();
            entry.LogableId = (id == 0) ? subject.GetId() : id;
            entry.LogableType = subject.GetType();
            entry.Description = description;
            entry.Options = options;
            entry.Username = Session.GetIdentity();
            
            LogRepository rep = new LogRepository();
            rep.Insert(entry);
        }

        /// <summary>
        /// Generates a description for the log entry
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="id"></param>
        /// <param name="delete"></param>
        /// <returns>Description</returns>
        private static string GenerateDescription(ILoggable<T> subject, ulong id, bool delete)
        {
            // Get subject Type
            string type = subject.GetType().Name;
            // Get subject Name
            string name = subject.GetLoggableName();
            
            // Determine if subject is being created or updated
            bool created = id == 0 || subject.GetRepository().GetById(subject.GetId()) == null;
            
            string changeType = delete ? "deleted" : created ? "created" : "updated";

            return $"{type} {name} was {changeType}";
        }
        
        /// <summary>
        /// Determines the change between two loggable entries.
        /// Returns a dictionary with the name of the properties that have changed and the changes.
        /// If the given entries are not of the same type, returns an empty dictionary.
        /// </summary>
        /// <param name="oldEntry"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        private static Dictionary<string, Change> GetChanges(ILoggable<T> oldEntry, ILoggable<T> newEntry)
        {
            Dictionary<string, Change> changes = new Dictionary<string, Change>();
            
            // Return empty dictionary if entries are not of the same type.
            if (oldEntry == null)
                return changes;

            Dictionary<string, string> oldValues = oldEntry.GetLoggableProperties();
            Dictionary<string, string> newValues = newEntry.GetLoggableProperties();
            foreach (var prop in oldValues)
            {
                string oldValue = oldValues[prop.Key];
                string newValue = newValues[prop.Key];
                
                if (newValue != oldValue)
                {
                    changes.Add(prop.Key, new Change(oldValue, newValue));
                }
            }

            return changes;
        }

        /*
        public static void LogTags(ILoggable<T> oldEntry, ILoggable<T> newEntry)
        {
            // return if given subjects are not assets,
            if (!(oldEntry is Asset && newEntry is Asset))
                return;
            
            AssetRepository rep = new AssetRepository();
            List<Tag> oldTags = rep.GetAssetTags((Asset) oldEntry);
            //List<Tag> newTags = ((Asset) newEntry)
        }

        public static void LogComments(ILoggable<T> oldEntry, ILoggable<T> newEntry)
        {
            // Compare comments before and after
        }
        */
    }
}
