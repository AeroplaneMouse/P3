using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Documents;
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
            Type objectType = this.GetType();
            PropertyInfo[] props = objectType.GetProperties();
            
            foreach (var prop in props)
            {
                string key = prop.Name;
                string value = objectType.GetProperty(key).GetValue(this, null).ToString();
                prevValues.Add(key, value);
                Console.WriteLine("Field " + key + " was saved with value: " + value);
            }
        }

        /// <summary>
        /// Get the changes in Json.
        /// Format is: Name, prevValue, newValue
        /// </summary>
        /// <returns>string</returns>
        public string GetChanges()
        {
            List<Tuple<string, string, string>> changes = new List<Tuple<string, string, string>>();
            Type objectType = this.GetType();
            PropertyInfo[] props = objectType.GetProperties();
            foreach (var prop in props)
            {
                string key = prop.Name;
                Console.WriteLine("Key is: " + key);
                //string newValue = prop.GetValue(this.GetType()).ToString();
                string newValue = objectType.GetProperty(key).GetValue(this, null).ToString();
                string oldValue = prevValues[key];
                if (oldValue != newValue)
                {
                    changes.Add(new Tuple<string, string, string>("Value: " + key, "Previous value: " + oldValue, "New value: " + newValue));
                }
            }
            return changes.Count == 0 ? "" : JsonConvert.SerializeObject(changes, Formatting.Indented);
        }
        
    }
}
