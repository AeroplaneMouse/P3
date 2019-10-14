using System;
using System.Linq;
using System.Reflection;
using Asset_Management_System.Authentication;
using Asset_Management_System.Logging;
using Asset_Management_System.Models;
using Asset_Management_System.Logging;

namespace Asset_Management_System.Controllers
{
    public class LogController : IUpdateObserver
    {
        /// <summary>
        /// Creates a log entry when notified by subject
        /// </summary>
        /// <param name="Subject"></param>
        public void Update(Model Subject)
        {
            string description = GenerateDescription(Subject);
            //Subject.SavePrevValues();
            string changes = Subject.GetChanges();
            Log.Write(Subject, description, changes);
            Console.WriteLine("Creating log entry: " + description);
        }

        /// <summary>
        /// Generates a description for the log entry
        /// </summary>
        /// <param name="subject"></param>
        /// <returns>
        /// Description
        /// </returns>
        private string GenerateDescription(Model subject)
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
            string changeType = subject.ID == 0 ? "created" : "updated";

            return $"{type} {name} was {changeType}";
        }
    }
}
