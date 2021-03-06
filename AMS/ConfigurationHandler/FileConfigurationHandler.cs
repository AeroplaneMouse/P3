using System;
using System.IO;
using System.Text;
using AMS.Authentication;
using SHA256 = SshNet.Security.Cryptography.SHA256;

namespace AMS.ConfigurationHandler
{
    public class FileConfigurationHandler : IConFigurationHandler
    {
        private readonly Session _session;
        private const string Path = "./DBConfig.txt";
        private string Domain;

        public FileConfigurationHandler(string domain)
        {
            Domain = domain;
        }

        public string GetConfigValue()
        {
            string configuration = FileEncryption.UserDataDecrypt(ComputeSha256Hash(Domain), Path);
            return configuration;
        }

        public void SetConfigValue(string newValue)
        {
            Session.ClearDBKey();
            FileEncryption.UserDataEncrypt(ComputeSha256Hash(Domain), newValue, Path);
        }

        public string LoadConfigValueFromExternalFile(string path)
        {
            return FileEncryption.UserDataDecrypt(ComputeSha256Hash(Domain), path);
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

        public void Clear()
        {
            SetConfigValue("");
        }
    }
}