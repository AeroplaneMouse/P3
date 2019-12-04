using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class Field
    {
        public enum FieldType
        {
            Textarea = 1,
            TextBox,
            NumberField,
            Date,
            Checkbox
        };

        public static IEnumerable<FieldType> GetTypes()
        {
            return new List<FieldType>()
            {
                FieldType.Textarea,
                FieldType.TextBox,
                FieldType.NumberField,
                FieldType.Date,
                FieldType.Checkbox
            };
        }
        
        public List<ulong> TagIDs { get; set; }
        
        [JsonIgnore]
        public List<Tag> TagList { get; set; }

        public string HashId { get; set; }

        public bool IsCustom { get; set; }

        public bool IsHidden = false;
        public string Label { get; set; }
        public string Content { get; set; }
        public bool Required { get; set; }
        public string Hash { get; }

        public FieldType Type { get; set; }

        /// <summary>
        /// Default constructor for initiating a new Field object.
        /// </summary>
        /// <param name="label">The label of the field</param>
        /// <param name="content">The content added to the field</param>
        /// <param name="isCustom"></param>
        /// <param name="required">A boolean, whether the field is required or not</param>
        /// <param name="type">Selecting the type of the field. 1= TextBox,2 = String,3= Int, 4 = Date, 5 = Boolean</param>
        /// <param name="defaultValue">The default value which should be entered into the field</param>
        public Field(string label, string content, FieldType type, bool required = false,
            bool isCustom = false)
        {
            // Creates unique hash
            this.HashId = CalculateMd5Hash(true);
            this.Label = label;
            this.Content = content;
            Type = type;
            this.Required = required;
            this.Hash = CalculateMd5Hash();
            this.IsCustom = isCustom;
            this.IsHidden = false;
            this.TagIDs = new List<ulong>();
        }
        
        [JsonConstructor]
        private Field(string label, string content, FieldType type, bool required,
            bool isCustom,List<ulong> tagIDs)
        {
            this.HashId = CalculateMd5Hash(true);
            this.Label = label;
            this.Content = content;
            Type = type;
            this.IsCustom = isCustom;
            this.Required = required;
            this.Hash = CalculateMd5Hash();
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
                {"Required", Required.ToString()},
                {"Type", Type.ToString()},
            };
            return output;
        }

        public override bool Equals(object obj)
        {
            if (obj is Field == false)
                return false;

            Field other = (Field) obj;
            return (this.Hash == other.Hash);
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
                hashString += this.Label + this.Required.ToString() + Type.ToString() + DateTime.Now;
            else
                hashString += this.Label + this.Required.ToString() + Type.ToString();

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

        public override string ToString() => $"{Label} : {Content} : {Type} : {Required} : {IsHidden}";
    }
}