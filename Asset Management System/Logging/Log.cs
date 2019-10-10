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
        
        public static List<Entry> GetEntries(Model model, string username = null)
        {
            LogRepository rep = new LogRepository();
            return rep.GetLogEntries(model.ID, model.GetType(), username);
        }
    }
}
