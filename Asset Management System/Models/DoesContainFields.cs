using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Asset_Management_System.Models
{
    [Serializable]
    public abstract class DoesContainFields : Model
    {
        public string SerializedFields { get; set; }

        [JsonIgnore] public List<Field> FieldsList { get; set; }

        private int IDCounter = 0;

        public string DateToStringConverter => CreatedAt.ToString("MM/dd/yyyy HH:mm:ss");

        public DoesContainFields()
        {
            FieldsList = new List<Field>();
        }

        /// <summary>
        /// This function is used for Serializing the list of fields, and their content to SerializedFields.
        /// </summary>
        public void SerializeFields()
        {
            SerializedFields = JsonConvert.SerializeObject(FieldsList, Formatting.Indented);
        }

        /// <summary>
        /// Reloads the fields from SerializedFields into a list of fields.
        /// </summary>
        public void DeserializeFields()
        {
            if (SerializedFields != null)
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
        /// <param name="fieldType"></param>
        /// <param name="content"></param>
        /// <param name="defaultValue"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public bool AddField(string name, int fieldType, string content, string defaultValue, bool required = false)
        {
            Field currentField = new Field(this.IDCounter, name, content, fieldType, defaultValue, required);

            this.IDCounter++;

            FieldsList.Add(currentField);
            SerializeFields();
            return true;
        }

        public bool AddField(Field input)
        {
            FieldsList.Add(input);
            return true;
        }

        /// <summary>
        /// Removes field from list of fields.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveField(int id)
        {
            var itemToRemove = FieldsList.SingleOrDefault(r => r.ID == id);
            if (itemToRemove != null)
            {
                FieldsList.Remove(itemToRemove);
            }

            SerializeFields();
            return true;
        }

        /// <summary>
        /// Removes a field from the list
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool RemoveField(Field field)
        {
            FieldsList.Remove(field);

            SerializeFields();
            return true;
        }
    }
}