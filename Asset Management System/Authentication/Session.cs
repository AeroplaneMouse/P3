using System;
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
        public event StatusUpdateEventHandler StatusUpdate;

        public Session()
        {
            string[] parts = GetIdentity().Split('\\');
            this.Domain = parts[0];
            this.Username = parts[1];
        }

        public bool Validate()
        {
            // Make LDAP connection
            StatusUpdateEventArgs args = new StatusUpdateEventArgs("");

            args.Message = "Establishing connection to LDAP...";
            StatusUpdate(args);
            Ldap ldap = new Ldap();
            try
            {
                ldap.Connect();
            }
            catch (LdapException e)
            {
                args.Message = "Unable to establish connection to LDAP";
                args.extraMessage = e.Message;
                StatusUpdate(args);
            }

            // Validate user

            if (ldap.UserExist("jakob"))
            {
                StatusUpdate.Invoke(new StatusUpdateEventArgs("User exist!"));
                Console.WriteLine("User exist!");
                return true;
            }
            else
            {
                StatusUpdate.Invoke(new StatusUpdateEventArgs("User doesn't exist!"));
                Console.WriteLine("User does not exist!");
                return false;
            }

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

            return false;
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
