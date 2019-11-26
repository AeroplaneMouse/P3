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
    public class Logger : ILogger
    {
        public ILogRepository _logRepository;

        public Logger(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public bool AddEntry(Model entity, ulong userId)
        {
            string description;
            string changes;
            string entryType;

            if (entity != null)
            {
                if (entity.Changes.Any())
                {
                    description = entity.GetType().ToString().Split('.').Last() + " with id " + entity.ID;
                    changes = this.GetChanges(entity);
                    entryType = "Update";
                    Console.WriteLine(entryType);
                }
                else if (entity.ID == 0)
                {
                    description = entity.GetType().ToString().Split('.').Last() + " with id " + entity.ID;
                    changes = this.GetPropertiesAndValues(entity);
                    entryType = "Create";
                    Console.WriteLine(entryType);
                }
                else
                {
                    description = entity.GetType().ToString().Split('.').Last() + " with id " + entity.ID;
                    changes = this.GetPropertiesAndValues(entity);
                    entryType = "Delete";
                    Console.WriteLine(entryType);
                }

                this.Write(entryType, description, userId, changes);

            }
            else
            {
                return false;
            }
            return true;
        }

        public bool AddEntry(string inputEntryType, string inputDescription, ulong userId = 0, string changes = "[]", Exception e = null)
        {
            string error;
            if (e != null)
            {
                error = "\nError message: " + e.Message + "\nStack trace:" + e.StackTrace;
            }
            else
            {
                error = "";
            }

            string description = inputDescription + error;

            return Write(inputEntryType, description, userId, changes);
        }

        private string GetPropertiesAndValues(Model entity)
        {
            // Create dictionary of all values of entity
            Dictionary<string, string> keyValuePairs = GetPropertyNamesAndValues(entity);

            // Determine changes and serialize to json
            string changes = JsonConvert.SerializeObject(keyValuePairs, Formatting.Indented);

            // Save the entry to database
            return changes;
        }

        private string GetChanges(Model entity)
        {
            Dictionary<string, Change> keyValuePairsOfChanges = new Dictionary<string, Change>();
            
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (entity.Changes.ContainsKey(property.Name)){
                    keyValuePairsOfChanges.Add(property.Name, new Change(entity.Changes[property.Name].ToString(), property.GetValue(entity).ToString()));
                }
            }

            return JsonConvert.SerializeObject(keyValuePairsOfChanges, Formatting.Indented);
        }

        private Dictionary<string, string> GetPropertyNamesAndValues(Model entity)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            // Create a description of the entry
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                keyValuePairs.Add(property.Name, property.GetValue(entity).ToString());
            }

            return keyValuePairs;
        }
       
        private bool Write(string entryType, string description, ulong userId = 0, string changes = "[]")
        {
            LogEntry entry = new LogEntry(userId, entryType, description, changes);

            Console.WriteLine("UserID: " + entry.UserId.ToString() + "\nEntry Type: " + entry.EntryType + "\nDescription: " + entry.Description + "\nChanges: " + changes);

            return _logRepository.Insert(entry);
        }
    }
}