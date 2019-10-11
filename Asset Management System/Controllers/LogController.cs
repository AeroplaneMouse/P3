using System;
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
        public void Update(Model Subject)
        {
            string description = GenerateDescription(Subject);
            Log.Write(Subject, description);
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
            // Get the name of the subject
            string name;
            if (subject.GetType().GetProperty("Name") != null)
            {
                name = subject.GetType().GetProperty("Name").Name;
            }
            else if (subject.GetType().GetProperty("Label") != null)
            {
                name = subject.GetType().GetProperty("Label").Name;
            }
            else
            {
                // If the subject has no name, leave it blank.
                name = "";
            }

            // Get subject Type
            string type = subject.GetType().ToString();
            // Determine if subjec is being created or updated
            string changeType = subject.ID == 0 ? "created" : "updated";

            return $"{type} {name} was {changeType}";
        }
    }
}
