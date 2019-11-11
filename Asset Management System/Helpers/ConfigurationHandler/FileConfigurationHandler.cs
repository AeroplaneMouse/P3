using System.Security.Cryptography;
using System.Text;
using Renci.SshNet;
using Session = Asset_Management_System.Authentication.Session;
using SHA256 = SshNet.Security.Cryptography.SHA256;

namespace Asset_Management_System.Helpers.ConfigurationHandler
{
    public class FileConfigurationHandler : IConFigurationHandler
    {
        private Session _session;
        private const string Path = "./DBConfig.txt";

        // Added constructor to get values from session
        public FileConfigurationHandler(Session session)
        {
            _session = session;
        }
        public string GetConfigValue()
        {
            string configuration = FileEncryption.UserDataDecrypt(ComputeSha256Hash(_session.Domain), Path);

            return configuration;
        }

        public void SetConfigValue(string newValue)
        {
            FileEncryption.UserDataEncrypt(ComputeSha256Hash(_session.Domain), newValue, Path);
        }


        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256 = new SHA256())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}