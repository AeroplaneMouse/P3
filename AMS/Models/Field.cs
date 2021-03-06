﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using AMS.Interfaces;
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
        public List<ITagable> TagList { get; set; }

        [JsonIgnore]
        public bool HasNoTagRelations => TagList.Count == 0;

        public string HashId { get; set; }

        public bool IsCustom { get; set; }

        public bool IsHidden { get; set; }
        public string Label { get; set; }
        public string Content { get; set; }
        public bool Required { get; set; }
        public string Hash => CalculateMd5Hash();

        public FieldType Type { get; set; }

        /// <summary>
        /// Default constructor for initiating a new Field object.
        /// </summary>
        /// <param name="label">The label of the field</param>
        /// <param name="content">The content added to the field</param>
        /// <param name="isCustom"></param>
        /// <param name="required">A boolean, whether the field is required or not</param>
        /// <param name="type">Selecting the type of the field. 1= TextBox,2 = String,3= Int, 4 = Date, 5 = Boolean</param>
        public Field(string label, string content, FieldType type, bool required = false,
            bool isCustom = false)
        {
            // Creates unique hash
            this.HashId = CalculateMd5Hash(true);
            
            this.Label = label;
            this.Content = content;
            Type = type;
            this.Required = required;
            this.IsCustom = isCustom;
            this.IsHidden = false;
            this.TagIDs = new List<ulong>();
        }
        
        public Field(string label, string content, FieldType type,string hashId, bool required,
            bool isCustom,bool isHidden,List<ulong> tagIDs)
        {
            this.Label = label;
            this.Content = content;
            Type = type;
            this.HashId = hashId;
            this.Required = required;
            this.IsCustom = isCustom;
            this.IsHidden = isHidden;
            this.TagIDs = tagIDs;
        }
        
        [JsonConstructor]
        private Field(string label, string content, FieldType type, bool required, bool isCustom,List<ulong> tagIDs)
        {
            this.HashId = CalculateMd5Hash(true);
            this.Label = label;
            this.Content = content;
            Type = type;
            this.IsCustom = isCustom;
            this.Required = required;
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
                hashString += Label + Type.ToString() + DateTime.Now.Millisecond;

            else
                hashString += Label + Type.ToString();

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