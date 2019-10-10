using System;
using System.Net;
using System.DirectoryServices;
using System.Security.Principal;
using System.Threading;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Security.Permissions;
using System.Collections;

namespace Asset_Management_System.Authentication
{
    public class Session
    {
        public readonly String Username;
        public readonly String Domain;
        public Boolean IsAdmin = true;

        public Session()
        {
            string[] parts = WindowsIdentity.GetCurrent().Name.Split('\\');
            this.Domain = parts[0];
            this.Username = parts[1];
        }

        public Boolean Validate()
        {
            // Create the new LDAP connection
            
            LdapDirectoryIdentifier ldi = new LdapDirectoryIdentifier("192.38.49.9", 389);
            LdapConnection ldapConnection = new LdapConnection(ldi);
            Console.WriteLine("LdapConnection is created successfully.");
            ldapConnection.AuthType = AuthType.Basic;
            ldapConnection.SessionOptions.ProtocolVersion = 3;
            NetworkCredential nc = new NetworkCredential("cn=admin,dc=srv,dc=aau,dc=dk", "ds303e19");
            ldapConnection.Bind(nc);
            Console.WriteLine("LdapConnection authentication success");
            string[] propertiesToQuery = { "cn", "memberUid", "ou", "objectClass" };

            string searchFilter = "cn=*";

            //SearchRequest search = new SearchRequest("people");


            SearchRequest searchRequest = new SearchRequest("dc=srv,dc=aau,dc=dk", searchFilter, System.DirectoryServices.Protocols.SearchScope.Subtree, propertiesToQuery);

            var response = (SearchResponse)ldapConnection.SendRequest(searchRequest);

            if(response.Entries.Count > 0){
                Console.WriteLine("Users found:");
                for (int i=0; i < response.Entries.Count; i++){
                    var userDN = response.Entries[i];
                    Console.WriteLine(userDN.DistinguishedName);
                }
            }

            Console.WriteLine();
            
            searchRequest = new SearchRequest("ou=people,dc=srv,dc=aau,dc=dk", searchFilter, System.DirectoryServices.Protocols.SearchScope.Subtree, propertiesToQuery);

            response = (SearchResponse)ldapConnection.SendRequest(searchRequest);

            if (response.Entries.Count > 0)
            {
                Console.WriteLine("Users in people:");
                for (int i = 0; i < response.Entries.Count; i++)
                {
                    var userDN = response.Entries[i];
                    Console.WriteLine(userDN.DistinguishedName);
                }
            }

            Console.WriteLine();

            //searchFilter = "cn=j*";

            searchRequest = new SearchRequest("cn=TestDepartment,ou=groups,dc=srv,dc=aau,dc=dk", searchFilter, System.DirectoryServices.Protocols.SearchScope.Subtree, propertiesToQuery);

            response = (SearchResponse)ldapConnection.SendRequest(searchRequest);

            if (response.Entries.Count > 0)
            {
                Console.WriteLine("Users in TestDepartment:");
                for (int i = 0; i < response.Entries.Count; i++)
                {
                    var userDN = response.Entries[i];
                    Console.WriteLine(userDN.DistinguishedName);
                    /*
                    foreach(DictionaryEntry attr in userDN.Attributes)
                    {
                        DirectoryAttribute attribute = (DirectoryAttribute) attr.Value;
                           
                        foreach(var isf in attribute.GetValues()){
                            Console.WriteLine(isf);
                        }
                       
                    }
                    */
                }
            }else{
                Console.WriteLine("Nothing found...");
            }


            /*
            foreach(DirectoryAttribute att in userDN.Attributes.Values){

                foreach(var item in att){
                    Console.WriteLine(item.ToString());
                }

                Console.WriteLine(att.Count);
            }
            */



            /*
            DirectoryEntry ent = new DirectoryEntry(ldapConnection);

            using (DirectorySearcher searcher = new DirectorySearcher(ent))
            {
                searcher.Filter = String.Format("({0}={1})", "cn", "jakob");
                var result = searcher.FindOne();
                if (result != null)
                {
                    var displayName = result.Properties["cn"];
                    Console.WriteLine(displayName);
                }
            }
            */

            ldapConnection.Dispose();
            /*
            using (DirectoryEntry entry = new DirectoryEntry("LDAP://192.38.49.9:389", "cn=admin,dc=srv,dc=aau,dc=dk", "ds303e19"))
            {
                try
                {
                    DirectorySearcher search = new DirectorySearcher(entry);
                    search.Filter = "cn=j*";
                    search.PropertiesToLoad.Add("cn");
                    SearchResult result = search.FindOne();
                    if (result != null)
                    {
                        Console.WriteLine("Got a result!");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //handle appropriately according to your requirements
                }
            }
            */

            /*
             * 
             * 
            try
            {

                using (DirectoryEntry myLdapConnection = new DirectoryEntry("LDAP://192.38.49.9,dc=srv,dc=aau,dc=dk", "admin", "ds303e19"))
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

            //Thread.Sleep(2000);
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
