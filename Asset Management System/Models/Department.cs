
namespace Asset_Management_System.Models
{
    class Department
    {
        public Department()
        {

        }

        private Department(long id, string name){
            this.ID = id;
            this.Name = name;
        }

        public long ID { get; }
        public string Name { get; set; }
    }
}