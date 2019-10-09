
namespace Asset_Management_System.Models
{
    public class Department : Model
    {
        public Department(string name)
        {
            Name = name;
        }

        private Department(ulong id, string name){
            this.ID = id;
            this.Name = name;
        }

        public string Name { get; set; }

        public override string ToString() => Name;
    }
}