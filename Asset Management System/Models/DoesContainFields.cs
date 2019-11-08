using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Unicode;
using Newtonsoft.Json;

namespace Asset_Management_System.Models
{
    [Serializable]
    public abstract class DoesContainFields : Model
    {
        public string SerializedFields { get; set; }

        [JsonIgnore] public List<Field> FieldsList { get; set; }
        
        //Used for formatting the DateTimeOutput when showing the elements within a database.
        public string DateToStringConverter => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        public DoesContainFields()
        {
            FieldsList = new List<Field>();
        }

        /// <summary>
        /// This function is used for Serializing the list of fields, and their content to SerializedFields.
        /// </summary>
        public void SerializeFields()
        {
            SerializedFields = JsonConvert.SerializeObject(FieldsList, Formatting.None);
            if(SerializedFields.Length == 0)
            {
                SerializedFields = "[]";
            }
        }

        /// <summary>
        /// Reloads the fields from SerializedFields into a list of fields.
        /// </summary>
        public void DeserializeFields()
        {
            if (SerializedFields != "[]")
            {
                FieldsList = JsonConvert.DeserializeObject<List<Field>>(SerializedFields);
            }
            else
            {
                FieldsList = new List<Field>();
            }
        }

        /// <summary>
        /// Adds a field to the list of fields. And Serializes the current state of fields.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="defaultValue"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public bool AddField(string name, Field.FieldType type, string content, string defaultValue, bool required = false)
        {
            Field currentField = new Field(name, content, type, defaultValue, required);

            FieldsList.Add(currentField);
            return true;
        }

        public bool AddField(Field input)
        {
            FieldsList.Add(input);
            return true;
        }
    }
}