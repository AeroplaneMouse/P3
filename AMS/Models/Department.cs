using System;
using System.ComponentModel;

namespace AMS.Models
{
    public class Department : Model
    {
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name {
            get {
                return this._name;
            }
            set {
                if (this.Name != null)
                {
                    this.Changes["Name"] = this.Name;
                }
                this._name = value;
            }
        }

        private Department(ulong id, string name, DateTime created_at, DateTime updated_at)
        {
            ID = id;
            Name = name;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
        }

        public Department()
        {
            
        }

        public static Department GetDefault() => new Department(0, "All departments", DateTime.Now, DateTime.Now);

        public override string ToString() => Name;
    }
}