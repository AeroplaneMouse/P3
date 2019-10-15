﻿using System;
using System.Net;
using System.DirectoryServices;
using System.Security.Principal;
using System.DirectoryServices.Protocols;
using Asset_Management_System.Events;

namespace Asset_Management_System.Authentication
{
    public class Session
    {
        public readonly String Username;
        public readonly String Domain;
        public Boolean IsAdmin = true;

        public Session()
        {
            string[] parts = GetIdentity().Split('\\');
            this.Domain = parts[0];
            this.Username = parts[1];
        }

        public event Events.StatusUpdateEventHandler StatusUpdate;


        public bool Validate()
        {
            /* Do LDAP Validation */
            /*
            Ldap ldap = new Ldap();
            
            ldap.Connect();
            //ldap.Search("", [], "cn=*");
            if (ldap.UserExist("jakob"))
            {
                StatusUpdate.Invoke(this, new StatusUpdateEventArgs("User exist! - Event call!"));
                Console.WriteLine("User exist!");
            }
            else
            {
                Console.WriteLine("User does not exist!");
            }


            ldap.Close();
            */

            return true;
        }

        private static DirectoryEntry createDirectoryEntry()
        {
            DirectoryEntry ldapConnection = new DirectoryEntry();
            ldapConnection.Path = "ldap://ldap.jumpcloud.com:389;ou=Users;o=5d8a453c62f4be26b5c4895e,dc=jumpcloud,dc=com";
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return ldapConnection;
        }

        public static string GetIdentity()
        {
            return WindowsIdentity.GetCurrent().Name;
        }

        //public SqlConnection Database { get; private set; }
    }
}
