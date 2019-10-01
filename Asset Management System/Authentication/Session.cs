using System;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Security.Principal;
using System.Configuration;

namespace Asset_Management_System
{
    class Session
    {
        public readonly String Username;
        public readonly String Domain;
        public Boolean IsAdmin = false;

        public Session()
        {


            string[] parts = WindowsIdentity.GetCurrent().Name.Split('\\');
            this.Domain = parts[0];
            this.Username = parts[1];

            
            
        }

        public Boolean Validate()
        {
            /*
            try
            {
                using (DirectoryEntry myLdapConnection = createDirectoryEntry())
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(myLdapConnection))
                    {
                        searcher.Filter = String.Format("({0}={1})", "cn", "tlorentzen");
                        var result = searcher.FindOne();
                        if (result != null)
                        {
                            var displayName = result.Properties["cn"];
                            Console.WriteLine(displayName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // if we get an error, it means we have a login failure.
                // Log specific exception
                Console.WriteLine(ex);
            }
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

        //public SqlConnection Database { get; private set; }
    }
}
