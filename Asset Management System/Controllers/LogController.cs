using Asset_Management_System.Authentication;
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
            Session session = new Session();
            string description = GenerateDescription(Subject, session);
            Log logEntry = new Log(Subject.ID, session, description);
        }

        /// <summary>
        /// Generates a description for the log entry
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="session"></param>
        /// <returns>
        /// Description
        /// </returns>
        private string GenerateDescription(Model subject, Session session)
        {
            string typeAndName;
            if (subject is Asset)
            {
                Asset asset = (Asset) subject;
                typeAndName = "Asset " + asset.Name;
            }
            else if (subject is Department)
            {
                Department department = (Department) subject;
                typeAndName = "Department" + department.Name;
            }
            else if (subject is Tag)
            {
                Tag tag = (Tag) subject;
                typeAndName =  "Tag" + tag.Label;
            }
            else
            {
                typeAndName = "Unknown subject";
            }

            string type = subject.GetType().ToString();
            return typeAndName + " was changed by " + session.Username;;
        }
    }
}