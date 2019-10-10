using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Field
    {
        /// <summary>
        /// Default constructor for initiating a new Field object.
        /// </summary>
        /// <param name="name">The label of the field</param>
        /// <param name="content">The content added to the field</param>
        /// <param name="required">A boolean, whether the field is required or not</param>
        /// <param name="fieldType">Selecting the type of the field. 1= TextBox,2 = String,3= Int, 4 = Date, 5 = Boolean</param>
        /// <param name="defaultValue">The default value which should be entered into the field</param>
        public Field(int ID, string name, string content, int fieldType, string defaultValue, bool required = false)
        {
            this.ID = ID;
            this.Name = name;
            this.Content = content;
            this.Required = required;
            if (fieldType <= 5)
            {
                this.FieldType = fieldType;
            }
            else
            {
                throw new ArgumentOutOfRangeException("fieldType","fieldType is out of range. Must be an integer between 1-5 (both included)");
            }
            this.DefaultValue = defaultValue;
        }

        public int ID { get; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool Required { get; set; }

        public int FieldType;

        public readonly string DefaultValue;
        
        public string CheckSum { get; set; }

        /// <summary>
        /// Returns the object information as a dictionary.
        /// </summary>
        /// <returns>Returns dictionary compilation of the object</returns>
        public Dictionary<string, string> GetInformation()
        {
            Dictionary<string, string> output = new Dictionary<string, string>
            {
                { "Name", Name },
                { "Description", Content },
                { "Required", Required.ToString() },
                { "FieldType", this.GetFieldType() },
                { "DefaultValue", DefaultValue }
            };
            return output;
        }

        /// <summary>
        /// Gets the saved content of the field.
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            return this.Content;
        }

        public bool UpdateContent(string input)
        {
            this.Content = input;
            return true;
        }

        /// <summary>
        /// Returns the FieldType of the field.
        /// </summary>
        /// <returns></returns>
        private string GetFieldType()
        {
            return this.FieldType switch
            {
                1 => "TextBox",
                2 => "String",
                3 => "Int",
                4 => "Date",
                5 => "Boolean",
                _ => "Field type invalid",
            };
        }
    }
}