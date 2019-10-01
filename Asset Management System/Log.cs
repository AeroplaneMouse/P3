using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Asset_Management_System.Authentication;
using Asset_Management_System.Models;

namespace Asset_Management_System
{
    class Log
    {
        public Log(int assetId,User doneBy,string comment)
        {
            this.AssetId = assetId;
            this.DoneBy = doneBy;
            this.Comment = comment;
            this.RegistrationTime = DateTime.Now;
            
        }

        public int ID { get; }

        public string Comment { get; set; }

        public DateTime RegistrationTime { get; set; }

        public User DoneBy { get; set; }
        
        public int AssetId;

        public bool UpdateComment(string newComment)
        {
            return true;
        }

        private bool Save()
        {
            //Todo Save to database stuff
            return true;
        }
    }
}
