using System;
using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : Page
    {
        private MainWindow Main;
        public Tags(MainWindow main)
        {
            InitializeComponent();
            Main = main;
        }
    }
}
