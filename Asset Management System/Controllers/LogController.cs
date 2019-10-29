﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Asset_Management_System.Authentication;
using Asset_Management_System.Logging;
using Asset_Management_System.Models;

namespace Asset_Management_System.Controllers
{
    public class LogController : IUpdateObserver
    {
        /// <summary>
        /// Creates a log entry when notified by subject
        /// </summary>
        /// <param name="Subject"></param>
        /// <param name="delete"></param>
        public void Update(Model Subject, bool delete)
        {
            string description = GenerateDescription(Subject, delete);
            //Subject.SavePrevValues();
            string changes = Subject.GetChanges();
            Log.Write(Subject, description, changes);
            Console.WriteLine("Creating log entry: " + description);
        }

        /// <summary>
        /// Generates a description for the log entry
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="delete"></param>
        /// <returns>Description</returns>
        private string GenerateDescription(Model subject, bool delete)
        {
            Type objectType = subject.GetType();
            PropertyInfo[] props = objectType.GetProperties();
            
            // Get the name of the subject
            string name;
            if (subject.GetType().GetProperty("Name") != null)
            {
                //name = subject.GetType().Name;
                name = objectType.GetProperty("Name").GetValue(subject, null).ToString();
            }
            else if (subject.GetType().GetProperty("Label") != null)
            {
                //name = subject.GetType().Name;
                name = objectType.GetProperty("Label").GetValue(subject, null).ToString();
            }
            else
            {
                // If the subject has no name, leave it blank.
                name = "";
            }

            // Get subject Type
            string type = subject.GetType().Name;
            // Determine if subject is being created or updated
            string changeType = delete ? "deleted" : subject.ID == 0 ? "created" : "updated";

            return $"{type} {name} was {changeType}";
        }

        public List<Change> GetChanges(object oldEntry, object newEntry)
        {
            List<Change> changes = new List<Change>();

            if (oldEntry.GetType() != newEntry.GetType())
                return changes;

            return changes;
        }
    }
}
