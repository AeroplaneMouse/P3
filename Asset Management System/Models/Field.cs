using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        /// <param name="selectedType">Selecting the type of the field. 1= Int,2 = String,3= TextBox, 4 = Date, 5 = Boolean</param>
        /// <param name="defaultValue">The default value which should be entered into the field</param>
        public Field(int ID, string name, string content, int fieldType, string defaultValue, bool required = false)
        {
            this.ID = ID;
            this.Name = name;
            this.Content = content;
            this.Required = required;
            this._fieldType = fieldType;
            this.DefaultValue = defaultValue;
        }

        public int ID { get; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool Required { get; set; }

        private int _fieldType;

        public readonly string DefaultValue;

        /// <summary>
        /// Returns the object information as a dictionary.
        /// </summary>
        /// <returns>Returns dictionary compilation of the object</returns>
        public Dictionary<string, string> GetInformation()
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            output.Add("Name", Name);
            output.Add("Description", Content);
            output.Add("Required", Required.ToString());
            output.Add("FieldType", this.GetFieldType());
            output.Add("DefaultValue", DefaultValue);
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
            switch (this._fieldType)
            {
                case 1:
                    return "TextBox";
                case 2:
                    return "String";
                case 3:
                    return "Int";
                case 4:
                    return "Date";
                case 5:
                    return "Boolean";
                default:
                    return "Field type invalid";
            }
        }
    }
}