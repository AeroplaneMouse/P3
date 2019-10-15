using System;
using System.Collections.Generic;
using System.Net;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.DirectoryServices.Protocols;
using System.Text;
using System.Windows.Media.Animation;
using Google.Protobuf.WellKnownTypes;

namespace Asset_Management_System.Authentication
{
    public class Ldap
    {
        private LdapDirectoryIdentifier _ldi;
        private LdapConnection _ldapConnection;
        private NetworkCredential nc;
        public string host = "192.38.49.9";
        public int port = 389;
        public int version = 3;
        public string domain = "srv.aau.dk";
        private AuthType _authType = AuthType.Basic;
        public string BindUsername = "admin";
        public string BindPassword = "ds303e19";

        public Ldap()
        {
            
        }

        public bool UserExist(string username)
        {
            string[] propertiesToQuery = { "cn" };
            SearchResponse response = Search("", propertiesToQuery, "cn=" + username);
            return response.Entries.Count == 1;
        }

        public SearchResponse Search(string path, string[] propertiesToFetch, string keyword="cn=*")
        {
            SearchRequest searchRequest = new SearchRequest(GenerateLdapQuery(path), keyword, System.DirectoryServices.Protocols.SearchScope.Subtree, propertiesToFetch);
            SearchResponse response = (SearchResponse)_ldapConnection.SendRequest(searchRequest);
            return response;
            /*
            if(response?.Entries.Count > 0){
                Console.WriteLine("Users found:");
                for (int i=0; i < response.Entries.Count; i++){
                    var userDN = response.Entries[i];
                    Console.WriteLine(userDN.DistinguishedName);
                }
            }
            */
        }

        public bool Connect()
        {
            try
            {
                _ldi = new LdapDirectoryIdentifier(host, port);
                _ldapConnection = new LdapConnection(_ldi);
                _ldapConnection.AuthType = _authType;
                _ldapConnection.SessionOptions.ProtocolVersion = version;
                nc = new NetworkCredential(GenerateLdapQuery("cn="+BindUsername), BindPassword);
                _ldapConnection.Bind(nc);
                return true;
            }
            catch (LdapException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void Close()
        {
            _ldapConnection?.Dispose();
        }

        private string GenerateLdapQuery(string path)
        {
            return (string.Empty == path ? "" : path+",") + MakeDomain();
        }

        private string MakeDomain()
        {
            StringBuilder dcQuery = new StringBuilder("");
            List<string> dcObj = new List<string>();
            string[] domainParts = domain.Split(".");
            
            foreach (string part in domainParts)
            {
                dcObj.Add($"dc={part}");
            }
            
            dcQuery.Append(string.Join(",", dcObj));
            return dcQuery.ToString();
        }
    }
}
