using System.Collections.Generic;
using Newtonsoft.Json;

namespace Asset_Management_System.Models
{
    public abstract class DoContainFields
    {
        public string SerializedFields { get; set; }

        public List<Field> FieldsList { get; set; }

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
        /// Adds a fields to the list of fields. For the content to be saved on the element, use SerializeFields afterwards.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fieldType"></param>
        /// <param name="content"></param>
        /// <param name="defaultValue"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        public bool AddField(string name, int fieldType, string content, string defaultValue, bool required = false)
        {
            Field currentField = new Field(name, content, fieldType, defaultValue, required);

            FieldsList.Add(currentField);
            ShowField();
            return true;
        }

        public void ShowField()
        {
            //Insert fancywancy XAML stuff
        }
    }
}