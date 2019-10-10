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
            string name;
            if (subject is Asset)
            {
                name = ((Asset)subject).Name;
            }
            else if (subject is Department)
            {
                name = ((Department)subject).Name;
            }
            else if (subject is Tag)
            {
                name = ((Tag)subject).Label;
            }
            else
            {
                name = "Unknown subject";
            }

            string type = subject.GetType().ToString();
            return $"{type} {name} was changed by {session.Username}";;
        }
    }
}