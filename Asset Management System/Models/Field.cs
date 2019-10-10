using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Field
    {
        /// <summary>
        /// Default constructor for initiating a new Field object.
        /// </summary>
        /// <param name="label">The label of the field</param>
        /// <param name="content">The content added to the field</param>
        /// <param name="required">A boolean, whether the field is required or not</param>
        /// <param name="fieldType">Selecting the type of the field. 1= TextBox,2 = String,3= Int, 4 = Date, 5 = Boolean</param>
        /// <param name="defaultValue">The default value which should be entered into the field</param>
        public Field(int ID, string label, string content, int fieldType, string defaultValue, bool required = false)
        {
            this.ID = ID;
            this.Label = label;
            this.Content = content;
            if (fieldType <= 5)
            {
                this.FieldType = fieldType;
            }
            else
            {
                throw new ArgumentOutOfRangeException("fieldType","fieldType is out of range. Must be an integer between 1-5 (both included)");
            }
            this.DefaultValue = defaultValue;
            this.Required = required;
        }

        public int ID { get; }
        public string Label { get; set; }
        public string Content { get; set; }
        public bool Required { get; set; }

        private int _fieldType;
        public int FieldType {
            get { return this._fieldType; }
            set {
                if (value <= 5 && value > 0)
                {
                    this._fieldType = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("fieldType", "fieldType is out of range. Must be an integer between 1-5 (both included)");
                }
            } }

        public readonly string DefaultValue;

        /// <summary>
        /// Returns the object information as a dictionary.
        /// </summary>
        /// <returns>Returns dictionary compilation of the object</returns>
        public Dictionary<string, string> GetInformation()
        {
            Dictionary<string, string> output = new Dictionary<string, string>
            {
                { "Label", Label },
                { "Description", Content },
                { "Required", Required.ToString() },
                { "FieldType", FieldType.ToString() },
                { "DefaultValue", DefaultValue }
            };
            return output;
        }

        /// <summary>
        /// Gets checksum of the field.
        /// </summary>
        /// <returns>The checksum</returns>
        /// <exception cref="NullReferenceException"></exception>
        public string GetChecksum()
        {
            string checksum = "";

            checksum += this.Label + this.Required.ToString() + this.FieldType.ToString() + this.DefaultValue;

            checksum = this.CalculateMD5Hash(checksum);

            return checksum;
        }

        /// <summary>
        /// Calculates a MD5 hash for a string. (by Jani Järvinen)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
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
    }
}