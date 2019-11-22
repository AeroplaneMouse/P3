using System.Collections.Generic;
using AMS.Authentication;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging.Interfaces;
using AMS.Models;
using Newtonsoft.Json;

namespace AMS.Logging
{
    public class Log : ILogger
    {
        public Dictionary<string, string> PreviousValues { get; private set; }
        private readonly ILogRepository _logRepository;

        public Log(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        /// <summary>
        /// Logs the creation of a item
        /// </summary>
        /// <param name="loggableValues"></param>
        /// <returns></returns>
        public bool LogCreate(ILoggableValues loggableValues)
        {
            var values = loggableValues.GetLoggableValues();
            // Get the id from the dictionary of values
            ulong id = ulong.Parse(values["ID"]);
            
            // Determine changes and serialize to json
            string serializedChanges = values.Count == 0 ? "[]" : JsonConvert.SerializeObject(values, Formatting.Indented);
            
            // Create a description of the entry
            string description = GenerateDescription(loggableValues, "Created");
            
            // Save the entry to database
            return Write(loggableValues, description, id, serializedChanges);
        }

        /// <summary>
        /// Logs the removal of an item
        /// </summary>
        /// <param name="loggableValues"></param>
        /// <returns></returns>
        public bool LogDelete(ILoggableValues loggableValues)
        {
            var values = loggableValues.GetLoggableValues();
            // Get the id from the dictionary of values
            ulong id = ulong.Parse(values["ID"]);

            // Create a description of the entry
            string description = GenerateDescription(loggableValues, "Deleted");
            
            // Save the entry to database
            return Write(loggableValues, description, id);
        }

        /// <summary>
        /// Logs an item update
        ///
        /// This method requires that the method SavePreviousValues
        /// has been called before the changes to be logged were made
        /// </summary>
        /// <param name="loggableValues"></param>
        /// <returns></returns>
        public bool LogUpdate(ILoggableValues loggableValues)
        {
            // Return false if previous values are not saved
            if (PreviousValues == null)
                return false;

            var values = loggableValues.GetLoggableValues();
            // Get the id from the dictionary of values
            ulong id = ulong.Parse(values["ID"]);
            
            // Determine changes and serialize to json
            Dictionary<string, Change> changes = GetChanges(values);
            string serializedChanges = changes.Count == 0 ? "[]" : JsonConvert.SerializeObject(changes, Formatting.Indented);
            
            // Create a description of the entry
            string description = GenerateDescription(loggableValues, "Updated");
            
            // Save the entry to database
            return Write(loggableValues, description, id, serializedChanges);
        }

        /// <summary>
        /// Saves the values of the loggable properties, as the previous values.
        /// This method should be called before the changes are made to the object.
        /// This method needs to be called before LogUpdate().
        /// </summary>
        /// <returns></returns>
        public bool SavePreviousValues(ILoggableValues previousLoggableValues)
        {
            PreviousValues = previousLoggableValues.GetLoggableValues();
            return PreviousValues.Count > 0;
        }
        
        /// <summary>
        /// Determines the change between two loggable entries.
        /// Returns a dictionary with the name of the properties that have changed and the changes.
        /// If the given entries are not of the same type, returns an empty dictionary.
        /// </summary>
        /// <param name="newValues"></param>
        /// <returns></returns>
        private  Dictionary<string, Change> GetChanges(Dictionary<string, string> newValues)
        {
            Dictionary<string, Change> changes = new Dictionary<string, Change>();   

            // Iterates through the values and creates a new change if they do not match
            foreach (var prop in PreviousValues)
            {
                string oldValue = PreviousValues[prop.Key];
                string newValue = newValues[prop.Key];
                
                if (newValue != oldValue)
                    changes.Add(prop.Key, new Change(oldValue, newValue));
            }

            return changes;
        }
        
       /// <summary>
       /// Generates a description for the log entry
       ///
       /// entryAction options: Created, Updated, Deleted
       /// </summary>
       /// <param name="loggableValues"></param>
       /// <param name="entryAction"></param>
       /// <returns></returns>
        private string GenerateDescription(ILoggableValues loggableValues, string entryAction)
        {
            // Get subject Type as string
            string type = loggableValues.GetLoggableTypeName();
            Dictionary<string, string> propDictionary = loggableValues.GetLoggableValues();
            
            // Get subject Name or ID
            var name = propDictionary.ContainsKey("Name") ? propDictionary["Name"] : propDictionary["ID"];
            
            return $"{type} {name} was {entryAction}";
        }
       
       /// <summary>
       /// Creates a new Entry with the given parameters,
       /// and saves it to the database.
       /// </summary>
       /// <param name="loggableValues"></param>
       /// <param name="description"></param>
       /// <param name="id"></param>
       /// <param name="options"></param>
       /// <returns></returns>
       private bool Write(ILoggableValues loggableValues, string description, ulong id,  string options="{}")
       {
           Entry entry = new Entry
           {
               LogableId = id,
               LogableType = loggableValues.GetLoggableTypeName(),
               Description = description,
               Options = options,
               Username = Session.GetIdentity()
           };

           return _logRepository.Insert(entry);
       }
    }
}