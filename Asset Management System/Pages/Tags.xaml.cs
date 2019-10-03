using System;
using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : Page
    {
        public Tags()
        {
            InitializeComponent();

            TagRepository rep = new TagRepository();

            Tag tag = rep.GetById(1);

            Console.WriteLine(tag.Name);

            Department dep1 = new Department("IT Department");
            Department dep2 = new Department("HR Department");

            DepartmentRepository dep = new DepartmentRepository();

            dep.Insert(dep1);
            dep.Insert(dep2);

        }
    }
}
