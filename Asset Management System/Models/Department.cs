
namespace Asset_Management_System.Models
{
    public class Department : Model
    {
        public Department(string name)
        {
            Name = name;
        }

        private Department(long id, string name){
            this.ID = id;
            this.Name = name;
        }

        public override string ToString() => Name;
    }
}