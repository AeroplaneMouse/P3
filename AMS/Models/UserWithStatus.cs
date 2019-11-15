using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models
{
    public class UserWithStatus : User
    {
        #region Public Properties

        public bool IsShown { get; set; }

        public string Status { get; set; }

        public string StatusColor
        {
            get
            {
                if (Status.CompareTo("Added") == 0)
                {
                    return "#00B600";
                }

                else if (Status.CompareTo("Removed") == 0)
                {
                    return "#E30000";
                }

                else if (Status.CompareTo("Conflicting") == 0)
                {
                    return "#FFCC1A";
                }

                else if (IsEnabled == false)
                {
                    return "#C0C0C0";
                }

                else
                {
                    return "#ffffff";
                }
            }
        }

        #endregion

        #region Constructor

        public UserWithStatus(User user) : base()
        {
            this.ID = user.ID;
            this.Username = user.Username;
            this.Domain = user.Domain;
            this.Description = user.Description;
            this.IsEnabled = user.IsEnabled;
            this.DefaultDepartment = user.DefaultDepartment;
            this.IsAdmin = user.IsAdmin;
            this.CreatedAt = user.CreatedAt;
            this.UpdatedAt = user.UpdatedAt;

            this.Status = String.Empty;
            this.IsShown = true;
        }

        #endregion


    }
}
