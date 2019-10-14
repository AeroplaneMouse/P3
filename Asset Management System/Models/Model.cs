using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Documents;
using Asset_Management_System.Controllers;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Type = System.Type;

namespace Asset_Management_System.Models
{
    public abstract class Model : IUpdateSubject
    {
        public ulong ID { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        protected List<IUpdateObserver> observers = new List<IUpdateObserver>();
        
        protected Dictionary<string, string> prevValues = new Dictionary<string, string>();

        public Model()
        {
            //TODO this is not the way this should be done, as it removes the opportunities that observer pattern provides
            LogController logController = new LogController();
            Attach(logController);
        }

        /// <summary>
        /// Attaches an observer to this subject. All attached observers vil be updated when notified
        /// </summary>
        /// <param name="observer"></param>
        public void Attach(IUpdateObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        /// <summary>
        /// Detaches an observer from this subject.
        /// </summary>
        /// <param name="observer"></param>
        public void Detach(IUpdateObserver observer)
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }

        /// <summary>
        /// Notifies all attached observers.
        /// </summary>
        public void Notify()
        {
            foreach (IUpdateObserver observer in observers)
            {
                observer.Update(this);
            }
        }

        public bool IsDirty()
        {
            return prevValues.Count > 0;
        }

        /// <summary>
        /// Saves all properties in a dictionary.
        /// Used for comparison when object is changed.
        /// </summary>
        public void SavePrevValues()
        {
            Type objectType = GetType();
            PropertyInfo[] props = objectType.GetProperties();
            
            foreach (var prop in props)
            {
                string key = prop.Name;
                string value = objectType.GetProperty(key)?.GetValue(this, null).ToString();
                prevValues.Add(key, value);
                //Console.WriteLine("Field " + key + " was saved with value: " + value);
            }
        }

        /// <summary>
        /// Get the changes in Json.
        /// Format is: Name, prevValue, newValue
        /// </summary>
        /// <returns>string</returns>
        public string GetChanges()
        {
            Dictionary<string, Change> changes = new Dictionary<string, Change>();
            Type objectType = this.GetType();
            PropertyInfo[] props = objectType.GetProperties();
            foreach (var prop in props)
            {
                string key = prop.Name;
                if (prevValues.ContainsKey(key))
                {
                    string newValue = objectType.GetProperty(key).GetValue(this, null).ToString();
                    string oldValue = prevValues[key];
                    if (oldValue != newValue)
                    {
                        changes.Add(key, new Change( oldValue, newValue));
                    }
                }
            }
            Console.WriteLine(changes.Count == 0 ? "" : JsonConvert.SerializeObject(changes, Formatting.Indented));
            return changes.Count == 0 ? "" : JsonConvert.SerializeObject(changes, Formatting.Indented);
        }
        
    }
}
