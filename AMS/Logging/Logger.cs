using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AMS.Logging.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using Newtonsoft.Json;

namespace AMS.Logging
{
    public class Logger : Ilogger
    {
        public ILogRepository _logRepository;

        public Logger(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        /// <summary>
        /// Adding an entry to the log, based on a model and the userId
        /// </summary>
        /// <param name="entity">The model being handled</param>
        /// <param name="userId">The id of the related user</param>
        /// <returns>Rather the entry was successfully added or not</returns>
        public bool AddEntry(Model entity, ulong userId, ulong entityId = 0)
        {
            string description;
            string changes;
            string entryType;
            ulong id;

            if (entity != null)
            {
                if (entity.IsDirty() && entityId == 0)
                {
                    id = entity.ID;
                    entryType = "Update";
                    changes = this.GetChanges(entity);
                }
                else if (entity.ID == 0 || entityId != 0)
                {
                    id = entityId;
                    entryType = "Create";
                    changes = this.GetPropertiesAndValues(entity);
                }
                else
                {
                    id = entity.ID;
                    entryType = "Delete";
                    changes = this.GetPropertiesAndValues(entity);
                }

                string name = entity is Asset ? ((Asset)entity).Name : (entity is Tag ? ((Tag)entity).Name : (entity is Department ? ((Department)entity).Name : null));

                if (name == null && entity is Comment)
                {
                    description = entity.GetType().ToString().Split('.').Last() + " with ID: " + id + " connected to asset with ID: " + ((Comment)entity).AssetID;
                }
                else if(name != null)
                {
                    description = entity.GetType().ToString().Split('.').Last() + ": '" + name + "' with ID: " + id;
                }
                else
                {
                    description = entity.GetType().ToString().Split('.').Last() + " with id " + id;
                }

                return this.Write(entryType, description, userId, id, entity.GetType(), changes);

            }
            
            return false;
        }

        /// <summary>
        /// Adding an entry to the log, based on custom an entry type, description, and potentiallu user id and exception
        /// </summary>
        /// <param name="inputEntryType">The type of the log entry</param>
        /// <param name="inputDescription">The description of the log entry (if an exception is given as input, the error message and stack trace will be added to the description)</param>
        /// <param name="userId">The id of the related user (optional)</param>
        /// <param name="changes">Changes made as a serialized JSON format (optional)</param>
        /// <param name="e">The exception to be logged (optional)</param>
        /// <returns>Rather the entry was successfully added or not</returns>
        public bool AddEntry(string inputEntryType, string inputDescription, ulong userId = 0, string changes = "[]")
        {
            if (inputEntryType == null)
                return false;

            return this.Write(inputEntryType, inputDescription, userId, 0, null, changes);
        }

        /// <summary>
        /// Adding an entry to the log, based on custom an entry type, description, and potentiallu user id and exception
        /// </summary>
        /// <param name="inputEntryType">The type of the log entry</param>
        /// <param name="inputDescription">The description of the log entry (if an exception is given as input, the error message and stack trace will be added to the description)</param>
        /// <param name="userId">The id of the related user (optional)</param>
        /// <param name="changes">Changes made as a serialized JSON format (optional)</param>
        /// <param name="e">The exception to be logged (optional)</param>
        /// <returns>Rather the entry was successfully added or not</returns>
        public bool AddEntry(Exception e = null)
        {
            string description;
            if (e != null)
            {
                description = "\nError message: " + e.Message + "\nStack trace:" + e.StackTrace;
            }
            else
            {
                description = "";
            }

            return this.Write("Exception", description);
        }

        /// <summary>
        /// A method to retrieved all properties and their values from a model
        /// </summary>
        /// <param name="entity">The model from which the properties should be retrieved</param>
        /// <returns>A serialized JSON string of a dictionary with properties and values</returns>
        private string GetPropertiesAndValues(Model entity)
        {
            // Create dictionary of all values of entity
            Dictionary<string, string> keyValuePairs = GetPropertyNamesAndValues(entity);

            // Determine changes and serialize to json
            string changes = JsonConvert.SerializeObject(keyValuePairs, Formatting.Indented);

            // Save the entry to database
            return changes;
        }

        /// <summary>
        /// A method to retrieved all properties and their values from a model
        /// </summary>
        /// <param name="entity">The model from which the properties should be retrieved</param>
        /// <returns>A a dictionary with properties and values of the model</returns>
        private Dictionary<string, string> GetPropertyNamesAndValues(Model entity)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            // Create a description of the entry
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                keyValuePairs.Add(property.Name, property.GetValue(entity)?.ToString());
            }

            return keyValuePairs;
        }

        /// <summary>
        /// A method to return the changes made to a model
        /// </summary>
        /// <param name="entity">The model from which the changes should be retrieved</param>
        /// <returns>A serialized JSON string of a directory with the properties, the old values, and the new values</returns>
        private string GetChanges(Model entity)
        {
            Dictionary<string, Change> keyValuePairsOfChanges = new Dictionary<string, Change>();

            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (entity.Changes.ContainsKey(property.Name))
                {
                    keyValuePairsOfChanges.Add(property.Name, new Change(entity.Changes[property.Name].ToString(), property.GetValue(entity)?.ToString()));
                }
            }

            return JsonConvert.SerializeObject(keyValuePairsOfChanges, Formatting.Indented);
        }

        /// <summary>
        /// Converts the parameters to a LogEntry and calls the repository insert function
        /// </summary>
        /// <param name="entryType">The entry type of the LogEntry</param>
        /// <param name="description">A description of the entry</param>
        /// <param name="userId">The id of the related user</param>
        /// <param name="loggedItemId">The id of the item to be logged</param>
        /// <param name="loggedItemType">The type of the item to be logged</param>
        /// <param name="changes">The changes made</param>
        /// <returns>Rather the entry was successfully inserted into the database</returns>
        private bool Write(string entryType, string description, ulong userId = 0, ulong loggedItemId = 0, Type loggedItemType = null, string changes = "[]")
        {
            LogEntry entry = new LogEntry(userId, entryType, description, loggedItemId, loggedItemType, changes);

            return _logRepository.Insert(entry);
        }
    }
}