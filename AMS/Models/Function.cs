using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AMS.Interfaces;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class Function
    {
        public enum FunctionType
        {
            Expiration = 1,
            TagRequire,
            Logger
        };

        public static IEnumerable<FunctionType> GetTypes()
        {
            return new List<FunctionType>()
            {
                FunctionType.Expiration,
                FunctionType.TagRequire,
                FunctionType.Logger
            };
        }

        public List<ulong> TagIDs { get; set; }
        public string HashId { get; set; }

        public string Label { get; set; }
        public string Content { get; set; }

        public string ExtraContent
        {
            get;
            set;
        }

        public string Hash
        {
            get => CalculateMd5Hash();
        }

        public FunctionType Type { get; set; }

        /// <summary>
        /// Default constructor for initiating a new Field object.
        /// </summary>
        /// <param name="label">The label of the field</param>
        /// <param name="content">The content added to the field</param>
        /// <param name="type">Selecting the type of the field. 1= TextBox,2 = String,3= Int, 4 = Date, 5 = Boolean</param>
        public Function(string label, string content, FunctionType type)
        {
            // Creates unique hash
            this.HashId = CalculateMd5Hash(true);
            this.Label = label;
            this.Content = content;
            Type = type;
            //this.Hash = CalculateMd5Hash();
            this.TagIDs = new List<ulong>();
        }

        public Function(string label, string content, FunctionType type, string hashId, List<ulong> tagIDs)
        {
            this.Label = label;
            this.Content = content;
            Type = type;
            this.HashId = hashId;
            //this.Hash = CalculateMd5Hash();

            this.TagIDs = tagIDs;
        }

        [JsonConstructor]
        private Function(string label, string content, FunctionType type, bool required,
            bool isCustom, List<ulong> tagIDs)
        {
            this.HashId = CalculateMd5Hash(true);
            this.Label = label;
            this.Content = content;
            Type = type;
            //this.Hash = CalculateMd5Hash();
            this.TagIDs = tagIDs ?? new List<ulong>();
        }

        /// <summary>
        /// Returns the object information as a dictionary.
        /// </summary>
        /// <returns>Returns dictionary compilation of the object</returns>
        public Dictionary<string, string> GetInformation()
        {
            Dictionary<string, string> output = new Dictionary<string, string>
            {
                {"Label", Label},
                {"Description", Content},
                {"Type", Type.ToString()},
            };
            return output;
        }

        public void UpdateHashID()
        {
            this.HashId = CalculateMd5Hash(true);
        }

        public override bool Equals(object obj)
        {
            if (obj is Field == false)
                return false;

            Field other = (Field) obj;
            return (this.HashId == other.HashId);
        }

        /// <summary>
        /// Calculates a MD5 hash for the input string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="uniqueHash"></param>
        /// <returns></returns>
        private string CalculateMd5Hash(bool uniqueHash = false)
        {
            string hashString = "";
            if (uniqueHash)
                hashString += this.Label + Type.ToString() + DateTime.Now;
            else
                hashString += this.Label + Type.ToString();

            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(hashString);
            byte[] hash = md5.ComputeHash(inputBytes);
            md5.Dispose();

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }

        public override string ToString() => $"{Label} : {Content} : {Type}";
    }
}
