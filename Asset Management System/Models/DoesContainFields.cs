using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Asset_Management_System.Models
{
    public abstract class DoesContainFields : Model
    {
        public string SerializedFields { get; set; }

        public List<Field> FieldsList { get; set; }

        private int IDCounter = 0;

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
            FieldsList = JsonConvert.DeserializeObject<List<Field>>(SerializedFields);
        }

        /// <summary>
        /// Adds a fields to the list of fields. And Serializes the current state of fields.
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
            ShowField();
            SerializeFields();
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
            ShowField();
            return true;
        }

        /// <summary>
        /// Gets checksum of all the contained fields.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public string GetChecksum()
        {
            string checksum = "";
            foreach (Field field in FieldsList)
            {
                Dictionary<string,string> information = field.GetInformation();
                checksum += information["Name"] + information["FieldType"] + information["Required"] + information["DefaultValue"];
            }
            using (var md5 = MD5.Create())
            {
                using (FileStream fs = new FileStream(checksum, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var fileHash = md5.ComputeHash(fs);
                    checksum = BitConverter.ToString(fileHash).Replace("-", "").ToLower();
                    fs.Close();
                }
            }

            return checksum;
        }

        public void ShowField()
        {
            //Insert fancywancy XAML stuff
        }
    }
}