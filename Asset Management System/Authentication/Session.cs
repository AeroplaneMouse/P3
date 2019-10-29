﻿using System;
using System.Security.Principal;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Authentication
{
    public class Session
    {
        public readonly User user;
        
        public string Username { get => GetIdentity().Split('\\')[1]; }
        
        public string Domain { get => GetIdentity().Split('\\')[0]; }

        public Session()
        {
            //Console.WriteLine("Username: "+GetIdentity());
            UserRepository rep = new UserRepository();
            user = rep.GetByUsername(GetIdentity());
        }

        public bool Authenticated()
        {
            //TODO: Real implementation of Authenticated()
            return user != null;
        }

        public bool IsAdmin()
        {
            //TODO: Real implementation of IsAdmin()
            return user.IsAdmin;
        }
        
        public static string GetIdentity()
        {
            return WindowsIdentity.GetCurrent().Name;
        }
    }
}
