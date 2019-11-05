using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Field
    {
        public enum FieldType { Textarea, Textbox, Integer, Date, Boolean};

        public static IEnumerable<FieldType> GetTypes()
        {
            return new List<FieldType>() {
                FieldType.Textarea,
                FieldType.Textbox,
                FieldType.Integer,
                FieldType.Date,
                FieldType.Boolean
            };
        }

        /// <summary>
        /// Default constructor for initiating a new Field object.
        /// </summary>
        /// <param name="label">The label of the field</param>
        /// <param name="content">The content added to the field</param>
        /// <param name="isCustom"></param>
        /// <param name="required">A boolean, whether the field is required or not</param>
        /// <param name="fieldType">Selecting the type of the field. 1= TextBox,2 = String,3= Int, 4 = Date, 5 = Boolean</param>
        /// <param name="defaultValue">The default value which should be entered into the field</param>
        public Field(string label, string content, FieldType type, string defaultValue, bool required = false,bool isCustom = false)
        {
            // Creates unique hash
            this.HashId = CalculateMd5Hash(true);
            
            this.Label = label;
            this.Content = content;
            Type = type;
            //if (type <= 5)
            //{
            //    this.FieldType = fieldType;
            //}
            //else
            //{
            //    throw new ArgumentOutOfRangeException("fieldType",
            //        "fieldType is out of range. Must be an integer between 1-5 (both included)");
            //}

            this.DefaultValue = defaultValue;
            this.Required = required;
            this.Hash = CalculateMd5Hash();
            this.IsCustom = isCustom;
        }

        public string HashId { get; set; }

        public bool IsCustom;

        public bool IsHidden = false;
        public string Label { get; set; }
        public string Content { get; set; }
        public bool Required { get; set; }
        public string Hash { get; }

        //private int _fieldType;

        public FieldType Type { get; set; }
        //{
        //    get => this._fieldType;
        //    set
        //    {
        //        if (value <= 5 && value > 0)
        //        {
        //            this._fieldType = value;
        //        }
        //        else
        //        {
        //            throw new ArgumentOutOfRangeException("fieldType",
        //                "fieldType is out of range. Must be an integer between 1-5 (both included)");
        //        }
        //    }
        //}

        public string DefaultValue;

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
                {"FieldType", Type.ToString()},
                {"DefaultValue", DefaultValue}
            };
            return output;
        }

        public override bool Equals(object obj)
        {
            if (obj is Field == false)
            {
                return false;
            }

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
            {
                hashString += this.Label + this.Required.ToString() + Type.ToString() + this.DefaultValue +
                              DateTime.Now;
            }
            else
            {
                hashString += this.Label + this.Required.ToString() + Type.ToString() + this.DefaultValue;
            }

            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(hashString);
            byte[] hash = md5.ComputeHash(inputBytes);
            md5.Dispose();

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return $"{ Label } : { Content } : { DefaultValue } : { Type } : { Required } : { IsHidden }";
        }
    }
}