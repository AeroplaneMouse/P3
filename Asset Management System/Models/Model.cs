using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    public abstract class Model : IUpdateSubject
    {
        public Int64 ID { get; protected set; }

        public String Name { get; set; }

        public DateTime CreatedAt;

        protected List<IUpdateObserver> observers = new List<IUpdateObserver>();
        public void Attach(IUpdateObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Detach(IUpdateObserver observer)
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }

        public void Notify()
        {
            foreach (IUpdateObserver observer in observers)
            {
                observer.Update(this);
            }
        }
    }
}
