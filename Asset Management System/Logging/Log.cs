using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Asset_Management_System.Authentication;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.Logging
{
    public static class Log
    {
        /// <summary>
        /// Creates an entry and inserts it into the database
        /// </summary>
        /// <param name="model"></param>
        /// <param name="description"></param>
        /// <param name="options"></param>
        public static void Write(Model model, string description, string options="{}")
        {
            Entry entry = new Entry();
            entry.LogableId = model.ID;
            entry.LogableType = model.GetType();
            entry.Description = description;
            entry.Options = options;
            entry.Username = Session.GetIdentity();
            
            LogRepository rep = new LogRepository();
            rep.Insert(entry);
        }
        
        /// <summary>
        /// Retrieves existing log entries for a model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="username"></param>
        /// <returns>List of entries</returns>
        public static IEnumerable<Entry> GetEntries(Model model, string username = null)
        {
            LogRepository rep = new LogRepository();
            return rep.GetLogEntries(model.ID, model.GetType(), username);
        }
    }
}
