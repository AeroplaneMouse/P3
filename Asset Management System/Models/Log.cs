using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Asset_Management_System.Authentication;
using Asset_Management_System.Models;

namespace Asset_Management_System
{
    public class Log
    {
        /// <summary>
        /// Default constructor for initiating a new Log object.
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="doneBy"></param>
        /// <param name="description"></param>
        public Log(ulong assetId, Session doneBy,string description)
        {
            this.AssetId = assetId;
            this.DoneBy = doneBy;
            this.Description = description;
            this.RegistrationTime = DateTime.Now;

            Save();
        }

        public int ID { get; }

        public string Description { get; set; }

        public DateTime RegistrationTime { get; set; }

        public Session DoneBy { get; set; }
        
        public ulong AssetId;

        /// <summary>
        /// Saves the log to database
        /// </summary>
        /// <returns></returns>
        private bool Save()
        {
            //Todo Save to database stuff
            return true;
        }
    }
}
