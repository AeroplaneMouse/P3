using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Asset_Management_System.Authentication;
using Asset_Management_System.Models;

namespace Asset_Management_System
{
    class Log : IUpdateObserver
    {
        public Log(long assetId,User doneBy,string description)
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

        public User DoneBy { get; set; }
        
        public long AssetId;

        private bool Save()
        {
            //Todo Save to database stuff
            return true;
        }

        public void Update(Model Subject)
        {
            Console.Write("Changed " + Subject.Name);
        }
    }
}
